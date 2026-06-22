# RitsuLib Migration Status Stub

This file stays only as a compatibility link for older docs, scripts, and
guards that still open `docs/migration.md`. Do not add migration tables,
runtime ledgers, package hashes, or patch inventory here.

Use the current sources of truth instead:

- `docs/features/ritsulib-migration/README.md` for the migration entry point,
  read order, RitsuLib-first rules, and stop lines.
- `docs/goals/migration.md` for success criteria, validation commands, and
  current next actions.
- `docs/integrations/ritsulib.md` for dependency version, installed runtime,
  public RitsuLib references, and API evidence.
- `docs/patch-inventory.md` for generated patch counts and remaining Harmony
  declarations.
- `PROJECT_STATE.md` and `docs/reviews/current-validation.md` for current proof
  boundaries.

Current boundary: Spire Plus is RitsuLib-only for beta.119 on Slay the Spire 2
`v0.107.1` with `STS2.RitsuLib` / `STS2-RitsuLib` `0.4.34`. beta.119 package
parity, runtime preflight, and source-workspace validation are current; the
retained beta.108 clicked Ancient UI smoke is previous-package evidence only.
Gameplay, gated Vakuu fight-option/victory return, save-load, replacement
behavior, co-op/fail-closed proof, independent QA, release readiness, and tester
handoff remain pending.

Batch 4c localization fallback patches, the visual-hover UI getter batch,
Ancient reward getter/relic hook patches, Aeonglass intent UI patches, and
Enemy Damage polish getter patches have
moved to RitsuLib `IPatchMethod` / `ModPatcher` in source.
Any higher-risk patch migration remains proposal-only until the owner approves
the exact scope and the same pass records source evidence, focused tests,
validation, and package-version decisions when artifacts change.
