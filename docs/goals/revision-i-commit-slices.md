# Revision I Commit Slices

Date: 2026-05-31

No commit was made. These are proposed review slices only.

## Slice A: Sts1Events Gate Fix

Files:

- `EZMicroBalanceCode/Sts1Events/Sts1EventsFeatureModule.cs`
- `tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs`
- `tests/EZMicroBalance.Tests/EngineeringGovernanceGuardTests.cs`

Purpose: keep `SPIREPLUS_STS1_EVENT_MODE` owned by `Sts1EventFeatureGate` instead of treating it as a generic FeatureRegistry disable override. This preserves CanaryOnly/AdditiveBatch1 reachability while default Off remains guarded.

Risk: source/test governance only; no runtime proof.

Rollback: restore the previous module/test assertions if owner decides Sts1Events should return to a generic disable-key model.

## Slice B: Runtime And RitsuLib Governance Docs

Files:

- `docs/features/ritsulib-migration/monthly-dev-spec.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/features/ritsulib-migration/runtime-hard-block-report-20260531.md`
- `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md`
- `docs/patch-inventory.md`

Purpose: keep runtime-smoke, warning, and patch-inventory truth aligned with HEAD `87820303`; STS2-RitsuLib is now installed, but runtime smoke remains blocked by missing live `godot.log` evidence.

Risk: documentation and generated inventory only.

Rollback: revert individual docs if owner rejects the governance wording.

## Slice C: Goal Review Docs

Files:

- `docs/goals/debug.md`
- `docs/goals/event.md`
- `docs/goals/migration.md`
- `docs/goals/refactor.md`

Purpose: preserve the current audit/review goal documents while marking runtime proof and commit readiness as incomplete.

Risk: large docs churn; owner review required before commit.

Rollback: restore selected goal docs from HEAD if owner wants to keep only compact active docs.

## Slice D: Revision I Owner-Review Packet

Files:

- `docs/goals/revision-i-final-report.md`
- `docs/goals/revision-i-owner-review-packet.md`
- `docs/goals/revision-i-parallel-commit-audit.md`
- `docs/goals/revision-i-commit-slices.md`
- `docs/goals/overnight-run-status.md`
- `docs/goals/overnight-run-ledger.md`
- `docs/goals/overnight-diff-ledger.md`
- `docs/goals/warning-ledger.md`
- `docs/reviews/current-validation.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

Purpose: make the current state reviewable without claiming runtime or release readiness.

Risk: docs-only; should not ship as player-facing release notes.

Rollback: remove Revision I packet files and restore prior status docs.

## Commit Decision

Recommended next step: owner reviews slices A-D and either approves selective commits or asks for rollback. Do not commit without explicit owner approval.
