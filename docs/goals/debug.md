# Debug Governance

This file is the current, compact debug-governance note for Spire Plus. The previous mojibake prompt-dump version was archived to `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md` and is traceability input only.

Use `PROJECT_STATE.md`, `docs/goals/migration.md`, `docs/features/ritsulib-migration/runtime-smoke-checklist.md`, and `docs/reviews/current-validation.md` for current runtime truth.

## Current Runtime Boundary

Current beta.99 package truth is RitsuLib-only on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `v0.4.32`: package parity and clicked RitsuLib settings UI proof are current, but direct Off loader proof still needs recapture after the settings-page I18N resource migration. Previous beta.96 Off proof at `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/` is previous-package startup/loading context only. Previous beta.93 AdditiveBatch1 proof remains previous-package enabled-mode registration context only.

Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, and beta.88/beta.93/beta.96 proof remains previous-package context. These rows are loader proof only; they do not prove beta.99 runtime, gameplay, clicked UI, save-load, current enabled-mode registration/gameplay, replacement behavior, multiplayer, independent QA, release readiness, or handoff readiness.

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
