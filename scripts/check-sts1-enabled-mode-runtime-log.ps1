param(
    [ValidateSet('Off', 'CanaryOnly', 'AdditiveBatch1')]
    [string]$Mode = 'Off',

    [string]$LogPath,
    [string]$AuditPath,
    [string]$ExpectedPackageVersion,
    [string]$ExpectedRitsuCompatBranch,
    [string]$ExpectedRitsuLibVersion,
    [string]$ExpectedGameVersion,
    [string]$RegistrationServicePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistrationService.cs',
    [string]$OutFile,
    [switch]$PrintExpected,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Required file not found: $resolved"
        exit 1
    }

    return [System.IO.File]::ReadAllText($resolved)
}

function Get-MethodSlice {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$MethodName
    )

    $startToken = "public static void $MethodName"
    $start = $Text.IndexOf($startToken, [System.StringComparison]::Ordinal)
    if ($start -lt 0) {
        Write-Error "Method start not found: $MethodName"
        exit 1
    }

    $end = $Text.IndexOf('content.Apply();', $start, [System.StringComparison]::Ordinal)
    if ($end -lt 0) {
        Write-Error "content.Apply marker not found for method: $MethodName"
        exit 1
    }

    return $Text.Substring($start, $end - $start)
}

function Get-Registrations {
    param([Parameter(Mandatory = $true)][string]$Block)

    $items = [System.Collections.Generic.List[object]]::new()

    foreach ($match in [regex]::Matches($Block, 'content\.ActEvent<\s*([A-Za-z0-9_]+)\s*,\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Kind = 'ActEvent'
            Act = $match.Groups[1].Value
            Event = $match.Groups[2].Value
        }) | Out-Null
    }

    foreach ($match in [regex]::Matches($Block, 'content\.SharedEvent<\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Kind = 'SharedEvent'
            Act = 'Shared'
            Event = $match.Groups[1].Value
        }) | Out-Null
    }

    return @($items)
}

function Get-ExpectedModeShape {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$ModeName
    )

    if ($ModeName -eq 'Off') {
        return [pscustomobject]@{
            Mode = $ModeName
            MethodName = ''
            ExpectedRegistrationCalls = 0
            ExpectedEventClasses = @()
            ExpectedRegistrationTuples = @()
            ExpectedEventTypes = 0
            ReasonNeedle = 'StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.'
            StartNeedle = ''
            SuccessNeedle = ''
        }
    }

    $methodName = if ($ModeName -eq 'CanaryOnly') { 'RegisterCanaryOnly' } else { 'RegisterAdditiveBatch1' }
    $slice = Get-MethodSlice -Text $Text -MethodName $methodName
    $registrations = @(Get-Registrations -Block $slice)
    $classes = @($registrations | Select-Object -ExpandProperty Event | Sort-Object -Unique)
    $tuples = @($registrations | ForEach-Object { "$($_.Kind):$($_.Act):$($_.Event)" })

    $reasonNeedle = if ($ModeName -eq 'CanaryOnly') {
        'StS1 events CanaryOnly mode: registering 4 canary events.'
    } else {
        'StS1 events AdditiveBatch1 mode: registering 10 verified-scope events.'
    }

    $startNeedle = if ($ModeName -eq 'CanaryOnly') {
        '[StS1 Events] Registering canary events'
    } else {
        '[StS1 Events] Registering AdditiveBatch1 events'
    }

    $successNeedle = if ($ModeName -eq 'CanaryOnly') {
        '[StS1 Events] Canary events registered successfully.'
    } else {
        '[StS1 Events] AdditiveBatch1 events registered successfully.'
    }

    return [pscustomobject]@{
        Mode = $ModeName
        MethodName = $methodName
        ExpectedRegistrationCalls = $registrations.Count
        ExpectedEventClasses = $classes
        ExpectedRegistrationTuples = $tuples
        ExpectedEventTypes = $classes.Count
        ReasonNeedle = $reasonNeedle
        StartNeedle = $startNeedle
        SuccessNeedle = $successNeedle
    }
}

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

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
}

