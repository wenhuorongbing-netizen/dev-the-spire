# TASK_STATUS - Current Task Short Status

## Current Goal

- M5 Revision P: beta.93 `v0.107.1` RitsuLib-only clean-loader truth, owner-ready planning, and validation/runtime handoff without overlapping same-repo processes.

## Current Facts

- Current package line: Spire Plus `v0.1.0-private-beta.93`.
- Current local game: Slay the Spire 2 `v0.107.1`.
- Current dependencies: STS2-RitsuLib `v0.4.31`, RitsuLib selected compat branch `lib/0.107.1`; no Spire Plus BaseLib project, manifest, package, or current runtime dependency.
- Current clean loader/registration proof: `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`.
- Scope of that proof: main menu reached, exactly STS2-RitsuLib plus Spire Plus loaded, 25/25 Spire Plus ModPatcher patches applied, Off packet verifier 43 / 0, AdditiveBatch1 10 event types / 14 registration calls, clean audit, retained enabled-mode verifier 31 / 0, and packet verifier 61 / 0.
- Not proven: gameplay, clicked UI, save-load, replacement functional behavior, multiplayer/fail-closed behavior, game-native AutoSlay batch stability, independent QA, release readiness, or tester handoff.
- Retained beta.85, beta.86, beta.87, beta.88, and beta.90 loader logs are previous-package, previous-game-version, or previous-dependency context unless a current doc explicitly names the beta.93 evidence paths.
- Debug recommendation: accept scaffold only. General info diagnostics stay internal-only behind `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`; preview diagnostics stay behind the localized preview setting; no debug expansion is authorized.
- StS1Events recommendation: staging-only. Loader/registration proof must not be treated as event gameplay proof.
- Batch 4c recommendation: proposal-only pending owner decision and fresh validation.
- Recapture worktree status before any handoff, staging, commit, or push decision.
- Coordination note: do not start runtime/game launch or broad cleanup without a focused validation lane; normal docs/source validation is allowed when this thread owns the current cleanup slice.

## Verification Result

- Latest recorded beta.93 validation is in `PROJECT_STATE.md`, `docs/test-ready-development-goal.md`, and `docs/dev-environment.md`: build 0 warnings / 0 errors, publish/package refresh, installed beta.93 package parity, runtime preflight 27 / 0, current source-workspace check 58 checks / 0 mismatches with two GDRE warnings, RitsuLib-only Off packet verification 43 / 0, AdditiveBatch1 enabled-mode verifier 31 / 0, and AdditiveBatch1 packet verification 61 / 0.
- Those recorded results are loader/registration and no-game guard validation only.

## Remaining Work

- Keep beta.93 RitsuLib-only docs and package evidence aligned while the user performs live/manual testing.
- Recapture exact HEAD and worktree state before any validation, handoff, commit, or push decision.
- Fill or explicitly defer the remaining live/manual runtime evidence rows in `docs/private-beta-verification-handoff.md` and `docs/release-evidence-status.md`.
- Prepare owner decision on commit slices after validation replay.
- Keep release-ready and live-ready claims blocked until gameplay/UI/save-load/co-op/QA/handoff evidence exists.
