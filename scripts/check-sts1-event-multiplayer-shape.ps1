param(
    [string]$ModelsRoot = 'EZMicroBalanceCode\Sts1Events\Models',
    [string]$ProjectPath = 'EZMicroBalance.csproj',
    [string]$MatrixPath = 'docs\features\sts1-events\multiplayer-is-shared-matrix.md',
    [string]$FailClosedPath = 'docs\features\sts1-events\multiplayer-fail-closed-guard.md',
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
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

function Add-ContainsCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    Add-Check -Name $Name -Passed ($Text.Contains($Needle)) -Detail "requires '$Needle'"
}

function Get-ModelSource {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $path = Join-Path $resolvedModelsRoot $RelativePath
    if (-not (Test-Path -LiteralPath $path)) {
        Add-Check -Name "source_exists_$($RelativePath.Replace('\', '_'))" -Passed $false -Detail "missing source $path"
        return ''
    }

    Add-Check -Name "source_exists_$($RelativePath.Replace('\', '_'))" -Passed $true -Detail 'source exists'
    return [System.IO.File]::ReadAllText($path)
}

$resolvedModelsRoot = Resolve-RepoPath $ModelsRoot
if (-not (Test-Path -LiteralPath $resolvedModelsRoot)) {
    Write-Error "Models root not found: $resolvedModelsRoot"
    exit 1
}

$project = Read-RepoText $ProjectPath
$matrix = Read-RepoText $MatrixPath
$failClosed = Read-RepoText $FailClosedPath

$modelFiles = @(Get-ChildItem -LiteralPath $resolvedModelsRoot -Recurse -Filter '*.cs' -File)
$sharedFiles = @(Get-ChildItem -LiteralPath (Join-Path $resolvedModelsRoot 'Shared') -Filter '*.cs' -File)
$act1Files = @(Get-ChildItem -LiteralPath (Join-Path $resolvedModelsRoot 'Act1') -Filter '*.cs' -File)
$act2Files = @(Get-ChildItem -LiteralPath (Join-Path $resolvedModelsRoot 'Act2') -Filter '*.cs' -File)
$act3Files = @(Get-ChildItem -LiteralPath (Join-Path $resolvedModelsRoot 'Act3') -Filter '*.cs' -File)

$compileExcludedMatches = [regex]::Matches($project, '<Compile\s+Remove="([^"]*Sts1Events/Models/[^"]+\.cs)"\s*/>')
$compileExcludedModelFiles = @($compileExcludedMatches | ForEach-Object { [System.IO.Path]::GetFileName($_.Groups[1].Value) } | Sort-Object -Unique)

$trueFiles = @()
$falseFiles = @()
foreach ($file in $modelFiles) {
    $source = [System.IO.File]::ReadAllText($file.FullName)
    if ($source.Contains('public override bool IsShared => true;')) {
        $trueFiles += $file
    } else {
        $falseFiles += $file
    }
}

Add-Check -Name 'model_files' -Passed ($modelFiles.Count -eq 48) -Detail "expected 48 model files, found $($modelFiles.Count)"
Add-Check -Name 'compiling_model_files' -Passed (($modelFiles.Count - $compileExcludedModelFiles.Count) -eq 47) -Detail "expected 47 compiling model files, found $($modelFiles.Count - $compileExcludedModelFiles.Count)"
Add-Check -Name 'compile_excluded_models' -Passed (($compileExcludedModelFiles -join ',') -eq 'Sts1Duplicator.cs') -Detail "expected Sts1Duplicator.cs compile-excluded, found '$($compileExcludedModelFiles -join ',')'"
Add-Check -Name 'shared_directory_file_count' -Passed ($sharedFiles.Count -eq 18) -Detail "expected 18 Shared files, found $($sharedFiles.Count)"
Add-Check -Name 'act1_file_count' -Passed ($act1Files.Count -eq 7) -Detail "expected 7 Act1 files, found $($act1Files.Count)"
Add-Check -Name 'act2_file_count' -Passed ($act2Files.Count -eq 14) -Detail "expected 14 Act2 files, found $($act2Files.Count)"
Add-Check -Name 'act3_file_count' -Passed ($act3Files.Count -eq 9) -Detail "expected 9 Act3 files, found $($act3Files.Count)"
Add-Check -Name 'is_shared_true_file_count' -Passed ($trueFiles.Count -eq 24) -Detail "expected 24 IsShared=true model files, found $($trueFiles.Count)"
Add-Check -Name 'is_shared_false_file_count' -Passed ($falseFiles.Count -eq 24) -Detail "expected 24 default-false model files, found $($falseFiles.Count)"

