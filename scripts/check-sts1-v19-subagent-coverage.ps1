param(
    [string]$GoalPath = 'docs\goals\event.md',
    [string]$CoveragePath = 'docs\features\sts1-events\v19-subagent-coverage.md',
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

$goalText = Read-RepoText $GoalPath
$coverageText = Read-RepoText $CoveragePath
$ledgerRows = @(Import-Csv -LiteralPath (Resolve-RepoPath $LedgerPath))

$requiredRoles = @(
    'BuildGate / Repo Health',
    'Runtime Environment Bootstrap',
    'Enabled-Mode Loader Subagent',
    'Wiki Parity Spec Auditor',
    'StS2 Source/API Auditor',
    'Feature Gate / Registration Engineer',
    'Canary Gameplay Subagent',
    'Simple Batch Gameplay Subagent',
    'Localization Gap Closure Subagent',
    'Asset + Image Subagent',
    'Event Pool / RNG / Save Subagent',
    'Multiplayer / IsShared Subagent',
    'Content Parity Subagent',
    'QA / Red-Team Subagent',
    'Release Documentation Subagent'
)

Add-ContainsCheck -Name 'goal_requires_subagents' -Text $goalText -Needle 'Enabled-Mode Loader Subagent'
Add-ContainsCheck -Name 'coverage_non_completion_claim' -Text $coverageText -Needle 'This is a coverage ledger, not a completion claim.'
Add-ContainsCheck -Name 'coverage_current_thread_static_only' -Text $coverageText -Needle 'read-only explorer subagents for static sidecar audits only'
Add-ContainsCheck -Name 'coverage_current_20260615_pause_audit' -Text $coverageText -Needle '2026-06-15 current-proof and coordination-pause wording audit'
Add-ContainsCheck -Name 'coverage_current_20260617_role_audit' -Text $coverageText -Needle '2026-06-17 subagent role coverage audit against `docs/goals/event.md` and `scripts/check-sts1-v19-subagent-coverage.ps1`'
Add-ContainsCheck -Name 'coverage_runtime_pause_boundary' -Text $coverageText -Needle 'The current coordination pause allows only read-only/static work'
Add-ContainsCheck -Name 'coverage_no_runtime_proof_claim' -Text $coverageText -Needle 'These audits did not create loader, gameplay, replacement, multiplayer, or QA proof.'
Add-ContainsCheck -Name 'coverage_shared_additive_loader_proof' -Text $coverageText -Needle 'later shared validation supplied separate CanaryOnly loader proof and AdditiveBatch1 loader/registration proof.'
Add-ContainsCheck -Name 'coverage_no_independent_qa_claim' -Text $coverageText -Needle 'Read-only explorer audits are not independent QA/Red-Team acceptance.'
Add-ContainsCheck -Name 'coverage_no_commit_push_release_authorization' -Text $coverageText -Needle 'This ledger does not authorize commit, push, release, or private-beta readiness claims.'
Add-ContainsCheck -Name 'coverage_post_pause_packet_checklist' -Text $coverageText -Needle '## Post-Pause Evidence Packet Checklist'
Add-ContainsCheck -Name 'coverage_packet_checklist_future_only' -Text $coverageText -Needle 'The retained CanaryOnly and AdditiveBatch1 packets are loader evidence only; missing future gameplay packets keep the mapped gameplay, replacement, multiplayer, QA, and handoff gates open.'
Add-ContainsCheck -Name 'coverage_canary_packet_required_files' -Text $coverageText -Needle 'Retained CanaryOnly packet at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` with `session-state.json`, `settings.save.before`, `game-release-info.json`, `godot.log.after-launch`, `godot-log-audit.json`, `enabled-mode-log-check.json`, `runtime-evidence-packet-check.json`, and `restore-state.json`'
Add-ContainsCheck -Name 'coverage_enabled_log_expected_switches' -Text $coverageText -Needle 'copied-log verifier used `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, `-ExpectedGameVersion`, `-OutFile`, and `-FailOnMismatch`'
Add-ContainsCheck -Name 'coverage_enabled_packet_metadata' -Text $coverageText -Needle 'packet verifier shows matching `Sts1EventModeEnvironment`, explicit package/Ritsu-compat/RitsuLib-version/game-version targets, no unsafe-mode env leakage'
Add-ContainsCheck -Name 'coverage_enabled_packet_no_legacy_bypass' -Text $coverageText -Needle 'no `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass'
Add-ContainsCheck -Name 'coverage_copied_log_class_only_boundary' -Text $coverageText -Needle 'Copied-log proof covers registration-call count and class set; Act-target tuple proof remains source-derived until future logs or gameplay evidence prove those targets directly.'
Add-ContainsCheck -Name 'coverage_canary_expected_runtime_shape' -Text $coverageText -Needle 'CanaryOnly 4 event types / 6 registration calls'
Add-ContainsCheck -Name 'coverage_additive_expected_runtime_shape' -Text $coverageText -Needle 'AdditiveBatch1 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'coverage_canary_gameplay_packet' -Text $coverageText -Needle 'Big Fish, Golden Idol, The Lab, and Divine Fountain screenshots, result logs, pre/post state notes, save-load proof, EN/ZHS render screenshots, and image/license disposition after CanaryOnly proof exists.'
Add-ContainsCheck -Name 'coverage_additive_retained_loader_packet' -Text $coverageText -Needle 'Previous beta.93 AdditiveBatch1 packet at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`'
Add-ContainsCheck -Name 'coverage_additive_loader_only_boundary' -Text $coverageText -Needle 'This closes O33 as loader/registration proof only; O51 still requires gameplay packet proof.'
Add-ContainsCheck -Name 'coverage_simple_batch_gameplay_packet' -Text $coverageText -Needle 'Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, and Shining Light screenshots, result logs, save-load proof, EN/ZHS render screenshots, image/license disposition, and independent QA with AdditiveBatch1 loader proof already retained.'
Add-ContainsCheck -Name 'coverage_direct_key_not_enabled_mode_proof' -Text $coverageText -Needle 'Closing only the direct Golden Idol key is not O33 AdditiveBatch1 proof or gameplay proof.'
Add-ContainsCheck -Name 'coverage_replacement_packet' -Text $coverageText -Needle 'Owner-approved debug/unsafe replacement packet proving unknown-room draw, act bucket, event-bag/no-repeat behavior, and save-load stability.'
Add-ContainsCheck -Name 'coverage_multiplayer_fail_closed_packet' -Text $coverageText -Needle 'Runtime fail-closed multiplayer proof before any multiplayer gameplay claim'
Add-ContainsCheck -Name 'coverage_qa_after_runtime_packets' -Text $coverageText -Needle 'Independent pass/fail QA only after current runtime/gameplay packets exist'
Add-ContainsCheck -Name 'coverage_owner_final_handoff_packet' -Text $coverageText -Needle 'Owner / Final Handoff | Make explicit owner decisions for commit/push scope, final blocked-gate summary, and the all-gates-green-before-completion invariant. | O72-O76'

foreach ($role in $requiredRoles) {
    $safeName = $role -replace '[^A-Za-z0-9]+', '_'
    Add-ContainsCheck -Name "goal_role_${safeName}" -Text $goalText -Needle $role
    Add-ContainsCheck -Name "coverage_role_${safeName}" -Text $coverageText -Needle "| $role |"
}

$blockedOrPendingRows = @($ledgerRows | Where-Object {
    $_.current_status -in @('blocked', 'current-pending', 'documentation-in-progress', 'known-gap', 'source-guarded')
})
$additiveLoaderRow = @($ledgerRows | Where-Object { $_.gate_id -eq 'O33' })
$runtimeProofRows = @($ledgerRows | Where-Object { $_.gate_id -in @('O41', 'O52', 'O58', 'O65', 'O75') })
$runtimeProofRowsStillOpen = @($runtimeProofRows | Where-Object { $_.current_status -ne 'current-pass' -and $_.current_status -ne 'static-pass' })

Add-Check -Name 'additive_loader_gate_current_pass' -Passed ($additiveLoaderRow.Count -eq 1 -and $additiveLoaderRow[0].current_status -eq 'current-pass') -Detail 'O33 must remain current-pass for beta.93 loader/registration proof'
Add-Check -Name 'runtime_subagent_gates_still_open' -Passed ($runtimeProofRowsStillOpen.Count -eq $runtimeProofRows.Count) -Detail 'gameplay/QA/handoff runtime gates must remain open until evidence exists'
Add-Check -Name 'ledger_has_open_subagent_related_rows' -Passed ($blockedOrPendingRows.Count -gt 0) -Detail 'ledger must still expose blocked/current-pending rows'

Add-ContainsCheck -Name 'coverage_buildgate_paused' -Text $coverageText -Needle 'BuildGate / Repo Health | runtime-validation-paused'
Add-ContainsCheck -Name 'coverage_runtime_bootstrap_paused' -Text $coverageText -Needle 'Runtime Environment Bootstrap | runtime-validation-paused / shared-loader-pass'
Add-ContainsCheck -Name 'coverage_enabled_loader_shared_pass' -Text $coverageText -Needle 'Enabled-Mode Loader Subagent | shared-loader-pass'
Add-ContainsCheck -Name 'coverage_canary_runtime_blocked' -Text $coverageText -Needle 'Canary Gameplay Subagent | runtime-blocked'
Add-ContainsCheck -Name 'coverage_simple_batch_runtime_blocked' -Text $coverageText -Needle 'Simple Batch Gameplay Subagent | runtime-blocked'
Add-ContainsCheck -Name 'coverage_replacement_runtime_blocked' -Text $coverageText -Needle 'Event Pool / RNG / Save Subagent | runtime-blocked'
Add-ContainsCheck -Name 'coverage_multiplayer_runtime_blocked' -Text $coverageText -Needle 'Multiplayer / IsShared Subagent | runtime-blocked'
Add-ContainsCheck -Name 'coverage_qa_blocked' -Text $coverageText -Needle 'QA / Red-Team Subagent | blocked'
Add-ContainsCheck -Name 'coverage_release_docs_in_progress' -Text $coverageText -Needle 'Release Documentation Subagent | documentation-in-progress'

Write-Output "subagent_roles=$($requiredRoles.Count)"
Write-Output "open_subagent_related_rows=$($blockedOrPendingRows.Count)"
Write-Output "runtime_proof_rows_open=$($runtimeProofRowsStillOpen.Count)"

$report = [pscustomobject]@{
    Roles = $requiredRoles
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
