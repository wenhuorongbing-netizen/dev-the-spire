param(
    [Parameter(Mandatory = $true)]
    [string]$EvidenceDir,

    [int]$MinRuns = 1,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ExpectedAncientIds = @(),

    [string]$ExpectedPackageVersion,

    [string]$ExpectedGameVersion,

    [string]$ExpectedRitsuLibVersion,

    [string]$ExpectedRitsuCompatBranch,

    [int]$ExpectedPatchCount = 0,

    [switch]$AllowMissingEventTraversal,

    [string]$OutFile,

    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Add-Check {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [Parameter(Mandatory = $true)][string]$Detail
    )

    $checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null

    if (-not $Passed) {
        $mismatches.Add("${Name}: $Detail") | Out-Null
    }
}

function Test-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return $null -ne $Object -and $Object.PSObject.Properties.Name -contains $Name
}

function Get-JsonValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $DefaultValue = $null
    )

    if (Test-JsonProperty -Object $Object -Name $Name) {
        return $Object.$Name
    }

    return $DefaultValue
}

function Get-ArrayValues {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return @()
    }

    if ($Value -is [System.Array]) {
        return @($Value)
    }

    return @($Value)
}

function Get-DelimitedStringTokens {
    param([AllowNull()]$Value)

    return @(Get-ArrayValues -Value $Value |
        ForEach-Object { [string]$_ } |
        ForEach-Object { $_ -split ',' })
}

function Get-NormalizedNonEmptyStringTokens {
    param([AllowNull()]$Value)

    return @(Get-DelimitedStringTokens -Value $Value |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { $_.Trim() })
}

function New-OrdinalStringSet {
    param([AllowNull()]$Values)

    $set = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($value in @(Get-ArrayValues -Value $Values)) {
        if (-not [string]::IsNullOrWhiteSpace([string]$value)) {
            $set.Add([string]$value) | Out-Null
        }
    }

    return ,$set
}

function Read-JsonOrNull {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$CheckName
    )

    try {
        return [System.IO.File]::ReadAllText($Path) | ConvertFrom-Json
    } catch {
        Add-Check -Name $CheckName -Passed $false -Detail "invalid JSON in $Path`: $($_.Exception.Message)"
        return $null
    }
}

function Resolve-ChildOrAbsolutePath {
    param(
        [Parameter(Mandatory = $true)][string]$BaseDir,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        if ([System.IO.Path]::IsPathRooted($Path)) {
            return [System.IO.Path]::GetFullPath($Path)
        }

        return [System.IO.Path]::GetFullPath((Join-Path $BaseDir $Path))
    } catch {
        return ''
    }
}

function ConvertTo-NormalizedPathOrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        return [System.IO.Path]::GetFullPath($Path)
    } catch {
        return ''
    }
}

function Test-PathInsideDirectory {
    param(
        [AllowEmptyString()][string]$Path,
        [AllowEmptyString()][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path) -or [string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $pathFull = [System.IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
        $dirFull = [System.IO.Path]::GetFullPath($Directory).TrimEnd('\', '/')
        $comparison = [System.StringComparison]::OrdinalIgnoreCase

        return $pathFull.Equals($dirFull, $comparison) -or $pathFull.StartsWith($dirFull + [System.IO.Path]::DirectorySeparatorChar, $comparison)
    } catch {
        return $false
    }
}

function Test-PathLeafSafe {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $false
    }

    try {
        return Test-Path -LiteralPath $Path -PathType Leaf
    } catch {
        return $false
    }
}

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        if (-not (Test-PathLeafSafe -Path $Path)) {
            return ''
        }

        $sha = [System.Security.Cryptography.SHA256]::Create()
        try {
            $stream = [System.IO.File]::OpenRead($Path)
            try {
                return ([System.BitConverter]::ToString($sha.ComputeHash($stream)) -replace '-', '').ToLowerInvariant()
            } finally {
                $stream.Dispose()
            }
        } finally {
            $sha.Dispose()
        }
    } catch {
        return ''
    }
}

function Contains-Text {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Test-OrderedTextSequence {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string[]]$Needles
    )

    $offset = 0
    foreach ($needle in $Needles) {
        if ([string]::IsNullOrWhiteSpace($needle)) {
            return $false
        }

        $index = $Text.IndexOf($needle, $offset, [System.StringComparison]::OrdinalIgnoreCase)
        if ($index -lt 0) {
            return $false
        }

        $offset = $index + $needle.Length
    }

    return $true
}

function Test-AllJsonPropertiesPresent {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name) -or $null -eq $item.$Name) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AllJsonPropertiesRetained {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name)) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AnyJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
            return $true
        }
    }

    return $false
}

function Test-NoJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
            return $false
        }
    }

    return $true
}

function Test-NoJsonPropertyFalse {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ((Test-JsonProperty -Object $item -Name $Name) -and $null -ne $item.$Name -and -not [bool]$item.$Name) {
            return $false
        }
    }

    return $true
}

function Test-AnyJsonPropertyStringEquals {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Value
    )

    foreach ($item in @($Items)) {
        if ([string]::Equals([string](Get-JsonValue -Object $item -Name $Name -DefaultValue ''), $Value, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function ConvertTo-DateTimeOffsetParseResult {
    param([AllowEmptyString()][string]$Text)

    [System.DateTimeOffset]$value = [System.DateTimeOffset]::MinValue
    $styles = [System.Globalization.DateTimeStyles]::AssumeUniversal -bor [System.Globalization.DateTimeStyles]::AdjustToUniversal
    $parsed = (-not [string]::IsNullOrWhiteSpace($Text)) -and [System.DateTimeOffset]::TryParse($Text, [System.Globalization.CultureInfo]::InvariantCulture, $styles, [ref]$value)

    [pscustomobject]@{
        Parsed = $parsed
        Value = $value
    }
}

function Test-BytePrefix {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Prefix,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Content
    )

    if ($Prefix.Length -gt $Content.Length) {
        return $false
    }

    for ($i = 0; $i -lt $Prefix.Length; $i++) {
        if ($Prefix[$i] -ne $Content[$i]) {
            return $false
        }
    }

    return $true
}

function Test-CurrentSliceBinding {
    param(
        [AllowEmptyString()][string]$BeforePath,
        [AllowEmptyString()][string]$AfterPath,
        [AllowEmptyString()][string]$CurrentPath
    )

    $result = [ordered]@{
        BeforeExists = Test-PathLeafSafe -Path $BeforePath
        AfterExists = Test-PathLeafSafe -Path $AfterPath
        CurrentExists = Test-PathLeafSafe -Path $CurrentPath
        PrefixMatches = $false
        SliceMatches = $false
        Detail = ''
    }

    if (-not $result.BeforeExists -or -not $result.AfterExists -or -not $result.CurrentExists) {
        $result.Detail = 'requires godot.log.before, godot.log.after-launch, and godot.log.current-iteration'
        return [pscustomobject]$result
    }

    try {
        $beforeBytes = [System.IO.File]::ReadAllBytes($BeforePath)
        $afterBytes = [System.IO.File]::ReadAllBytes($AfterPath)
        $currentBytes = [System.IO.File]::ReadAllBytes($CurrentPath)
        $result.PrefixMatches = Test-BytePrefix -Prefix $beforeBytes -Content $afterBytes
        if (-not $result.PrefixMatches) {
            $result.Detail = 'godot.log.after-launch does not have godot.log.before as a byte prefix'
            return [pscustomobject]$result
        }

        $sliceLength = $afterBytes.Length - $beforeBytes.Length
        if ($currentBytes.Length -ne $sliceLength) {
            $result.Detail = "current slice length $($currentBytes.Length) does not match after-before length $sliceLength"
            return [pscustomobject]$result
        }

        for ($i = 0; $i -lt $sliceLength; $i++) {
            if ($currentBytes[$i] -ne $afterBytes[$beforeBytes.Length + $i]) {
                $result.Detail = "current slice differs from after-launch at byte $i after the before-log prefix"
                return [pscustomobject]$result
            }
        }

        $result.SliceMatches = $true
        $result.Detail = 'godot.log.current-iteration matches godot.log.after-launch after the godot.log.before byte prefix'
        return [pscustomobject]$result
    } catch {
        $result.Detail = $_.Exception.Message
        return [pscustomobject]$result
    }
}

function Get-PatchCountLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][int]$ExpectedCount
    )

    if ($ExpectedCount -le 0) {
        return 0
    }

    $count = [regex]::Escape([string]$ExpectedCount)
    $patchSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+\[Patcher - SpirePlus\]\s+Patch application complete:\s+$count\s+applied,\s+0\s+ignored,\s+0\s+failed,\s+$count\s+total\s*$"
    $registeredSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+ModPatcher applied\s+$count\s+patches\s+\($count\s+registered\)\.?\s*$"
    if ([regex]::IsMatch($Text, $patchSummary) -and [regex]::IsMatch($Text, $registeredSummary)) {
        return 1
    }

    return 0
}

function ConvertTo-AuditSummary {
    param(
        [Parameter(Mandatory = $true)][string]$Json,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $items = @($Json | ConvertFrom-Json)
    $dirtyItems = 0
    $hitCount = 0
    $itemPaths = [System.Collections.Generic.List[string]]::new()
    $itemLengths = [System.Collections.Generic.List[long]]::new()
    $itemSha256s = [System.Collections.Generic.List[string]]::new()

    foreach ($item in $items) {
        if (-not (Test-JsonProperty -Object $item -Name 'Clean') -or -not [bool]$item.Clean) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Path') -and -not [string]::IsNullOrWhiteSpace([string]$item.Path)) {
            $normalizedItemPath = ConvertTo-NormalizedPathOrEmpty -Path ([string]$item.Path)
            if (-not [string]::IsNullOrWhiteSpace($normalizedItemPath)) {
                $itemPaths.Add($normalizedItemPath) | Out-Null
            }
        }

        if (Test-JsonProperty -Object $item -Name 'Length') {
            $itemLengths.Add([long]$item.Length) | Out-Null
        }

        if ((Test-JsonProperty -Object $item -Name 'Sha256') -and -not [string]::IsNullOrWhiteSpace([string]$item.Sha256)) {
            $itemSha256s.Add(([string]$item.Sha256).ToLowerInvariant()) | Out-Null
        }

        if (-not (Test-JsonProperty -Object $item -Name 'SignatureHits')) {
            continue
        }

        foreach ($hit in @($item.SignatureHits)) {
            if (Test-JsonProperty -Object $hit -Name 'Count') {
                $hitCount += [int]$hit.Count
            }
        }
    }

    return [pscustomobject]@{
        Path = $Path
        Items = $items.Count
        ItemPaths = @($itemPaths)
        ItemLengths = @($itemLengths)
        ItemSha256s = @($itemSha256s)
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0)
    }
}

function Read-AuditSummary {
    param([Parameter(Mandatory = $true)][string]$Path)

    $json = [System.IO.File]::ReadAllText($Path)
    return ConvertTo-AuditSummary -Json $json -Path $Path
}

function Invoke-RecomputedAuditSummary {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $auditJson = (& $logAuditScript -Path $LogPath | Out-String)
    if ([string]::IsNullOrWhiteSpace($auditJson)) {
        throw "audit-godot-log.ps1 returned empty output for $LogPath"
    }

    return ConvertTo-AuditSummary -Json $auditJson -Path '<recomputed>'
}

if (-not (Test-Path -LiteralPath $EvidenceDir -PathType Container)) {
    throw "EvidenceDir does not exist: $EvidenceDir"
}

$resolvedEvidenceDir = [System.IO.Path]::GetFullPath((Resolve-Path -LiteralPath $EvidenceDir).Path)
$planPath = Join-Path $resolvedEvidenceDir 'autoslay-plan.json'
$summaryPath = Join-Path $resolvedEvidenceDir 'autoslay-summary.json'
$expectedSts1Mode = ''

Write-Output "evidence_dir=$resolvedEvidenceDir"

$planExists = Test-Path -LiteralPath $planPath -PathType Leaf
$summaryExists = Test-Path -LiteralPath $summaryPath -PathType Leaf
Add-Check -Name 'autoslay_plan_exists' -Passed $planExists -Detail 'requires autoslay-plan.json'
Add-Check -Name 'autoslay_summary_exists' -Passed $summaryExists -Detail 'requires autoslay-summary.json from a launched game-native AutoSlay batch'

$plan = $null
$summary = $null
if ($planExists) {
    $plan = Read-JsonOrNull -Path $planPath -CheckName 'autoslay_plan_json_valid'
    if ($null -ne $plan) {
        Add-Check -Name 'autoslay_plan_json_valid' -Passed $true -Detail 'autoslay-plan.json parsed'
    }
}
if ($summaryExists) {
    $summary = Read-JsonOrNull -Path $summaryPath -CheckName 'autoslay_summary_json_valid'
    if ($null -ne $summary) {
        Add-Check -Name 'autoslay_summary_json_valid' -Passed $true -Detail 'autoslay-summary.json parsed'
    }
}

