param(
    [Parameter(Mandatory = $true)]
    [string]$EvidenceDir,

    [int]$MinRuns = 1,

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

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $BaseDir $Path))
}

function Test-PathInsideDirectory {
    param(
        [AllowEmptyString()][string]$Path,
        [Parameter(Mandatory = $true)][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $false
    }

    $pathFull = [System.IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
    $dirFull = [System.IO.Path]::GetFullPath($Directory).TrimEnd('\', '/')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    return $pathFull.Equals($dirFull, $comparison) -or $pathFull.StartsWith($dirFull + [System.IO.Path]::DirectorySeparatorChar, $comparison)
}

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
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

function Test-BytePrefix {
    param(
        [Parameter(Mandatory = $true)][byte[]]$Prefix,
        [Parameter(Mandatory = $true)][byte[]]$Content
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
        [Parameter(Mandatory = $true)][string]$BeforePath,
        [Parameter(Mandatory = $true)][string]$AfterPath,
        [Parameter(Mandatory = $true)][string]$CurrentPath
    )

    $result = [ordered]@{
        BeforeExists = Test-Path -LiteralPath $BeforePath -PathType Leaf
        AfterExists = Test-Path -LiteralPath $AfterPath -PathType Leaf
        CurrentExists = Test-Path -LiteralPath $CurrentPath -PathType Leaf
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
            $itemPaths.Add([System.IO.Path]::GetFullPath([string]$item.Path)) | Out-Null
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
if ($null -ne $plan) {
    $runnerKind = [string](Get-JsonValue -Object $plan -Name 'RunnerKind' -DefaultValue '')
    $invocation = [string](Get-JsonValue -Object $plan -Name 'Invocation' -DefaultValue '')
    $expectedSts1Mode = [string](Get-JsonValue -Object $plan -Name 'Sts1EventMode' -DefaultValue '')
    $sourceWorkspace = Get-JsonValue -Object $plan -Name 'SourceWorkspace' -DefaultValue $null
    $planSeeds = @(Get-ArrayValues -Value (Get-JsonValue -Object $plan -Name 'Seeds' -DefaultValue @()) | ForEach-Object { [string]$_ })
    $nonEmptyPlanSeeds = @($planSeeds | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $duplicatePlanSeeds = @($nonEmptyPlanSeeds | Group-Object | Where-Object { $_.Count -gt 1 })

    Add-Check -Name 'plan_runner_kind_is_game_native_autoslay' -Passed ([string]::Equals($runnerKind, 'GameNativeAutoSlay', [System.StringComparison]::Ordinal)) -Detail "RunnerKind must be GameNativeAutoSlay; found '$runnerKind'"
    Add-Check -Name 'plan_invocation_calls_autoslayer_start' -Passed (Contains-Text -Text $invocation -Needle 'AutoSlayer.Start(seed, logFile)') -Detail 'Invocation must record the launcher/mod hook that calls AutoSlayer.Start(seed, logFile)'
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
    $sourceWorkspaceExists = -not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckPath) -and (Test-Path -LiteralPath $sourceWorkspaceCheckPath -PathType Leaf)
    Add-Check -Name 'plan_source_workspace_summary_present' -Passed ($null -ne $sourceWorkspace) -Detail 'SourceWorkspace summary must retain source version/disposition and evidence-use policy'
    if ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_checked' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Checked' -DefaultValue $false)) -Detail 'SourceWorkspace.Checked must be true'
        Add-Check -Name 'plan_source_workspace_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'SourceWorkspace must record that source inspection is not runtime proof'
        Add-Check -Name 'plan_source_workspace_disposition_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue ''))) -Detail 'SourceWorkspace must retain the recovered-source disposition'
        Add-Check -Name 'plan_source_workspace_matches_installed_game' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'MatchesInstalledGame' -DefaultValue $false)) -Detail 'game-native AutoSlay proof requires a source snapshot that matches the installed game'
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
            Add-Check -Name 'plan_source_workspace_report_passed' -Passed ([bool](Get-JsonValue -Object $sourceReport -Name 'Passed' -DefaultValue $false)) -Detail 'source-workspace report must have Passed=true'
            Add-Check -Name 'plan_source_workspace_report_mismatches_empty' -Passed ($reportMismatches.Count -eq 0) -Detail "source-workspace report mismatches must be empty; found $($reportMismatches.Count)"
            Add-Check -Name 'plan_source_workspace_policy_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $policy -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'source-workspace report must record that source inspection alone is not runtime proof'
            Add-Check -Name 'plan_source_workspace_policy_local_source_reference_only' -Passed ([bool](Get-JsonValue -Object $policy -Name 'LocalSourceReferenceOnly' -DefaultValue $false)) -Detail 'source-workspace report must record local-source-reference-only policy'
            Add-Check -Name 'plan_source_workspace_policy_authorized_local_install_only' -Passed ([bool](Get-JsonValue -Object $policy -Name 'AuthorizedLocalInstallOnly' -DefaultValue $false)) -Detail 'source-workspace report must record authorized-local-install-only policy'
            Add-Check -Name 'plan_source_workspace_policy_third_party_dumps_prohibited' -Passed ([bool](Get-JsonValue -Object $policy -Name 'ThirdPartyDumpsProhibited' -DefaultValue $false)) -Detail 'source-workspace report must record that third-party dumps are prohibited'
            Add-Check -Name 'plan_source_workspace_policy_autoslay_still_requires_launch' -Passed ([bool](Get-JsonValue -Object $policy -Name 'GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence' -DefaultValue $false)) -Detail 'source-workspace report must keep AutoSlay source checks separate from runtime proof'
            foreach ($name in @('StartSeedLogFileSignature', 'NonInteractiveCheck', 'DebugSeedOverride', 'AutoCardSelector', 'AncientDialogueHandler', 'EventOptionSelectionLog', 'EventTriggeredCombatLog', 'EventCombatStartedLog')) {
                Add-Check -Name "plan_source_workspace_autoslay_$name" -Passed ([bool](Get-JsonValue -Object $autoSlay -Name $name -DefaultValue $false)) -Detail "AutoSlay source-contract field $name must be true"
            }

            $reportRecoveredSource = Get-JsonValue -Object $sourceReport -Name 'RecoveredSource' -DefaultValue $null
            $reportGame = Get-JsonValue -Object $sourceReport -Name 'Game' -DefaultValue $null
            Add-Check -Name 'plan_source_workspace_report_matches_installed_game' -Passed ([bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -Detail 'RecoveredSource.MatchesInstalledGame must be true for game-native AutoSlay proof'
            if ($null -ne $sourceWorkspace) {
                $summaryMatchesReport =
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Passed' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $sourceReport -Name 'Passed' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Version' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceCommit' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Commit' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Version' -DefaultValue '')) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Disposition' -DefaultValue '')) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'MatchesInstalledGame' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true) -eq [bool](Get-JsonValue -Object $policy -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true)) -and
                    ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $policy -Name 'NotRuntimeProof' -DefaultValue $false)) -and
                    ([string](Get-JsonValue -Object $sourceWorkspace -Name 'ReportSha256' -DefaultValue '') -eq $sourceWorkspaceCheckSha256)
                Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $summaryMatchesReport -Detail 'SourceWorkspace summary must match the retained source-workspace report and ReportSha256'
            }
        }
    } elseif ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $false -Detail 'SourceWorkspace summary cannot be trusted without a valid retained source-workspace report'
    }
}

