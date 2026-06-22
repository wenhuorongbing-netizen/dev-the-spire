# RitsuLib Monthly Dev Spec Stub

This file is retained only because guarded historical docs and scripts still
reference `monthly-dev-spec.md`. It is not a planning source and must not grow
new migration tables, runtime evidence ledgers, or package handoff notes.

Use the current sources of truth instead:

- `PROJECT_STATE.md` for the current package/runtime state and blockers.
- `docs/features/ritsulib-migration/README.md` for RitsuLib-first migration
  rules and read order.
- `docs/goals/migration.md` for migration success criteria and validation.
- `docs/integrations/ritsulib.md` for RitsuLib version/API evidence.
- `runtime-smoke-checklist.md` for future runtime evidence commands.
- `batch-4c-candidates.md` for the completed low-risk localization migration
  record.

Current boundary: beta.107 is RitsuLib-only and has package parity, runtime
preflight, source-workspace validation, and smoke-level clicked Ancient UI
proof. Settings, loader, and screenshot evidence remain scoped evidence only;
gameplay, enabled-mode, save-load, replacement, co-op, QA, release readiness,
and handoff require separate current proof. Batch 4c localization migration is
source-only until a fresh package/runtime pass proves all 52 migrated patch
classes apply in the installed game.
