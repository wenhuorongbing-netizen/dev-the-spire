# Revision J Commit Slices

Date: 2026-05-31
HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`

No commit is authorized. These slices are proposed for owner review only.

## Slice A: Runtime Dependency Truth And Smoke Runbook

Files:

- `AGENTS.md`
- `PROJECT_STATE.md`
- `README.md`
- `docs/issues.md`
- `docs/integrations/ritsulib.md`
- `docs/migration.md`
- `docs/restructure.md`
- `docs/features/ritsulib-migration/monthly-dev-spec.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/features/ritsulib-migration/runtime-hard-block-report-20260531.md`
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
- `docs/reviews/current-validation.md`

Purpose: record that `STS2-RitsuLib v0.3.10` is installed locally while live runtime proof remains blocked by non-clean loader evidence, not by a missing `godot.log`.

Risk: docs could be misread as runtime validation if not paired with the hard-blocker wording.

Rollback: restore dependency-missing wording if owner rejects documenting local install state.

## Slice B: Runtime Smoke Helper Safety

Files:

- `scripts/spire-plus-live-session.ps1`
- `scripts/README.md`
- `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`

Purpose: keep `STS2-RitsuLib` enabled/preserved during controlled live smoke sessions that use `-MoveOtherMods`.

Risk: local helper behavior changes; owner must approve before relying on it for smoke evidence.

Rollback: remove `STS2-RitsuLib` from the helper allowed list and avoid `-MoveOtherMods` for RitsuLib runtime smoke.

## Slice C: Sts1Events Blocker Truth

Files:

- `docs/features/sts1-events/hard-stop-blocker-report-v14.md`
- `docs/features/sts1-events/multiplayer-fail-closed-guard.md`
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md`

Purpose: keep Sts1Events staging-only and block formalization until Off/CanaryOnly runtime logs, warning cleanup/acceptance, localization render proof, asset proof, save/load proof, and QA pass exist.

Risk: none to runtime; docs-only governance risk.

Rollback: restore previous blocker text.

## Slice D: Revision J Owner-Review Packet

Files:

- `docs/goals/revision-j-final-report.md`
- `docs/goals/revision-j-owner-review-packet.md`
- `docs/goals/revision-j-runtime-hard-blocker.md`
- `docs/goals/revision-j-runtime-smoke-plan.md`
- `docs/goals/revision-j-dirty-ledger.md`
- `docs/goals/revision-j-commit-slices.md`
- `docs/goals/overnight-run-status.md`
- `docs/goals/overnight-run-ledger.md`
- `docs/goals/overnight-diff-ledger.md`
- `docs/goals/warning-ledger.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

Purpose: make Revision J reviewable without claiming runtime readiness.

Risk: owner-review docs are high-churn and should not be treated as player-facing release notes.

Rollback: delete/revert Revision J docs if owner rejects this packet.

## Slice E: Historical Revision I Artifact Corrections

Files:

- `docs/goals/revision-i-commit-slices.md`
- `docs/goals/revision-i-owner-review-packet.md`
- `harness/TASK_RESULT.md`

Purpose: reconcile historical Revision I handoff language with the later local dependency install state.

Risk: changes historical packet semantics.

Rollback: restore Revision I artifacts if owner wants them immutable.

## Decision

Do not commit any slice until the owner explicitly authorizes it.