function Contains-Text {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    if ([string]::IsNullOrWhiteSpace($Needle)) {
        return $false
    }

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Get-GameVersionLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedVersion
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) {
        return 0
    }

    $numericVersion = $ExpectedVersion.Trim()
    if ($numericVersion.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        $numericVersion = $numericVersion.Substring(1)
    }

    $labelVersion = "v$numericVersion"
    $numericEscaped = [regex]::Escape($numericVersion)
    $labelEscaped = [regex]::Escape($labelVersion)
    $patterns = @(
        "(?im)\brelease\s*=\s*$labelEscaped\b",
        "(?im)\bHost Version:\s*$labelEscaped\b",
        "(?im)\bRelease Version:\s*$labelEscaped\b",
        "(?im)\bHost version label\s*=\s*$labelEscaped\s+numeric\s*=\s*$numericEscaped\b"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RitsuCompatBranchLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedBranch
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedBranch)) {
        return 0
    }

    $branchEscaped = [regex]::Escape($ExpectedBranch.Trim())
    $patterns = @(
        "(?im)\[compat branch:\s*$branchEscaped\]",
        "(?im)\bpicked variant\s+$branchEscaped\b"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RitsuLibVersionLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedVersion
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) {
        return 0
    }

    $version = $ExpectedVersion.Trim()
    if ($version.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        $version = $version.Substring(1)
    }

    $versionEscaped = [regex]::Escape($version)
    $patterns = @(
        "(?im)\bRitsuLib Version:\s*$versionEscaped\s+\[compat branch:",
        "(?im)\[com\.ritsukage\.sts2-RitsuLib\]\s+Version:\s*$versionEscaped\s+\[compat branch:",
        "(?im)\bRitsuLib\s+$versionEscaped\s+bootstrap starting\b",
        "(?im)\*\s+RitsuLib\s+\[STS2-RitsuLib\]\s+\($versionEscaped\)"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RegisteredEventClassesFromLog {
    param([AllowEmptyString()][string[]]$Lines)

    $registeredMatches = [System.Collections.Generic.List[object]]::new()
    $actPattern = 'Registered\s+act\s+event:\s+(Sts1[A-Za-z0-9_]+)\b.*?->\s*([A-Za-z0-9_]+)\b'
    $sharedPattern = 'Registered\s+shared\s+event:\s+(Sts1[A-Za-z0-9_]+)\b'
    $genericPattern = 'Registered\s+.*\bevent:\s+(Sts1[A-Za-z0-9_]+)\b'

    foreach ($line in $Lines) {
        $matched = $false

        foreach ($match in [regex]::Matches($line, $actPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $actName = $match.Groups[2].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'ActEvent'
                Act = $actName
                Tuple = "ActEvent:${actName}:${eventClass}"
                Line = $line
            }) | Out-Null
            $matched = $true
        }

        if ($matched) {
            continue
        }

        foreach ($match in [regex]::Matches($line, $sharedPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'SharedEvent'
                Act = 'Shared'
                Tuple = "SharedEvent:Shared:${eventClass}"
                Line = $line
            }) | Out-Null
            $matched = $true
        }

        if ($matched) {
            continue
        }

        foreach ($match in [regex]::Matches($line, $genericPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'UnknownEvent'
                Act = 'Unknown'
                Tuple = "UnknownEvent:Unknown:${eventClass}"
                Line = $line
            }) | Out-Null
        }
    }

    return @($registeredMatches)
}

