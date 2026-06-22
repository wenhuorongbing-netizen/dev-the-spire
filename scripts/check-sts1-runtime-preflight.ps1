param(
    [string]$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2',
    [string]$ExpectedGameVersion = 'v0.107.1',
    [string]$ExpectedRitsuLibVersion = '0.4.34',
    [string]$ExpectedRitsuCompatBranch = '0.107.1',
    [string]$ExpectedPackageVersion = 'v0.1.0-private-beta.123',
    [string]$ExpectedModId = 'EZMicroBalance',
    [string]$ExpectedRitsuModId = 'STS2-RitsuLib',
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
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

function Read-JsonFile {
    param([Parameter(Mandatory = $true)][string]$Path)

    return ([System.IO.File]::ReadAllText($Path) | ConvertFrom-Json)
}

function Get-KeyValue {
    param(
        [Parameter(Mandatory = $true)][object[]]$Lines,
        [Parameter(Mandatory = $true)][string]$Key
    )

    foreach ($line in $Lines) {
        $text = "$line"
        if ($text -match "^$([regex]::Escape($Key))=(.*)$") {
            return $Matches[1]
        }
    }

    return $null
}

function Invoke-ExpectedShape {
    param([Parameter(Mandatory = $true)][string]$Mode)

    $scriptPath = Join-Path $PSScriptRoot 'check-sts1-enabled-mode-runtime-log.ps1'
    if (-not (Test-Path -LiteralPath $scriptPath -PathType Leaf)) {
        Add-Check -Name "${Mode}_expected_shape_script_exists" -Passed $false -Detail "missing $scriptPath"
        return $null
    }

    $global:LASTEXITCODE = 0
    $output = @(& $scriptPath -Mode $Mode -PrintExpected 2>&1)
    $exitCode = if ($null -eq $LASTEXITCODE) { 0 } else { $LASTEXITCODE }

    Add-Check -Name "${Mode}_expected_shape_exit_zero" -Passed ($exitCode -eq 0) -Detail "exit_code=$exitCode"

    return [pscustomobject]@{
        Calls = Get-KeyValue -Lines $output -Key 'expected_registration_calls'
        Types = Get-KeyValue -Lines $output -Key 'expected_event_types'
        RuntimeStatus = Get-KeyValue -Lines $output -Key 'runtime_log_status'
        Mismatches = Get-KeyValue -Lines $output -Key 'mismatches'
    }
}

$resolvedGameRoot = [System.IO.Path]::GetFullPath($GameRoot)
$repoSpirePlusManifestPath = Join-Path $repoRoot 'EZMicroBalance.json'
$releaseInfoPath = Join-Path $resolvedGameRoot 'release_info.json'
$ritsuRoot = Join-Path $resolvedGameRoot 'mods\STS2-RitsuLib'
$ritsuManifestPath = Join-Path $ritsuRoot 'mod_manifest.json'
$ritsuCompatPath = Join-Path $ritsuRoot "lib\$ExpectedRitsuCompatBranch\compat-target.txt"
$ritsuDllPath = Join-Path $ritsuRoot "lib\$ExpectedRitsuCompatBranch\STS2-RitsuLib.dll"
$ritsuDirectDllPath = Join-Path $ritsuRoot 'STS2-RitsuLib.dll'
$spirePlusRoot = Join-Path $resolvedGameRoot 'mods\EZMicroBalance'
$spirePlusManifestPath = Join-Path $spirePlusRoot 'EZMicroBalance.json'

Write-Output "game_root=$resolvedGameRoot"
Write-Output "expected_game_version=$ExpectedGameVersion"
Write-Output "expected_ritsu_lib_version=$ExpectedRitsuLibVersion"
Write-Output "expected_ritsu_compat_branch=$ExpectedRitsuCompatBranch"
Write-Output "expected_package_version=$ExpectedPackageVersion"

Add-Check -Name 'repo_spire_plus_manifest_exists' -Passed (Test-Path -LiteralPath $repoSpirePlusManifestPath -PathType Leaf) -Detail $repoSpirePlusManifestPath
if (Test-Path -LiteralPath $repoSpirePlusManifestPath -PathType Leaf) {
    $repoSpireManifest = Read-JsonFile $repoSpirePlusManifestPath
    Write-Output "repo_spire_plus_manifest=$repoSpirePlusManifestPath"
    Write-Output "repo_spire_plus_id=$($repoSpireManifest.id)"
    Write-Output "repo_spire_plus_name=$($repoSpireManifest.name)"
    Write-Output "repo_spire_plus_version=$($repoSpireManifest.version)"
    Add-Check -Name 'repo_spire_plus_id_matches_expected' -Passed ("$($repoSpireManifest.id)" -eq $ExpectedModId) -Detail "actual=$($repoSpireManifest.id)"
    Add-Check -Name 'repo_spire_plus_name_is_player_facing' -Passed ("$($repoSpireManifest.name)" -eq 'Spire Plus') -Detail "actual=$($repoSpireManifest.name)"
    Add-Check -Name 'repo_spire_plus_version_matches_expected' -Passed ("$($repoSpireManifest.version)" -eq $ExpectedPackageVersion) -Detail "actual=$($repoSpireManifest.version)"
}

Add-Check -Name 'game_root_exists' -Passed (Test-Path -LiteralPath $resolvedGameRoot -PathType Container) -Detail $resolvedGameRoot
Add-Check -Name 'game_release_info_exists' -Passed (Test-Path -LiteralPath $releaseInfoPath -PathType Leaf) -Detail $releaseInfoPath
if (Test-Path -LiteralPath $releaseInfoPath -PathType Leaf) {
    $releaseInfo = Read-JsonFile $releaseInfoPath
    Write-Output "game_version=$($releaseInfo.version)"
    Add-Check -Name 'game_version_matches_expected' -Passed ("$($releaseInfo.version)" -eq $ExpectedGameVersion) -Detail "actual=$($releaseInfo.version)"
}

Add-Check -Name 'ritsu_manifest_exists' -Passed (Test-Path -LiteralPath $ritsuManifestPath -PathType Leaf) -Detail $ritsuManifestPath
if (Test-Path -LiteralPath $ritsuManifestPath -PathType Leaf) {
    $ritsuManifest = Read-JsonFile $ritsuManifestPath
    Write-Output "ritsu_manifest=$ritsuManifestPath"
    Write-Output "ritsu_id=$($ritsuManifest.id)"
    Write-Output "ritsu_version=$($ritsuManifest.version)"
    Add-Check -Name 'ritsu_id_matches_expected' -Passed ("$($ritsuManifest.id)" -eq $ExpectedRitsuModId) -Detail "actual=$($ritsuManifest.id)"
    Add-Check -Name 'ritsu_version_matches_expected' -Passed ("$($ritsuManifest.version)" -eq $ExpectedRitsuLibVersion) -Detail "actual=$($ritsuManifest.version)"
}

$hasRitsuCompatTarget = Test-Path -LiteralPath $ritsuCompatPath -PathType Leaf
$hasRitsuCompatDll = Test-Path -LiteralPath $ritsuDllPath -PathType Leaf
$hasRitsuDirectDll = Test-Path -LiteralPath $ritsuDirectDllPath -PathType Leaf

Add-Check -Name 'ritsu_runtime_dll_exists' -Passed ($hasRitsuCompatDll -or $hasRitsuDirectDll) -Detail "variant=$ritsuDllPath direct=$ritsuDirectDllPath"
Add-Check -Name 'ritsu_direct_or_variant_layout_detected' -Passed ($hasRitsuCompatTarget -or $hasRitsuDirectDll) -Detail "variant_target=$ritsuCompatPath direct=$ritsuDirectDllPath"
Add-Check -Name 'ritsu_compat_target_exists' -Passed ($hasRitsuCompatTarget -or $hasRitsuDirectDll) -Detail $ritsuCompatPath
if (Test-Path -LiteralPath $ritsuCompatPath -PathType Leaf) {
    $compatTarget = ([System.IO.File]::ReadAllText($ritsuCompatPath)).Trim()
    Write-Output "ritsu_compat_target=$compatTarget"
    Add-Check -Name 'ritsu_compat_target_matches_expected' -Passed ($compatTarget -eq $ExpectedRitsuCompatBranch) -Detail "actual=$compatTarget"
} elseif ($hasRitsuDirectDll) {
    Write-Output "ritsu_runtime_layout=direct-nuget"
    Write-Output "ritsu_direct_dll=$ritsuDirectDllPath"
}
Add-Check -Name 'ritsu_compat_dll_exists' -Passed ($hasRitsuCompatDll -or $hasRitsuDirectDll) -Detail $ritsuDllPath

Add-Check -Name 'spire_plus_manifest_exists' -Passed (Test-Path -LiteralPath $spirePlusManifestPath -PathType Leaf) -Detail $spirePlusManifestPath
if (Test-Path -LiteralPath $spirePlusManifestPath -PathType Leaf) {
    $spireManifest = Read-JsonFile $spirePlusManifestPath
    Write-Output "spire_plus_manifest=$spirePlusManifestPath"
    Write-Output "spire_plus_id=$($spireManifest.id)"
    Write-Output "spire_plus_name=$($spireManifest.name)"
    Write-Output "spire_plus_version=$($spireManifest.version)"
    Add-Check -Name 'spire_plus_id_matches_expected' -Passed ("$($spireManifest.id)" -eq $ExpectedModId) -Detail "actual=$($spireManifest.id)"
    Add-Check -Name 'spire_plus_name_is_player_facing' -Passed ("$($spireManifest.name)" -eq 'Spire Plus') -Detail "actual=$($spireManifest.name)"
    Add-Check -Name 'spire_plus_version_matches_expected' -Passed ("$($spireManifest.version)" -eq $ExpectedPackageVersion) -Detail "actual=$($spireManifest.version)"
}

$canaryShape = Invoke-ExpectedShape -Mode 'CanaryOnly'
if ($null -ne $canaryShape) {
    Write-Output "canary_expected_registration_calls=$($canaryShape.Calls)"
    Write-Output "canary_expected_event_types=$($canaryShape.Types)"
    Add-Check -Name 'canary_expected_shape_calls' -Passed ($canaryShape.Calls -eq '6') -Detail "actual=$($canaryShape.Calls)"
    Add-Check -Name 'canary_expected_shape_types' -Passed ($canaryShape.Types -eq '4') -Detail "actual=$($canaryShape.Types)"
    Add-Check -Name 'canary_expected_shape_source_only' -Passed ($canaryShape.RuntimeStatus -eq 'not-validated-print-expected-only') -Detail "runtime_log_status=$($canaryShape.RuntimeStatus)"
    Add-Check -Name 'canary_expected_shape_mismatches_zero' -Passed ($canaryShape.Mismatches -eq '0') -Detail "mismatches=$($canaryShape.Mismatches)"
}

$batch1Shape = Invoke-ExpectedShape -Mode 'AdditiveBatch1'
if ($null -ne $batch1Shape) {
    Write-Output "additive_batch1_expected_registration_calls=$($batch1Shape.Calls)"
    Write-Output "additive_batch1_expected_event_types=$($batch1Shape.Types)"
    Add-Check -Name 'additive_batch1_expected_shape_calls' -Passed ($batch1Shape.Calls -eq '14') -Detail "actual=$($batch1Shape.Calls)"
    Add-Check -Name 'additive_batch1_expected_shape_types' -Passed ($batch1Shape.Types -eq '10') -Detail "actual=$($batch1Shape.Types)"
    Add-Check -Name 'additive_batch1_expected_shape_source_only' -Passed ($batch1Shape.RuntimeStatus -eq 'not-validated-print-expected-only') -Detail "runtime_log_status=$($batch1Shape.RuntimeStatus)"
    Add-Check -Name 'additive_batch1_expected_shape_mismatches_zero' -Passed ($batch1Shape.Mismatches -eq '0') -Detail "mismatches=$($batch1Shape.Mismatches)"
}

$report = [pscustomobject]@{
    GameRoot = $resolvedGameRoot
    ExpectedGameVersion = $ExpectedGameVersion
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    ExpectedPackageVersion = $ExpectedPackageVersion
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
    $resolvedOutFile = if ([System.IO.Path]::IsPathRooted($OutFile)) {
        [System.IO.Path]::GetFullPath($OutFile)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutFile))
    }

    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
