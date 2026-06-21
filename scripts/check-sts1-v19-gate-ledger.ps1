param(
    [string]$LedgerPath = 'docs\features\sts1-events\v19-gate-ledger.csv',
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

function Get-GateIdSetDetail {
    param([AllowEmptyCollection()][AllowNull()][string[]]$GateIds)

    if ($null -eq $GateIds -or $GateIds.Count -eq 0) {
        return ''
    }

    return (@($GateIds | Sort-Object { [int]($_ -replace '^O', '') }) -join ',')
}

function Add-GateIdSetCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [AllowEmptyCollection()][AllowNull()][object[]]$RowsForStatus,
        [Parameter(Mandatory = $true)][string[]]$ExpectedIds
    )

    $actualIds = @($RowsForStatus | Select-Object -ExpandProperty gate_id)
    $actualSet = Get-GateIdSetDetail -GateIds $actualIds
    $expectedSet = Get-GateIdSetDetail -GateIds $ExpectedIds

    Add-Check -Name $Name -Passed ($actualSet -eq $expectedSet) -Detail "expected $expectedSet but found $actualSet"
}

$resolvedLedger = Resolve-RepoPath $LedgerPath
if (-not (Test-Path -LiteralPath $resolvedLedger)) {
    Write-Error "Required ledger not found: $resolvedLedger"
    exit 1
}

$rows = @(Import-Csv -LiteralPath $resolvedLedger)
$expectedIds = @(0..76 | ForEach-Object { "O$_" })
$actualIds = @($rows | Select-Object -ExpandProperty gate_id)
$allowedStatuses = @(
    'historical-pass-current-paused',
    'previous-package-pass',
    'static-pass',
    'current-pass',
    'current-pending',
    'blocked',
    'source-guarded',
    'documentation-in-progress',
    'known-gap'
)

$expectedStatuses = @{
    O0 = 'historical-pass-current-paused'
    O1 = 'historical-pass-current-paused'
    O2 = 'historical-pass-current-paused'
    O3 = 'historical-pass-current-paused'
    O4 = 'historical-pass-current-paused'
    O5 = 'historical-pass-current-paused'
    O6 = 'static-pass'
    O7 = 'historical-pass-current-paused'
    O8 = 'historical-pass-current-paused'
    O9 = 'historical-pass-current-paused'
    O10 = 'current-pending'
    O11 = 'static-pass'
    O12 = 'static-pass'
    O13 = 'previous-package-pass'
    O14 = 'static-pass'
    O15 = 'static-pass'
    O16 = 'static-pass'
    O17 = 'source-guarded'
    O18 = 'static-pass'
    O19 = 'known-gap'
    O20 = 'static-pass'
    O21 = 'current-pass'
    O22 = 'current-pass'
    O23 = 'current-pass'
    O24 = 'current-pass'
    O25 = 'previous-package-pass'
    O26 = 'blocked'
    O27 = 'blocked'
    O28 = 'blocked'
    O29 = 'blocked'
    O30 = 'static-pass'
    O31 = 'blocked'
    O32 = 'blocked'
    O33 = 'current-pass'
    O34 = 'blocked'
    O35 = 'blocked'
    O36 = 'blocked'
    O37 = 'blocked'
    O38 = 'blocked'
    O39 = 'current-pass'
    O40 = 'blocked'
    O41 = 'blocked'
    O42 = 'blocked'
    O43 = 'blocked'
    O44 = 'blocked'
    O45 = 'blocked'
    O46 = 'blocked'
    O47 = 'blocked'
    O48 = 'blocked'
    O49 = 'blocked'
    O50 = 'blocked'
    O51 = 'blocked'
    O52 = 'blocked'
    O53 = 'source-guarded'
    O54 = 'blocked'
    O55 = 'blocked'
    O56 = 'blocked'
    O57 = 'blocked'
    O58 = 'blocked'
    O59 = 'static-pass'
    O60 = 'static-pass'
    O61 = 'static-pass'
    O62 = 'static-pass'
    O63 = 'static-pass'
    O64 = 'blocked'
    O65 = 'current-pending'
    O66 = 'documentation-in-progress'
    O67 = 'documentation-in-progress'
    O68 = 'documentation-in-progress'
    O69 = 'documentation-in-progress'
    O70 = 'documentation-in-progress'
    O71 = 'documentation-in-progress'
    O72 = 'current-pending'
    O73 = 'current-pending'
    O74 = 'current-pending'
    O75 = 'blocked'
    O76 = 'static-pass'
}