$sharedWithoutTrue = @($sharedFiles | Where-Object { -not ([System.IO.File]::ReadAllText($_.FullName).Contains('public override bool IsShared => true;')) })
$sharedWithoutTrueNames = @($sharedWithoutTrue | ForEach-Object { $_.Name })
Add-Check -Name 'all_shared_directory_models_are_is_shared_true' -Passed ($sharedWithoutTrue.Count -eq 0) -Detail "Shared files without IsShared=true: $($sharedWithoutTrueNames -join ',')"

$combatModels = @(
    'Act1\Sts1DeadAdventurer.cs',
    'Act1\Sts1ScorpionNest.cs',
    'Act1\Sts1TreasureOoze.cs',
    'Act2\Sts1MaskedBandits.cs',
    'Act3\Sts1MindBloom.cs',
    'Act3\Sts1MysteriousSphere.cs'
)

foreach ($relativePath in $combatModels) {
    $source = Get-ModelSource $relativePath
    Add-Check -Name "combat_${relativePath}_is_shared_true".Replace('\', '_').Replace('.', '_') -Passed ($source.Contains('public override bool IsShared => true;')) -Detail "$relativePath must declare IsShared=true"
    Add-Check -Name "combat_${relativePath}_documents_enter_combat".Replace('\', '_').Replace('.', '_') -Passed ($source.Contains('EnterCombatWithoutExitingEvent')) -Detail "$relativePath must document EnterCombatWithoutExitingEvent requirement or TODO"
}

foreach ($relativePath in @('Act1\Sts1Joust.cs', 'Act1\Sts1TheSsssserpent.cs')) {
    $source = Get-ModelSource $relativePath
    Add-Check -Name "noncombat_${relativePath}_is_default_false".Replace('\', '_').Replace('.', '_') -Passed (-not $source.Contains('public override bool IsShared => true;')) -Detail "$relativePath should remain default IsShared=false"
    Add-Check -Name "noncombat_${relativePath}_has_no_enter_combat".Replace('\', '_').Replace('.', '_') -Passed (-not $source.Contains('EnterCombatWithoutExitingEvent')) -Detail "$relativePath should not enter combat"
}

Add-ContainsCheck -Name 'matrix_total_model_count_current' -Text $matrix -Needle '| Total event models in this matrix | 48 |'
Add-ContainsCheck -Name 'matrix_compiling_model_count_current' -Text $matrix -Needle '| Compiling event models in this matrix | 47 |'
Add-ContainsCheck -Name 'matrix_is_shared_true_count_current' -Text $matrix -Needle '| **IsShared = true** | **24**'
Add-ContainsCheck -Name 'matrix_is_shared_false_count_current' -Text $matrix -Needle '| **IsShared = false** | **24**'
Add-ContainsCheck -Name 'matrix_current_source_inventory_note' -Text $matrix -Needle 'Current source inventory: 48 model files, 47 compiling'
Add-ContainsCheck -Name 'matrix_shared_includes_purifier' -Text $matrix -Needle 'Sts1Purifier.cs'
Add-ContainsCheck -Name 'matrix_shared_includes_golden_shrine' -Text $matrix -Needle 'Sts1GoldenShrine.cs'
Add-ContainsCheck -Name 'matrix_joust_debatable_noncombat' -Text $matrix -Needle '| Joust | Combat | `false` |'
Add-ContainsCheck -Name 'matrix_serpent_debatable_noncombat' -Text $matrix -Needle '| The Ssssserpent | Combat | `false` |'
Add-ContainsCheck -Name 'matrix_runtime_not_live_proof' -Text $matrix -Needle 'It is not live co-op proof'

Add-ContainsCheck -Name 'fail_closed_current_runtime_pending' -Text $failClosed -Needle 'Current runtime/co-op fail-closed: PENDING'
Add-ContainsCheck -Name 'fail_closed_source_level_verified_only' -Text $failClosed -Needle 'Source-level fail-closed: VERIFIED by guard tests.'
Add-ContainsCheck -Name 'fail_closed_current_off_not_coop' -Text $failClosed -Needle 'previous beta.93 `v0.107.1` Off and AdditiveBatch1 loader proof is clean, but it is not co-op or gameplay evidence, and CanaryOnly-specific claims require fresh current-version proof.'

$report = [pscustomobject]@{
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

    $report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
