# M5 Week 1 Commit Slices

Date: 2026-06-02
HEAD: `3f01cb7e (HEAD -> main, origin/main, origin/HEAD) sprint 4`
Spec: `docs/goals/debug.md` M5 July 2026

No commit is authorized. These slices are proposed for owner review only.

## Slice A: Sts1Events Canary Null-Safety Fixes

Files:

- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1BigFish.cs`
- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1DivineFountain.cs`
- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs`
- `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1TheLab.cs`

Purpose: null-safety fixes using `Owner is not { } owner` pattern matching to guard against nullable Owner references. These are the 4 canary event files.

Risk: medium; source changes to Sts1Events staging code. May reduce some of the 89 nullable warnings.

Rollback: `git checkout HEAD~1 -- EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1BigFish.cs` etc.

## Slice B: Harness and Status Updates

Files:

- `docs/goals/overnight-run-status.md`
- `docs/goals/overnight-run-ledger.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

Purpose: update harness docs to reflect sprint4/M5 state.

Risk: low; docs-only.

Rollback: restore previous versions.

## Slice C: M5 Week 1 Owner-Review Packet

Files (new):

- `docs/goals/m5-week1-owner-review-packet.md`
- `docs/goals/m5-week1-runtime-hard-blocker.md`
- `docs/goals/m5-week1-runtime-smoke-plan.md`
- `docs/goals/m5-week1-dirty-ledger.md`
- `docs/goals/m5-week1-warning-ledger.md`
- `docs/goals/m5-week1-commit-slices.md`

Purpose: M5 Week 1 owner-review artifacts.

Risk: low; new docs only.

Rollback: delete the new files.

## Decision

Do not commit any slice until the owner explicitly authorizes it.