Add-Check -Name 'ledger_rows' -Passed ($rows.Count -eq 77) -Detail "expected 77 rows, found $($rows.Count)"
Add-Check -Name 'ledger_unique_gate_ids' -Passed ((@($actualIds | Sort-Object -Unique)).Count -eq $actualIds.Count) -Detail 'gate ids must be unique'

foreach ($expectedId in $expectedIds) {
    $matchingRows = @($rows | Where-Object { $_.gate_id -eq $expectedId })
    Add-Check -Name "gate_${expectedId}_exists" -Passed ($matchingRows.Count -eq 1) -Detail "$expectedId must exist exactly once"

    if ($matchingRows.Count -eq 1) {
        $row = $matchingRows[0]
        $expectedStatus = $expectedStatuses[$expectedId]
        Add-Check -Name "gate_${expectedId}_status" -Passed ($row.current_status -eq $expectedStatus) -Detail "$expectedId expected $expectedStatus but found $($row.current_status)"
        Add-Check -Name "gate_${expectedId}_has_requirement" -Passed (-not [string]::IsNullOrWhiteSpace($row.requirement)) -Detail "$expectedId must have requirement text"
        Add-Check -Name "gate_${expectedId}_has_evidence" -Passed (-not [string]::IsNullOrWhiteSpace($row.evidence)) -Detail "$expectedId must have evidence text"
        Add-Check -Name "gate_${expectedId}_has_next_action" -Passed (-not [string]::IsNullOrWhiteSpace($row.next_action)) -Detail "$expectedId must have next action text"
    }
}

$unknownStatuses = @($rows | Where-Object { $allowedStatuses -notcontains $_.current_status })
Add-Check -Name 'all_statuses_allowed' -Passed ($unknownStatuses.Count -eq 0) -Detail "unknown statuses: $((@($unknownStatuses | Select-Object -ExpandProperty current_status -Unique)) -join ',')"

