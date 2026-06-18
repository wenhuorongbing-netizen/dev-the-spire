param(
    [string]$SourceRoot = 'source code',

    [string]$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2',

    [string]$GodotExe = '.tools\godot-4.5.1-mono\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe',

    [string]$GodotConsoleExe,

    [string]$RitsuLibRoot,

    [string]$ExpectedGameVersion,

    [string]$ExpectedRitsuLibVersion,

    [string]$ExpectedRitsuCompatBranch,

    [string]$ExpectedPackageVersion,

    [switch]$RequireCurrentSourceSnapshot,

    [switch]$RequireCleanGdreExport,

    [string]$OutFile,

    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

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
        [Parameter(Mandatory = $true)][string]$Detail,
        [ValidateSet('fail', 'warn')]
        [string]$Severity = 'fail'
    )

    $checks.Add([pscustomobject]@{
        Name = $Name
        Severity = $Severity
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null

    if ($Passed) {
        return
    }

    if ($Severity -eq 'warn') {
        $warnings.Add("${Name}: $Detail") | Out-Null
    } else {
        $mismatches.Add("${Name}: $Detail") | Out-Null
    }
}

function Read-JsonOrNull {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$CheckName
    )

    try {
        return Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        Add-Check -Name $CheckName -Passed $false -Detail "invalid JSON in $Path`: $($_.Exception.Message)"
        return $null
    }
}

function Normalize-Version {
    param([AllowEmptyString()][string]$Version)

    if ([string]::IsNullOrWhiteSpace($Version)) {
        return ''
    }

    $normalized = $Version.Trim()
    if ($normalized.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $normalized.Substring(1)
    }

    return $normalized
}

function Test-GitIgnored {
    param([Parameter(Mandatory = $true)][string]$Path)

    & git -C $repoRoot check-ignore -q -- $Path 2>$null
    return $LASTEXITCODE -eq 0
}

function Get-GitTrackedPathCount {
    param([Parameter(Mandatory = $true)][string]$Path)

    $tracked = @(& git -C $repoRoot ls-files -- $Path)
    return $tracked.Count
}

function Get-HashOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
}

