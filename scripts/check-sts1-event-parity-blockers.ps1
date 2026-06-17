param(
    [string]$CanonicalMatrixPath = 'docs\features\sts1-events\canonical-event-matrix.csv',
    [string]$StatusBoardPath = 'docs\features\sts1-events\status-board.md',
    [string]$ContentParityPath = 'docs\features\sts1-events\content-parity-gaps.md',
    [string]$CombatBlockersPath = 'docs\features\sts1-events\combat-blockers-report.md',
    [string]$ModelsRoot = 'EZMicroBalanceCode\Sts1Events\Models',
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

function Get-MatrixRow {
    param([Parameter(Mandatory = $true)][string]$Id)

    $row = $matrixRows | Where-Object { $_.wiki_entry_id -eq $Id } | Select-Object -First 1
    if (-not $row) {
        Add-Check -Name "matrix_has_$Id" -Passed $false -Detail 'missing canonical matrix row'
        return $null
    }

    Add-Check -Name "matrix_has_$Id" -Passed $true -Detail 'canonical matrix row present'
    return $row
}

function Add-MatrixStatusCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Id,
        [Parameter(Mandatory = $true)][string]$ExpectedStatus
    )

    $row = Get-MatrixRow $Id
    if ($row) {
        Add-Check -Name "matrix_${Id}_status" -Passed ($row.status -eq $ExpectedStatus) -Detail "expected status $ExpectedStatus, found $($row.status)"
    }
}

$statusBoard = Read-RepoText $StatusBoardPath
$contentParity = Read-RepoText $ContentParityPath
$combatBlockers = Read-RepoText $CombatBlockersPath
$resolvedCanonicalMatrix = Resolve-RepoPath $CanonicalMatrixPath
$resolvedModelsRoot = Resolve-RepoPath $ModelsRoot

if (-not (Test-Path -LiteralPath $resolvedCanonicalMatrix)) {
    Write-Error "Canonical matrix not found: $resolvedCanonicalMatrix"
    exit 1
}

if (-not (Test-Path -LiteralPath $resolvedModelsRoot)) {
    Write-Error "Models root not found: $resolvedModelsRoot"
    exit 1
}

$matrixRows = @(Import-Csv -LiteralPath $resolvedCanonicalMatrix)

$releaseGateTemporarySubstitutes = @(
    'Golden Idol',
    'Face Trader',
    'Nest',
    'Vampires',
    'Mind Bloom',
    'Winding Halls'
)

$canonicalTemporarySubstituteIds = @(
    'sts1_face_trader',
    'sts1_nest',
    'sts1_vampires',
    'sts1_mind_bloom',
    'sts1_winding_halls'
)

$fullCombatBlockerIds = @(
    'sts1_dead_adventurer',
    'sts1_scorpion_nest',
    'sts1_treasure_ooze',
    'sts1_masked_bandits',
    'sts1_mysterious_sphere'
)

foreach ($id in $canonicalTemporarySubstituteIds) {
    Add-MatrixStatusCheck -Id $id -ExpectedStatus 'temporary-substitute'
}

$goldenIdol = Get-MatrixRow 'sts1_golden_idol'
if ($goldenIdol) {
    Add-Check -Name 'matrix_golden_idol_marks_relic_gap' -Passed ($goldenIdol.parity_gap -like '*Golden Idol relic*') -Detail "Golden Idol parity gap must mention missing Golden Idol relic; found '$($goldenIdol.parity_gap)'"
}

foreach ($id in $fullCombatBlockerIds) {
    Add-MatrixStatusCheck -Id $id -ExpectedStatus 'blocked'
    $row = Get-MatrixRow $id
    if ($row) {
        Add-Check -Name "matrix_${id}_missing_encounter_model" -Passed ($row.parity_gap -like '*Missing encounter model*') -Detail "expected Missing encounter model parity gap; found '$($row.parity_gap)'"
    }
}

Add-MatrixStatusCheck -Id 'sts1_nloth' -ExpectedStatus 'blocked'
$nloth = Get-MatrixRow 'sts1_nloth'
if ($nloth) {
    Add-Check -Name 'matrix_nloth_relic_select_blocked' -Passed ($nloth.parity_gap -like '*RelicSelectCmd*') -Detail "N'loth blocker must mention RelicSelectCmd; found '$($nloth.parity_gap)'"
}

foreach ($id in @('sts1_joust', 'sts1_the_ssssserpent')) {
    Add-MatrixStatusCheck -Id $id -ExpectedStatus 'compiled'
    $row = Get-MatrixRow $id
    if ($row) {
        Add-Check -Name "matrix_${id}_non_combat" -Passed ($row.parity_gap -like '*no combat branch*') -Detail "expected no combat branch note; found '$($row.parity_gap)'"
    }
}

Add-ContainsCheck -Name 'status_board_temporary_substitutes_count' -Text $statusBoard -Needle '### Temporary Substitutes (6)'
foreach ($eventName in $releaseGateTemporarySubstitutes) {
    Add-ContainsCheck -Name "status_board_lists_substitute_$($eventName.Replace(' ', '_'))" -Text $statusBoard -Needle "| $eventName |"
}

