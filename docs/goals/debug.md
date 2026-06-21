# Debug Governance

This file is the current, compact debug-governance note for Spire Plus. The previous mojibake prompt-dump version was archived to `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md` and is traceability input only.

Use `PROJECT_STATE.md`, `docs/goals/migration.md`, `docs/features/ritsulib-migration/runtime-smoke-checklist.md`, and `docs/reviews/current-validation.md` for current runtime truth.

## Current Runtime Boundary

Current beta.93 loader truth is RitsuLib-only on Slay the Spire 2 `v0.107.1`: `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` reached main menu with exactly STS2-RitsuLib `v0.4.31` and Spire Plus `v0.1.0-private-beta.93`, no Spire Plus BaseLib dependency, 25/25 Spire Plus patches applied, clean audits, Off packet verifier 43 / 0, AdditiveBatch1 enabled-mode verifier 31 / 0, and AdditiveBatch1 packet verifier 61 / 0.

Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, beta.88 remains previous BaseLib-backed context, and beta.93 is the current RitsuLib-only loader/registration proof. These rows are loader proof only; they do not prove gameplay, clicked UI, save-load, replacement behavior, multiplayer, independent QA, release readiness, or handoff readiness.

## Debug Decision

Debug scaffold status: accept scaffold, do not expand.

- General diagnostics remain internal-only behind `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`.
- Preview diagnostics remain behind the localized `ShowPreviewDebugLogs` setting.
- Do not add broad player-facing debug settings.
- Do not formalize StS1Events, Batch 4c migration, or longhaul audit work from this debug note.
- Do not claim live-ready or release-ready status from source/static/build/test/package validation.

## Current Stop Line

Keep StS1Events staging-only until current runtime event encounter proof, gameplay proof, save-load proof, EN/ZHS render proof, replacement functional proof, multiplayer/fail-closed proof, independent QA, and tester handoff evidence exist.

For implementation work, start from `docs/test-ready-development-goal.md` and `docs/issues.md`, not from archived debug prompts.
