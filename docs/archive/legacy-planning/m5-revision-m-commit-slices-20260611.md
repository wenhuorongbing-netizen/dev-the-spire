# M5 Revision M Commit Slices

Date: 2026-06-11
Status: planning only; no commit or push authorized from this paused validation lane.

## Slice 1 - v0.107.0 Runtime Drift Source Fix

Candidate files:

- `EZMicroBalanceCode/Ancients/Patches/BrightestFlameExhaustDrawPatch.cs`
- `EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs`
- `EZMicroBalanceCode/Ancients/Patches/DistinguishedCapePatches.cs`
- `EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs`
- `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs` if included in the same source-fix commit
- `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs`
- `tests/EZMicroBalance.Tests/AncientHighRiskSourceGuardTests.cs`
- `tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs`

Purpose: replace stale ModPatcher getter names and keep Ectoplasm on the `v0.107.0` gold modifier API.

## Slice 2 - beta.85 Package/Hash Documentation

Candidate files:

- `EZMicroBalance.json`
- package/hash handoff docs under `docs/`

Purpose: record the visible beta.85 package line and hash tables. Commit only after package checker, opt-in artifact tests, and intended handoff scope are confirmed.

## Slice 3 - StS1 Event Staging Updates

Candidate files:

- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1BigFish.cs`
- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1DivineFountain.cs`
- `EZMicroBalance/localization/eng/sts1_events.json`
- `EZMicroBalance/localization/zhs/sts1_events.json`
- `docs/features/sts1-events/**`
- `tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs`

Purpose: keep StS1 event staging text/eligibility and guards together. This should remain default-Off/staged until runtime event proof exists.

## Slice 4 - Revision M Governance Docs

Candidate files:

- `docs/goals/m5-revision-m-runtime-drift-report.md`
- `docs/goals/m5-revision-m-patch-failure-ledger.md`
- `docs/goals/m5-revision-m-owner-review-packet.md`
- `docs/goals/m5-revision-m-commit-slices.md`
- `docs/goals/m5-revision-m-version-decision.md`
- `docs/goals/m5-revision-m-final-report.md`
- `docs/reviews/current-validation.md`
- `docs/integrations/ritsulib.md`
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
- `PROJECT_STATE.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`
- `harness/TASK_RESULT.md`

Purpose: make the active blocker and resume lane unambiguous.

## Commit Rule

Do not commit any slice until validation is replayed without overlapping same-repo processes. If validation fails, split the failing source slice from docs and report the exact blocker instead.