Add-ContainsCheck -Name 'status_board_blocked_partial_count' -Text $statusBoard -Needle '### Blocked / Partial Events (7 rows)'
foreach ($eventName in @('Dead Adventurer', 'Scorpion Nest', 'Treasure Ooze', 'Masked Bandits', 'Mysterious Sphere', 'Mind Bloom (War option)', "N'loth")) {
    Add-ContainsCheck -Name "status_board_lists_blocker_$($eventName.Replace(' ', '_'))" -Text $statusBoard -Needle "| $eventName |"
}

Add-ContainsCheck -Name 'status_board_non_combat_section' -Text $statusBoard -Needle '### Native-Equivalent Act 1 Non-Combat Events (2)'
Add-ContainsCheck -Name 'status_board_joust_non_combat' -Text $statusBoard -Needle '| Joust | compiled | Gold-bet event; no combat branch in current source. |'
Add-ContainsCheck -Name 'status_board_serpent_non_combat' -Text $statusBoard -Needle '| The Ssssserpent | compiled | Gold+curse trade; no combat branch in current source. |'

Add-ContainsCheck -Name 'content_parity_release_gate_note' -Text $contentParity -Needle 'Current release-gate non-parity rows'
Add-ContainsCheck -Name 'content_parity_dependency_totals_current' -Text $contentParity -Needle '35 native-equivalent / 4 direct game-object substitutes / 7 blocked / 1 custom-required'
Add-ContainsCheck -Name 'content_parity_warns_not_parity_complete' -Text $contentParity -Needle 'Do not call any of these parity-complete'
Add-ContainsCheck -Name 'content_parity_vampires_custom_required' -Text $contentParity -Needle 'Requires a custom Bite card model'
Add-ContainsCheck -Name 'content_parity_mind_bloom_blocked_partial' -Text $contentParity -Needle 'War option (fight Act 1 boss) has a BLOCKED/TODO stub'

Add-ContainsCheck -Name 'combat_report_full_blocker_count' -Text $combatBlockers -Needle '| Fully blocked combat events | 5 |'
Add-ContainsCheck -Name 'combat_report_partial_blocker_count' -Text $combatBlockers -Needle '| Partially blocked events | 1'
Add-ContainsCheck -Name 'combat_report_joust_not_combat' -Text $combatBlockers -Needle 'this event is NOT a combat event'
Add-ContainsCheck -Name 'combat_report_enter_combat_requirement' -Text $combatBlockers -Needle 'EnterCombatWithoutExitingEvent'

$sourceNeedles = @(
    @{ Name = 'source_golden_idol_random_relic_substitute'; Path = 'Shared\Sts1GoldenIdol.cs'; Needle = 'RelicFactory.PullNextRelicFromFront(owner)' },
    @{ Name = 'source_face_trader_random_relic_substitute'; Path = 'Shared\Sts1FaceTrader.cs'; Needle = 'temporary-substitute: StS1 face relics' },
    @{ Name = 'source_nest_clumsy_substitute'; Path = 'Act2\Sts1Nest.cs'; Needle = 'using Clumsy as substitute' },
    @{ Name = 'source_vampires_bite_missing'; Path = 'Act2\Sts1Vampires.cs'; Needle = 'temporary-substitute: Bite card does not exist' },
    @{ Name = 'source_winding_halls_debt_substitute'; Path = 'Act3\Sts1WindingHalls.cs'; Needle = 'using Debt as substitute' },
    @{ Name = 'source_mind_bloom_war_blocked'; Path = 'Act3\Sts1MindBloom.cs'; Needle = 'BLOCKED: Enter combat with random Act 1 boss requires encounter model.' },
    @{ Name = 'source_nloth_relic_select_blocked'; Path = 'Act2\Sts1Nloth.cs'; Needle = 'BLOCKED: No RelicSelectCmd API' },
    @{ Name = 'source_dead_adventurer_combat_todo'; Path = 'Act1\Sts1DeadAdventurer.cs'; Needle = 'TODO: Enter combat with random elite' },
    @{ Name = 'source_scorpion_nest_combat_todo'; Path = 'Act1\Sts1ScorpionNest.cs'; Needle = 'TODO: Enter combat with 3 Louses' },
    @{ Name = 'source_treasure_ooze_combat_todo'; Path = 'Act1\Sts1TreasureOoze.cs'; Needle = 'TODO: Enter combat with large slime' },
    @{ Name = 'source_masked_bandits_combat_todo'; Path = 'Act2\Sts1MaskedBandits.cs'; Needle = 'TODO: Enter combat with 3 bandits' },
    @{ Name = 'source_mysterious_sphere_combat_todo'; Path = 'Act3\Sts1MysteriousSphere.cs'; Needle = 'TODO: Enter combat with 2 Orb Walkers' }
)

foreach ($item in $sourceNeedles) {
    $sourcePath = Join-Path $resolvedModelsRoot $item.Path
    if (-not (Test-Path -LiteralPath $sourcePath)) {
        Add-Check -Name $item.Name -Passed $false -Detail "missing source file $sourcePath"
        continue
    }

    $source = [System.IO.File]::ReadAllText($sourcePath)
    Add-ContainsCheck -Name $item.Name -Text $source -Needle $item.Needle
}

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
