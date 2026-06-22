# Documentation Restructure Boundary

This is the current documentation cleanup rule set, not an old move plan.
Do not execute historical directory-move tables from archived prompts or older
versions of this file.

## Current Reading Path

Start every development pass from:

1. `PROJECT_STATE.md`
2. `docs/README.md`
3. `docs/test-ready-development-goal.md`
4. `docs/issues.md`
5. `docs/review.md`

For migration work, use `docs/features/ritsulib-migration/README.md` as the
entry point, then read `docs/goals/migration.md`,
`docs/integrations/ritsulib.md`, and `docs/reviews/current-validation.md`.
The RitsuLib integration lane is current for beta.108: compile package,
manifest dependency, package parity and source-workspace validation are RitsuLib-only;
beta.99, beta.96, and beta.93 loader or settings proof remain previous-package evidence only.

## Cleanup Rules

- Keep one active entry point per task line. Add new material to the entry
  point or feature folder instead of creating another root-level status file.
- Replace stale active docs with short boundary stubs when scripts or tests
  still read them.
- Archive prompt dumps, old audits, and superseded planning packets under
  `docs/archive/`; do not leave them in the active reading path.
- Delete only files that are not referenced by tests, scripts, docs indexes, or
  current feature docs. Run a reference scan before deletion.
- Update `docs/README.md`, `docs/PROJECT_MAP.md`, and `docs/doc-inventory.md`
  whenever moving, archiving, or deleting documentation.
- Keep release-critical blockers in `docs/issues.md`,
  `docs/release-evidence-status.md`, or the relevant feature checklist, never
  only in archived files.

## Stop Lines

- Do not move broad directory trees while code behavior is changing.
- Do not delete docs guarded by `scripts/check-sts1-event-current-doc-claims.ps1`
  or `scripts/check-sts1-static-file-hygiene.ps1`; compress them first and then
  update the guards in a separate validation pass.
- Do not turn loader, settings, or screenshot evidence into gameplay,
  save-load, co-op, QA, release, or handoff claims.
- Do not add any runtime dependency besides STS2-RitsuLib for Spire Plus
  without owner approval and same-pass manifest, package, docs, and guard
  updates.

## Validation

For docs-only cleanup, run the touched focused tests,
`scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch`,
`scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch`,
`dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, and
`git diff --check`.

For code/config changes, also run `dotnet build`. For package, resource,
localization, manifest, export, or tester-handoff changes, also run publish,
package refresh, installed-package parity, runtime preflight, and the relevant
artifact guards.
