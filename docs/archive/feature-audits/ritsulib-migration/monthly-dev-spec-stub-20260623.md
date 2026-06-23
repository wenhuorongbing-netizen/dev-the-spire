# RitsuLib Monthly Dev Spec Stub

Archived 2026-06-23. This stub is historical compatibility context only; do
not use it as a current planning source.

This file is retained only because guarded historical docs and scripts still
reference `monthly-dev-spec.md`. It is not a planning source and must not grow
new migration tables, runtime evidence ledgers, or package handoff notes.

Use the current sources of truth instead:

- `PROJECT_STATE.md` for the current package/runtime state and blockers.
- `docs/features/ritsulib-migration/README.md` for RitsuLib-first migration
  rules and read order.
- `docs/goals/migration.md` for migration success criteria and validation.
- `docs/integrations/ritsulib.md` for RitsuLib version/API evidence.
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md` for future
  runtime evidence commands.
- `docs/archive/feature-audits/ritsulib-migration/batch-4c-candidates-20260623.md`
  for the completed low-risk localization migration record.

Current replacement boundary: beta.135 is RitsuLib-only and has package parity,
runtime preflight, and source-workspace validation for the 169/0 source state.
Previous beta.128 clicked UI smoke proves forced Ancient UI paths only for that
older package. Settings, loader, and screenshot evidence remain scoped evidence only; gameplay, enabled-mode, save-load, replacement, co-op, QA, release readiness, and handoff require separate current proof.