function Get-FirstRegexGroup {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    $match = [regex]::Match($Text, $Pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    if (-not $match.Success -or $match.Groups.Count -lt 2) {
        return ''
    }

    return $match.Groups[1].Value
}

$sourceRootFull = Resolve-RepoPath -Path $SourceRoot
$gameRootFull = Resolve-RepoPath -Path $GameRoot
$godotExeFull = Resolve-RepoPath -Path $GodotExe
if (-not $GodotConsoleExe) {
    $godotDir = Split-Path -Parent $godotExeFull
    $godotLeaf = [System.IO.Path]::GetFileNameWithoutExtension($godotExeFull)
    $GodotConsoleExe = Join-Path $godotDir "$($godotLeaf)_console.exe"
}

$godotConsoleExeFull = Resolve-RepoPath -Path $GodotConsoleExe
if (-not $RitsuLibRoot) {
    $RitsuLibRoot = Join-Path $gameRootFull 'mods\STS2-RitsuLib'
}

$ritsuLibRootFull = Resolve-RepoPath -Path $RitsuLibRoot
$repoManifestPath = Join-Path $repoRoot 'EZMicroBalance.json'
$installedManifestPath = Join-Path $gameRootFull 'mods\EZMicroBalance\EZMicroBalance.json'
$sourceReleaseInfoPath = Join-Path $sourceRootFull 'release_info.json'
$gameReleaseInfoPath = Join-Path $gameRootFull 'release_info.json'
$sourceProjectPath = Join-Path $sourceRootFull 'project.godot'
$gdreExportLogPath = Join-Path $sourceRootFull 'gdre_export.log'
$ritsuManifestPath = Join-Path $ritsuLibRootFull 'mod_manifest.json'
$ritsuVariantsPath = Join-Path $ritsuLibRootFull 'ritsulib-variants.json'
$ritsuViewerPath = Join-Path $ritsuLibRootFull 'viewer\index.html'

Add-Check -Name 'source_root_exists' -Passed (Test-Path -LiteralPath $sourceRootFull -PathType Container) -Detail "expected recovered source folder at $sourceRootFull"
Add-Check -Name 'source_project_exists' -Passed (Test-Path -LiteralPath $sourceProjectPath -PathType Leaf) -Detail "expected Godot project file at $sourceProjectPath"
Add-Check -Name 'source_release_info_exists' -Passed (Test-Path -LiteralPath $sourceReleaseInfoPath -PathType Leaf) -Detail "expected source release_info.json at $sourceReleaseInfoPath"
Add-Check -Name 'installed_game_release_info_exists' -Passed (Test-Path -LiteralPath $gameReleaseInfoPath -PathType Leaf) -Detail "expected installed game release_info.json at $gameReleaseInfoPath"
Add-Check -Name 'godot_editor_exists' -Passed (Test-Path -LiteralPath $godotExeFull -PathType Leaf) -Detail "expected Godot editor executable at $godotExeFull"
Add-Check -Name 'godot_console_exists' -Passed (Test-Path -LiteralPath $godotConsoleExeFull -PathType Leaf) -Detail "expected Godot console executable at $godotConsoleExeFull"
Add-Check -Name 'gdre_export_log_exists' -Passed (Test-Path -LiteralPath $gdreExportLogPath -PathType Leaf) -Detail "expected GDRE export log at $gdreExportLogPath"
Add-Check -Name 'repo_manifest_exists' -Passed (Test-Path -LiteralPath $repoManifestPath -PathType Leaf) -Detail "expected repo manifest at $repoManifestPath"
Add-Check -Name 'installed_manifest_exists' -Passed (Test-Path -LiteralPath $installedManifestPath -PathType Leaf) -Detail "expected installed Spire Plus manifest at $installedManifestPath"

$sourceReleaseInfo = $null
$gameReleaseInfo = $null
$repoManifest = $null
$installedManifest = $null
if (Test-Path -LiteralPath $sourceReleaseInfoPath -PathType Leaf) {
    $sourceReleaseInfo = Read-JsonOrNull -Path $sourceReleaseInfoPath -CheckName 'source_release_info_json_valid'
}

if (Test-Path -LiteralPath $gameReleaseInfoPath -PathType Leaf) {
    $gameReleaseInfo = Read-JsonOrNull -Path $gameReleaseInfoPath -CheckName 'installed_game_release_info_json_valid'
}

if (Test-Path -LiteralPath $repoManifestPath -PathType Leaf) {
    $repoManifest = Read-JsonOrNull -Path $repoManifestPath -CheckName 'repo_manifest_json_valid'
}

if (Test-Path -LiteralPath $installedManifestPath -PathType Leaf) {
    $installedManifest = Read-JsonOrNull -Path $installedManifestPath -CheckName 'installed_manifest_json_valid'
}

$sourceVersion = if ($sourceReleaseInfo -and $sourceReleaseInfo.version) { [string]$sourceReleaseInfo.version } else { '' }
$gameVersion = if ($gameReleaseInfo -and $gameReleaseInfo.version) { [string]$gameReleaseInfo.version } else { '' }
$normalizedSourceVersion = Normalize-Version -Version $sourceVersion
$normalizedGameVersion = Normalize-Version -Version $gameVersion
$expectedGameNormalized = Normalize-Version -Version $ExpectedGameVersion

if ($sourceReleaseInfo) {
    Add-Check -Name 'source_release_info_json_valid' -Passed $true -Detail "source version=$sourceVersion commit=$($sourceReleaseInfo.commit)"
}

if ($gameReleaseInfo) {
    Add-Check -Name 'installed_game_release_info_json_valid' -Passed $true -Detail "installed game version=$gameVersion commit=$($gameReleaseInfo.commit)"
}

if ($repoManifest) {
    Add-Check -Name 'repo_manifest_json_valid' -Passed $true -Detail "repo package version=$($repoManifest.version)"
    Add-Check -Name 'repo_manifest_id_stable' -Passed ([string]$repoManifest.id -eq 'EZMicroBalance') -Detail "repo manifest id=$($repoManifest.id)"
    Add-Check -Name 'repo_manifest_name_spire_plus' -Passed ([string]$repoManifest.name -eq 'Spire Plus') -Detail "repo manifest name=$($repoManifest.name)"
}

if ($installedManifest) {
    Add-Check -Name 'installed_manifest_json_valid' -Passed $true -Detail "installed package version=$($installedManifest.version)"
    Add-Check -Name 'installed_manifest_id_stable' -Passed ([string]$installedManifest.id -eq 'EZMicroBalance') -Detail "installed manifest id=$($installedManifest.id)"
    Add-Check -Name 'installed_manifest_name_spire_plus' -Passed ([string]$installedManifest.name -eq 'Spire Plus') -Detail "installed manifest name=$($installedManifest.name)"
}

if ($repoManifest -and $installedManifest) {
    Add-Check -Name 'repo_installed_package_versions_match' -Passed ([string]$repoManifest.version -eq [string]$installedManifest.version) -Detail "repo version=$($repoManifest.version) installed version=$($installedManifest.version)"
}

if ($ExpectedPackageVersion -and $repoManifest) {
    Add-Check -Name 'repo_package_version_matches_expected' -Passed ([string]$repoManifest.version -eq $ExpectedPackageVersion) -Detail "repo version=$($repoManifest.version) expected=$ExpectedPackageVersion"
}

if ($ExpectedPackageVersion -and $installedManifest) {
    Add-Check -Name 'installed_package_version_matches_expected' -Passed ([string]$installedManifest.version -eq $ExpectedPackageVersion) -Detail "installed version=$($installedManifest.version) expected=$ExpectedPackageVersion"
}

if ($ExpectedGameVersion -and $normalizedGameVersion) {
    Add-Check -Name 'installed_game_version_matches_expected' -Passed ($normalizedGameVersion -eq $expectedGameNormalized) -Detail "installed version=$gameVersion expected=$ExpectedGameVersion"
}

if ($normalizedSourceVersion -and $normalizedGameVersion) {
    $sourceVersionSeverity = if ($RequireCurrentSourceSnapshot) { 'fail' } else { 'warn' }
    Add-Check `
        -Name 'source_version_matches_installed_game' `
        -Passed ($normalizedSourceVersion -eq $normalizedGameVersion) `
        -Detail "source version=$sourceVersion installed version=$gameVersion" `
        -Severity $sourceVersionSeverity
}

if (Test-Path -LiteralPath $sourceProjectPath -PathType Leaf) {
    $projectText = Get-Content -LiteralPath $sourceProjectPath -Raw -Encoding UTF8
    Add-Check -Name 'godot_project_has_csharp_feature' -Passed ($projectText.IndexOf('"C#"', [System.StringComparison]::OrdinalIgnoreCase) -ge 0 -or $projectText.IndexOf('_custom_features="dotnet"', [System.StringComparison]::OrdinalIgnoreCase) -ge 0) -Detail 'project.godot should declare C#/dotnet support'
    Add-Check -Name 'godot_project_has_45_feature' -Passed ($projectText.IndexOf('"4.5"', [System.StringComparison]::OrdinalIgnoreCase) -ge 0) -Detail 'project.godot should declare Godot 4.5 feature'
    Add-Check -Name 'godot_project_main_scene_exists' -Passed ($projectText.IndexOf('run/main_scene="res://scenes/game.tscn"', [System.StringComparison]::OrdinalIgnoreCase) -ge 0) -Detail 'project.godot should point at res://scenes/game.tscn'
}

$gdreSummary = [ordered]@{
    Exists = $false
    ToolVersion = ''
    EngineVersion = ''
    OpeningFile = ''
    ExtractedFiles = ''
    FailedScripts = ''
    ParseErrorCount = 0
    RecoveryFinished = $false
}

if (Test-Path -LiteralPath $gdreExportLogPath -PathType Leaf) {
    $gdreLog = Get-Content -LiteralPath $gdreExportLogPath -Raw -Encoding UTF8
    $gdreSummary.Exists = $true
    $gdreSummary.ToolVersion = Get-FirstRegexGroup -Text $gdreLog -Pattern 'GDRE Tools ([^\r\n]+)'
    $gdreSummary.EngineVersion = Get-FirstRegexGroup -Text $gdreLog -Pattern 'Detected Engine Version:\s*([^\r\n]+)'
    $gdreSummary.OpeningFile = Get-FirstRegexGroup -Text $gdreLog -Pattern 'Opening file:\s*([^\r\n]+)'
    $gdreSummary.ExtractedFiles = Get-FirstRegexGroup -Text $gdreLog -Pattern 'Extracted\s+([0-9]+)\s+files'
    $gdreSummary.FailedScripts = Get-FirstRegexGroup -Text $gdreLog -Pattern 'Failed scripts:\s*([0-9]+)'
    $gdreSummary.ParseErrorCount = [regex]::Matches($gdreLog, '(?im)^\s*ERROR:\s+Parse Error:').Count
    $gdreSummary.RecoveryFinished = $gdreLog.IndexOf('Recovery finished', [System.StringComparison]::OrdinalIgnoreCase) -ge 0

    Add-Check -Name 'gdre_log_recovery_finished' -Passed ([bool]$gdreSummary.RecoveryFinished) -Detail 'GDRE export log should contain Recovery finished'
    Add-Check -Name 'gdre_log_engine_version_godot_451' -Passed ([string]$gdreSummary.EngineVersion -eq '4.5.1') -Detail "GDRE detected engine version '$($gdreSummary.EngineVersion)'"
    $gdreCleanSeverity = if ($RequireCleanGdreExport) { 'fail' } else { 'warn' }
    $failedScriptsCount = if ($gdreSummary.FailedScripts) { [int]$gdreSummary.FailedScripts } else { -1 }
    Add-Check -Name 'gdre_log_failed_scripts_zero' -Passed ($failedScriptsCount -eq 0) -Detail "GDRE failed scripts=$failedScriptsCount" -Severity $gdreCleanSeverity
    Add-Check -Name 'gdre_log_parse_errors_zero' -Passed ([int]$gdreSummary.ParseErrorCount -eq 0) -Detail "GDRE parse errors=$($gdreSummary.ParseErrorCount)" -Severity $gdreCleanSeverity
}

$sourceRootIgnored = Test-GitIgnored -Path 'source code'
$toolsRootIgnored = Test-GitIgnored -Path '.tools'
$godotCacheIgnored = Test-GitIgnored -Path '.godot'
$sourceRootTrackedFileCount = Get-GitTrackedPathCount -Path 'source code'
$godotOpenProjectCommand = "Godot project reference: executable=$godotExeFull project=$sourceProjectPath"
$sourceSnapshotDisposition = if ($normalizedSourceVersion -and $normalizedGameVersion -and $normalizedSourceVersion -eq $normalizedGameVersion) {
    'current-source-match'
} elseif ($normalizedSourceVersion -and $normalizedGameVersion) {
    'historical-source-version-mismatch'
} else {
    'unknown-source-version'
}

Add-Check -Name 'source_root_is_git_ignored' -Passed $sourceRootIgnored -Detail 'source code/ must stay ignored local scratch'
Add-Check -Name 'tools_root_is_git_ignored' -Passed $toolsRootIgnored -Detail '.tools/ must stay ignored local tooling'
Add-Check -Name 'godot_cache_is_git_ignored' -Passed $godotCacheIgnored -Detail '.godot/ must stay ignored editor cache'
Add-Check -Name 'source_root_has_no_tracked_files' -Passed ($sourceRootTrackedFileCount -eq 0) -Detail 'source code/ must not contain tracked original game files'
Add-Check -Name 'godot_open_command_prepared' -Passed ((Test-Path -LiteralPath $godotExeFull -PathType Leaf) -and (Test-Path -LiteralPath $sourceProjectPath -PathType Leaf)) -Detail "open project reference: $godotOpenProjectCommand"

Add-Check -Name 'ritsulib_manifest_exists' -Passed (Test-Path -LiteralPath $ritsuManifestPath -PathType Leaf) -Detail "expected RitsuLib manifest at $ritsuManifestPath"
Add-Check -Name 'ritsulib_variants_exists' -Passed (Test-Path -LiteralPath $ritsuVariantsPath -PathType Leaf) -Detail "expected RitsuLib variants at $ritsuVariantsPath"
Add-Check -Name 'ritsulib_viewer_exists' -Passed (Test-Path -LiteralPath $ritsuViewerPath -PathType Leaf) -Detail 'RitsuLib viewer exists; it is a log viewer, not an unpacker or monkey runner'

$ritsuManifest = $null
$ritsuVariants = $null
if (Test-Path -LiteralPath $ritsuManifestPath -PathType Leaf) {
    $ritsuManifest = Read-JsonOrNull -Path $ritsuManifestPath -CheckName 'ritsulib_manifest_json_valid'
}

if (Test-Path -LiteralPath $ritsuVariantsPath -PathType Leaf) {
    $ritsuVariants = Read-JsonOrNull -Path $ritsuVariantsPath -CheckName 'ritsulib_variants_json_valid'
}

$ritsuVersion = if ($ritsuManifest -and $ritsuManifest.version) { [string]$ritsuManifest.version } else { '' }
if ($ritsuManifest) {
    Add-Check -Name 'ritsulib_manifest_json_valid' -Passed $true -Detail "RitsuLib version=$ritsuVersion"
}

$expectedRitsuVersionNormalized = Normalize-Version -Version $ExpectedRitsuLibVersion
if ($ExpectedRitsuLibVersion -and $ritsuVersion) {
    Add-Check -Name 'ritsulib_version_matches_expected' -Passed ((Normalize-Version -Version $ritsuVersion) -eq $expectedRitsuVersionNormalized) -Detail "RitsuLib version=$ritsuVersion expected=$ExpectedRitsuLibVersion"
}

$matchedVariant = $null
if ($ritsuVariants -and $ritsuVariants.variants -and $normalizedGameVersion) {
    $expectedCompat = if ($ExpectedRitsuCompatBranch) { Normalize-Version -Version $ExpectedRitsuCompatBranch } else { $normalizedGameVersion }
    $matchedVariant = @($ritsuVariants.variants | Where-Object {
        [string]$_.compatTarget -eq $expectedCompat
    } | Select-Object -First 1)

    Add-Check -Name 'ritsulib_variant_matches_installed_game' -Passed ($matchedVariant.Count -gt 0) -Detail "expected compat target $expectedCompat"
    if ($matchedVariant.Count -gt 0) {
        $variantDll = Join-Path $ritsuLibRootFull ([string]$matchedVariant[0].directory)
        $variantDll = Join-Path $variantDll ([string]$matchedVariant[0].assembly)
        Add-Check -Name 'ritsulib_variant_dll_exists' -Passed (Test-Path -LiteralPath $variantDll -PathType Leaf) -Detail "expected RitsuLib variant DLL at $variantDll"

        $compatTargetPath = Join-Path (Split-Path -Parent $variantDll) 'compat-target.txt'
        Add-Check -Name 'ritsulib_compat_target_file_exists' -Passed (Test-Path -LiteralPath $compatTargetPath -PathType Leaf) -Detail "expected compat-target.txt at $compatTargetPath"
        if (Test-Path -LiteralPath $compatTargetPath -PathType Leaf) {
            $compatTargetText = (Get-Content -LiteralPath $compatTargetPath -Raw -Encoding UTF8).Trim()
            Add-Check -Name 'ritsulib_compat_target_file_matches_variant' -Passed ($compatTargetText -eq [string]$matchedVariant[0].compatTarget) -Detail "compat-target.txt=$compatTargetText variant=$($matchedVariant[0].compatTarget)"
        }

        if ($matchedVariant[0].sha256) {
            $dllHash = Get-HashOrNull -Path $variantDll
            Add-Check -Name 'ritsulib_variant_dll_hash_matches' -Passed ($dllHash -eq [string]$matchedVariant[0].sha256) -Detail "variant hash=$dllHash expected=$($matchedVariant[0].sha256)"
        }
    }
}

$report = [pscustomobject]@{
    SchemaVersion = 1
    CreatedAt = (Get-Date).ToString('o')
    Passed = $mismatches.Count -eq 0
    RepoRoot = $repoRoot
    SourceRoot = $sourceRootFull
    GameRoot = $gameRootFull
    GodotExe = $godotExeFull
    GodotConsoleExe = $godotConsoleExeFull
    RitsuLibRoot = $ritsuLibRootFull
    Game = [pscustomobject]@{
        Version = $gameVersion
        Commit = if ($gameReleaseInfo -and $gameReleaseInfo.commit) { [string]$gameReleaseInfo.commit } else { '' }
    }
    SpirePlus = [pscustomobject]@{
        RepoVersion = if ($repoManifest -and $repoManifest.version) { [string]$repoManifest.version } else { '' }
        InstalledVersion = if ($installedManifest -and $installedManifest.version) { [string]$installedManifest.version } else { '' }
    }
    RitsuLib = [pscustomobject]@{
        Version = $ritsuVersion
        CompatBranch = if ($matchedVariant -and $matchedVariant.Count -gt 0) { [string]$matchedVariant[0].compatTarget } else { '' }
    }
    Godot = [pscustomobject]@{
        ExeExists = Test-Path -LiteralPath $godotExeFull -PathType Leaf
        ConsoleExeExists = Test-Path -LiteralPath $godotConsoleExeFull -PathType Leaf
        ExpectedVersion = '4.5.1'
        DetectedFromGdreLog = [string]$gdreSummary.EngineVersion
        OpenProjectCommand = $godotOpenProjectCommand
    }
    RecoveredSource = [pscustomobject]@{
        Version = $sourceVersion
        Commit = if ($sourceReleaseInfo -and $sourceReleaseInfo.commit) { [string]$sourceReleaseInfo.commit } else { '' }
        MatchesInstalledGame = $normalizedSourceVersion -and $normalizedGameVersion -and $normalizedSourceVersion -eq $normalizedGameVersion
        Disposition = $sourceSnapshotDisposition
        FailedScripts = $gdreSummary.FailedScripts
        ParseErrors = $gdreSummary.ParseErrorCount
    }
    GitProtection = [pscustomobject]@{
        SourceCodeIgnored = $sourceRootIgnored
        ToolsIgnored = $toolsRootIgnored
        GodotCacheIgnored = $godotCacheIgnored
        SourceCodeTrackedFileCount = $sourceRootTrackedFileCount
    }
    EvidenceUsePolicy = [pscustomobject]@{
        NoLaunch = $true
        NotRuntimeProof = $true
        LocalSourceReferenceOnly = $true
        AuthorizedLocalInstallOnly = $true
        ThirdPartyDumpsProhibited = $true
        SourceRootMustStayIgnored = $true
        OriginalGameSourceMustNotBeTracked = $true
        RitsuLibViewerIsLogViewerOnly = $true
        RefreshSourceSnapshotBeforeCurrentApiClaims = $sourceSnapshotDisposition -ne 'current-source-match'
        RuntimeProofStillRequiresLaunchEvidence = $true
        AllowedRecordedEvidence = @(
            'short signatures',
            'local paths',
            'observed version metadata',
            'hashes',
            'conclusions'
        )
        ProhibitedTrackedEvidence = @(
            'original game source files',
            'extracted serialized game resources',
            'large decompiled code chunks',
            'unlicensed original game art'
        )
    }
    SourceVersion = $sourceVersion
    InstalledGameVersion = $gameVersion
    RitsuLibVersion = $ritsuVersion
    GdreExport = [pscustomobject]$gdreSummary
    Checks = $checks
    Warnings = $warnings
    Mismatches = $mismatches
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) severity=$($check.Severity) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "warnings=$($warnings.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($warning in $warnings) {
    Write-Output "warning $warning"
}

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath -Path $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
