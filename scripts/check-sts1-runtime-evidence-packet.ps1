param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Off', 'CanaryOnly', 'AdditiveBatch1')]
    [string]$Mode,

    [Parameter(Mandatory = $true)]
    [string]$EvidenceDir,

    [string]$LogFileName = 'godot.log.after-launch',
    [string]$AuditFileName = 'godot-log-audit.json',
    [string]$SessionStateFileName = 'session-state.json',
    [string]$RestoreStateFileName = 'restore-state.json',
    [string]$SettingsBeforeFileName = 'settings.save.before',
    [string]$GameReleaseInfoFileName = 'game-release-info.json',
    [string]$EnabledModeVerifierPath = 'scripts\check-sts1-enabled-mode-runtime-log.ps1',
    [string]$ExpectedPackageVersion,
    [string]$ExpectedRitsuCompatBranch,
    [string]$ExpectedRitsuLibVersion,
    [string]$ExpectedGameVersion,
    [string]$OutFile,
    [switch]$AllowMissingSessionState,
    [switch]$AllowMissingRestoreState,
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

function Resolve-EvidenceFile {
    param([Parameter(Mandatory = $true)][string]$Name)
    return [System.IO.Path]::GetFullPath((Join-Path $resolvedEvidenceDir $Name))
}

function Read-JsonFile {
    param([Parameter(Mandatory = $true)][string]$Path)

    $json = [System.IO.File]::ReadAllText($Path)
    return $json | ConvertFrom-Json
}

function Test-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object) {
        return $false
    }

    foreach ($property in @($Object.PSObject.Properties)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function Get-JsonValueOrNull {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object) {
        return $null
    }

    foreach ($property in @($Object.PSObject.Properties)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            return $property.Value
        }
    }

    return $null
}

function Get-JsonStringOrEmpty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $value = Get-JsonValueOrNull -Object $Object -Name $Name
    if ($null -eq $value) {
        return ''
    }

    return [string]$value
}

function Get-JsonArrayOrEmpty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $value = Get-JsonValueOrNull -Object $Object -Name $Name
    if ($null -eq $value) {
        return @()
    }

    return @($value)
}