function ConvertTo-AuditSummary {
    param(
        [Parameter(Mandatory = $true)][string]$Json,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $items = @($Json | ConvertFrom-Json)
    $hitCount = 0
    $dirtyItems = 0
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

        if (Test-JsonProperty -Object $item -Name 'SignatureHits') {
            foreach ($hit in @($item.SignatureHits)) {
                if (Test-JsonProperty -Object $hit -Name 'Count') {
                    $hitCount += [int]$hit.Count
                }
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

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Audit file not found: $resolved"
        exit 1
    }

    $json = [System.IO.File]::ReadAllText($resolved)
    return ConvertTo-AuditSummary -Json $json -Path $resolved
}

function Invoke-RecomputedAuditSummary {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $auditJson = (& $logAuditScript -Path $LogPath | Out-String)
    if ([string]::IsNullOrWhiteSpace($auditJson)) {
        throw "audit-godot-log.ps1 returned empty output for $LogPath"
    }

    return ConvertTo-AuditSummary -Json $auditJson -Path '<recomputed>'
}

$registrationService = Read-RepoText $RegistrationServicePath
$expected = Get-ExpectedModeShape -Text $registrationService -ModeName $Mode

Write-Output "mode=$($expected.Mode)"
Write-Output "expected_registration_calls=$($expected.ExpectedRegistrationCalls)"
Write-Output "expected_event_types=$($expected.ExpectedEventTypes)"
Write-Output "expected_event_classes=$((@($expected.ExpectedEventClasses) | Sort-Object) -join ',')"
Write-Output "expected_registration_tuples=$((@($expected.ExpectedRegistrationTuples) | Sort-Object) -join ',')"

$report = [ordered]@{
    Mode = $Mode
    ExpectedRegistrationCalls = $expected.ExpectedRegistrationCalls
    ExpectedEventTypes = $expected.ExpectedEventTypes
    ExpectedEventClasses = @($expected.ExpectedEventClasses)
    ExpectedRegistrationTuples = @($expected.ExpectedRegistrationTuples)
    RuntimeLogStatus = 'not-validated'
    Checks = $checks
    Mismatches = $mismatches
}

if ($PrintExpected -and -not $LogPath) {
    Write-Output 'runtime_log_status=not-validated-print-expected-only'

    if ($OutFile) {
        $resolvedOutFile = Resolve-RepoPath $OutFile
        $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
        if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
            [void][System.IO.Directory]::CreateDirectory($outDir)
        }

        [pscustomobject]$report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
    }

    Write-Output 'checks=0'
    Write-Output 'mismatches=0'
    exit 0
}

if (-not $LogPath) {
    Write-Error 'LogPath is required unless -PrintExpected is used without a log.'
    exit 1
}

$resolvedLogPath = Resolve-RepoPath $LogPath
if (-not (Test-Path -LiteralPath $resolvedLogPath)) {
    Write-Error "Log file not found: $resolvedLogPath"
    exit 1
}

$logText = [System.IO.File]::ReadAllText($resolvedLogPath)
$lines = if ($logText.Length -eq 0) { @() } else { $logText -split '\r?\n' }
$registeredEventMatches = @(Get-RegisteredEventClassesFromLog -Lines $lines)
$observedClasses = @($registeredEventMatches | Select-Object -ExpandProperty ClassName | Sort-Object -Unique)
$expectedClasses = @($expected.ExpectedEventClasses | Sort-Object -Unique)
$missingClasses = @($expectedClasses | Where-Object { $observedClasses -notcontains $_ })
$unexpectedClasses = @($observedClasses | Where-Object { $expectedClasses -notcontains $_ })
$observedTuples = @($registeredEventMatches | Select-Object -ExpandProperty Tuple | Sort-Object)
$expectedTuples = @($expected.ExpectedRegistrationTuples | Sort-Object)
$missingTuples = @($expectedTuples | Where-Object { $observedTuples -notcontains $_ })
$unexpectedTuples = @($observedTuples | Where-Object { $expectedTuples -notcontains $_ })

$reasonHits = [regex]::Matches($logText, [regex]::Escape($expected.ReasonNeedle)).Count
$enabledFeatureLineHits = [regex]::Matches($logText, 'Feature Sts1Events .*bootstrap=enabled, live=Enabled').Count
$disabledFeatureLineHits = [regex]::Matches($logText, 'Feature Sts1Events .*bootstrap=disabled, live=Disabled').Count
$startHits = if ([string]::IsNullOrWhiteSpace($expected.StartNeedle)) { 0 } else { [regex]::Matches($logText, [regex]::Escape($expected.StartNeedle)).Count }
$successHits = if ([string]::IsNullOrWhiteSpace($expected.SuccessNeedle)) { 0 } else { [regex]::Matches($logText, [regex]::Escape($expected.SuccessNeedle)).Count }
$ritsuInactiveHits = [regex]::Matches($logText, 'RitsuLib not active; skipping .*event registration').Count
$unsafeModeHits = [regex]::Matches($logText, 'AdditiveAllDraft|ReplaceUnknownEventsPrototype').Count
$hasExpectedPackageVersion = -not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)
$hasExpectedRitsuCompatBranch = -not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)
$hasExpectedRitsuLibVersion = -not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)
$hasExpectedGameVersion = -not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)
$expectedRitsuCompatBranchLineHits = if ($hasExpectedRitsuCompatBranch) { Get-RitsuCompatBranchLineHits -Text $logText -ExpectedBranch $ExpectedRitsuCompatBranch } else { 0 }
$expectedRitsuLibVersionLineHits = if ($hasExpectedRitsuLibVersion) { Get-RitsuLibVersionLineHits -Text $logText -ExpectedVersion $ExpectedRitsuLibVersion } else { 0 }
$expectedGameVersionLineHits = if ($hasExpectedGameVersion) { Get-GameVersionLineHits -Text $logText -ExpectedVersion $ExpectedGameVersion } else { 0 }

