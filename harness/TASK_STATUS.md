# TASK_STATUS - Current Task Short Status

## Current Goal

- Runtime Proof + Governance Closure for RitsuLib migration.

## Completed In Current Governance Run

- Verified current HEAD: `24d4fe9a` on `main`.
- Verified STS2-RitsuLib is missing at checked D/E mod paths; E-drive BaseLib and Spire Plus package folders exist.
- Preserved dirty shared worktree state while adding diagnostics-only architecture canary evidence and tests.
- Added warning triage issue for 89 Sts1Events nullable warnings.
- Updated runtime smoke, migration monthly spec, and validation docs to keep Batch 4c blocked.

## Verification Result

- Build: 0 errors, 89 warnings (`CS8604` = 54, `CS8602` = 34, `CS8625` = 1) in Sts1Events staging code.
- Tests: current no-game target is 461 passed, 0 failed, 21 skipped, 482 total.
- Runtime smoke: hard blocked until STS2-RitsuLib is installed and fresh `godot.log` evidence is captured.
- Format/diff-check: rerun as part of final validation before handoff.

## Remaining Work

- Install STS2-RitsuLib at `<GameRoot>\mods\STS2-RitsuLib`.
- Run Off mode smoke and prove 0 Sts1Events registrations.
- Run CanaryOnly smoke and prove exactly 4 canary registrations.
- Keep AdditiveBatch1 prototype-only until Off/CanaryOnly pass.
- Keep AdditiveAllDraft and ReplaceUnknownEventsPrototype dev-only/unsafe.
- Do not start Batch 4c, high-risk migration, or new gameplay before runtime smoke passes.