function Contains-Text {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Format-SortedSet {
    param([AllowEmptyCollection()][AllowNull()][string[]]$Items)

    if ($null -eq $Items -or @($Items).Count -eq 0) {
        return ''
    }

    return (@($Items | Sort-Object -Unique) -join ',')
}

function Test-PathInsideString {
    param(
        [AllowEmptyString()][string]$Child,
        [AllowEmptyString()][string]$Parent
    )

    if ([string]::IsNullOrWhiteSpace($Child) -or [string]::IsNullOrWhiteSpace($Parent)) {
        return $false
    }

    try {
        $childFull = [System.IO.Path]::GetFullPath($Child).TrimEnd('\', '/')
        $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\', '/')
    } catch {
        return $false
    }

    $comparison = [System.StringComparison]::OrdinalIgnoreCase
    return $childFull.Equals($parentFull, $comparison) -or $childFull.StartsWith($parentFull + '\', $comparison)
}

function Test-PathInsideAnyString {
    param(
        [AllowEmptyString()][string]$Child,
        [AllowEmptyCollection()][AllowNull()][string[]]$Parents
    )

    foreach ($parent in @($Parents)) {
        if (Test-PathInsideString -Child $Child -Parent $parent) {
            return $true
        }
    }

    return $false
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

function Resolve-ChildPathOrEmpty {
    param(
        [AllowEmptyString()][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$ChildName
    )

    if ([string]::IsNullOrWhiteSpace($BasePath)) {
        return ''
    }

    try {
        return [System.IO.Path]::GetFullPath((Join-Path $BasePath $ChildName))
    } catch {
        return ''
    }
}

function Get-PathLeafOrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        $pathFull = [System.IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
        return [System.IO.Path]::GetFileName($pathFull)
    } catch {
        return ''
    }
}

function Get-PathParentLeafOrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        $pathFull = [System.IO.Path]::GetFullPath($Path)
        $parent = [System.IO.Path]::GetDirectoryName($pathFull)
        if ([string]::IsNullOrWhiteSpace($parent)) {
            return ''
        }

        return [System.IO.Path]::GetFileName($parent.TrimEnd('\', '/'))
    } catch {
        return ''
    }
}

function Test-Sha256Hex {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[0-9a-fA-F]{64}$'
}

function Test-BytePrefix {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Prefix,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Content
    )

    if ($Content.Length -lt $Prefix.Length) {
        return $false
    }

    for ($i = 0; $i -lt $Prefix.Length; $i++) {
        if ($Content[$i] -ne $Prefix[$i]) {
            return $false
        }
    }

    return $true
}

function Write-CurrentSliceFromBeforeAfter {
    param(
        [Parameter(Mandatory = $true)][string]$BeforePath,
        [Parameter(Mandatory = $true)][string]$AfterPath,
        [Parameter(Mandatory = $true)][string]$CurrentPath
    )

    $beforeBytes = [System.IO.File]::ReadAllBytes($BeforePath)
    $afterBytes = [System.IO.File]::ReadAllBytes($AfterPath)
    if (-not (Test-BytePrefix -Prefix $beforeBytes -Content $afterBytes)) {
        return $false
    }

    $sliceLength = $afterBytes.Length - $beforeBytes.Length
    $sliceBytes = [byte[]]::new($sliceLength)
    if ($sliceLength -gt 0) {
        [System.Array]::Copy($afterBytes, $beforeBytes.Length, $sliceBytes, 0, $sliceLength)
    }

    [System.IO.File]::WriteAllBytes($CurrentPath, $sliceBytes)
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

$resolvedEvidenceDir = Resolve-RepoPath $EvidenceDir
$verifierPath = Resolve-RepoPath $EnabledModeVerifierPath

if (-not (Test-Path -LiteralPath $resolvedEvidenceDir -PathType Container)) {
    Write-Error "Evidence directory not found: $resolvedEvidenceDir"
    exit 1
}

if (-not (Test-Path -LiteralPath $verifierPath -PathType Leaf)) {
    Write-Error "Enabled-mode verifier not found: $verifierPath"
    exit 1
}

$logPath = Resolve-EvidenceFile $LogFileName
$auditPath = Resolve-EvidenceFile $AuditFileName
$beforeLogPath = Resolve-EvidenceFile 'godot.log.before'
$currentLogPath = Resolve-EvidenceFile 'godot.log.current-iteration'
$currentAuditPath = Resolve-EvidenceFile 'godot-log-current-iteration-audit.json'
$sessionStatePath = Resolve-EvidenceFile $SessionStateFileName
$restoreStatePath = Resolve-EvidenceFile $RestoreStateFileName
$settingsBeforePath = Resolve-EvidenceFile $SettingsBeforeFileName
$gameReleaseInfoPath = Resolve-EvidenceFile $GameReleaseInfoFileName
$enabledModeLogCheckPath = Resolve-EvidenceFile 'enabled-mode-log-check.json'
$canonicalLogPath = $logPath
$canonicalAuditPath = $auditPath
$canonicalLogName = $LogFileName
$canonicalAuditName = $AuditFileName
$currentSliceDerived = $false
$currentSliceDerivationError = ''

if ($Mode -ne 'Off') {
    $canonicalLogPath = $currentLogPath
    $canonicalLogName = 'godot.log.current-iteration'

    if (-not (Test-Path -LiteralPath $currentLogPath -PathType Leaf)) {
        if ((Test-Path -LiteralPath $beforeLogPath -PathType Leaf) -and (Test-Path -LiteralPath $logPath -PathType Leaf)) {
            try {
                $currentSliceDerived = Write-CurrentSliceFromBeforeAfter -BeforePath $beforeLogPath -AfterPath $logPath -CurrentPath $currentLogPath
                if (-not $currentSliceDerived) {
                    $currentSliceDerivationError = 'godot.log.after-launch does not have godot.log.before as a byte prefix'
                }
            } catch {
                $currentSliceDerivationError = $_.Exception.Message
            }
        } else {
            $currentSliceDerivationError = 'requires godot.log.current-iteration or both godot.log.before and godot.log.after-launch'
        }
    }

    if (Test-Path -LiteralPath $currentLogPath -PathType Leaf) {
        if ($currentSliceDerived) {
            $canonicalAuditPath = $currentAuditPath
            $canonicalAuditName = 'godot-log-current-iteration-audit.json'
            if (Test-Path -LiteralPath $logAuditScript -PathType Leaf) {
                & $logAuditScript -Path $currentLogPath -OutFile $currentAuditPath | Out-Null
            }
        } elseif (Test-Path -LiteralPath $currentAuditPath -PathType Leaf) {
            $canonicalAuditPath = $currentAuditPath
            $canonicalAuditName = 'godot-log-current-iteration-audit.json'
        } else {
            $canonicalAuditPath = $auditPath
            $canonicalAuditName = $AuditFileName
        }
    }
}

$currentSliceBinding = [pscustomobject]@{
    BeforeExists = $false
    AfterExists = $false
    CurrentExists = $false
    PrefixMatches = $false
    SliceMatches = $false
    Detail = 'not checked for Off mode'
}

if ($Mode -ne 'Off') {
    $currentSliceBinding = Test-CurrentSliceBinding -BeforePath $beforeLogPath -AfterPath $logPath -CurrentPath $currentLogPath
}

Write-Output "mode=$Mode"
Write-Output "evidence_dir=$resolvedEvidenceDir"
Write-Output "canonical_log_path=$canonicalLogPath"
Write-Output "canonical_audit_path=$canonicalAuditPath"

Add-Check -Name 'godot_log_exists' -Passed (Test-Path -LiteralPath $canonicalLogPath -PathType Leaf) -Detail "requires $canonicalLogName"
Add-Check -Name 'audit_json_exists' -Passed (Test-Path -LiteralPath $canonicalAuditPath -PathType Leaf) -Detail "requires $canonicalAuditName"
Add-Check -Name 'settings_before_exists' -Passed (Test-Path -LiteralPath $settingsBeforePath -PathType Leaf) -Detail "requires $SettingsBeforeFileName"

$gameReleaseInfoExists = Test-Path -LiteralPath $gameReleaseInfoPath -PathType Leaf
if ($Mode -ne 'Off') {
    Add-Check -Name 'game_release_info_exists_for_enabled_mode' -Passed $gameReleaseInfoExists -Detail "enabled-mode packets require $GameReleaseInfoFileName"
}

$sessionRequired = -not [bool]$AllowMissingSessionState
$restoreRequired = -not [bool]$AllowMissingRestoreState
$sessionExists = Test-Path -LiteralPath $sessionStatePath -PathType Leaf
$restoreExists = Test-Path -LiteralPath $restoreStatePath -PathType Leaf

Add-Check -Name 'session_state_exists' -Passed ((-not $sessionRequired) -or $sessionExists) -Detail "requires $SessionStateFileName unless -AllowMissingSessionState is set"
Add-Check -Name 'restore_state_exists' -Passed ((-not $restoreRequired) -or $restoreExists) -Detail "requires $RestoreStateFileName unless -AllowMissingRestoreState is set"

$logText = ''
if (Test-Path -LiteralPath $canonicalLogPath -PathType Leaf) {
    $logText = [System.IO.File]::ReadAllText($canonicalLogPath)
    Add-Check -Name 'godot_log_non_empty' -Passed ($logText.Length -gt 0) -Detail "$canonicalLogName must be non-empty"
}

if ($Mode -ne 'Off') {
    $currentLogExists = Test-Path -LiteralPath $currentLogPath -PathType Leaf
    $canonicalUsesCurrentSlice = [System.StringComparer]::OrdinalIgnoreCase.Equals([System.IO.Path]::GetFullPath($canonicalLogPath), [System.IO.Path]::GetFullPath($currentLogPath))
    Add-Check -Name 'enabled_before_log_exists' -Passed ([bool]$currentSliceBinding.BeforeExists) -Detail 'enabled-mode packets require godot.log.before so current slice provenance is reviewable'
    Add-Check -Name 'enabled_after_launch_log_exists' -Passed ([bool]$currentSliceBinding.AfterExists) -Detail "enabled-mode packets require $LogFileName as full forensic log context"
    Add-Check -Name 'enabled_current_iteration_log_exists_or_derived' -Passed $currentLogExists -Detail "requires godot.log.current-iteration or a derivable slice from godot.log.before + $LogFileName; $currentSliceDerivationError"
    Add-Check -Name 'current_slice_derived_from_before_after' -Passed ($currentLogExists -and ((-not $currentSliceDerived) -or [string]::IsNullOrWhiteSpace($currentSliceDerivationError))) -Detail 'retained current slice exists, or was derived only when godot.log.before is a byte prefix of godot.log.after-launch'
    Add-Check -Name 'current_slice_matches_before_after' -Passed ([bool]$currentSliceBinding.SliceMatches) -Detail $currentSliceBinding.Detail
    Add-Check -Name 'enabled_mode_log_verifier_uses_current_slice' -Passed $canonicalUsesCurrentSlice -Detail 'enabled-mode nested verifier must receive godot.log.current-iteration, not the full copied log'
    Add-Check -Name 'full_log_not_used_as_canonical_verifier_input' -Passed $canonicalUsesCurrentSlice -Detail 'godot.log.after-launch is forensic context only for enabled-mode packets'
    if ($currentSliceDerived) {
        Add-Check -Name 'derived_current_slice_audit_generated' -Passed (Test-Path -LiteralPath $currentAuditPath -PathType Leaf) -Detail 'derived current slices require a fresh audit generated from godot.log.current-iteration'
    }

    Add-Check -Name 'enabled_expected_package_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) -Detail 'Enabled-mode packets must be checked with -ExpectedPackageVersion'
    Add-Check -Name 'enabled_expected_ritsu_compat_branch_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) -Detail 'Enabled-mode packets must be checked with -ExpectedRitsuCompatBranch'
    Add-Check -Name 'enabled_expected_ritsu_lib_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) -Detail 'Enabled-mode packets must be checked with -ExpectedRitsuLibVersion'
    Add-Check -Name 'enabled_expected_game_version_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) -Detail 'Enabled-mode packets must be checked with -ExpectedGameVersion'
    Add-Check -Name 'enabled_outfile_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($OutFile)) -Detail 'Enabled-mode packet checks must be run with -OutFile so runtime-evidence-packet-check.json is retained'
    Add-Check -Name 'enabled_session_state_cannot_be_legacy_optional' -Passed (-not [bool]$AllowMissingSessionState) -Detail 'Enabled-mode packets must not use -AllowMissingSessionState'
    Add-Check -Name 'enabled_restore_state_cannot_be_legacy_optional' -Passed (-not [bool]$AllowMissingRestoreState) -Detail 'Enabled-mode packets must not use -AllowMissingRestoreState'
}

if ($ExpectedPackageVersion) {
    Add-Check -Name 'expected_package_version_in_log' -Passed (Contains-Text -Text $logText -Needle $ExpectedPackageVersion) -Detail "expected package version '$ExpectedPackageVersion' in log"
}

if ($ExpectedRitsuCompatBranch) {
    $expectedRitsuCompatBranchLineHits = Get-RitsuCompatBranchLineHits -Text $logText -ExpectedBranch $ExpectedRitsuCompatBranch
    Add-Check -Name 'expected_ritsu_compat_branch_in_log' -Passed ($expectedRitsuCompatBranchLineHits -gt 0) -Detail "expected explicit RitsuLib compat branch line for '$ExpectedRitsuCompatBranch' in log"
}

if ($ExpectedRitsuLibVersion) {
    $expectedRitsuLibVersionLineHits = Get-RitsuLibVersionLineHits -Text $logText -ExpectedVersion $ExpectedRitsuLibVersion
    Add-Check -Name 'expected_ritsu_lib_version_in_log' -Passed ($expectedRitsuLibVersionLineHits -gt 0) -Detail "expected explicit RitsuLib package version '$ExpectedRitsuLibVersion' in log"
}

if ($ExpectedGameVersion) {
    $gameReleaseInfoText = ''
    if ($gameReleaseInfoExists) {
        $gameReleaseInfoText = [System.IO.File]::ReadAllText($gameReleaseInfoPath)
    }

    if ($gameReleaseInfoExists) {
        Add-Check -Name 'expected_game_version_in_release_info' -Passed (Contains-Text -Text $gameReleaseInfoText -Needle $ExpectedGameVersion) -Detail "expected game version '$ExpectedGameVersion' in $GameReleaseInfoFileName"
    } elseif ($Mode -eq 'Off') {
        $expectedGameVersionLineHits = Get-GameVersionLineHits -Text $logText -ExpectedVersion $ExpectedGameVersion
        Add-Check -Name 'expected_game_version_in_log_legacy_off_packet' -Passed ($expectedGameVersionLineHits -gt 0) -Detail "expected explicit game version line for '$ExpectedGameVersion' in log because $GameReleaseInfoFileName is absent"
    } else {
        Add-Check -Name 'expected_game_version_in_release_info' -Passed $false -Detail "expected game version '$ExpectedGameVersion' in $GameReleaseInfoFileName"
    }
}

$sessionState = $null
$movedMods = @()
$movedCurrentRuns = @()
if ($sessionExists) {
    $sessionState = Read-JsonFile $sessionStatePath
    $allowedModIds = @(Get-JsonArrayOrEmpty -Object $sessionState -Name 'AllowedModIds' | ForEach-Object { [string]$_ })
    $expectedAllowedModIds = @('BaseLib', 'STS2-RitsuLib', 'EZMicroBalance')
    $allowedModSet = Format-SortedSet -Items $allowedModIds
    $expectedAllowedModSet = Format-SortedSet -Items $expectedAllowedModIds
    $recordedSts1EventMode = Get-JsonStringOrEmpty -Object $sessionState -Name 'Sts1EventModeEnvironment'
    $recordedUnsafeMode = Get-JsonStringOrEmpty -Object $sessionState -Name 'Sts1UnsafeModeEnvironment'
    $disableSpirePlus = if (Test-JsonProperty -Object $sessionState -Name 'DisableSpirePlus') { [bool](Get-JsonValueOrNull -Object $sessionState -Name 'DisableSpirePlus') } else { $true }
    $moveOtherMods = if (Test-JsonProperty -Object $sessionState -Name 'MoveOtherMods') { [bool](Get-JsonValueOrNull -Object $sessionState -Name 'MoveOtherMods') } else { $false }
    $moveCurrentRuns = if (Test-JsonProperty -Object $sessionState -Name 'MoveCurrentRuns') { [bool](Get-JsonValueOrNull -Object $sessionState -Name 'MoveCurrentRuns') } else { $false }
    $hasMovedMods = Test-JsonProperty -Object $sessionState -Name 'MovedMods'
    $hasMovedCurrentRuns = Test-JsonProperty -Object $sessionState -Name 'MovedCurrentRuns'
    $movedMods = if ($hasMovedMods) { @(Get-JsonArrayOrEmpty -Object $sessionState -Name 'MovedMods') } else { @() }
    $movedCurrentRuns = if ($hasMovedCurrentRuns) { @(Get-JsonArrayOrEmpty -Object $sessionState -Name 'MovedCurrentRuns') } else { @() }
    $modsRoot = Get-JsonStringOrEmpty -Object $sessionState -Name 'ModsRoot'
    $gameRoot = Get-JsonStringOrEmpty -Object $sessionState -Name 'GameRoot'
    $logPath = Get-JsonStringOrEmpty -Object $sessionState -Name 'LogPath'
    $normalizedLogPath = ConvertTo-NormalizedPathOrEmpty -Path $logPath
    $sessionLogPathMatchesRetainedLog = -not [string]::IsNullOrWhiteSpace($normalizedLogPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedLogPath, $canonicalLogPath)
    $sessionLogPathMatchesLiveLogShape = [System.StringComparer]::OrdinalIgnoreCase.Equals((Get-PathLeafOrEmpty -Path $normalizedLogPath), 'godot.log') -and [System.StringComparer]::OrdinalIgnoreCase.Equals((Get-PathParentLeafOrEmpty -Path $normalizedLogPath), 'logs')
    $steamSaves = @(Get-JsonArrayOrEmpty -Object $sessionState -Name 'SteamSaves' | ForEach-Object { [string]$_ } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $defaultSaves = @(Get-JsonArrayOrEmpty -Object $sessionState -Name 'DefaultSaves' | ForEach-Object { [string]$_ } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $currentRunSourceRoots = @($steamSaves + $defaultSaves)
    $expectedModsRoot = Resolve-ChildPathOrEmpty -BasePath $gameRoot -ChildName 'mods'
    $normalizedModsRoot = ConvertTo-NormalizedPathOrEmpty -Path $modsRoot

    Add-Check -Name 'session_allows_baselib' -Passed ($allowedModIds -contains 'BaseLib') -Detail 'session-state AllowedModIds must include BaseLib'
    Add-Check -Name 'session_allows_ritsulib' -Passed ($allowedModIds -contains 'STS2-RitsuLib') -Detail 'session-state AllowedModIds must include STS2-RitsuLib'
    Add-Check -Name 'session_allows_spire_plus' -Passed ($allowedModIds -contains 'EZMicroBalance') -Detail 'session-state AllowedModIds must include EZMicroBalance'
    Add-Check -Name 'session_allowed_mod_ids_exact' -Passed ($allowedModSet -eq $expectedAllowedModSet) -Detail "session-state AllowedModIds expected exactly '$expectedAllowedModSet' but found '$allowedModSet'"
    Add-Check -Name 'session_does_not_disable_spire_plus' -Passed (-not $disableSpirePlus) -Detail 'DisableSpirePlus must be false for StS1 runtime smoke'
    Add-Check -Name 'session_move_other_mods' -Passed $moveOtherMods -Detail 'MoveOtherMods should be true for isolated StS1 runtime smoke'
    Add-Check -Name 'session_move_current_runs' -Passed $moveCurrentRuns -Detail 'MoveCurrentRuns should be true for StS1 runtime smoke'
    Add-Check -Name 'session_moved_mods_field_recorded' -Passed $hasMovedMods -Detail 'session-state must record MovedMods, even if the list is empty'
    Add-Check -Name 'session_moved_current_runs_field_recorded' -Passed $hasMovedCurrentRuns -Detail 'session-state must record MovedCurrentRuns, even if the list is empty'
    Add-Check -Name 'session_has_game_root' -Passed (-not [string]::IsNullOrWhiteSpace($gameRoot)) -Detail 'session-state GameRoot must be recorded'
    Add-Check -Name 'session_has_mods_root' -Passed (-not [string]::IsNullOrWhiteSpace($modsRoot)) -Detail 'session-state ModsRoot must be recorded'
    Add-Check -Name 'session_mods_root_matches_game_root' -Passed (-not [string]::IsNullOrWhiteSpace($normalizedModsRoot) -and -not [string]::IsNullOrWhiteSpace($expectedModsRoot) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedModsRoot, $expectedModsRoot)) -Detail "session-state ModsRoot must equal GameRoot\\mods; expected '$expectedModsRoot', found '$normalizedModsRoot'"
    Add-Check -Name 'session_has_log_path' -Passed (-not [string]::IsNullOrWhiteSpace($logPath)) -Detail 'session-state LogPath must be recorded'
    Add-Check -Name 'session_log_path_well_formed' -Passed ($sessionLogPathMatchesRetainedLog -or $sessionLogPathMatchesLiveLogShape) -Detail "session-state LogPath must be the retained canonical log or live logs\\godot.log path; found '$normalizedLogPath'"

    $isolatedModsRoot = [System.IO.Path]::GetFullPath((Join-Path $resolvedEvidenceDir 'isolated-mods'))
    $allowedMovedModsBuilder = [System.Collections.Generic.List[string]]::new()
    $missingMovedModNamesBuilder = [System.Collections.Generic.List[string]]::new()
    $movedModSourcesOutsideRootBuilder = [System.Collections.Generic.List[string]]::new()
    $movedModDestinationsOutsideIsolationBuilder = [System.Collections.Generic.List[string]]::new()
    $currentRunNames = @('current_run.save', 'current_run.save.backup', 'current_run_mp.save', 'current_run_mp.save.backup')
    $currentRunDestinationRoot = [System.IO.Path]::GetFullPath((Join-Path $resolvedEvidenceDir 'temporarily-removed-current-runs'))
    $missingMovedCurrentRunNamesBuilder = [System.Collections.Generic.List[string]]::new()
    $unexpectedMovedCurrentRunNamesBuilder = [System.Collections.Generic.List[string]]::new()
    $movedCurrentRunSourcesOutsideRootsBuilder = [System.Collections.Generic.List[string]]::new()
    $movedCurrentRunDestinationsOutsideRootBuilder = [System.Collections.Generic.List[string]]::new()
    $movedCurrentRunPathNameMismatchesBuilder = [System.Collections.Generic.List[string]]::new()

    foreach ($movedMod in $movedMods) {
        $movedModName = Get-JsonStringOrEmpty -Object $movedMod -Name 'Name'
        $movedModFrom = Get-JsonStringOrEmpty -Object $movedMod -Name 'From'
        $movedModTo = Get-JsonStringOrEmpty -Object $movedMod -Name 'To'
        $movedModFromLeaf = Get-PathLeafOrEmpty -Path $movedModFrom

        if ([string]::IsNullOrWhiteSpace($movedModName)) {
            $missingMovedModNamesBuilder.Add($movedModFrom) | Out-Null
        }

        if ($allowedModIds -contains $movedModName) {
            $allowedMovedModsBuilder.Add("name:$movedModName") | Out-Null
        }

        if ($allowedModIds -contains $movedModFromLeaf) {
            $allowedMovedModsBuilder.Add("source:$movedModFromLeaf") | Out-Null
        }

        if (-not (Test-PathInsideString -Child $movedModFrom -Parent $modsRoot)) {
            $movedModSourcesOutsideRootBuilder.Add($movedModFrom) | Out-Null
        }

        if (-not (Test-PathInsideString -Child $movedModTo -Parent $isolatedModsRoot)) {
            $movedModDestinationsOutsideIsolationBuilder.Add($movedModTo) | Out-Null
        }
    }

    foreach ($movedCurrentRun in $movedCurrentRuns) {
        $movedCurrentRunName = Get-JsonStringOrEmpty -Object $movedCurrentRun -Name 'Name'
        $movedCurrentRunFrom = Get-JsonStringOrEmpty -Object $movedCurrentRun -Name 'From'
        $movedCurrentRunTo = Get-JsonStringOrEmpty -Object $movedCurrentRun -Name 'To'
        $movedCurrentRunFromLeaf = Get-PathLeafOrEmpty -Path $movedCurrentRunFrom
        $movedCurrentRunToLeaf = Get-PathLeafOrEmpty -Path $movedCurrentRunTo

        if ([string]::IsNullOrWhiteSpace($movedCurrentRunName)) {
            $missingMovedCurrentRunNamesBuilder.Add($movedCurrentRunFrom) | Out-Null
        } elseif ($currentRunNames -notcontains $movedCurrentRunName) {
            $unexpectedMovedCurrentRunNamesBuilder.Add($movedCurrentRunName) | Out-Null
        }

        if (-not [string]::IsNullOrWhiteSpace($movedCurrentRunName)) {
            if (-not [string]::IsNullOrWhiteSpace($movedCurrentRunFromLeaf) -and $movedCurrentRunFromLeaf -ne $movedCurrentRunName) {
                $movedCurrentRunPathNameMismatchesBuilder.Add("from:$movedCurrentRunFromLeaf/name:$movedCurrentRunName") | Out-Null
            }

            if (-not [string]::IsNullOrWhiteSpace($movedCurrentRunToLeaf) -and $movedCurrentRunToLeaf -ne $movedCurrentRunName) {
                $movedCurrentRunPathNameMismatchesBuilder.Add("to:$movedCurrentRunToLeaf/name:$movedCurrentRunName") | Out-Null
            }
        }

        if (-not (Test-PathInsideAnyString -Child $movedCurrentRunFrom -Parents $currentRunSourceRoots)) {
            $movedCurrentRunSourcesOutsideRootsBuilder.Add($movedCurrentRunFrom) | Out-Null
        }

        if (-not (Test-PathInsideString -Child $movedCurrentRunTo -Parent $currentRunDestinationRoot)) {
            $movedCurrentRunDestinationsOutsideRootBuilder.Add($movedCurrentRunTo) | Out-Null
        }
    }

    $allowedMovedMods = @($allowedMovedModsBuilder | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Sort-Object -Unique)
    $missingMovedModNames = @($missingMovedModNamesBuilder)
    $movedModSourcesOutsideRoot = @($movedModSourcesOutsideRootBuilder)
    $movedModDestinationsOutsideIsolation = @($movedModDestinationsOutsideIsolationBuilder)
    $missingMovedCurrentRunNames = @($missingMovedCurrentRunNamesBuilder)
    $unexpectedMovedCurrentRunNames = @($unexpectedMovedCurrentRunNamesBuilder | Sort-Object -Unique)
    $movedCurrentRunSourcesOutsideRoots = @($movedCurrentRunSourcesOutsideRootsBuilder)
    $movedCurrentRunDestinationsOutsideRoot = @($movedCurrentRunDestinationsOutsideRootBuilder)
    $movedCurrentRunPathNameMismatches = @($movedCurrentRunPathNameMismatchesBuilder | Sort-Object -Unique)

    Add-Check -Name 'session_moved_mod_names_present' -Passed (@($missingMovedModNames).Count -eq 0) -Detail 'every moved-mod entry must record Name'
    Add-Check -Name 'session_moved_mods_do_not_include_allowed_mods' -Passed (@($allowedMovedMods).Count -eq 0) -Detail "allowed mods must not be moved out; moved allowed mods: $($allowedMovedMods -join ',')"
    Add-Check -Name 'session_moved_mod_sources_under_mods_root' -Passed (@($movedModSourcesOutsideRoot).Count -eq 0) -Detail "moved-mod From paths must be under session ModsRoot; outside: $($movedModSourcesOutsideRoot -join ',')"
    Add-Check -Name 'session_moved_mod_destinations_under_isolated_mods' -Passed (@($movedModDestinationsOutsideIsolation).Count -eq 0) -Detail "moved-mod To paths must be under evidence isolated-mods; outside: $($movedModDestinationsOutsideIsolation -join ',')"
    Add-Check -Name 'session_moved_current_run_names_present' -Passed (@($missingMovedCurrentRunNames).Count -eq 0) -Detail 'every moved-current-run entry must record Name'
    Add-Check -Name 'session_moved_current_run_names_allowed' -Passed (@($unexpectedMovedCurrentRunNames).Count -eq 0) -Detail "moved-current-run Name values must be known current-run files; unexpected: $($unexpectedMovedCurrentRunNames -join ',')"
    Add-Check -Name 'session_moved_current_run_sources_under_save_roots' -Passed (@($movedCurrentRunSourcesOutsideRoots).Count -eq 0) -Detail "moved-current-run From paths must be under recorded SteamSaves/DefaultSaves; outside: $($movedCurrentRunSourcesOutsideRoots -join ',')"
    Add-Check -Name 'session_moved_current_run_destinations_under_removed_runs' -Passed (@($movedCurrentRunDestinationsOutsideRoot).Count -eq 0) -Detail "moved-current-run To paths must be under evidence temporarily-removed-current-runs; outside: $($movedCurrentRunDestinationsOutsideRoot -join ',')"
    Add-Check -Name 'session_moved_current_run_paths_match_names' -Passed (@($movedCurrentRunPathNameMismatches).Count -eq 0) -Detail "moved-current-run From/To leaf names must match Name; mismatches: $($movedCurrentRunPathNameMismatches -join ',')"

    if ($Mode -eq 'Off') {
        $offModeClean = [string]::IsNullOrWhiteSpace($recordedSts1EventMode) -or $recordedSts1EventMode -eq 'Off'
        Add-Check -Name 'session_sts1_mode_env_off_or_empty' -Passed $offModeClean -Detail "Off packet must record empty or Off SPIREPLUS_STS1_EVENT_MODE; found '$recordedSts1EventMode'"
    } else {
        Add-Check -Name 'session_sts1_mode_env_recorded' -Passed (Test-JsonProperty -Object $sessionState -Name 'Sts1EventModeEnvironment') -Detail 'session-state must record Sts1EventModeEnvironment for enabled-mode evidence'
        Add-Check -Name 'session_sts1_mode_env_matches_mode' -Passed ($recordedSts1EventMode -eq $Mode) -Detail "expected SPIREPLUS_STS1_EVENT_MODE '$Mode' but session recorded '$recordedSts1EventMode'"
        Add-Check -Name 'session_no_unsafe_sts1_mode_env' -Passed ([string]::IsNullOrWhiteSpace($recordedUnsafeMode)) -Detail 'CanaryOnly/AdditiveBatch1 evidence must not set SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES'
    }
}

if ($restoreExists) {
    $restoreState = Read-JsonFile $restoreStatePath
    $hasRestoredModCountField = $restoreState.PSObject.Properties.Name -contains 'RestoredModCount'
    $hasRestoredCurrentRunCountField = $restoreState.PSObject.Properties.Name -contains 'RestoredCurrentRunCount'

    Add-Check -Name 'restore_has_timestamp' -Passed (-not [string]::IsNullOrWhiteSpace((Get-JsonStringOrEmpty -Object $restoreState -Name 'RestoredAt'))) -Detail 'restore-state RestoredAt must be recorded'
    Add-Check -Name 'restore_mod_count_recorded' -Passed $hasRestoredModCountField -Detail 'restore-state must record RestoredModCount'
    Add-Check -Name 'restore_current_run_count_recorded' -Passed $hasRestoredCurrentRunCountField -Detail 'restore-state must record RestoredCurrentRunCount'

    if ($sessionExists -and $hasRestoredModCountField) {
        $expectedRestoredModCount = @($movedMods).Count
        $actualRestoredModCount = [int]$restoreState.RestoredModCount
        Add-Check -Name 'restore_mod_count_matches_session_moved_mods' -Passed ($actualRestoredModCount -eq $expectedRestoredModCount) -Detail "RestoredModCount expected $expectedRestoredModCount from session MovedMods but found $actualRestoredModCount"
    }

    if ($sessionExists -and $hasRestoredCurrentRunCountField) {
        $expectedRestoredCurrentRunCount = @($movedCurrentRuns).Count
        $actualRestoredCurrentRunCount = [int]$restoreState.RestoredCurrentRunCount
        Add-Check -Name 'restore_current_run_count_matches_session_moved_runs' -Passed ($actualRestoredCurrentRunCount -eq $expectedRestoredCurrentRunCount) -Detail "RestoredCurrentRunCount expected $expectedRestoredCurrentRunCount from session MovedCurrentRuns but found $actualRestoredCurrentRunCount"
    }

    $settingsHash = Get-JsonStringOrEmpty -Object $restoreState -Name 'SettingsHashAfterRestore'
    $settingsBackupHash = Get-JsonStringOrEmpty -Object $restoreState -Name 'SettingsBackupHashAfterRestore'
    $hasRestoreHashes = -not [string]::IsNullOrWhiteSpace($settingsHash) -and -not [string]::IsNullOrWhiteSpace($settingsBackupHash)
    $restoreHashesAreSha256 = (Test-Sha256Hex -Value $settingsHash) -and (Test-Sha256Hex -Value $settingsBackupHash)
    Add-Check -Name 'restore_hashes_recorded' -Passed $hasRestoreHashes -Detail 'restore-state settings hashes must be recorded'
    Add-Check -Name 'restore_hashes_sha256_format' -Passed $restoreHashesAreSha256 -Detail 'restore-state settings hashes must be 64-character SHA256 hex strings'

    if ($hasRestoreHashes -and $restoreHashesAreSha256) {
        Add-Check -Name 'restore_settings_hashes_match' -Passed ($settingsHash -eq $settingsBackupHash) -Detail 'settings hash after restore must equal backup hash after restore'
    }
}

if ((Test-Path -LiteralPath $canonicalLogPath -PathType Leaf) -and (Test-Path -LiteralPath $canonicalAuditPath -PathType Leaf)) {
    $verifierParams = @{
        Mode = $Mode
        LogPath = $canonicalLogPath
        AuditPath = $canonicalAuditPath
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
        $verifierParams['ExpectedPackageVersion'] = $ExpectedPackageVersion
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
        $verifierParams['ExpectedRitsuCompatBranch'] = $ExpectedRitsuCompatBranch
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) {
        $verifierParams['ExpectedRitsuLibVersion'] = $ExpectedRitsuLibVersion
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
        $verifierParams['ExpectedGameVersion'] = $ExpectedGameVersion
    }

    if ($Mode -ne 'Off') {
        $verifierParams['OutFile'] = $enabledModeLogCheckPath
    }

    $verifierOutput = @(& $verifierPath @verifierParams 2>&1)
    $verifierMismatchLine = @($verifierOutput | Where-Object { "$_" -match '^mismatches=(\d+)$' } | Select-Object -Last 1)
    $verifierMismatchCount = $null
    $verifierMismatchLines = @($verifierMismatchLine)
    if ($verifierMismatchLines.Count -gt 0 -and "$($verifierMismatchLines[0])" -match '^mismatches=(\d+)$') {
        $verifierMismatchCount = [int]$Matches[1]
    }

    Add-Check -Name 'enabled_mode_log_verifier_ran' -Passed ($null -ne $verifierMismatchCount) -Detail 'enabled-mode verifier must emit mismatches count'
    if ($null -ne $verifierMismatchCount) {
        Add-Check -Name 'enabled_mode_log_verifier_clean' -Passed ($verifierMismatchCount -eq 0) -Detail "enabled-mode verifier mismatches=$verifierMismatchCount"
    }

    if ($Mode -ne 'Off') {
        Add-Check -Name 'enabled_mode_log_check_json_retained' -Passed (Test-Path -LiteralPath $enabledModeLogCheckPath -PathType Leaf) -Detail 'enabled-mode packet verifier must retain enabled-mode-log-check.json'
    }

    foreach ($line in $verifierOutput) {
        Write-Output "log_verifier $line"
    }
}

$report = [pscustomobject]@{
    Mode = $Mode
    EvidenceDir = $resolvedEvidenceDir
    CanonicalLogPath = $canonicalLogPath
    CanonicalAuditPath = $canonicalAuditPath
    CurrentSliceDerivedFromBeforeAfter = $currentSliceDerived
    CurrentSliceMatchesBeforeAfter = [bool]$currentSliceBinding.SliceMatches
    CurrentSliceBindingDetail = $currentSliceBinding.Detail
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedGameVersion = $ExpectedGameVersion
    Checks = $checks
    Mismatches = $mismatches
}

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

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