Write-Output "log_path=$resolvedLogPath"
Write-Output "observed_registered_event_lines=$($registeredEventMatches.Count)"
Write-Output "observed_event_types=$($observedClasses.Count)"
Write-Output "observed_event_classes=$(($observedClasses | Sort-Object) -join ',')"
Write-Output "observed_registration_tuples=$(($observedTuples | Sort-Object) -join ',')"
Write-Output "missing_registration_tuples=$(($missingTuples | Sort-Object) -join ',')"
Write-Output "unexpected_registration_tuples=$(($unexpectedTuples | Sort-Object) -join ',')"
Write-Output "mode_reason_hits=$reasonHits"
Write-Output "enabled_feature_line_hits=$enabledFeatureLineHits"
Write-Output "disabled_feature_line_hits=$disabledFeatureLineHits"
Write-Output "registration_start_hits=$startHits"
Write-Output "registration_success_hits=$successHits"
if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
    Write-Output "expected_package_version=$ExpectedPackageVersion"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
    Write-Output "expected_ritsu_compat_branch=$ExpectedRitsuCompatBranch"
    Write-Output "expected_ritsu_compat_branch_line_hits=$expectedRitsuCompatBranchLineHits"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) {
    Write-Output "expected_ritsu_lib_version=$ExpectedRitsuLibVersion"
    Write-Output "expected_ritsu_lib_version_line_hits=$expectedRitsuLibVersionLineHits"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
    Write-Output "expected_game_version=$ExpectedGameVersion"
    Write-Output "expected_game_version_line_hits=$expectedGameVersionLineHits"
}

Add-Check -Name 'mode_reason_present' -Passed ($reasonHits -gt 0) -Detail "expected log reason '$($expected.ReasonNeedle)'"
Add-Check -Name 'ritsulib_active_for_mode' -Passed ($ritsuInactiveHits -eq 0) -Detail 'RitsuLib inactive registration warning must be absent'

if ($hasExpectedPackageVersion) {
    Add-Check -Name 'expected_package_version_in_log' -Passed (Contains-Text -Text $logText -Needle $ExpectedPackageVersion) -Detail "expected package version '$ExpectedPackageVersion' in log"
}

if ($hasExpectedRitsuCompatBranch) {
    Add-Check -Name 'expected_ritsu_compat_branch_in_log' -Passed ($expectedRitsuCompatBranchLineHits -gt 0) -Detail "expected explicit RitsuLib compat branch line for '$ExpectedRitsuCompatBranch' in log"
}

if ($hasExpectedRitsuLibVersion) {
    Add-Check -Name 'expected_ritsu_lib_version_in_log' -Passed ($expectedRitsuLibVersionLineHits -gt 0) -Detail "expected explicit RitsuLib package version '$ExpectedRitsuLibVersion' in log"
}

if ($hasExpectedGameVersion) {
    Add-Check -Name 'expected_game_version_in_log' -Passed ($expectedGameVersionLineHits -gt 0) -Detail "expected explicit game version line for '$ExpectedGameVersion' in log"
}