$rowContentNeedles = @(
    [pscustomobject]@{ GateId = 'O0'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records beta.93 RitsuLib-only package/runtime baseline and requires exact HEAD/worktree recapture for later governance/test follow-up commits' },
    [pscustomobject]@{ GateId = 'O1'; Field = 'next_action'; Needle = 'Rerun build after any code/config change or before handoff' },
    [pscustomobject]@{ GateId = 'O2'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records current trusted split/focused lanes plus retained split coverage 475 / 0 / 21 / 496' },
    [pscustomobject]@{ GateId = 'O3'; Field = 'next_action'; Needle = 'Refresh skipped-test explanation with current test output before handoff' },
    [pscustomobject]@{ GateId = 'O4'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records beta.93 zero-warning validation' },
    [pscustomobject]@{ GateId = 'O5'; Field = 'next_action'; Needle = 'Rerun dotnet format after edits or before handoff' },
    [pscustomobject]@{ GateId = 'O6'; Field = 'evidence'; Needle = 'git diff --check is static and rerun during coordination pause' },
    [pscustomobject]@{ GateId = 'O7'; Field = 'next_action'; Needle = 'Refresh patch inventory after coordination pause' },
    [pscustomobject]@{ GateId = 'O8'; Field = 'next_action'; Needle = 'Refresh classifier after coordination pause' },
    [pscustomobject]@{ GateId = 'O9'; Field = 'evidence'; Needle = 'ZIP SHA 56636753F598B360B3798ED681ED84C3CA08CEC173E7EBA70134F4BC68EF964A' },
    [pscustomobject]@{ GateId = 'O10'; Field = 'next_action'; Needle = 'Owner/agent must classify exact scope before commit or handoff' },
    [pscustomobject]@{ GateId = 'O11'; Field = 'requirement'; Needle = 'Status board current and no generic Done' },
    [pscustomobject]@{ GateId = 'O12'; Field = 'requirement'; Needle = 'Act mapping' },
    [pscustomobject]@{ GateId = 'O12'; Field = 'evidence'; Needle = 'Overgrowth + Underdocks' },
    [pscustomobject]@{ GateId = 'O12'; Field = 'evidence'; Needle = '10 types / 14 calls' },
    [pscustomobject]@{ GateId = 'O13'; Field = 'next_action'; Needle = 'Do not extend Off proof to enabled modes' },
    [pscustomobject]@{ GateId = 'O14'; Field = 'requirement'; Needle = 'CanaryOnly source identity exact 4 event types / 6 calls' },
    [pscustomobject]@{ GateId = 'O14'; Field = 'next_action'; Needle = 'Keep source identity aligned with runtime verifier expectations' },
    [pscustomobject]@{ GateId = 'O15'; Field = 'requirement'; Needle = 'AdditiveBatch1 source identity exact 10 types / 14 calls' },
    [pscustomobject]@{ GateId = 'O15'; Field = 'next_action'; Needle = 'Keep source identity aligned with runtime verifier expectations' },
    [pscustomobject]@{ GateId = 'O16'; Field = 'requirement'; Needle = 'AdditiveAllDraft unsafe gate source guarded' },
    [pscustomobject]@{ GateId = 'O17'; Field = 'next_action'; Needle = 'Runtime replacement proof still required' },
    [pscustomobject]@{ GateId = 'O18'; Field = 'requirement'; Needle = 'Per-event spec registration notes current' },
    [pscustomobject]@{ GateId = 'O19'; Field = 'next_action'; Needle = 'Close 33-key gap only in versioned resource pass' },
    [pscustomobject]@{ GateId = 'O20'; Field = 'evidence'; Needle = 'check-sts1-event-static-suite.ps1' },
    [pscustomobject]@{ GateId = 'O21'; Field = 'requirement'; Needle = 'Current v0.107.1 game path and dependency path recorded' },
    [pscustomobject]@{ GateId = 'O21'; Field = 'evidence'; Needle = 'v0.107.1' },
    [pscustomobject]@{ GateId = 'O21'; Field = 'evidence'; Needle = 'beta.93' },
    [pscustomobject]@{ GateId = 'O22'; Field = 'requirement'; Needle = 'STS2-RitsuLib v0.4.32 direct NuGet runtime layout installed' },
    [pscustomobject]@{ GateId = 'O23'; Field = 'evidence'; Needle = '.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/godot.log.current-iteration' },
    [pscustomobject]@{ GateId = 'O23'; Field = 'evidence'; Needle = 'v0.107.1' },
    [pscustomobject]@{ GateId = 'O23'; Field = 'evidence'; Needle = 'v0.1.0-private-beta.93' },
    [pscustomobject]@{ GateId = 'O24'; Field = 'evidence'; Needle = 'runtime-evidence-packet-check.json checks=61 mismatches=0' },
    [pscustomobject]@{ GateId = 'O24'; Field = 'evidence'; Needle = 'enabled-mode verifier checks=31 mismatches=0' },
    [pscustomobject]@{ GateId = 'O24'; Field = 'next_action'; Needle = 'Preserve as current RitsuLib-only AdditiveBatch1 loader proof only' },
    [pscustomobject]@{ GateId = 'O25'; Field = 'requirement'; Needle = 'Retained v0.107 CanaryOnly enabled-mode smoke exact 4 event types / 6 calls' },
    [pscustomobject]@{ GateId = 'O25'; Field = 'evidence'; Needle = '.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/godot.log.after-launch' },
    [pscustomobject]@{ GateId = 'O25'; Field = 'evidence'; Needle = 'enabled-mode-log-check.json checks=20 mismatches=0' },
    [pscustomobject]@{ GateId = 'O25'; Field = 'next_action'; Needle = 'Preserve as beta.85 CanaryOnly loader proof only' },
    [pscustomobject]@{ GateId = 'O26'; Field = 'requirement'; Needle = 'Big Fish runtime screenshot/result proof' },
    [pscustomobject]@{ GateId = 'O26'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log' },
    [pscustomobject]@{ GateId = 'O27'; Field = 'requirement'; Needle = 'Golden Idol runtime screenshot/result proof' },
    [pscustomobject]@{ GateId = 'O27'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log' },
    [pscustomobject]@{ GateId = 'O28'; Field = 'requirement'; Needle = 'The Lab runtime screenshot/result proof' },
    [pscustomobject]@{ GateId = 'O28'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log' },
    [pscustomobject]@{ GateId = 'O29'; Field = 'requirement'; Needle = 'Divine Fountain runtime screenshot/result proof' },
    [pscustomobject]@{ GateId = 'O29'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log' },
    [pscustomobject]@{ GateId = 'O30'; Field = 'requirement'; Needle = 'Canary event code review remains current' },
    [pscustomobject]@{ GateId = 'O30'; Field = 'evidence'; Needle = 'canary-source-api-proof.md and static source/docs' },
    [pscustomobject]@{ GateId = 'O31'; Field = 'requirement'; Needle = 'Canary result logs for all four events' },
    [pscustomobject]@{ GateId = 'O31'; Field = 'evidence'; Needle = 'No current result logs' },
    [pscustomobject]@{ GateId = 'O32'; Field = 'requirement'; Needle = 'Canary pre/post state evidence' },
    [pscustomobject]@{ GateId = 'O32'; Field = 'evidence'; Needle = 'No current pre/post state evidence' },
    [pscustomobject]@{ GateId = 'O33'; Field = 'requirement'; Needle = 'Current v0.107.1 AdditiveBatch1 enabled-mode smoke exact 10 types / 14 calls' },
    [pscustomobject]@{ GateId = 'O33'; Field = 'evidence'; Needle = 'enabled-mode verifier checks=31 mismatches=0' },
    [pscustomobject]@{ GateId = 'O33'; Field = 'evidence'; Needle = 'runtime-evidence-packet-check.json checks=61 mismatches=0' },
    [pscustomobject]@{ GateId = 'O33'; Field = 'next_action'; Needle = 'Loader-registration proof only; gameplay/render/save-load rows remain separate' },
    [pscustomobject]@{ GateId = 'O34'; Field = 'requirement'; Needle = 'Canary save/load proof' },
    [pscustomobject]@{ GateId = 'O34'; Field = 'evidence'; Needle = 'No save/load evidence' },
    [pscustomobject]@{ GateId = 'O35'; Field = 'requirement'; Needle = 'Canary EN render proof' },
    [pscustomobject]@{ GateId = 'O35'; Field = 'evidence'; Needle = 'No current EN screenshot/render proof' },
    [pscustomobject]@{ GateId = 'O36'; Field = 'requirement'; Needle = 'Canary ZHS render proof' },
    [pscustomobject]@{ GateId = 'O36'; Field = 'evidence'; Needle = 'No current ZHS screenshot/render proof' },
    [pscustomobject]@{ GateId = 'O37'; Field = 'requirement'; Needle = 'Canary image/license/render decision' },
    [pscustomobject]@{ GateId = 'O37'; Field = 'evidence'; Needle = 'No redistributable StS1 event art decision' },
    [pscustomobject]@{ GateId = 'O38'; Field = 'requirement'; Needle = 'Canary parity-gap disposition' },
    [pscustomobject]@{ GateId = 'O38'; Field = 'evidence'; Needle = 'Golden Idol relic substitute and image gaps remain non-parity' },
    [pscustomobject]@{ GateId = 'O39'; Field = 'requirement'; Needle = 'Canary runtime audit packet complete' },
    [pscustomobject]@{ GateId = 'O39'; Field = 'evidence'; Needle = 'runtime-evidence-packet-check.json checks=45 mismatches=0' },
    [pscustomobject]@{ GateId = 'O40'; Field = 'requirement'; Needle = 'Canary gameplay notes integrated into docs' },
    [pscustomobject]@{ GateId = 'O40'; Field = 'evidence'; Needle = 'No current gameplay evidence to summarize' },
    [pscustomobject]@{ GateId = 'O41'; Field = 'requirement'; Needle = 'Canary owner/QA acceptance' },
    [pscustomobject]@{ GateId = 'O41'; Field = 'evidence'; Needle = 'No independent QA after runtime proof' },
    [pscustomobject]@{ GateId = 'O42'; Field = 'requirement'; Needle = 'Purifier runtime proof' },
    [pscustomobject]@{ GateId = 'O42'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O43'; Field = 'requirement'; Needle = 'Upgrade Shrine runtime proof' },
    [pscustomobject]@{ GateId = 'O43'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O44'; Field = 'requirement'; Needle = 'Golden Shrine runtime proof' },
    [pscustomobject]@{ GateId = 'O44'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O45'; Field = 'requirement'; Needle = 'The Cleric runtime proof' },
    [pscustomobject]@{ GateId = 'O45'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O46'; Field = 'requirement'; Needle = 'Old Beggar runtime proof' },
    [pscustomobject]@{ GateId = 'O46'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O47'; Field = 'requirement'; Needle = 'Shining Light runtime proof' },
    [pscustomobject]@{ GateId = 'O47'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log' },
    [pscustomobject]@{ GateId = 'O48'; Field = 'requirement'; Needle = 'Simple batch save/load proof' },
    [pscustomobject]@{ GateId = 'O48'; Field = 'evidence'; Needle = 'No current save/load evidence' },
    [pscustomobject]@{ GateId = 'O49'; Field = 'requirement'; Needle = 'Simple batch EN/ZHS render proof' },
    [pscustomobject]@{ GateId = 'O49'; Field = 'evidence'; Needle = 'No current bilingual render screenshots' },
    [pscustomobject]@{ GateId = 'O50'; Field = 'requirement'; Needle = 'Simple batch image/license/render decision' },
    [pscustomobject]@{ GateId = 'O50'; Field = 'evidence'; Needle = 'No redistributable StS1 event art decision' },
    [pscustomobject]@{ GateId = 'O51'; Field = 'requirement'; Needle = 'Simple batch runtime audit packet complete' },
    [pscustomobject]@{ GateId = 'O51'; Field = 'evidence'; Needle = 'Clean AdditiveBatch1 loader packet exists but no simple-batch gameplay packet' },
    [pscustomobject]@{ GateId = 'O52'; Field = 'requirement'; Needle = 'Simple batch independent QA' },
    [pscustomobject]@{ GateId = 'O52'; Field = 'evidence'; Needle = 'QA impossible before gameplay proof' },
    [pscustomobject]@{ GateId = 'O53'; Field = 'requirement'; Needle = 'ReplacementPrototype source gate present' },
    [pscustomobject]@{ GateId = 'O53'; Field = 'evidence'; Needle = 'feature-gate checker and replacement source' },
    [pscustomobject]@{ GateId = 'O53'; Field = 'next_action'; Needle = 'Keep debug/unsafe gate' },
    [pscustomobject]@{ GateId = 'O54'; Field = 'requirement'; Needle = 'Replacement unknown-room proof' },
    [pscustomobject]@{ GateId = 'O54'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence' },
    [pscustomobject]@{ GateId = 'O55'; Field = 'requirement'; Needle = 'Replacement act-bucket proof' },
    [pscustomobject]@{ GateId = 'O55'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence' },
    [pscustomobject]@{ GateId = 'O56'; Field = 'requirement'; Needle = 'Replacement event-bag/no-repeat proof' },
    [pscustomobject]@{ GateId = 'O56'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence' },
    [pscustomobject]@{ GateId = 'O57'; Field = 'requirement'; Needle = 'Replacement save/load proof' },
    [pscustomobject]@{ GateId = 'O57'; Field = 'evidence'; Needle = 'No debug runtime replacement save/load evidence' },
    [pscustomobject]@{ GateId = 'O58'; Field = 'requirement'; Needle = 'Multiplayer fail-closed runtime proof' },
    [pscustomobject]@{ GateId = 'O58'; Field = 'evidence'; Needle = 'No current multiplayer runtime proof' },
    [pscustomobject]@{ GateId = 'O59'; Field = 'requirement'; Needle = 'IsShared source matrix current' },
    [pscustomobject]@{ GateId = 'O59'; Field = 'evidence'; Needle = 'multiplayer-shape checker' },
    [pscustomobject]@{ GateId = 'O60'; Field = 'next_action'; Needle = 'Keep combat blocked until encounter models exist' },
    [pscustomobject]@{ GateId = 'O61'; Field = 'requirement'; Needle = 'Temporary substitutes marked non-parity' },
    [pscustomobject]@{ GateId = 'O62'; Field = 'requirement'; Needle = 'Content parity gaps current' },
    [pscustomobject]@{ GateId = 'O63'; Field = 'next_action'; Needle = 'Keep zero tracked StS1 original images' },
    [pscustomobject]@{ GateId = 'O64'; Field = 'requirement'; Needle = 'ZHS screenshots for StS1 events' },
    [pscustomobject]@{ GateId = 'O64'; Field = 'evidence'; Needle = 'No current ZHS runtime screenshots' },
    [pscustomobject]@{ GateId = 'O65'; Field = 'evidence'; Needle = 'No independent QA after current runtime evidence' },
    [pscustomobject]@{ GateId = 'O66'; Field = 'evidence'; Needle = 'docs/reviews/current-validation.md references v19 map' },
    [pscustomobject]@{ GateId = 'O67'; Field = 'evidence'; Needle = 'status-board records beta.93 RitsuLib-only AdditiveBatch1 current proof plus beta.85/beta.87/beta.88 previous-context loader proof' },
    [pscustomobject]@{ GateId = 'O68'; Field = 'evidence'; Needle = 'hard-stop report records coordination blocker' },
    [pscustomobject]@{ GateId = 'O69'; Field = 'evidence'; Needle = 'private-beta handoff references v19 map' },
    [pscustomobject]@{ GateId = 'O70'; Field = 'evidence'; Needle = 'release checklist references v19 map' },
    [pscustomobject]@{ GateId = 'O71'; Field = 'evidence'; Needle = 'hard-stop report owner actions listed' },
    [pscustomobject]@{ GateId = 'O72'; Field = 'requirement'; Needle = 'no unsupported commit decision' },
    [pscustomobject]@{ GateId = 'O73'; Field = 'requirement'; Needle = 'no unsupported push decision' },
    [pscustomobject]@{ GateId = 'O74'; Field = 'requirement'; Needle = 'final summary lists blocked gates' },
    [pscustomobject]@{ GateId = 'O75'; Field = 'next_action'; Needle = 'Do not mark event.md complete' },
    [pscustomobject]@{ GateId = 'O76'; Field = 'requirement'; Needle = 'hard-stop is not completion invariant preserved' }
)

foreach ($needle in $rowContentNeedles) {
    $row = $rows | Where-Object { $_.gate_id -eq $needle.GateId } | Select-Object -First 1
    $actual = if ($null -ne $row) { [string]$row.($needle.Field) } else { '' }
    $checkName = "gate_$($needle.GateId)_$($needle.Field)_contains_$($needle.Needle -replace '[^A-Za-z0-9]+', '_')"
    Add-Check -Name $checkName -Passed ($actual.Contains($needle.Needle)) -Detail "$($needle.GateId) $($needle.Field) must contain '$($needle.Needle)'"
}

$claimText = ($rows | ForEach-Object { "$($_.current_status) $($_.evidence) $($_.next_action)" }) -join "`n"
Add-Check -Name 'no_green_or_done_status_claims' -Passed (-not [regex]::IsMatch($claimText, '\b(GREEN|Done|release-ready|live-ready)\b', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) -Detail 'ledger must not contain green/done/release-ready/live-ready claims'

$blockedRows = @($rows | Where-Object { $_.current_status -eq 'blocked' })
$previousPackageRows = @($rows | Where-Object { $_.current_status -eq 'previous-package-pass' })
$currentPendingRows = @($rows | Where-Object { $_.current_status -eq 'current-pending' })
$currentPassRows = @($rows | Where-Object { $_.current_status -eq 'current-pass' })
$staticPassRows = @($rows | Where-Object { $_.current_status -eq 'static-pass' })
$sourceGuardedRows = @($rows | Where-Object { $_.current_status -eq 'source-guarded' })
$documentationRows = @($rows | Where-Object { $_.current_status -eq 'documentation-in-progress' })
$knownGapRows = @($rows | Where-Object { $_.current_status -eq 'known-gap' })
$historicalPausedRows = @($rows | Where-Object { $_.current_status -eq 'historical-pass-current-paused' })

Add-Check -Name 'blocked_gate_count' -Passed ($blockedRows.Count -eq 31) -Detail "expected 31 blocked gates, found $($blockedRows.Count)"
Add-Check -Name 'previous_package_gate_count' -Passed ($previousPackageRows.Count -eq 2) -Detail "expected 2 previous-package-pass gates, found $($previousPackageRows.Count)"
Add-Check -Name 'current_pending_gate_count' -Passed ($currentPendingRows.Count -eq 5) -Detail "expected 5 current-pending gates, found $($currentPendingRows.Count)"
Add-Check -Name 'current_pass_gate_count' -Passed ($currentPassRows.Count -eq 6) -Detail "expected 6 current-pass gates, found $($currentPassRows.Count)"
Add-Check -Name 'static_pass_gate_count' -Passed ($staticPassRows.Count -eq 15) -Detail "expected 15 static-pass gates, found $($staticPassRows.Count)"
Add-Check -Name 'source_guarded_gate_count' -Passed ($sourceGuardedRows.Count -eq 2) -Detail "expected 2 source-guarded gates, found $($sourceGuardedRows.Count)"
Add-Check -Name 'documentation_gate_count' -Passed ($documentationRows.Count -eq 6) -Detail "expected 6 documentation-in-progress gates, found $($documentationRows.Count)"
Add-Check -Name 'known_gap_gate_count' -Passed ($knownGapRows.Count -eq 1) -Detail "expected 1 known-gap gate, found $($knownGapRows.Count)"
Add-Check -Name 'historical_paused_gate_count' -Passed ($historicalPausedRows.Count -eq 9) -Detail "expected 9 historical-pass-current-paused gates, found $($historicalPausedRows.Count)"

$expectedBlockedGateIds = @(
    @(26..29 | ForEach-Object { "O$_" })
    'O31'
    'O32'
    @(34..38 | ForEach-Object { "O$_" })
    'O40'
    'O41'
    @(42..52 | ForEach-Object { "O$_" })
    @(54..58 | ForEach-Object { "O$_" })
    'O64'
    'O75'
)
$expectedCurrentPendingGateIds = @('O10', 'O65', 'O72', 'O73', 'O74')
$expectedPreviousPackageGateIds = @('O13', 'O25')
$expectedCurrentPassGateIds = @('O21', 'O22', 'O23', 'O24', 'O33', 'O39')
$expectedStaticPassGateIds = @('O6', 'O11', 'O12', 'O14', 'O15', 'O16', 'O18', 'O20', 'O30', 'O59', 'O60', 'O61', 'O62', 'O63', 'O76')
$expectedSourceGuardedGateIds = @('O17', 'O53')
$expectedDocumentationGateIds = @('O66', 'O67', 'O68', 'O69', 'O70', 'O71')
$expectedKnownGapGateIds = @('O19')
$expectedHistoricalPausedGateIds = @('O0', 'O1', 'O2', 'O3', 'O4', 'O5', 'O7', 'O8', 'O9')

Add-GateIdSetCheck -Name 'blocked_gate_ids' -RowsForStatus $blockedRows -ExpectedIds $expectedBlockedGateIds
Add-GateIdSetCheck -Name 'previous_package_gate_ids' -RowsForStatus $previousPackageRows -ExpectedIds $expectedPreviousPackageGateIds
Add-GateIdSetCheck -Name 'current_pending_gate_ids' -RowsForStatus $currentPendingRows -ExpectedIds $expectedCurrentPendingGateIds
Add-GateIdSetCheck -Name 'current_pass_gate_ids' -RowsForStatus $currentPassRows -ExpectedIds $expectedCurrentPassGateIds
Add-GateIdSetCheck -Name 'static_pass_gate_ids' -RowsForStatus $staticPassRows -ExpectedIds $expectedStaticPassGateIds
Add-GateIdSetCheck -Name 'source_guarded_gate_ids' -RowsForStatus $sourceGuardedRows -ExpectedIds $expectedSourceGuardedGateIds
Add-GateIdSetCheck -Name 'documentation_in_progress_gate_ids' -RowsForStatus $documentationRows -ExpectedIds $expectedDocumentationGateIds
Add-GateIdSetCheck -Name 'known_gap_gate_ids' -RowsForStatus $knownGapRows -ExpectedIds $expectedKnownGapGateIds
Add-GateIdSetCheck -Name 'historical_paused_gate_ids' -RowsForStatus $historicalPausedRows -ExpectedIds $expectedHistoricalPausedGateIds

Write-Output "ledger_path=$resolvedLedger"
Write-Output "ledger_rows=$($rows.Count)"
Write-Output "blocked_gates=$($blockedRows.Count)"
Write-Output "previous_package_gates=$($previousPackageRows.Count)"
Write-Output "current_pending_gates=$($currentPendingRows.Count)"
Write-Output "current_pass_gates=$($currentPassRows.Count)"
Write-Output "static_pass_gates=$($staticPassRows.Count)"
Write-Output "source_guarded_gates=$($sourceGuardedRows.Count)"
Write-Output "documentation_in_progress_gates=$($documentationRows.Count)"
Write-Output "known_gap_gates=$($knownGapRows.Count)"
Write-Output "historical_paused_gates=$($historicalPausedRows.Count)"

$report = [pscustomobject]@{
    LedgerPath = $resolvedLedger
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