$planSeeds = @()
$launcherKind = ''
$launcherPath = ''
$launcherSha256 = ''
$hookId = ''
$hookAssembly = ''
$invocationCommand = ''
if ($null -ne $plan) {
    $runnerKind = [string](Get-JsonValue -Object $plan -Name 'RunnerKind' -DefaultValue '')
    $invocation = [string](Get-JsonValue -Object $plan -Name 'Invocation' -DefaultValue '')
    $launcherKind = [string](Get-JsonValue -Object $plan -Name 'LauncherKind' -DefaultValue '')
    $launcherPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $plan -Name 'LauncherPath' -DefaultValue ''))
    $launcherSha256 = [string](Get-JsonValue -Object $plan -Name 'LauncherSha256' -DefaultValue '')
    $hookId = [string](Get-JsonValue -Object $plan -Name 'HookId' -DefaultValue '')
    $hookAssembly = [string](Get-JsonValue -Object $plan -Name 'HookAssembly' -DefaultValue '')
    $invocationCommand = [string](Get-JsonValue -Object $plan -Name 'InvocationCommand' -DefaultValue '')
    $launcherExists = Test-PathLeafSafe -Path $launcherPath
    $expectedSts1Mode = [string](Get-JsonValue -Object $plan -Name 'Sts1EventMode' -DefaultValue '')
    $sourceWorkspace = Get-JsonValue -Object $plan -Name 'SourceWorkspace' -DefaultValue $null
    $planSeeds = @(Get-ArrayValues -Value (Get-JsonValue -Object $plan -Name 'Seeds' -DefaultValue @()) | ForEach-Object { [string]$_ })
    $nonEmptyPlanSeeds = @($planSeeds | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $duplicatePlanSeeds = @($nonEmptyPlanSeeds | Group-Object | Where-Object { $_.Count -gt 1 })

    Add-Check -Name 'plan_runner_kind_is_game_native_autoslay' -Passed ([string]::Equals($runnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)) -Detail "RunnerKind must be GameNativeAutoSlay; found '$runnerKind'"
    Add-Check -Name 'plan_invocation_calls_autoslayer_start' -Passed (Contains-Text -Text $invocation -Needle 'AutoSlayer.Start(seed, logFile)') -Detail 'Invocation must record the launcher/mod hook that calls AutoSlayer.Start(seed, logFile)'
    Add-Check -Name 'plan_launcher_kind_present' -Passed (-not [string]::IsNullOrWhiteSpace($launcherKind)) -Detail 'LauncherKind must identify the retained launcher/mod-hook proof type'
    Add-Check -Name 'plan_launcher_path_present' -Passed (-not [string]::IsNullOrWhiteSpace($launcherPath)) -Detail 'LauncherPath must retain the launcher/mod-hook proof artifact'
    Add-Check -Name 'plan_launcher_path_under_evidence_dir' -Passed (Test-PathInsideDirectory -Path $launcherPath -Directory $resolvedEvidenceDir) -Detail 'LauncherPath must stay inside the evidence directory'
    Add-Check -Name 'plan_launcher_path_exists' -Passed $launcherExists -Detail 'LauncherPath must point at a retained launcher/mod-hook proof artifact'
    Add-Check -Name 'plan_launcher_sha256_present' -Passed (-not [string]::IsNullOrWhiteSpace($launcherSha256)) -Detail 'LauncherSha256 must bind the retained launcher/mod-hook proof artifact'
    if ($launcherExists -and -not [string]::IsNullOrWhiteSpace($launcherSha256)) {
        Add-Check -Name 'plan_launcher_sha256_matches' -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $launcherPath), $launcherSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'LauncherSha256 must match LauncherPath'
    }
    Add-Check -Name 'plan_hook_id_present' -Passed (-not [string]::IsNullOrWhiteSpace($hookId)) -Detail 'HookId must identify the concrete mod hook that starts game-native AutoSlay'
    Add-Check -Name 'plan_hook_assembly_present' -Passed (-not [string]::IsNullOrWhiteSpace($hookAssembly)) -Detail 'HookAssembly must identify the assembly that owns the AutoSlay hook'
    Add-Check -Name 'plan_invocation_command_calls_autoslayer_start' -Passed (Contains-Text -Text $invocationCommand -Needle 'AutoSlayer.Start(seed, logFile)') -Detail 'InvocationCommand must record the exact launcher/mod-hook command that calls AutoSlayer.Start(seed, logFile)'
    Add-Check -Name 'plan_seed_count_meets_minimum' -Passed ($planSeeds.Count -ge $MinRuns) -Detail "Seeds count must be at least $MinRuns; found $($planSeeds.Count)"
    Add-Check -Name 'plan_seeds_all_non_empty' -Passed ($nonEmptyPlanSeeds.Count -eq $planSeeds.Count) -Detail 'all plan Seeds entries must be non-empty'
    Add-Check -Name 'plan_seeds_unique' -Passed ($duplicatePlanSeeds.Count -eq 0) -Detail "plan Seeds must be unique; duplicate seed groups=$($duplicatePlanSeeds.Count)"
    Add-Check -Name 'plan_sts1_event_mode_present' -Passed (-not [string]::IsNullOrWhiteSpace($expectedSts1Mode)) -Detail 'Sts1EventMode must be retained for per-run log verifier binding'
    Add-Check -Name 'allow_missing_event_traversal_not_proof_mode' -Passed (-not ($AllowMissingEventTraversal -and $FailOnMismatch)) -Detail '-AllowMissingEventTraversal is incompatible with -FailOnMismatch proof-mode verification'
    Add-Check -Name 'expected_package_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) -Detail 'AutoSlay proof packets must be checked with -ExpectedPackageVersion'
    Add-Check -Name 'expected_game_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) -Detail 'AutoSlay proof packets must be checked with -ExpectedGameVersion'
    Add-Check -Name 'expected_ritsu_lib_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) -Detail 'AutoSlay proof packets must be checked with -ExpectedRitsuLibVersion'
    Add-Check -Name 'expected_ritsu_compat_branch_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) -Detail 'AutoSlay proof packets must be checked with -ExpectedRitsuCompatBranch'
    Add-Check -Name 'expected_patch_count_parameter_provided' -Passed ($ExpectedPatchCount -gt 0) -Detail 'AutoSlay proof packets must be checked with -ExpectedPatchCount'

    if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
        $actual = [string](Get-JsonValue -Object $plan -Name 'PackageVersion' -DefaultValue '')
        Add-Check -Name 'plan_package_version_matches_expected' -Passed ([string]::Equals($actual, $ExpectedPackageVersion, [System.StringComparison]::Ordinal)) -Detail "PackageVersion expected '$ExpectedPackageVersion'; found '$actual'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
        $actual = [string](Get-JsonValue -Object $plan -Name 'GameVersion' -DefaultValue '')
        Add-Check -Name 'plan_game_version_matches_expected' -Passed ([string]::Equals($actual, $ExpectedGameVersion, [System.StringComparison]::Ordinal)) -Detail "GameVersion expected '$ExpectedGameVersion'; found '$actual'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) {
        $actual = [string](Get-JsonValue -Object $plan -Name 'RitsuLibVersion' -DefaultValue '')
        Add-Check -Name 'plan_ritsulib_version_matches_expected' -Passed ([string]::Equals($actual, $ExpectedRitsuLibVersion, [System.StringComparison]::Ordinal)) -Detail "RitsuLibVersion expected '$ExpectedRitsuLibVersion'; found '$actual'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
        $actual = [string](Get-JsonValue -Object $plan -Name 'RitsuCompatBranch' -DefaultValue '')
        Add-Check -Name 'plan_ritsu_compat_branch_matches_expected' -Passed ([string]::Equals($actual, $ExpectedRitsuCompatBranch, [System.StringComparison]::Ordinal)) -Detail "RitsuCompatBranch expected '$ExpectedRitsuCompatBranch'; found '$actual'"
    }

    $sourceWorkspaceCheckPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $plan -Name 'SourceWorkspaceCheckPath' -DefaultValue ''))
    $sourceWorkspaceCheckSha256 = [string](Get-JsonValue -Object $plan -Name 'SourceWorkspaceCheckSha256' -DefaultValue '')
    $sourceWorkspaceExists = Test-PathLeafSafe -Path $sourceWorkspaceCheckPath
    Add-Check -Name 'plan_source_workspace_summary_present' -Passed ($null -ne $sourceWorkspace) -Detail 'SourceWorkspace summary must retain source version/disposition and evidence-use policy'
    if ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_checked' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Checked' -DefaultValue $false)) -Detail 'SourceWorkspace.Checked must be true'
        Add-Check -Name 'plan_source_workspace_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'SourceWorkspace must record that source inspection is not runtime proof'
        Add-Check -Name 'plan_source_workspace_disposition_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue ''))) -Detail 'SourceWorkspace must retain the recovered-source disposition'
        Add-Check -Name 'plan_source_workspace_matches_installed_game' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'MatchesInstalledGame' -DefaultValue $false)) -Detail 'game-native AutoSlay proof requires a source snapshot that matches the installed game'
        Add-Check -Name 'plan_source_workspace_authorized_source_origin_verified' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false)) -Detail 'SourceWorkspace summary must retain authorized source-origin verification'
        Add-Check -Name 'plan_source_workspace_origin_matches_installed_game_pck' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false)) -Detail 'game-native AutoSlay proof requires GDRE Opening file to installed PCK binding'
        Add-Check -Name 'plan_source_workspace_ritsulib_version_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVersion' -DefaultValue ''))) -Detail 'SourceWorkspace summary must retain RitsuLib manifest version'
        Add-Check -Name 'plan_source_workspace_ritsulib_variant_dll_hash_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantDllSha256' -DefaultValue ''))) -Detail 'SourceWorkspace summary must retain selected RitsuLib variant DLL hash'
    }
    Add-Check -Name 'plan_source_workspace_check_path_present' -Passed (-not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckPath)) -Detail 'SourceWorkspaceCheckPath must bind the packet to check-local-godot-source-workspace.ps1 output'
    Add-Check -Name 'plan_source_workspace_check_under_evidence_dir' -Passed (Test-PathInsideDirectory -Path $sourceWorkspaceCheckPath -Directory $resolvedEvidenceDir) -Detail 'SourceWorkspaceCheckPath must stay inside the evidence directory'
    Add-Check -Name 'plan_source_workspace_check_exists' -Passed $sourceWorkspaceExists -Detail 'requires retained local source-workspace JSON report'
    Add-Check -Name 'plan_source_workspace_check_hash_present' -Passed (-not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckSha256)) -Detail 'SourceWorkspaceCheckSha256 must be retained'
    if ($sourceWorkspaceExists -and -not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckSha256)) {
        Add-Check -Name 'plan_source_workspace_check_hash_matches' -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $sourceWorkspaceCheckPath), $sourceWorkspaceCheckSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'SourceWorkspaceCheckSha256 must match the retained source-workspace report'
    }

    if ($sourceWorkspaceExists) {
        $sourceReport = Read-JsonOrNull -Path $sourceWorkspaceCheckPath -CheckName 'plan_source_workspace_check_json_valid'
        if ($null -ne $sourceReport) {
            Add-Check -Name 'plan_source_workspace_check_json_valid' -Passed $true -Detail 'retained source-workspace JSON parsed'
            $autoSlay = Get-JsonValue -Object $sourceReport -Name 'AutoSlay' -DefaultValue $null
            $policy = Get-JsonValue -Object $sourceReport -Name 'EvidenceUsePolicy' -DefaultValue $null
            $reportMismatches = @(Get-ArrayValues -Value (Get-JsonValue -Object $sourceReport -Name 'Mismatches' -DefaultValue @()))
            Add-Check -Name 'plan_source_workspace_schema_version_one' -Passed ([int](Get-JsonValue -Object $sourceReport -Name 'SchemaVersion' -DefaultValue 0) -eq 1) -Detail 'retained source-workspace report must come from schema version 1'
            foreach ($name in @('CreatedAt', 'RepoRoot', 'SourceRoot', 'GameRoot')) {
                Add-Check -Name "plan_source_workspace_$($name.ToLowerInvariant())_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceReport -Name $name -DefaultValue ''))) -Detail "retained source-workspace report must include $name from check-local-godot-source-workspace.ps1"
            }
            Add-Check -Name 'plan_source_workspace_report_passed' -Passed ([bool](Get-JsonValue -Object $sourceReport -Name 'Passed' -DefaultValue $false)) -Detail 'source-workspace report must have Passed=true'
            Add-Check -Name 'plan_source_workspace_report_mismatches_empty' -Passed ($reportMismatches.Count -eq 0) -Detail "source-workspace report mismatches must be empty; found $($reportMismatches.Count)"
            Add-Check -Name 'plan_source_workspace_policy_no_launch' -Passed ([bool](Get-JsonValue -Object $policy -Name 'NoLaunch' -DefaultValue $false)) -Detail 'source-workspace report must record that the checker did not launch Godot or the game'
            Add-Check -Name 'plan_source_workspace_policy_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $policy -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'source-workspace report must record that source inspection alone is not runtime proof'
            Add-Check -Name 'plan_source_workspace_policy_local_source_reference_only' -Passed ([bool](Get-JsonValue -Object $policy -Name 'LocalSourceReferenceOnly' -DefaultValue $false)) -Detail 'source-workspace report must record local-source-reference-only policy'
            Add-Check -Name 'plan_source_workspace_policy_authorized_local_install_only' -Passed ([bool](Get-JsonValue -Object $policy -Name 'AuthorizedLocalInstallOnly' -DefaultValue $false)) -Detail 'source-workspace report must record authorized-local-install-only policy'
            Add-Check -Name 'plan_source_workspace_policy_authorized_source_origin_verified' -Passed ([bool](Get-JsonValue -Object $policy -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false)) -Detail 'source-workspace report must verify the GDRE Opening file against the installed game PCK'
            Add-Check -Name 'plan_source_workspace_policy_third_party_dumps_prohibited' -Passed ([bool](Get-JsonValue -Object $policy -Name 'ThirdPartyDumpsProhibited' -DefaultValue $false)) -Detail 'source-workspace report must record that third-party dumps are prohibited'
            Add-Check -Name 'plan_source_workspace_policy_runtime_proof_still_requires_launch' -Passed ([bool](Get-JsonValue -Object $policy -Name 'RuntimeProofStillRequiresLaunchEvidence' -DefaultValue $false)) -Detail 'source-workspace report must record that runtime proof still requires launch evidence'
            Add-Check -Name 'plan_source_workspace_policy_autoslay_still_requires_launch' -Passed ([bool](Get-JsonValue -Object $policy -Name 'GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence' -DefaultValue $false)) -Detail 'source-workspace report must keep AutoSlay source checks separate from runtime proof'
            foreach ($name in @('StartSeedLogFileSignature', 'NonInteractiveCheck', 'DebugSeedOverride', 'AutoCardSelector', 'AncientDialogueHandler', 'EventOptionSelectionLog', 'EventTriggeredCombatLog', 'EventCombatStartedLog')) {
                Add-Check -Name "plan_source_workspace_autoslay_$name" -Passed ([bool](Get-JsonValue -Object $autoSlay -Name $name -DefaultValue $false)) -Detail "AutoSlay source-contract field $name must be true"
            }

            $reportRecoveredSource = Get-JsonValue -Object $sourceReport -Name 'RecoveredSource' -DefaultValue $null
            $reportGame = Get-JsonValue -Object $sourceReport -Name 'Game' -DefaultValue $null
            $reportRitsuLib = Get-JsonValue -Object $sourceReport -Name 'RitsuLib' -DefaultValue $null
            Add-Check -Name 'plan_source_workspace_report_matches_installed_game' -Passed ([bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -Detail 'RecoveredSource.MatchesInstalledGame must be true for game-native AutoSlay proof'
            Add-Check -Name 'plan_source_workspace_report_origin_matches_installed_game_pck' -Passed ([bool](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false)) -Detail 'RecoveredSource.OriginMatchesInstalledGamePck must be true for game-native AutoSlay proof'
            Add-Check -Name 'plan_source_workspace_report_ritsulib_present' -Passed ($null -ne $reportRitsuLib) -Detail 'source-workspace report must retain RitsuLib provenance'
            if ($null -ne $reportRitsuLib) {
                foreach ($name in @('Version', 'CompatBranch', 'ManifestPath', 'ManifestSha256', 'VariantsPath', 'VariantsSha256', 'VariantDllPath', 'VariantDllSha256', 'ExpectedVariantDllSha256', 'CompatTargetPath', 'CompatTargetText')) {
                    Add-Check -Name "plan_source_workspace_report_ritsulib_$($name.ToLowerInvariant())_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $reportRitsuLib -Name $name -DefaultValue ''))) -Detail "source-workspace report must include RitsuLib.$name"
                }
                Add-Check -Name 'plan_source_workspace_report_ritsulib_version_matches_expected' -Passed ([string]::Equals([string](Get-JsonValue -Object $reportRitsuLib -Name 'Version' -DefaultValue ''), $ExpectedRitsuLibVersion, [System.StringComparison]::Ordinal)) -Detail "source-workspace report RitsuLib.Version must match expected '$ExpectedRitsuLibVersion'"
                Add-Check -Name 'plan_source_workspace_report_ritsulib_compat_matches_expected' -Passed ([string]::Equals([string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatBranch' -DefaultValue ''), $ExpectedRitsuCompatBranch, [System.StringComparison]::Ordinal)) -Detail "source-workspace report RitsuLib.CompatBranch must match expected '$ExpectedRitsuCompatBranch'"
                Add-Check -Name 'plan_source_workspace_report_ritsulib_variant_hash_matches_expected' -Passed ([string]::Equals([string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantDllSha256' -DefaultValue ''), [string](Get-JsonValue -Object $reportRitsuLib -Name 'ExpectedVariantDllSha256' -DefaultValue ''), [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'source-workspace report selected RitsuLib variant DLL hash must match variants metadata'
            }
            if ($null -ne $sourceWorkspace) {
                $summaryMatchesReport =
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Passed' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $sourceReport -Name 'Passed' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Version' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceCommit' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Commit' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Branch' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceMainAssemblyHash' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'MainAssemblyHash' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Version' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameCommit' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Commit' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Branch' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameMainAssemblyHash' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'MainAssemblyHash' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Disposition' -DefaultValue '')) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'MatchesInstalledGame' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'OriginPckPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginPckPath' -DefaultValue '')) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'Version' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatBranch' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibManifestPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ManifestPath' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibManifestSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ManifestSha256' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantsPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantsPath' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantsSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantsSha256' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantDllPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantDllPath' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantDllSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantDllSha256' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibExpectedVariantDllSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ExpectedVariantDllSha256' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatTargetPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatTargetPath' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatTargetText' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatTargetText' -DefaultValue '')) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true) -eq [bool](Get-JsonValue -Object $policy -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true)) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $policy -Name 'NotRuntimeProof' -DefaultValue $false)) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $policy -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'ReportSha256' -DefaultValue '') -eq $sourceWorkspaceCheckSha256)
                Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $summaryMatchesReport -Detail 'SourceWorkspace summary must match the retained source-workspace report and ReportSha256'
            }
        }
    } elseif ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $false -Detail 'SourceWorkspace summary cannot be trusted without a valid retained source-workspace report'
    }
}

$summaryRuns = @()
$expectedAncientIdTokens = @(Get-DelimitedStringTokens -Value $ExpectedAncientIds)
$expectedAncientIdsForCoverage = @(Get-NormalizedNonEmptyStringTokens -Value $ExpectedAncientIds)
$duplicateExpectedAncientIds = @($expectedAncientIdsForCoverage | Group-Object | Where-Object { $_.Count -gt 1 })
$planExpectedAncientIdsForCoverage = @()
$missingPlanExpectedAncientIds = @()
$unexpectedPlanExpectedAncientIds = @()
$observedAncientIds = @()
$missingExpectedAncientIds = @()
Add-Check -Name 'expected_ancient_ids_required_for_proof_mode' -Passed (-not $FailOnMismatch -or $expectedAncientIdsForCoverage.Count -gt 0) -Detail 'AutoSlay proof packets must be checked with -ExpectedAncientIds when -FailOnMismatch is used'
if ($expectedAncientIdTokens.Count -gt 0) {
    Add-Check -Name 'expected_ancient_ids_all_non_empty' -Passed ($expectedAncientIdsForCoverage.Count -eq $expectedAncientIdTokens.Count) -Detail 'ExpectedAncientIds entries must be non-empty when supplied'
    Add-Check -Name 'expected_ancient_ids_unique' -Passed ($duplicateExpectedAncientIds.Count -eq 0) -Detail "ExpectedAncientIds must be unique; duplicate groups=$($duplicateExpectedAncientIds.Count)"

    if ($null -ne $plan) {
        $planExpectedAncientIdTokens = @(Get-DelimitedStringTokens -Value (Get-JsonValue -Object $plan -Name 'ExpectedAncientIds' -DefaultValue @()))
        $planExpectedAncientIdsForCoverage = @(Get-NormalizedNonEmptyStringTokens -Value (Get-JsonValue -Object $plan -Name 'ExpectedAncientIds' -DefaultValue @()))
        $duplicatePlanExpectedAncientIds = @($planExpectedAncientIdsForCoverage | Group-Object | Where-Object { $_.Count -gt 1 })
        $planExpectedAncientIdSet = New-OrdinalStringSet -Values $planExpectedAncientIdsForCoverage
        $expectedAncientIdSet = New-OrdinalStringSet -Values $expectedAncientIdsForCoverage
        $missingPlanExpectedAncientIds = @($expectedAncientIdsForCoverage | Where-Object { -not $planExpectedAncientIdSet.Contains($_) })
        $unexpectedPlanExpectedAncientIds = @($planExpectedAncientIdsForCoverage | Where-Object { -not $expectedAncientIdSet.Contains($_) })

        Add-Check -Name 'plan_expected_ancient_ids_present' -Passed ($planExpectedAncientIdTokens.Count -gt 0) -Detail 'autoslay-plan.json must retain ExpectedAncientIds when target coverage is requested'
        Add-Check -Name 'plan_expected_ancient_ids_all_non_empty' -Passed ($planExpectedAncientIdsForCoverage.Count -eq $planExpectedAncientIdTokens.Count) -Detail 'autoslay-plan.json ExpectedAncientIds entries must be non-empty when supplied'
        Add-Check -Name 'plan_expected_ancient_ids_unique' -Passed ($duplicatePlanExpectedAncientIds.Count -eq 0) -Detail "autoslay-plan.json ExpectedAncientIds must be unique; duplicate groups=$($duplicatePlanExpectedAncientIds.Count)"
        Add-Check -Name 'plan_expected_ancient_ids_match_parameter' -Passed ($missingPlanExpectedAncientIds.Count -eq 0 -and $unexpectedPlanExpectedAncientIds.Count -eq 0) -Detail "autoslay-plan.json ExpectedAncientIds must match -ExpectedAncientIds; missing=$($missingPlanExpectedAncientIds -join ',') unexpected=$($unexpectedPlanExpectedAncientIds -join ',')"
    }
}
if ($null -ne $summary) {
    $runnerKind = [string](Get-JsonValue -Object $summary -Name 'RunnerKind' -DefaultValue '')
    $summaryRuns = @(Get-ArrayValues -Value (Get-JsonValue -Object $summary -Name 'Runs' -DefaultValue @()))
    $totalRuns = [int](Get-JsonValue -Object $summary -Name 'TotalRuns' -DefaultValue -1)
    $failedRuns = [int](Get-JsonValue -Object $summary -Name 'FailedRuns' -DefaultValue -1)

    Add-Check -Name 'summary_runner_kind_is_game_native_autoslay' -Passed ([string]::Equals($runnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)) -Detail "RunnerKind must be GameNativeAutoSlay; found '$runnerKind'"
    Add-Check -Name 'summary_passed' -Passed ([bool](Get-JsonValue -Object $summary -Name 'Passed' -DefaultValue $false)) -Detail 'autoslay-summary.json Passed must be true for proof packets'
    Add-Check -Name 'summary_total_runs_matches_runs_array' -Passed ($totalRuns -eq $summaryRuns.Count) -Detail "TotalRuns must match Runs array count; TotalRuns=$totalRuns Runs=$($summaryRuns.Count)"
    Add-Check -Name 'summary_total_runs_meets_minimum' -Passed ($totalRuns -ge $MinRuns) -Detail "TotalRuns must be at least $MinRuns; found $totalRuns"
    Add-Check -Name 'summary_failed_runs_zero' -Passed ($failedRuns -eq 0) -Detail "FailedRuns must be 0 for proof packets; found $failedRuns"
    Add-Check -Name 'summary_run_count_matches_plan_seed_count' -Passed ($summaryRuns.Count -eq $planSeeds.Count) -Detail "Runs array count must match plan Seeds count; runs=$($summaryRuns.Count) seeds=$($planSeeds.Count)"
    $summaryRunSeeds = @($summaryRuns | ForEach-Object { [string](Get-JsonValue -Object $_ -Name 'Seed' -DefaultValue '') })
    $nonEmptySummaryRunSeeds = @($summaryRunSeeds | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $duplicateSummaryRunSeeds = @($nonEmptySummaryRunSeeds | Group-Object | Where-Object { $_.Count -gt 1 })
    $sortedPlanSeeds = @($planSeeds | Sort-Object)
    $sortedRunSeeds = @($summaryRunSeeds | Sort-Object)
    Add-Check -Name 'summary_run_seeds_present_for_all_runs' -Passed ($nonEmptySummaryRunSeeds.Count -eq $summaryRuns.Count) -Detail 'every summary run must retain a non-empty Seed'
    Add-Check -Name 'summary_run_seeds_unique' -Passed ($duplicateSummaryRunSeeds.Count -eq 0) -Detail "summary run Seeds must be unique; duplicate seed groups=$($duplicateSummaryRunSeeds.Count)"
    Add-Check -Name 'summary_run_seeds_match_plan_seeds' -Passed ([string]::Equals([string]::Join("`n", $sortedPlanSeeds), [string]::Join("`n", $sortedRunSeeds), [System.StringComparison]::Ordinal)) -Detail 'summary run Seeds must exactly match autoslay-plan.json Seeds'

    $observedAncientIdSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($run in $summaryRuns) {
        $runAncientId = [string](Get-JsonValue -Object $run -Name 'AncientId' -DefaultValue '')
        if (-not [string]::IsNullOrWhiteSpace($runAncientId)) {
            $observedAncientIdSet.Add($runAncientId) | Out-Null
        }
    }
    $observedAncientIds = @($observedAncientIdSet | Sort-Object)
    if ($expectedAncientIdsForCoverage.Count -gt 0) {
        $missingExpectedAncientIds = @($expectedAncientIdsForCoverage | Where-Object { -not $observedAncientIdSet.Contains($_) })
        Add-Check -Name 'summary_expected_ancient_ids_observed' -Passed ($missingExpectedAncientIds.Count -eq 0) -Detail "ExpectedAncientIds missing=$($missingExpectedAncientIds -join ',') observed=$($observedAncientIds -join ',')"
    }
}

$eventTraversalObserved = $false
for ($i = 0; $i -lt $summaryRuns.Count; $i++) {
    $run = $summaryRuns[$i]
    $runName = "run_{0:D4}" -f ($i + 1)
    $expectedRunEvidenceDirName = "run-{0:D4}" -f ($i + 1)
    $seed = [string](Get-JsonValue -Object $run -Name 'Seed' -DefaultValue '')
    $summaryRunPassed = [bool](Get-JsonValue -Object $run -Name 'Passed' -DefaultValue $false)
    $exitCode = [int](Get-JsonValue -Object $run -Name 'ExitCode' -DefaultValue -999)
    $summaryFailureReasonCodes = @(Get-ArrayValues -Value (Get-JsonValue -Object $run -Name 'FailureReasonCodes' -DefaultValue @()))
    $summaryHangSignals = @(Get-ArrayValues -Value (Get-JsonValue -Object $run -Name 'HangSignals' -DefaultValue @()))
    $eventKind = [string](Get-JsonValue -Object $run -Name 'EventKind' -DefaultValue '')
    $ancientId = [string](Get-JsonValue -Object $run -Name 'AncientId' -DefaultValue '')
    $runResultPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'RunResultPath' -DefaultValue ''))
    $runEvidenceDir = if (-not [string]::IsNullOrWhiteSpace($runResultPath)) { [System.IO.Path]::GetDirectoryName($runResultPath) } else { '' }
    $runEvidenceDirValid = -not [string]::IsNullOrWhiteSpace($runEvidenceDir) -and (Test-PathInsideDirectory -Path $runEvidenceDir -Directory $resolvedEvidenceDir)
    $runtimeProbeSamplesPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'RuntimeProbeSamplesPath' -DefaultValue ''))
    $autoSlayLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'AutoSlayLogPath' -DefaultValue ''))
    $beforeLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'GodotLogBeforePath' -DefaultValue ''))
    $afterLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'GodotLogAfterLaunchPath' -DefaultValue ''))
    $currentLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'GodotLogCurrentIterationPath' -DefaultValue ''))
    $auditPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'GodotLogAuditPath' -DefaultValue ''))
    $sts1ModeCheckPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'Sts1ModeLogCheckPath' -DefaultValue ''))
    $autoSlayLogSha256 = [string](Get-JsonValue -Object $run -Name 'AutoSlayLogSha256' -DefaultValue '')
    $beforeLogLengthBytes = [long](Get-JsonValue -Object $run -Name 'GodotLogBeforeLengthBytes' -DefaultValue -1)
    $beforeLogSha256 = [string](Get-JsonValue -Object $run -Name 'GodotLogBeforeSha256' -DefaultValue '')
    $afterLogLengthBytes = [long](Get-JsonValue -Object $run -Name 'GodotLogAfterLaunchLengthBytes' -DefaultValue -1)
    $afterLogSha256 = [string](Get-JsonValue -Object $run -Name 'GodotLogAfterLaunchSha256' -DefaultValue '')
    $currentLogLengthBytes = [long](Get-JsonValue -Object $run -Name 'GodotLogCurrentIterationLengthBytes' -DefaultValue -1)
    $currentLogSha256 = [string](Get-JsonValue -Object $run -Name 'GodotLogCurrentIterationSha256' -DefaultValue '')
    $runResultExists = Test-PathLeafSafe -Path $runResultPath
    $runtimeProbeSamplesExists = Test-PathLeafSafe -Path $runtimeProbeSamplesPath
    $autoSlayLogExists = Test-PathLeafSafe -Path $autoSlayLogPath
    $beforeLogExists = Test-PathLeafSafe -Path $beforeLogPath
    $afterLogExists = Test-PathLeafSafe -Path $afterLogPath
    $currentLogExists = Test-PathLeafSafe -Path $currentLogPath
    $auditExists = Test-PathLeafSafe -Path $auditPath
    $sts1ModeCheckExists = Test-PathLeafSafe -Path $sts1ModeCheckPath
    $observedRuntimeProbeProcessIds = @()
    $observedRuntimeProbeStartTimes = @()
    $observedRuntimeProbePaths = @()
    $observedRuntimeProbeExpectedProcessIds = @()
    $observedRuntimeProbeExpectedStartTimes = @()
    $observedRuntimeProbeExpectedPaths = @()
    $identityMismatchRuntimeProbeSamples = @()
    $runtimeProbeRuntimeMaxLogLength = -1L

    Add-Check -Name "${runName}_seed_present" -Passed (-not [string]::IsNullOrWhiteSpace($seed)) -Detail 'each AutoSlay run must retain its seed'
    Add-Check -Name "${runName}_seed_listed_in_plan" -Passed ($planSeeds -contains $seed) -Detail "seed '$seed' must be listed in autoslay-plan.json Seeds"
    Add-Check -Name "${runName}_summary_run_passed_true" -Passed $summaryRunPassed -Detail 'each summary run must record Passed=true for proof packets'
    Add-Check -Name "${runName}_summary_run_failure_reason_codes_empty" -Passed ($summaryFailureReasonCodes.Count -eq 0) -Detail "summary run FailureReasonCodes must be empty; found $($summaryFailureReasonCodes.Count)"
    Add-Check -Name "${runName}_summary_run_hang_signals_empty" -Passed ($summaryHangSignals.Count -eq 0) -Detail "summary run HangSignals must be empty; found $($summaryHangSignals.Count)"
    Add-Check -Name "${runName}_exit_code_zero" -Passed ($exitCode -eq 0) -Detail "ExitCode must be 0 for proof packets; found $exitCode"
    Add-Check -Name "${runName}_event_kind_is_ancient" -Passed ([string]::Equals($eventKind, 'Ancient', [System.StringComparison]::Ordinal)) -Detail "EventKind must be Ancient for Ancient AutoSlay proof; found '$eventKind'"
    Add-Check -Name "${runName}_ancient_id_present" -Passed (-not [string]::IsNullOrWhiteSpace($ancientId)) -Detail 'AncientId must identify the Ancient dialogue/options traversed by this run'
    Add-Check -Name "${runName}_run_result_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($runResultPath)) -Detail 'RunResultPath must retain launch provenance for each run'
    Add-Check -Name "${runName}_run_result_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $runResultPath -Directory $resolvedEvidenceDir) -Detail 'RunResultPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_run_result_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($runResultPath)) -and [System.IO.Path]::GetFileName($runResultPath) -eq 'run-result.json') -Detail 'RunResultPath must end with run-result.json'
    Add-Check -Name "${runName}_run_result_parent_expected" -Passed ($runEvidenceDirValid -and [System.IO.Path]::GetFileName($runEvidenceDir) -eq $expectedRunEvidenceDirName) -Detail "RunResultPath must live under the expected per-seed directory '$expectedRunEvidenceDirName'"
    Add-Check -Name "${runName}_run_result_exists" -Passed $runResultExists -Detail 'RunResultPath must point at retained run-result.json'
    Add-Check -Name "${runName}_runtime_probe_samples_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($runtimeProbeSamplesPath)) -Detail 'RuntimeProbeSamplesPath must retain process/window/log timeline samples for hang triage'
    Add-Check -Name "${runName}_runtime_probe_samples_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $runtimeProbeSamplesPath -Directory $resolvedEvidenceDir) -Detail 'RuntimeProbeSamplesPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_runtime_probe_samples_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $runtimeProbeSamplesPath -Directory $runEvidenceDir)) -Detail 'RuntimeProbeSamplesPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_runtime_probe_samples_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($runtimeProbeSamplesPath)) -and [System.IO.Path]::GetFileName($runtimeProbeSamplesPath) -eq 'runtime-probe-samples.json') -Detail 'RuntimeProbeSamplesPath must end with runtime-probe-samples.json'
    Add-Check -Name "${runName}_runtime_probe_samples_exists" -Passed $runtimeProbeSamplesExists -Detail 'RuntimeProbeSamplesPath must point at retained runtime-probe-samples.json'
    Add-Check -Name "${runName}_autoslay_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($autoSlayLogPath)) -Detail 'AutoSlayLogPath must be retained'
    Add-Check -Name "${runName}_autoslay_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $autoSlayLogPath -Directory $resolvedEvidenceDir) -Detail 'AutoSlayLogPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_autoslay_log_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $autoSlayLogPath -Directory $runEvidenceDir)) -Detail 'AutoSlayLogPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_autoslay_log_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($autoSlayLogPath)) -and [System.IO.Path]::GetFileName($autoSlayLogPath) -eq 'autoslay.log') -Detail 'AutoSlayLogPath must end with autoslay.log'
    Add-Check -Name "${runName}_autoslay_log_exists" -Passed $autoSlayLogExists -Detail 'AutoSlay log file must exist'
    Add-Check -Name "${runName}_autoslay_log_hash_present" -Passed (-not [string]::IsNullOrWhiteSpace($autoSlayLogSha256)) -Detail 'AutoSlayLogSha256 must be retained for each run'
    Add-Check -Name "${runName}_before_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($beforeLogPath)) -Detail 'GodotLogBeforePath must retain the pre-launch shared godot.log snapshot'
    Add-Check -Name "${runName}_before_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $beforeLogPath -Directory $resolvedEvidenceDir) -Detail 'GodotLogBeforePath must stay inside the evidence directory'
    Add-Check -Name "${runName}_before_log_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $beforeLogPath -Directory $runEvidenceDir)) -Detail 'GodotLogBeforePath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_before_log_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($beforeLogPath)) -and [System.IO.Path]::GetFileName($beforeLogPath) -eq 'godot.log.before') -Detail 'GodotLogBeforePath must end with godot.log.before'
    Add-Check -Name "${runName}_before_log_exists" -Passed $beforeLogExists -Detail 'GodotLogBeforePath must point at a retained pre-launch log'
    Add-Check -Name "${runName}_before_log_length_recorded" -Passed ($beforeLogLengthBytes -ge 0) -Detail 'GodotLogBeforeLengthBytes must be retained and non-negative'
    Add-Check -Name "${runName}_before_log_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($beforeLogSha256)) -Detail 'GodotLogBeforeSha256 must be retained'
    Add-Check -Name "${runName}_after_launch_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($afterLogPath)) -Detail 'GodotLogAfterLaunchPath must retain the post-launch shared godot.log snapshot'
    Add-Check -Name "${runName}_after_launch_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $afterLogPath -Directory $resolvedEvidenceDir) -Detail 'GodotLogAfterLaunchPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_after_launch_log_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $afterLogPath -Directory $runEvidenceDir)) -Detail 'GodotLogAfterLaunchPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_after_launch_log_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($afterLogPath)) -and [System.IO.Path]::GetFileName($afterLogPath) -eq 'godot.log.after-launch') -Detail 'GodotLogAfterLaunchPath must end with godot.log.after-launch'
    Add-Check -Name "${runName}_after_launch_log_exists" -Passed $afterLogExists -Detail 'GodotLogAfterLaunchPath must point at a retained post-launch log'
    Add-Check -Name "${runName}_after_launch_log_length_recorded" -Passed ($afterLogLengthBytes -ge 0) -Detail 'GodotLogAfterLaunchLengthBytes must be retained and non-negative'
    Add-Check -Name "${runName}_after_launch_log_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($afterLogSha256)) -Detail 'GodotLogAfterLaunchSha256 must be retained'
    Add-Check -Name "${runName}_current_iteration_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($currentLogPath)) -Detail 'GodotLogCurrentIterationPath must be retained'
    Add-Check -Name "${runName}_current_iteration_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $currentLogPath -Directory $resolvedEvidenceDir) -Detail 'GodotLogCurrentIterationPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_current_iteration_log_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $currentLogPath -Directory $runEvidenceDir)) -Detail 'GodotLogCurrentIterationPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_current_iteration_log_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($currentLogPath)) -and [System.IO.Path]::GetFileName($currentLogPath) -eq 'godot.log.current-iteration') -Detail 'GodotLogCurrentIterationPath must end with godot.log.current-iteration'
    Add-Check -Name "${runName}_current_iteration_log_exists" -Passed $currentLogExists -Detail 'GodotLogCurrentIterationPath must point at a retained current-iteration log'
    Add-Check -Name "${runName}_current_iteration_log_length_recorded" -Passed ($currentLogLengthBytes -ge 0) -Detail 'GodotLogCurrentIterationLengthBytes must be retained and non-negative'
    Add-Check -Name "${runName}_current_iteration_log_hash_present" -Passed (-not [string]::IsNullOrWhiteSpace($currentLogSha256)) -Detail 'GodotLogCurrentIterationSha256 must be retained for each run'
    Add-Check -Name "${runName}_audit_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($auditPath)) -Detail 'GodotLogAuditPath must retain the audit of godot.log.current-iteration'
    Add-Check -Name "${runName}_audit_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $auditPath -Directory $resolvedEvidenceDir) -Detail 'GodotLogAuditPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_audit_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $auditPath -Directory $runEvidenceDir)) -Detail 'GodotLogAuditPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_audit_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($auditPath)) -and [System.IO.Path]::GetFileName($auditPath) -eq 'godot-log-audit.json') -Detail 'GodotLogAuditPath must end with godot-log-audit.json'
    Add-Check -Name "${runName}_audit_exists" -Passed $auditExists -Detail 'GodotLogAuditPath must point at retained godot-log-audit.json'
    Add-Check -Name "${runName}_sts1_mode_check_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($sts1ModeCheckPath)) -Detail 'Sts1ModeLogCheckPath must retain the StS1 mode verifier report'
    Add-Check -Name "${runName}_sts1_mode_check_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $sts1ModeCheckPath -Directory $resolvedEvidenceDir) -Detail 'Sts1ModeLogCheckPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_sts1_mode_check_under_run_dir" -Passed ($runEvidenceDirValid -and (Test-PathInsideDirectory -Path $sts1ModeCheckPath -Directory $runEvidenceDir)) -Detail 'Sts1ModeLogCheckPath must stay inside the per-seed run evidence directory'
    Add-Check -Name "${runName}_sts1_mode_check_leaf_expected" -Passed ((-not [string]::IsNullOrWhiteSpace($sts1ModeCheckPath)) -and [System.IO.Path]::GetFileName($sts1ModeCheckPath) -eq 'sts1-mode-log-check.json') -Detail 'Sts1ModeLogCheckPath must end with sts1-mode-log-check.json'
    Add-Check -Name "${runName}_sts1_mode_check_exists" -Passed $sts1ModeCheckExists -Detail 'Sts1ModeLogCheckPath must point at retained sts1-mode-log-check.json'

    $autoSlayLog = ''
    $currentLog = ''
    if ($autoSlayLogExists) {
        $autoSlayLog = [System.IO.File]::ReadAllText($autoSlayLogPath)
    }
    if ($currentLogExists) {
        $currentLog = [System.IO.File]::ReadAllText($currentLogPath)
    }

    if (-not [string]::IsNullOrWhiteSpace($autoSlayLogSha256) -and $autoSlayLogExists) {
        Add-Check -Name "${runName}_autoslay_log_hash_matches" -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $autoSlayLogPath), $autoSlayLogSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'AutoSlayLogSha256 must match AutoSlayLogPath'
    }
    if ($beforeLogExists) {
        Add-Check -Name "${runName}_before_log_length_matches" -Passed ($beforeLogLengthBytes -eq [long](Get-Item -LiteralPath $beforeLogPath).Length) -Detail 'GodotLogBeforeLengthBytes must match retained godot.log.before bytes'
        Add-Check -Name "${runName}_before_log_sha256_matches" -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $beforeLogPath), $beforeLogSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'GodotLogBeforeSha256 must match GodotLogBeforePath'
    }
    if ($afterLogExists) {
        Add-Check -Name "${runName}_after_launch_log_length_matches" -Passed ($afterLogLengthBytes -eq [long](Get-Item -LiteralPath $afterLogPath).Length) -Detail 'GodotLogAfterLaunchLengthBytes must match retained godot.log.after-launch bytes'
        Add-Check -Name "${runName}_after_launch_log_sha256_matches" -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $afterLogPath), $afterLogSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'GodotLogAfterLaunchSha256 must match GodotLogAfterLaunchPath'
    }
    if ($currentLogExists) {
        Add-Check -Name "${runName}_current_iteration_log_length_matches" -Passed ($currentLogLengthBytes -eq [long](Get-Item -LiteralPath $currentLogPath).Length) -Detail 'GodotLogCurrentIterationLengthBytes must match retained godot.log.current-iteration bytes'
    }
    if (-not [string]::IsNullOrWhiteSpace($currentLogSha256) -and $currentLogExists) {
        Add-Check -Name "${runName}_current_iteration_log_hash_matches" -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $currentLogPath), $currentLogSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'GodotLogCurrentIterationSha256 must match GodotLogCurrentIterationPath'
    }

    if ($runtimeProbeSamplesExists) {
        try {
            $probeSamplesJson = [System.IO.File]::ReadAllText($runtimeProbeSamplesPath)
            $probeSamplesParsed = $probeSamplesJson | ConvertFrom-Json
            $probeSamples = @($probeSamplesParsed)
            Add-Check -Name "${runName}_runtime_probe_samples_json_valid" -Passed $true -Detail 'runtime-probe-samples.json parsed'
            Add-Check -Name "${runName}_runtime_probe_samples_non_empty" -Passed ($probeSamples.Count -gt 0) -Detail 'runtime-probe-samples.json must contain process/window/log samples'
            Add-Check -Name "${runName}_runtime_probe_samples_phase_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Phase') -Detail 'every probe sample must retain Phase'
            Add-Check -Name "${runName}_runtime_probe_samples_sampled_at_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'SampledAt') -Detail 'every probe sample must retain SampledAt'
            Add-Check -Name "${runName}_runtime_probe_samples_log_exists_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'LogExists') -Detail 'every probe sample must retain LogExists'
            Add-Check -Name "${runName}_runtime_probe_samples_log_length_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'LogLengthBytes') -Detail 'every probe sample must retain LogLengthBytes'
            Add-Check -Name "${runName}_runtime_probe_samples_log_last_write_time_field_retained" -Passed (Test-AllJsonPropertiesRetained -Items $probeSamples -Name 'LogLastWriteTimeUtc') -Detail 'every probe sample must retain LogLastWriteTimeUtc, even when the log is absent'
            Add-Check -Name "${runName}_runtime_probe_samples_main_menu_phase_observed" -Passed (Test-AnyJsonPropertyStringEquals -Items $probeSamples -Name 'Phase' -Value 'main-menu') -Detail 'runtime-probe-samples.json must include at least one main-menu phase sample'
            Add-Check -Name "${runName}_runtime_probe_samples_runtime_phase_observed" -Passed (Test-AnyJsonPropertyStringEquals -Items $probeSamples -Name 'Phase' -Value 'runtime') -Detail 'runtime-probe-samples.json must include at least one runtime phase sample'
            Add-Check -Name "${runName}_runtime_probe_samples_process_id_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessId') -Detail 'every probe sample must retain ProcessId'
            Add-Check -Name "${runName}_runtime_probe_samples_process_start_time_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessStartTimeUtc') -Detail 'every probe sample must retain ProcessStartTimeUtc'
            Add-Check -Name "${runName}_runtime_probe_samples_process_path_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessPath') -Detail 'every probe sample must retain ProcessPath'
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_id_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessId') -Detail 'every probe sample must retain ExpectedGameProcessId'
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_start_time_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessStartTimeUtc') -Detail 'every probe sample must retain ExpectedGameProcessStartTimeUtc'
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_path_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessPath') -Detail 'every probe sample must retain ExpectedGameProcessPath'
            Add-Check -Name "${runName}_runtime_probe_samples_process_id_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessIdMatchesExpected') -Detail 'every probe sample must retain ProcessIdMatchesExpected'
            Add-Check -Name "${runName}_runtime_probe_samples_process_start_time_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessStartTimeMatchesExpected') -Detail 'every probe sample must retain ProcessStartTimeMatchesExpected'
            Add-Check -Name "${runName}_runtime_probe_samples_process_path_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessPathMatchesExpected') -Detail 'every probe sample must retain ProcessPathMatchesExpected'
            Add-Check -Name "${runName}_runtime_probe_samples_process_identity_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessIdentityMatchesExpected') -Detail 'every probe sample must retain ProcessIdentityMatchesExpected'
            Add-Check -Name "${runName}_runtime_probe_samples_process_observed_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessObserved') -Detail 'every probe sample must retain ProcessObserved'
            Add-Check -Name "${runName}_runtime_probe_samples_main_window_observed_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'MainWindowObserved') -Detail 'every probe sample must retain MainWindowObserved'
            Add-Check -Name "${runName}_runtime_probe_samples_hung_window_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'HungWindow') -Detail 'every probe sample must retain HungWindow'
            Add-Check -Name "${runName}_runtime_probe_samples_responding_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Responding') -Detail 'every probe sample must retain Responding'
            Add-Check -Name "${runName}_runtime_probe_samples_stale_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'StaleProcessCount') -Detail 'every probe sample must retain StaleProcessCount'
            Add-Check -Name "${runName}_runtime_probe_samples_current_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'CurrentProcessCount') -Detail 'every probe sample must retain CurrentProcessCount'
            Add-Check -Name "${runName}_runtime_probe_samples_unknown_start_time_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'UnknownStartTimeProcessCount') -Detail 'every probe sample must retain UnknownStartTimeProcessCount'
            Add-Check -Name "${runName}_runtime_probe_samples_ambiguous_current_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'AmbiguousCurrentProcessCount') -Detail 'every probe sample must retain AmbiguousCurrentProcessCount'
            Add-Check -Name "${runName}_runtime_probe_samples_process_observed" -Passed (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'ProcessObserved') -Detail 'at least one probe sample must observe SlayTheSpire2'
            Add-Check -Name "${runName}_runtime_probe_samples_main_window_observed" -Passed (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'MainWindowObserved') -Detail 'at least one probe sample must observe the main game window'
            Add-Check -Name "${runName}_runtime_probe_samples_no_hung_window" -Passed (Test-NoJsonPropertyTrue -Items $probeSamples -Name 'HungWindow') -Detail 'probe samples must not report hung windows'
            Add-Check -Name "${runName}_runtime_probe_samples_no_not_responding" -Passed (Test-NoJsonPropertyFalse -Items $probeSamples -Name 'Responding') -Detail 'probe samples must not report Responding=false'
            $staleProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
            Add-Check -Name "${runName}_runtime_probe_samples_no_stale_processes" -Passed ($staleProcessSamples.Count -eq 0) -Detail 'probe samples must record StaleProcessCount=0 so shared godot.log evidence cannot come from a pre-existing process'
            $unknownStartTimeSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
            Add-Check -Name "${runName}_runtime_probe_samples_no_unknown_start_times" -Passed ($unknownStartTimeSamples.Count -eq 0) -Detail 'probe samples must record UnknownStartTimeProcessCount=0 so unreadable process StartTime cannot be treated as current evidence'
            $ambiguousCurrentProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
            Add-Check -Name "${runName}_runtime_probe_samples_no_ambiguous_current_processes" -Passed ($ambiguousCurrentProcessSamples.Count -eq 0) -Detail 'probe samples must record AmbiguousCurrentProcessCount=0 so evidence is bound to one launched process'
            $currentProcessCountSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
            Add-Check -Name "${runName}_runtime_probe_samples_single_current_process" -Passed ($currentProcessCountSamples.Count -eq 0) -Detail 'probe samples must record CurrentProcessCount=1 for the launched SlayTheSpire2 process'
            $observedRuntimeProbeProcessIds = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ProcessId' -DefaultValue 0) } |
                Where-Object { $_ -gt 0 } |
                Sort-Object -Unique)
            Add-Check -Name "${runName}_runtime_probe_samples_single_positive_process_id" -Passed ($observedRuntimeProbeProcessIds.Count -eq 1) -Detail "observed probe samples must bind to one positive process id; count=$($observedRuntimeProbeProcessIds.Count)"
            $observedRuntimeProbeStartTimes = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object {
                    $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue ''))
                    if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                } |
                Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                Sort-Object -Unique)
            $observedRuntimeProbePaths = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                Sort-Object -Unique)
            $observedRuntimeProbeExpectedProcessIds = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0) } |
                Where-Object { $_ -gt 0 } |
                Sort-Object -Unique)
            $observedRuntimeProbeExpectedStartTimes = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object {
                    $parse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue ''))
                    if ([bool]$parse.Parsed) { $parse.Value.ToString('o') }
                } |
                Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                Sort-Object -Unique)
            $observedRuntimeProbeExpectedPaths = @($probeSamples |
                Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } |
                Sort-Object -Unique)
            $identityMismatchRuntimeProbeSamples = @($probeSamples | Where-Object {
                [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                (-not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                    -not [bool](Get-JsonValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                    -not [bool](Get-JsonValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                    -not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
            })
            Add-Check -Name "${runName}_runtime_probe_samples_single_process_start_time" -Passed ($observedRuntimeProbeStartTimes.Count -eq 1) -Detail "observed probe samples must bind to one process start time; count=$($observedRuntimeProbeStartTimes.Count) values=$($observedRuntimeProbeStartTimes -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_single_process_path" -Passed ($observedRuntimeProbePaths.Count -eq 1) -Detail "observed probe samples must bind to one process path; count=$($observedRuntimeProbePaths.Count) values=$($observedRuntimeProbePaths -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_single_expected_process_id" -Passed ($observedRuntimeProbeExpectedProcessIds.Count -eq 1) -Detail "observed probe samples must bind to one expected process id; count=$($observedRuntimeProbeExpectedProcessIds.Count) values=$($observedRuntimeProbeExpectedProcessIds -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_single_expected_process_start_time" -Passed ($observedRuntimeProbeExpectedStartTimes.Count -eq 1) -Detail "observed probe samples must bind to one expected process start time; count=$($observedRuntimeProbeExpectedStartTimes.Count) values=$($observedRuntimeProbeExpectedStartTimes -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_single_expected_process_path" -Passed ($observedRuntimeProbeExpectedPaths.Count -eq 1) -Detail "observed probe samples must bind to one expected process path; count=$($observedRuntimeProbeExpectedPaths.Count) values=$($observedRuntimeProbeExpectedPaths -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_all_match_expected_identity" -Passed ($identityMismatchRuntimeProbeSamples.Count -eq 0) -Detail 'observed probe samples must report ProcessIdMatchesExpected, ProcessStartTimeMatchesExpected, ProcessPathMatchesExpected, and ProcessIdentityMatchesExpected as true'
            $runtimeProbeLogLengths = @($probeSamples |
                Where-Object {
                    [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'runtime', [System.StringComparison]::Ordinal) -and
                    [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                } |
                ForEach-Object { [long](Get-JsonValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1) } |
                Where-Object { $_ -ge 0 })
            $runtimeProbeRuntimeMaxLogLength = if ($runtimeProbeLogLengths.Count -gt 0) {
                [long](@($runtimeProbeLogLengths | Sort-Object -Descending)[0])
            } else {
                -1L
            }
        } catch {
            Add-Check -Name "${runName}_runtime_probe_samples_json_valid" -Passed $false -Detail "invalid probe samples JSON in $runtimeProbeSamplesPath`: $($_.Exception.Message)"
        }
    }

    if ($beforeLogExists -and $afterLogExists -and $currentLogExists) {
        $sliceBinding = Test-CurrentSliceBinding -BeforePath $beforeLogPath -AfterPath $afterLogPath -CurrentPath $currentLogPath
        Add-Check -Name "${runName}_current_iteration_log_matches_after_launch_prefix" -Passed ([bool]$sliceBinding.PrefixMatches) -Detail $sliceBinding.Detail
        Add-Check -Name "${runName}_current_iteration_log_matches_after_launch_slice" -Passed ([bool]$sliceBinding.SliceMatches) -Detail $sliceBinding.Detail
    } else {
        Add-Check -Name "${runName}_current_iteration_log_matches_after_launch_prefix" -Passed $false -Detail 'requires retained before, after-launch, and current-iteration logs'
        Add-Check -Name "${runName}_current_iteration_log_matches_after_launch_slice" -Passed $false -Detail 'requires retained before, after-launch, and current-iteration logs'
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
        Add-Check -Name "${runName}_expected_package_version_in_current_log" -Passed (Contains-Text -Text $currentLog -Needle $ExpectedPackageVersion) -Detail "current-iteration log must contain package version $ExpectedPackageVersion"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
        $expectedGameMarker = if ($ExpectedGameVersion.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) { "release = $ExpectedGameVersion" } else { "release = v$ExpectedGameVersion" }
        Add-Check -Name "${runName}_expected_game_version_in_current_log" -Passed (Contains-Text -Text $currentLog -Needle $expectedGameMarker) -Detail "current-iteration log must contain game marker '$expectedGameMarker'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion) -and -not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
        $expectedRitsuMarker = "RitsuLib Version: $ExpectedRitsuLibVersion [compat branch: $ExpectedRitsuCompatBranch]"
        Add-Check -Name "${runName}_expected_ritsulib_marker_in_current_log" -Passed (Contains-Text -Text $currentLog -Needle $expectedRitsuMarker) -Detail "current-iteration log must contain RitsuLib marker '$expectedRitsuMarker'"
    }
    if ($ExpectedPatchCount -gt 0) {
        Add-Check -Name "${runName}_expected_patch_count_in_current_log" -Passed ((Get-PatchCountLineHits -Text $currentLog -ExpectedCount $ExpectedPatchCount) -gt 0) -Detail "current-iteration log must contain exact Spire Plus patch-count markers for $ExpectedPatchCount patches"
    }

    Add-Check -Name "${runName}_autoslay_log_started_seed" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Starting run with seed=$seed")) -Detail 'AutoSlay log must contain the start marker for this seed'
    Add-Check -Name "${runName}_autoslay_log_completed_seed" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Run completed successfully with seed=$seed")) -Detail 'AutoSlay log must contain the completion marker for this seed'
    Add-Check -Name "${runName}_autoslay_log_no_failed_seed" -Passed (-not ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Run failed with seed=$seed"))) -Detail 'proof packets must not contain a RunFailed marker for this seed'
    Add-Check -Name "${runName}_current_log_contains_autoslay_start" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $currentLog -Needle "Starting run with seed=$seed")) -Detail 'current-iteration godot log must contain the AutoSlay start marker'
    if (-not [string]::IsNullOrWhiteSpace($ancientId)) {
        Add-Check -Name "${runName}_autoslay_log_contains_ancient_id" -Passed (Contains-Text -Text $autoSlayLog -Needle $ancientId) -Detail "AutoSlay sidecar log must contain AncientId '$ancientId'"
        Add-Check -Name "${runName}_current_log_contains_ancient_id" -Passed (Contains-Text -Text $currentLog -Needle $ancientId) -Detail "current-iteration godot log must contain AncientId '$ancientId'"
    }

    $eventSequence = @(
        "Starting run with seed=$seed",
        'Entering Event room',
        'Detected Ancient event, clicking through dialogue',
        'Selecting event option:',
        "Run completed successfully with seed=$seed"
    )
    $autoSlayEventSequenceObserved = (-not [string]::IsNullOrWhiteSpace($seed)) -and (Test-OrderedTextSequence -Text $autoSlayLog -Needles $eventSequence)
    $currentLogEventSequenceObserved = (-not [string]::IsNullOrWhiteSpace($seed)) -and (Test-OrderedTextSequence -Text $currentLog -Needles $eventSequence)
    $runEventTraversal = $autoSlayEventSequenceObserved -and $currentLogEventSequenceObserved
    $eventTraversalObserved = $eventTraversalObserved -or $runEventTraversal
    Add-Check -Name "${runName}_autoslay_log_event_sequence_observed" -Passed ($AllowMissingEventTraversal -or $autoSlayEventSequenceObserved) -Detail 'AutoSlay sidecar log must contain ordered start, Entering Event room, Detected Ancient event, Selecting event option, and completion markers'
    Add-Check -Name "${runName}_current_log_event_sequence_observed" -Passed ($AllowMissingEventTraversal -or $currentLogEventSequenceObserved) -Detail 'current-iteration godot log must contain ordered start, Entering Event room, Detected Ancient event, Selecting event option, and completion markers'
    Add-Check -Name "${runName}_event_room_traversal_observed" -Passed ($AllowMissingEventTraversal -or $runEventTraversal) -Detail 'AutoSlay event proof requires ordered event traversal in both sidecar and current-iteration godot logs'

    if ($auditExists) {
        try {
            $auditSummary = Read-AuditSummary -Path $auditPath
            $auditItemPaths = @($auditSummary.ItemPaths)
            $auditItemLengths = @($auditSummary.ItemLengths)
            $auditItemSha256s = @($auditSummary.ItemSha256s)
            $expectedAuditPath = ConvertTo-NormalizedPathOrEmpty -Path $currentLogPath
            $expectedAuditLength = if ($currentLogExists) { [long](Get-Item -LiteralPath $currentLogPath).Length } else { -1L }
            $expectedAuditSha256 = Get-FileSha256OrEmpty -Path $currentLogPath
            Add-Check -Name "${runName}_audit_clean" -Passed ([bool]$auditSummary.Clean) -Detail "audit must have zero dirty items and zero signature hits; dirty=$($auditSummary.DirtyItems), hits=$($auditSummary.SignatureHitCount)"
            Add-Check -Name "${runName}_audit_has_single_scanned_path" -Passed ($auditItemPaths.Count -eq 1) -Detail "audit JSON must retain exactly one scanned Path; found $($auditItemPaths.Count)"
            Add-Check -Name "${runName}_audit_path_matches_current_iteration_log" -Passed ($auditItemPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemPaths[0], $expectedAuditPath)) -Detail 'godot-log-audit.json must be produced from the retained godot.log.current-iteration slice'
            Add-Check -Name "${runName}_audit_has_single_length" -Passed ($auditItemLengths.Count -eq 1) -Detail "audit JSON must retain exactly one Length; found $($auditItemLengths.Count)"
            Add-Check -Name "${runName}_audit_length_matches_current_iteration_log" -Passed ($auditItemLengths.Count -eq 1 -and $auditItemLengths[0] -eq $expectedAuditLength) -Detail 'godot-log-audit.json Length must match the retained godot.log.current-iteration bytes'
            Add-Check -Name "${runName}_audit_has_single_sha256" -Passed ($auditItemSha256s.Count -eq 1) -Detail "audit JSON must retain exactly one Sha256; found $($auditItemSha256s.Count)"
            Add-Check -Name "${runName}_audit_sha256_matches_current_iteration_log" -Passed ($auditItemSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], $expectedAuditSha256)) -Detail 'godot-log-audit.json Sha256 must match the retained godot.log.current-iteration bytes'

            if (-not $currentLogExists) {
                Add-Check -Name "${runName}_audit_recomputed_from_current_iteration_log" -Passed $false -Detail 'cannot recompute audit because godot.log.current-iteration is missing'
            } elseif (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
                Add-Check -Name "${runName}_audit_recompute_script_exists" -Passed $false -Detail "missing audit script: $logAuditScript"
            } else {
                $recomputedAuditSummary = Invoke-RecomputedAuditSummary -LogPath $currentLogPath
                $recomputedPaths = @($recomputedAuditSummary.ItemPaths)
                $recomputedSha256s = @($recomputedAuditSummary.ItemSha256s)
                Add-Check -Name "${runName}_audit_recomputed_from_current_iteration_log" -Passed ($recomputedPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$recomputedPaths[0], $expectedAuditPath)) -Detail 'packet checker must recompute the audit from the retained current-iteration log'
                Add-Check -Name "${runName}_audit_recomputed_clean" -Passed ([bool]$recomputedAuditSummary.Clean) -Detail "recomputed audit must have zero dirty items and zero signature hits; dirty=$($recomputedAuditSummary.DirtyItems), hits=$($recomputedAuditSummary.SignatureHitCount)"
                Add-Check -Name "${runName}_audit_signature_counts_match_recomputed" -Passed ($auditSummary.DirtyItems -eq $recomputedAuditSummary.DirtyItems -and $auditSummary.SignatureHitCount -eq $recomputedAuditSummary.SignatureHitCount) -Detail "retained audit signature counts must match recomputed counts; retained dirty=$($auditSummary.DirtyItems), retained hits=$($auditSummary.SignatureHitCount), recomputed dirty=$($recomputedAuditSummary.DirtyItems), recomputed hits=$($recomputedAuditSummary.SignatureHitCount)"
                Add-Check -Name "${runName}_audit_sha256_matches_recomputed" -Passed ($auditItemSha256s.Count -eq 1 -and $recomputedSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], [string]$recomputedSha256s[0])) -Detail 'retained audit Sha256 must match the recomputed audit Sha256'
            }
        } catch {
            Add-Check -Name "${runName}_audit_json_valid" -Passed $false -Detail "invalid audit JSON in $auditPath`: $($_.Exception.Message)"
        }
    }

    if ($sts1ModeCheckExists) {
        $sts1ModeCheck = Read-JsonOrNull -Path $sts1ModeCheckPath -CheckName "${runName}_sts1_mode_log_check_json_valid"
        if ($null -ne $sts1ModeCheck) {
            Add-Check -Name "${runName}_sts1_mode_log_check_json_valid" -Passed $true -Detail 'sts1-mode-log-check.json parsed'
            $sts1Mode = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'Mode' -DefaultValue '')
            $sts1LogPath = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'LogPath' -DefaultValue '')
            $sts1LogLength = Get-JsonValue -Object $sts1ModeCheck -Name 'LogLength' -DefaultValue $null
            $sts1LogSha256 = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'LogSha256' -DefaultValue '')
            $sts1Mismatches = @(Get-ArrayValues -Value (Get-JsonValue -Object $sts1ModeCheck -Name 'Mismatches' -DefaultValue @()))
            $sts1FailedChecks = @(Get-ArrayValues -Value (Get-JsonValue -Object $sts1ModeCheck -Name 'Checks' -DefaultValue @()) | Where-Object {
                -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false)
            })
            $expectedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path $currentLogPath
            $expectedSts1LogLength = if ($currentLogExists) { [long](Get-Item -LiteralPath $currentLogPath).Length } else { -1L }
            $expectedSts1LogSha256 = Get-FileSha256OrEmpty -Path $currentLogPath
            Add-Check -Name "${runName}_sts1_mode_log_check_mismatches_empty" -Passed ($sts1Mismatches.Count -eq 0) -Detail "sts1-mode-log-check.json must have zero mismatches; found $($sts1Mismatches.Count)"
            Add-Check -Name "${runName}_sts1_mode_log_check_all_checks_passed" -Passed ($sts1FailedChecks.Count -eq 0) -Detail "sts1-mode-log-check.json contains $($sts1FailedChecks.Count) failed checks"
            Add-Check -Name "${runName}_sts1_mode_log_check_mode_matches_plan" -Passed (-not [string]::IsNullOrWhiteSpace($expectedSts1Mode) -and $sts1Mode -eq $expectedSts1Mode) -Detail "sts1-mode-log-check.json Mode must match autoslay-plan Sts1EventMode '$expectedSts1Mode'; found '$sts1Mode'"
            $normalizedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path $sts1LogPath
            Add-Check -Name "${runName}_sts1_mode_log_check_log_path_matches_current_iteration_log" -Passed (-not [string]::IsNullOrWhiteSpace($normalizedSts1LogPath) -and -not [string]::IsNullOrWhiteSpace($expectedSts1LogPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedSts1LogPath, $expectedSts1LogPath)) -Detail 'sts1-mode-log-check.json LogPath must match the retained godot.log.current-iteration slice'
            Add-Check -Name "${runName}_sts1_mode_log_check_log_length_matches_current_iteration_log" -Passed ($null -ne $sts1LogLength -and [long]$sts1LogLength -eq $expectedSts1LogLength) -Detail 'sts1-mode-log-check.json LogLength must match the retained godot.log.current-iteration bytes'
            Add-Check -Name "${runName}_sts1_mode_log_check_log_sha256_matches_current_iteration_log" -Passed (-not [string]::IsNullOrWhiteSpace($sts1LogSha256) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($sts1LogSha256, $expectedSts1LogSha256)) -Detail 'sts1-mode-log-check.json LogSha256 must match the retained godot.log.current-iteration bytes'
        }
    }

    if ($runResultExists) {
        $runResult = Read-JsonOrNull -Path $runResultPath -CheckName "${runName}_run_result_json_valid"
        if ($null -ne $runResult) {
            Add-Check -Name "${runName}_run_result_json_valid" -Passed $true -Detail 'run-result.json parsed'
            $resultAutoSlayLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'AutoSlayLogPath' -DefaultValue ''))
            $resultRuntimeProbeSamplesPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'RuntimeProbeSamplesPath' -DefaultValue ''))
            $resultBeforeLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'GodotLogBeforePath' -DefaultValue ''))
            $resultAfterLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'GodotLogAfterLaunchPath' -DefaultValue ''))
            $resultCurrentLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'GodotLogCurrentIterationPath' -DefaultValue ''))
            $resultBeforeLogLengthBytes = [long](Get-JsonValue -Object $runResult -Name 'GodotLogBeforeLengthBytes' -DefaultValue -1)
            $resultBeforeLogSha256 = [string](Get-JsonValue -Object $runResult -Name 'GodotLogBeforeSha256' -DefaultValue '')
            $resultAfterLogLengthBytes = [long](Get-JsonValue -Object $runResult -Name 'GodotLogAfterLaunchLengthBytes' -DefaultValue -1)
            $resultAfterLogSha256 = [string](Get-JsonValue -Object $runResult -Name 'GodotLogAfterLaunchSha256' -DefaultValue '')
            $resultCurrentLogLengthBytes = [long](Get-JsonValue -Object $runResult -Name 'GodotLogCurrentIterationLengthBytes' -DefaultValue -1)
            $resultCurrentLogSha256 = [string](Get-JsonValue -Object $runResult -Name 'GodotLogCurrentIterationSha256' -DefaultValue '')
            $resultAuditPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'GodotLogAuditPath' -DefaultValue ''))
            $resultSts1ModeCheckPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'Sts1ModeLogCheckPath' -DefaultValue ''))
            $resultInvocation = [string](Get-JsonValue -Object $runResult -Name 'Invocation' -DefaultValue '')
            $resultFailureReasonCodes = @(Get-ArrayValues -Value (Get-JsonValue -Object $runResult -Name 'FailureReasonCodes' -DefaultValue @()))
            $resultHangSignals = @(Get-ArrayValues -Value (Get-JsonValue -Object $runResult -Name 'HangSignals' -DefaultValue @()))
            $resultLauncherKind = [string](Get-JsonValue -Object $runResult -Name 'LauncherKind' -DefaultValue '')
            $resultLauncherPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $runResult -Name 'LauncherPath' -DefaultValue ''))
            $resultLauncherSha256 = [string](Get-JsonValue -Object $runResult -Name 'LauncherSha256' -DefaultValue '')
            $resultHookId = [string](Get-JsonValue -Object $runResult -Name 'HookId' -DefaultValue '')
            $resultHookAssembly = [string](Get-JsonValue -Object $runResult -Name 'HookAssembly' -DefaultValue '')
            $resultInvocationCommand = [string](Get-JsonValue -Object $runResult -Name 'InvocationCommand' -DefaultValue '')
            Add-Check -Name "${runName}_run_result_schema_version_one" -Passed ([int](Get-JsonValue -Object $runResult -Name 'SchemaVersion' -DefaultValue 0) -eq 1) -Detail 'run-result.json SchemaVersion must be 1'
            Add-Check -Name "${runName}_run_result_launch_true" -Passed ([bool](Get-JsonValue -Object $runResult -Name 'Launch' -DefaultValue $false)) -Detail 'run-result.json must record Launch=true'
            Add-Check -Name "${runName}_run_result_runner_kind_game_native_autoslay" -Passed ([string]::Equals([string](Get-JsonValue -Object $runResult -Name 'RunnerKind' -DefaultValue ''), 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)) -Detail 'run-result.json RunnerKind must be GameNativeAutoSlay'
            Add-Check -Name "${runName}_run_result_invocation_calls_autoslayer_start" -Passed (Contains-Text -Text $resultInvocation -Needle 'AutoSlayer.Start(seed, logFile)') -Detail 'run-result.json Invocation must record the launcher/mod hook that calls AutoSlayer.Start(seed, logFile)'
            Add-Check -Name "${runName}_run_result_launcher_kind_matches_plan" -Passed ([string]::Equals($resultLauncherKind, $launcherKind, [System.StringComparison]::Ordinal)) -Detail 'run-result.json LauncherKind must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_launcher_path_matches_plan" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultLauncherPath, $launcherPath)) -Detail 'run-result.json LauncherPath must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_launcher_sha256_matches_plan" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultLauncherSha256, $launcherSha256)) -Detail 'run-result.json LauncherSha256 must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_hook_id_matches_plan" -Passed ([string]::Equals($resultHookId, $hookId, [System.StringComparison]::Ordinal)) -Detail 'run-result.json HookId must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_hook_assembly_matches_plan" -Passed ([string]::Equals($resultHookAssembly, $hookAssembly, [System.StringComparison]::Ordinal)) -Detail 'run-result.json HookAssembly must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_invocation_command_matches_plan" -Passed ([string]::Equals($resultInvocationCommand, $invocationCommand, [System.StringComparison]::Ordinal)) -Detail 'run-result.json InvocationCommand must match autoslay-plan.json'
            Add-Check -Name "${runName}_run_result_invocation_command_calls_autoslayer_start" -Passed (Contains-Text -Text $resultInvocationCommand -Needle 'AutoSlayer.Start(seed, logFile)') -Detail 'run-result.json InvocationCommand must record the exact launcher/mod-hook command that calls AutoSlayer.Start(seed, logFile)'
            Add-Check -Name "${runName}_run_result_seed_matches_summary" -Passed ([string]::Equals([string](Get-JsonValue -Object $runResult -Name 'Seed' -DefaultValue ''), $seed, [System.StringComparison]::Ordinal)) -Detail 'run-result.json Seed must match autoslay-summary.json run Seed'
            Add-Check -Name "${runName}_run_result_event_kind_matches_summary" -Passed ([string]::Equals([string](Get-JsonValue -Object $runResult -Name 'EventKind' -DefaultValue ''), $eventKind, [System.StringComparison]::Ordinal)) -Detail 'run-result.json EventKind must match autoslay-summary.json run EventKind'
            Add-Check -Name "${runName}_run_result_ancient_id_matches_summary" -Passed ([string]::Equals([string](Get-JsonValue -Object $runResult -Name 'AncientId' -DefaultValue ''), $ancientId, [System.StringComparison]::Ordinal)) -Detail 'run-result.json AncientId must match autoslay-summary.json run AncientId'
            Add-Check -Name "${runName}_run_result_passed_true" -Passed ([bool](Get-JsonValue -Object $runResult -Name 'Passed' -DefaultValue $false)) -Detail 'run-result.json Passed must be true for proof packets'
            Add-Check -Name "${runName}_run_result_passed_matches_summary" -Passed ([bool](Get-JsonValue -Object $runResult -Name 'Passed' -DefaultValue $false) -eq $summaryRunPassed) -Detail 'run-result.json Passed must match autoslay-summary.json run Passed'
            Add-Check -Name "${runName}_run_result_failure_reason_codes_empty" -Passed ($resultFailureReasonCodes.Count -eq 0) -Detail "run-result.json FailureReasonCodes must be empty; found $($resultFailureReasonCodes.Count)"
            Add-Check -Name "${runName}_run_result_hang_signals_empty" -Passed ($resultHangSignals.Count -eq 0) -Detail "run-result.json HangSignals must be empty; found $($resultHangSignals.Count)"
            Add-Check -Name "${runName}_run_result_failure_reason_codes_match_summary" -Passed ([string]::Equals([string]::Join("`n", @($summaryFailureReasonCodes | ForEach-Object { [string]$_ })), [string]::Join("`n", @($resultFailureReasonCodes | ForEach-Object { [string]$_ })), [System.StringComparison]::Ordinal)) -Detail 'run-result.json FailureReasonCodes must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_hang_signals_match_summary" -Passed ([string]::Equals([string]::Join("`n", @($summaryHangSignals | ForEach-Object { [string]$_ })), [string]::Join("`n", @($resultHangSignals | ForEach-Object { [string]$_ })), [System.StringComparison]::Ordinal)) -Detail 'run-result.json HangSignals must match autoslay-summary.json'
            $resultProcessId = [int](Get-JsonValue -Object $runResult -Name 'ProcessId' -DefaultValue 0)
            $resultProcessStartTimeParse = ConvertTo-DateTimeOffsetParseResult -Text ([string](Get-JsonValue -Object $runResult -Name 'ProcessStartTimeUtc' -DefaultValue ''))
            $resultProcessStartTimeText = if ([bool]$resultProcessStartTimeParse.Parsed) { $resultProcessStartTimeParse.Value.ToString('o') } else { '' }
            $resultProcessPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $runResult -Name 'ProcessPath' -DefaultValue ''))
            Add-Check -Name "${runName}_run_result_process_id_positive" -Passed ($resultProcessId -gt 0) -Detail 'run-result.json must retain a positive launched process id'
            Add-Check -Name "${runName}_run_result_process_start_time_parseable" -Passed ([bool]$resultProcessStartTimeParse.Parsed) -Detail 'run-result.json must retain parseable ProcessStartTimeUtc for the launched game process'
            Add-Check -Name "${runName}_run_result_process_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($resultProcessPath)) -Detail 'run-result.json must retain ProcessPath for the launched game process'
            Add-Check -Name "${runName}_run_result_process_id_matches_runtime_probe_samples" -Passed ($observedRuntimeProbeProcessIds.Count -eq 1 -and $observedRuntimeProbeProcessIds[0] -eq $resultProcessId) -Detail "run-result.json ProcessId must match the single observed probe ProcessId; result=$resultProcessId observed=$($observedRuntimeProbeProcessIds -join ',')"
            Add-Check -Name "${runName}_run_result_process_start_time_matches_runtime_probe_samples" -Passed ($observedRuntimeProbeStartTimes.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultProcessStartTimeText) -and [string]::Equals($observedRuntimeProbeStartTimes[0], $resultProcessStartTimeText, [System.StringComparison]::Ordinal)) -Detail "run-result.json ProcessStartTimeUtc must match the single observed probe ProcessStartTimeUtc; result=$resultProcessStartTimeText observed=$($observedRuntimeProbeStartTimes -join ',')"
            Add-Check -Name "${runName}_run_result_process_path_matches_runtime_probe_samples" -Passed ($observedRuntimeProbePaths.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultProcessPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($observedRuntimeProbePaths[0], $resultProcessPath)) -Detail "run-result.json ProcessPath must match the single observed probe ProcessPath; result=$resultProcessPath observed=$($observedRuntimeProbePaths -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_id_matches_run_result" -Passed ($observedRuntimeProbeExpectedProcessIds.Count -eq 1 -and $observedRuntimeProbeExpectedProcessIds[0] -eq $resultProcessId) -Detail "runtime-probe-samples.json ExpectedGameProcessId must match run-result.json ProcessId; result=$resultProcessId observed=$($observedRuntimeProbeExpectedProcessIds -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_start_time_matches_run_result" -Passed ($observedRuntimeProbeExpectedStartTimes.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultProcessStartTimeText) -and [string]::Equals($observedRuntimeProbeExpectedStartTimes[0], $resultProcessStartTimeText, [System.StringComparison]::Ordinal)) -Detail "runtime-probe-samples.json ExpectedGameProcessStartTimeUtc must match run-result.json ProcessStartTimeUtc; result=$resultProcessStartTimeText observed=$($observedRuntimeProbeExpectedStartTimes -join ',')"
            Add-Check -Name "${runName}_runtime_probe_samples_expected_process_path_matches_run_result" -Passed ($observedRuntimeProbeExpectedPaths.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultProcessPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($observedRuntimeProbeExpectedPaths[0], $resultProcessPath)) -Detail "runtime-probe-samples.json ExpectedGameProcessPath must match run-result.json ProcessPath; result=$resultProcessPath observed=$($observedRuntimeProbeExpectedPaths -join ',')"
            $startTimestampText = [string](Get-JsonValue -Object $runResult -Name 'StartTimestamp' -DefaultValue '')
            $endTimestampText = [string](Get-JsonValue -Object $runResult -Name 'EndTimestamp' -DefaultValue '')
            $startTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $startTimestampText
            $endTimestampParse = ConvertTo-DateTimeOffsetParseResult -Text $endTimestampText
            Add-Check -Name "${runName}_run_result_start_timestamp_present" -Passed (-not [string]::IsNullOrWhiteSpace($startTimestampText)) -Detail 'run-result.json must retain StartTimestamp'
            Add-Check -Name "${runName}_run_result_end_timestamp_present" -Passed (-not [string]::IsNullOrWhiteSpace($endTimestampText)) -Detail 'run-result.json must retain EndTimestamp'
            Add-Check -Name "${runName}_run_result_start_timestamp_parseable" -Passed ([bool]$startTimestampParse.Parsed) -Detail "run-result.json StartTimestamp must parse as a timestamp; found '$startTimestampText'"
            Add-Check -Name "${runName}_run_result_end_timestamp_parseable" -Passed ([bool]$endTimestampParse.Parsed) -Detail "run-result.json EndTimestamp must parse as a timestamp; found '$endTimestampText'"
            Add-Check -Name "${runName}_run_result_timestamp_order_valid" -Passed ([bool]$startTimestampParse.Parsed -and [bool]$endTimestampParse.Parsed -and $startTimestampParse.Value -le $endTimestampParse.Value) -Detail "run-result.json StartTimestamp must be earlier than or equal to EndTimestamp; start='$startTimestampText' end='$endTimestampText'"
            Add-Check -Name "${runName}_run_result_exit_code_zero" -Passed ([int](Get-JsonValue -Object $runResult -Name 'ExitCode' -DefaultValue -999) -eq 0) -Detail 'run-result.json ExitCode must be 0'
            Add-Check -Name "${runName}_run_result_stale_process_count_zero" -Passed ([int](Get-JsonValue -Object $runResult -Name 'StaleProcessCount' -DefaultValue -1) -eq 0) -Detail 'run-result.json StaleProcessCount must be 0 so shared godot.log evidence is attributable'
            Add-Check -Name "${runName}_run_result_autoslay_log_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAutoSlayLogPath, $autoSlayLogPath)) -Detail 'run-result.json AutoSlayLogPath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_autoslay_log_hash_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals([string](Get-JsonValue -Object $runResult -Name 'AutoSlayLogSha256' -DefaultValue ''), $autoSlayLogSha256)) -Detail 'run-result.json AutoSlayLogSha256 must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_runtime_probe_samples_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultRuntimeProbeSamplesPath, $runtimeProbeSamplesPath)) -Detail 'run-result.json RuntimeProbeSamplesPath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_before_log_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultBeforeLogPath, $beforeLogPath)) -Detail 'run-result.json GodotLogBeforePath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_before_log_length_matches_summary" -Passed ($resultBeforeLogLengthBytes -eq $beforeLogLengthBytes) -Detail 'run-result.json GodotLogBeforeLengthBytes must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_before_log_sha256_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultBeforeLogSha256, $beforeLogSha256)) -Detail 'run-result.json GodotLogBeforeSha256 must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_after_launch_log_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAfterLogPath, $afterLogPath)) -Detail 'run-result.json GodotLogAfterLaunchPath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_after_launch_log_length_matches_summary" -Passed ($resultAfterLogLengthBytes -eq $afterLogLengthBytes) -Detail 'run-result.json GodotLogAfterLaunchLengthBytes must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_after_launch_log_sha256_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAfterLogSha256, $afterLogSha256)) -Detail 'run-result.json GodotLogAfterLaunchSha256 must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_current_iteration_log_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultCurrentLogPath, $currentLogPath)) -Detail 'run-result.json GodotLogCurrentIterationPath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_current_iteration_log_length_matches_summary" -Passed ($resultCurrentLogLengthBytes -eq $currentLogLengthBytes) -Detail 'run-result.json GodotLogCurrentIterationLengthBytes must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_current_iteration_log_hash_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultCurrentLogSha256, $currentLogSha256)) -Detail 'run-result.json GodotLogCurrentIterationSha256 must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_audit_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAuditPath, $auditPath)) -Detail 'run-result.json GodotLogAuditPath must match autoslay-summary.json'
            Add-Check -Name "${runName}_run_result_sts1_mode_check_path_matches_summary" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultSts1ModeCheckPath, $sts1ModeCheckPath)) -Detail 'run-result.json Sts1ModeLogCheckPath must match autoslay-summary.json'
            $mainMenuObservation = Get-JsonValue -Object $runResult -Name 'MainMenuObservation' -DefaultValue $null
            Add-Check -Name "${runName}_run_result_main_menu_observation_exists" -Passed ($null -ne $mainMenuObservation) -Detail 'run-result.json must retain MainMenuObservation telemetry'
            if ($null -ne $mainMenuObservation) {
                Add-Check -Name "${runName}_run_result_main_menu_observation_passed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'Passed' -DefaultValue $false)) -Detail 'MainMenuObservation.Passed must be true'
                Add-Check -Name "${runName}_run_result_main_menu_reached" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'MainMenuReached' -DefaultValue $false)) -Detail 'MainMenuObservation.MainMenuReached must be true'
                Add-Check -Name "${runName}_run_result_main_menu_process_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.ProcessObserved must be true'
                Add-Check -Name "${runName}_run_result_main_menu_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear before main menu'
                Add-Check -Name "${runName}_run_result_main_menu_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung before main menu'
                Add-Check -Name "${runName}_run_result_main_menu_no_stale_process" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'StaleProcessObserved' -DefaultValue $true)) -Detail 'main-menu observation must not see stale pre-existing SlayTheSpire2 processes'
                Add-Check -Name "${runName}_run_result_main_menu_stale_process_count_zero" -Passed ([int](Get-JsonValue -Object $mainMenuObservation -Name 'MaxStaleProcessCount' -DefaultValue -1) -eq 0) -Detail 'MainMenuObservation.MaxStaleProcessCount must be 0'
                Add-Check -Name "${runName}_run_result_main_menu_log_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.LogObserved must be true'
                Add-Check -Name "${runName}_run_result_main_menu_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'MainMenuObservation.NoLogGrowthTimeoutExceeded must be false'
            }

            $runtimeObservation = Get-JsonValue -Object $runResult -Name 'RuntimeObservation' -DefaultValue $null
            Add-Check -Name "${runName}_run_result_runtime_observation_exists" -Passed ($null -ne $runtimeObservation) -Detail 'run-result.json must retain RuntimeObservation telemetry'
            if ($null -ne $runtimeObservation) {
                Add-Check -Name "${runName}_run_result_runtime_observation_passed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'Passed' -DefaultValue $false)) -Detail 'RuntimeObservation.Passed must be true'
                Add-Check -Name "${runName}_run_result_runtime_process_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.ProcessObserved must be true'
                Add-Check -Name "${runName}_run_result_runtime_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear during runtime observation'
                Add-Check -Name "${runName}_run_result_runtime_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung during runtime observation'
                Add-Check -Name "${runName}_run_result_runtime_no_stale_process" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'StaleProcessObserved' -DefaultValue $true)) -Detail 'runtime observation must not see stale pre-existing SlayTheSpire2 processes'
                Add-Check -Name "${runName}_run_result_runtime_stale_process_count_zero" -Passed ([int](Get-JsonValue -Object $runtimeObservation -Name 'MaxStaleProcessCount' -DefaultValue -1) -eq 0) -Detail 'RuntimeObservation.MaxStaleProcessCount must be 0'
                Add-Check -Name "${runName}_run_result_runtime_log_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.LogObserved must be true'
                $runtimeObservationLogGrew = [bool](Get-JsonValue -Object $runtimeObservation -Name 'LogGrew' -DefaultValue $false)
                $runtimeObservationInitialLogLength = [long](Get-JsonValue -Object $runtimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1)
                $runtimeObservationFinalLogLength = [long](Get-JsonValue -Object $runtimeObservation -Name 'LogFinalLengthBytes' -DefaultValue -1)
                Add-Check -Name "${runName}_run_result_runtime_log_grew" -Passed $runtimeObservationLogGrew -Detail 'RuntimeObservation.LogGrew must be true so a retained static godot.log cannot satisfy runtime health'
                Add-Check -Name "${runName}_run_result_runtime_log_initial_length_present" -Passed ((Test-JsonProperty -Object $runtimeObservation -Name 'LogInitialLengthBytes') -and $runtimeObservationInitialLogLength -ge 0) -Detail 'RuntimeObservation.LogInitialLengthBytes must retain the pre-runtime log length'
                Add-Check -Name "${runName}_run_result_runtime_log_final_length_present" -Passed ((Test-JsonProperty -Object $runtimeObservation -Name 'LogFinalLengthBytes') -and $runtimeObservationFinalLogLength -ge 0) -Detail 'RuntimeObservation.LogFinalLengthBytes must retain the post-runtime log length'
                Add-Check -Name "${runName}_run_result_runtime_log_length_growth_matches_log_grew" -Passed ($runtimeObservationLogGrew -and $runtimeObservationInitialLogLength -ge 0 -and $runtimeObservationFinalLogLength -gt $runtimeObservationInitialLogLength) -Detail "RuntimeObservation.LogGrew=true must be backed by final log length growth; initial=$runtimeObservationInitialLogLength final=$runtimeObservationFinalLogLength"
                Add-Check -Name "${runName}_runtime_probe_samples_log_growth_matches_runtime_observation" -Passed ($runtimeObservationLogGrew -and $runtimeObservationInitialLogLength -ge 0 -and $runtimeProbeRuntimeMaxLogLength -gt $runtimeObservationInitialLogLength) -Detail "runtime-phase probe sample LogLengthBytes must exceed RuntimeObservation.LogInitialLengthBytes; initial=$runtimeObservationInitialLogLength maxRuntimeSample=$runtimeProbeRuntimeMaxLogLength"
                Add-Check -Name "${runName}_run_result_runtime_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'RuntimeObservation.NoLogGrowthTimeoutExceeded must be false'
            }
        }
    }
}

