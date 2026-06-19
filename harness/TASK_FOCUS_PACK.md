# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- M5 Revision N: keep beta.88 current-loader truth accurate, prepare owner-ready commit/validation/runtime plans, and avoid overlapping validation or runtime processes.
- The previous beta.85 `v0.107.0` Revision M target is historical. Current local truth is beta.88 on Slay the Spire 2 `v0.107.1` with BaseLib `v3.3.0` and STS2-RitsuLib `v0.4.24`.
- Current beta.88 AdditiveBatch1 proof is clean for loader/registration only. Broader gameplay/live/release proof is still pending.

## Acceptance Criteria

- Revision N docs exist and point at beta.88 as current truth:
  - `docs/goals/m5-revision-n-final-report.md`
  - `docs/goals/m5-revision-n-owner-commit-packet.md`
  - `docs/goals/m5-revision-n-validation-replay.md`
  - `docs/goals/m5-revision-n-runtime-evidence-plan.md`
- Harness state no longer routes current work through stale beta.85 / RitsuLib `v0.4.16` language.
- Owner decisions remain explicit: no commit, push, Batch 4c migration, debug expansion, StS1 formalization, release-ready claim, or live-ready claim without the required evidence.
- Validation commands are planned but not run until coordination allows one clean lane.
- Loader proof and manual/gameplay proof remain separated.

## Blocked Scope

- New `dotnet build`, `dotnet test`, `dotnet format`, `dotnet publish`, package/release-evidence validation, runtime/game launch, process cleanup, staging, commit, or push actions from this thread during the coordination pause.
- Batch 4c patch migration without owner approval.
- Batch 5, PR7, or high-risk migration.
- Debug expansion.
- StS1Events formalization from loader proof alone.
- Longhaul audit recovery before owner gates close.
- Release-ready or live-ready claims from loader logs.

## Related Files Or Modules

- `PROJECT_STATE.md`
- `docs/goals/debug.md`
- `docs/goals/m5-revision-n-final-report.md`
- `docs/goals/m5-revision-n-owner-commit-packet.md`
- `docs/goals/m5-revision-n-validation-replay.md`
- `docs/goals/m5-revision-n-runtime-evidence-plan.md`
- `docs/reviews/current-validation.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/integrations/ritsulib.md`
- `docs/testing/runtime-monkey-stability.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

## Risks

- Stale beta.85/beta.87 language can cause overclaims if it is not read through the beta.88 override.
- Existing validation records were produced by other lanes and must not be represented as commands run in this paused pass.
- Dirty worktree scope can change while same-repo threads are active; recapture status before owner or handoff decisions.
- Current beta.88 loader proof still does not cover gameplay, UI, save-load, co-op, independent QA, or release handoff.
