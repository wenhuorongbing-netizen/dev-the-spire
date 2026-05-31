# TASK_STATUS - Current Task Short Status

## Current Goal

- Complete all achievable migration.md phases

## Completed

- PR 1-4: Done (baseline, docs, harness, move-only refactor)
- PR 5: Compile/manifest dependency attempted; runtime/package/handoff unverified
- PR 6 Batch 1: Partial diagnostics/bootstrap scaffold; runtime unverified
  - RitsuLibBootstrap.cs: RitsuLib logger + Harmony patch application
  - MainFile.cs: uses RitsuLibBootstrap.ApplyPatches()
  - Guard manifest updated with new source file + coverage root

## Verification Result

- Build: 0 errors, 89 warnings in Revision H replay (all CS8602/CS8604/CS8625 nullable reference warnings in Sts1Events staging code)
- Tests: failing on current dirty state in Revision H replay (13 failed, 427 passed, 21 skipped, 461 total before host crash)
- Format: clean in Revision H replay; `git diff --check` fails on trailing whitespace in `docs/goals/event.md`

## Remaining Work

- PR 6 Batch 4: Patch class migration to IPatchMethod (63 files)
- PR 6 Batch 5: High-risk patches (blocked on evidence backlog)
- RitsuLib runtime verification (loader smoke, godot.log evidence)
- Upgrade RitsuLib to compat package when published on NuGet
- Resolve Revision H hard blocker before any commit: guarded doc deletions, untracked architecture canary, Sts1Events enum/test alignment, package hash/site metadata, and diff whitespace.