Add-Check -Name 'batch_event_room_traversal_observed' -Passed ($AllowMissingEventTraversal -or $eventTraversalObserved) -Detail 'at least one run must prove event-room traversal for event.md monkey proof'

$passed = $mismatches.Count -eq 0
foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"
foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if (-not [string]::IsNullOrWhiteSpace($OutFile)) {
    $report = [pscustomobject]@{
        Passed = $passed
        EvidenceDir = $resolvedEvidenceDir
        MinRuns = $MinRuns
        ExpectedAncientIds = $expectedAncientIdsForCoverage
        PlanExpectedAncientIds = $planExpectedAncientIdsForCoverage
        MissingPlanExpectedAncientIds = $missingPlanExpectedAncientIds
        UnexpectedPlanExpectedAncientIds = $unexpectedPlanExpectedAncientIds
        ObservedAncientIds = $observedAncientIds
        MissingExpectedAncientIds = $missingExpectedAncientIds
        EventTraversalRequired = -not [bool]$AllowMissingEventTraversal
        EventTraversalObserved = $eventTraversalObserved
        Checks = $checks
        Mismatches = $mismatches
    }
    $report | ConvertTo-Json -Depth 8 | Out-File -LiteralPath $OutFile -Encoding UTF8
}

if ($FailOnMismatch -and -not $passed) {
    exit 1
}