if ($Mode -eq 'Off') {
    Add-Check -Name 'off_feature_line_disabled' -Passed ($disabledFeatureLineHits -gt 0) -Detail 'expected Feature Sts1Events bootstrap=disabled, live=Disabled'
    Add-Check -Name 'off_no_registered_sts1_event_lines' -Passed ($registeredEventMatches.Count -eq 0) -Detail 'Off mode must have zero registered StS1 event lines'
    Add-Check -Name 'off_no_registration_start' -Passed (-not [regex]::IsMatch($logText, '\[StS1 Events\]\s+Registering')) -Detail 'Off mode must not start StS1 registration'
    Add-Check -Name 'off_no_registration_success' -Passed (-not [regex]::IsMatch($logText, '\[StS1 Events\].*registered successfully')) -Detail 'Off mode must not complete StS1 registration'
} else {
    Add-Check -Name 'enabled_expected_package_version_parameter_provided' -Passed $hasExpectedPackageVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedPackageVersion'
    Add-Check -Name 'enabled_expected_ritsu_compat_branch_parameter_provided' -Passed $hasExpectedRitsuCompatBranch -Detail 'Enabled-mode copied logs must be checked with -ExpectedRitsuCompatBranch'
    Add-Check -Name 'enabled_expected_ritsu_lib_version_parameter_provided' -Passed $hasExpectedRitsuLibVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedRitsuLibVersion'
    Add-Check -Name 'enabled_expected_game_version_parameter_provided' -Passed $hasExpectedGameVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedGameVersion'
    Add-Check -Name 'enabled_audit_path_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($AuditPath)) -Detail 'Enabled-mode copied-log checks must include -AuditPath for godot-log-audit.json'
    Add-Check -Name 'enabled_outfile_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($OutFile)) -Detail 'Enabled-mode copied-log checks must be run with -OutFile so enabled-mode-log-check.json is retained'

    Add-Check -Name 'enabled_feature_line_present' -Passed ($enabledFeatureLineHits -gt 0) -Detail 'expected Feature Sts1Events bootstrap=enabled, live=Enabled'
    Add-Check -Name 'registration_start_present' -Passed ($startHits -gt 0) -Detail "expected '$($expected.StartNeedle)'"
    Add-Check -Name 'registration_success_present' -Passed ($successHits -gt 0) -Detail "expected '$($expected.SuccessNeedle)'"
    Add-Check -Name 'observed_registration_call_count' -Passed ($registeredEventMatches.Count -eq $expected.ExpectedRegistrationCalls) -Detail "expected $($expected.ExpectedRegistrationCalls) registered event lines, observed $($registeredEventMatches.Count)"
    Add-Check -Name 'observed_event_type_count' -Passed ($observedClasses.Count -eq $expected.ExpectedEventTypes) -Detail "expected $($expected.ExpectedEventTypes), observed $($observedClasses.Count)"
    Add-Check -Name 'observed_event_classes_match_expected' -Passed ($missingClasses.Count -eq 0 -and $unexpectedClasses.Count -eq 0) -Detail "missing=$($missingClasses -join ','); unexpected=$($unexpectedClasses -join ',')"
    Add-Check -Name 'observed_registration_tuples_match_expected' -Passed ($missingTuples.Count -eq 0 -and $unexpectedTuples.Count -eq 0) -Detail "missing=$($missingTuples -join ','); unexpected=$($unexpectedTuples -join ',')"
    Add-Check -Name 'no_unsafe_mode_runtime_lines' -Passed ($unsafeModeHits -eq 0) -Detail 'CanaryOnly/AdditiveBatch1 proof logs must not use unsafe StS1 modes'
}