$summaryRuns = @()
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
}

$eventTraversalObserved = $false
for ($i = 0; $i -lt $summaryRuns.Count; $i++) {
    $run = $summaryRuns[$i]
    $runName = "run_{0:D4}" -f ($i + 1)
    $seed = [string](Get-JsonValue -Object $run -Name 'Seed' -DefaultValue '')
    $exitCode = [int](Get-JsonValue -Object $run -Name 'ExitCode' -DefaultValue -999)
    $autoSlayLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'AutoSlayLogPath' -DefaultValue ''))
    $currentLogPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $run -Name 'GodotLogCurrentIterationPath' -DefaultValue ''))
    $autoSlayLogSha256 = [string](Get-JsonValue -Object $run -Name 'AutoSlayLogSha256' -DefaultValue '')
    $autoSlayLogExists = -not [string]::IsNullOrWhiteSpace($autoSlayLogPath) -and (Test-Path -LiteralPath $autoSlayLogPath -PathType Leaf)
    $currentLogExists = -not [string]::IsNullOrWhiteSpace($currentLogPath) -and (Test-Path -LiteralPath $currentLogPath -PathType Leaf)

    Add-Check -Name "${runName}_seed_present" -Passed (-not [string]::IsNullOrWhiteSpace($seed)) -Detail 'each AutoSlay run must retain its seed'
    Add-Check -Name "${runName}_seed_listed_in_plan" -Passed ($planSeeds -contains $seed) -Detail "seed '$seed' must be listed in autoslay-plan.json Seeds"
    Add-Check -Name "${runName}_exit_code_zero" -Passed ($exitCode -eq 0) -Detail "ExitCode must be 0 for proof packets; found $exitCode"
    Add-Check -Name "${runName}_autoslay_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($autoSlayLogPath)) -Detail 'AutoSlayLogPath must be retained'
    Add-Check -Name "${runName}_autoslay_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $autoSlayLogPath -Directory $resolvedEvidenceDir) -Detail 'AutoSlayLogPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_autoslay_log_exists" -Passed $autoSlayLogExists -Detail 'AutoSlay log file must exist'
    Add-Check -Name "${runName}_autoslay_log_hash_present" -Passed (-not [string]::IsNullOrWhiteSpace($autoSlayLogSha256)) -Detail 'AutoSlayLogSha256 must be retained for each run'
    Add-Check -Name "${runName}_current_iteration_log_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($currentLogPath)) -Detail 'GodotLogCurrentIterationPath must be retained'
    Add-Check -Name "${runName}_current_iteration_log_under_evidence_dir" -Passed (Test-PathInsideDirectory -Path $currentLogPath -Directory $resolvedEvidenceDir) -Detail 'GodotLogCurrentIterationPath must stay inside the evidence directory'
    Add-Check -Name "${runName}_current_iteration_log_exists" -Passed $currentLogExists -Detail 'GodotLogCurrentIterationPath must point at a retained current-iteration log'

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

    Add-Check -Name "${runName}_autoslay_log_started_seed" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Starting run with seed=$seed")) -Detail 'AutoSlay log must contain the start marker for this seed'
    Add-Check -Name "${runName}_autoslay_log_completed_seed" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Run completed successfully with seed=$seed")) -Detail 'AutoSlay log must contain the completion marker for this seed'
    Add-Check -Name "${runName}_autoslay_log_no_failed_seed" -Passed (-not ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $autoSlayLog -Needle "Run failed with seed=$seed"))) -Detail 'proof packets must not contain a RunFailed marker for this seed'
    Add-Check -Name "${runName}_current_log_contains_autoslay_start" -Passed ((-not [string]::IsNullOrWhiteSpace($seed)) -and (Contains-Text -Text $currentLog -Needle "Starting run with seed=$seed")) -Detail 'current-iteration godot log must contain the AutoSlay start marker'

    $runEventTraversal =
        (Contains-Text -Text $autoSlayLog -Needle 'Entering Event room') -and
        (Contains-Text -Text $autoSlayLog -Needle 'Selecting event option:')
    $eventTraversalObserved = $eventTraversalObserved -or $runEventTraversal
    Add-Check -Name "${runName}_event_room_traversal_observed" -Passed ($AllowMissingEventTraversal -or $runEventTraversal) -Detail 'AutoSlay event proof requires Entering Event room and Selecting event option markers'
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