$auditSummary = $null
if ($AuditPath) {
    $auditSummary = Read-AuditSummary -Path $AuditPath
    Write-Output "audit_path=$($auditSummary.Path)"
    Write-Output "audit_items=$($auditSummary.Items)"
    Write-Output "audit_signature_hits=$($auditSummary.SignatureHitCount)"
    Write-Output "audit_dirty_items=$($auditSummary.DirtyItems)"
    Add-Check -Name 'audit_clean' -Passed ([bool]$auditSummary.Clean) -Detail 'expected audit JSON to have zero dirty items and zero signature hits'

    $auditItemPaths = @($auditSummary.ItemPaths)
    $auditItemLengths = @($auditSummary.ItemLengths)
    $auditItemSha256s = @($auditSummary.ItemSha256s)
    $expectedAuditPath = [System.IO.Path]::GetFullPath($resolvedLogPath)
    $expectedAuditLength = [long](Get-Item -LiteralPath $resolvedLogPath).Length
    $expectedAuditSha256 = Get-FileSha256OrEmpty -Path $resolvedLogPath
    Add-Check -Name 'audit_has_single_scanned_path' -Passed ($auditItemPaths.Count -eq 1) -Detail "audit JSON must retain exactly one scanned Path; found $($auditItemPaths.Count)"
    Add-Check -Name 'audit_path_matches_log_path' -Passed ($auditItemPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemPaths[0], $expectedAuditPath)) -Detail 'godot-log-audit.json must be produced from the copied log passed as -LogPath'
    Add-Check -Name 'audit_has_single_length' -Passed ($auditItemLengths.Count -eq 1) -Detail "audit JSON must retain exactly one Length; found $($auditItemLengths.Count)"
    Add-Check -Name 'audit_length_matches_log_path' -Passed ($auditItemLengths.Count -eq 1 -and $auditItemLengths[0] -eq $expectedAuditLength) -Detail 'godot-log-audit.json Length must match the copied log bytes'
    Add-Check -Name 'audit_has_single_sha256' -Passed ($auditItemSha256s.Count -eq 1) -Detail "audit JSON must retain exactly one Sha256; found $($auditItemSha256s.Count)"
    Add-Check -Name 'audit_sha256_matches_log_path' -Passed ($auditItemSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], $expectedAuditSha256)) -Detail 'godot-log-audit.json Sha256 must match the copied log bytes'

    if (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
        Add-Check -Name 'audit_recompute_script_exists' -Passed $false -Detail "missing audit script: $logAuditScript"
    } else {
        $recomputedAuditSummary = Invoke-RecomputedAuditSummary -LogPath $resolvedLogPath
        $recomputedPaths = @($recomputedAuditSummary.ItemPaths)
        $recomputedSha256s = @($recomputedAuditSummary.ItemSha256s)
        Add-Check -Name 'audit_recomputed_from_log_path' -Passed ($recomputedPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$recomputedPaths[0], $expectedAuditPath)) -Detail 'verifier must recompute the audit from the copied log passed as -LogPath'
        Add-Check -Name 'audit_recomputed_clean' -Passed ([bool]$recomputedAuditSummary.Clean) -Detail "recomputed audit must have zero dirty items and zero signature hits; dirty=$($recomputedAuditSummary.DirtyItems), hits=$($recomputedAuditSummary.SignatureHitCount)"
        Add-Check -Name 'audit_signature_counts_match_recomputed' -Passed ($auditSummary.DirtyItems -eq $recomputedAuditSummary.DirtyItems -and $auditSummary.SignatureHitCount -eq $recomputedAuditSummary.SignatureHitCount) -Detail "retained audit signature counts must match recomputed counts; retained dirty=$($auditSummary.DirtyItems), retained hits=$($auditSummary.SignatureHitCount), recomputed dirty=$($recomputedAuditSummary.DirtyItems), recomputed hits=$($recomputedAuditSummary.SignatureHitCount)"
        Add-Check -Name 'audit_sha256_matches_recomputed' -Passed ($auditItemSha256s.Count -eq 1 -and $recomputedSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], [string]$recomputedSha256s[0])) -Detail 'retained audit Sha256 must match the recomputed audit Sha256'
    }
} else {
    Write-Output 'audit_status=not-provided'
}

$report.RuntimeLogStatus = 'validated'
$report['LogPath'] = $resolvedLogPath
$report['LogLength'] = [long](Get-Item -LiteralPath $resolvedLogPath).Length
$report['LogSha256'] = Get-FileSha256OrEmpty -Path $resolvedLogPath
$report['ObservedRegisteredEventLines'] = $registeredEventMatches.Count
$report['ObservedEventTypes'] = $observedClasses.Count
$report['ObservedEventClasses'] = $observedClasses
$report['ObservedRegistrationTuples'] = $observedTuples
$report['MissingRegistrationTuples'] = $missingTuples
$report['UnexpectedRegistrationTuples'] = $unexpectedTuples
$report['ModeReasonHits'] = $reasonHits
$report['EnabledFeatureLineHits'] = $enabledFeatureLineHits
$report['DisabledFeatureLineHits'] = $disabledFeatureLineHits
$report['RegistrationStartHits'] = $startHits
$report['RegistrationSuccessHits'] = $successHits
$report['ExpectedPackageVersion'] = $ExpectedPackageVersion
$report['ExpectedRitsuCompatBranch'] = $ExpectedRitsuCompatBranch
$report['ExpectedRitsuLibVersion'] = $ExpectedRitsuLibVersion
$report['ExpectedGameVersion'] = $ExpectedGameVersion
$report['Audit'] = $auditSummary

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    [pscustomobject]$report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
