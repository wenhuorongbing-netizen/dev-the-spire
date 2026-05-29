# Overnight Diff Ledger — M2 Revision F

Date: 2026-05-29
Verified: `git status --short` at Revision F replay (11:45 CEST)

## Summary

| Category | Count | Description |
|---|---|---|
| Modified (tracked) | 8 | Unstaged modifications to existing tracked files |
| Untracked | 1 | New file |
| **Total dirty** | **9** | All classified by batch script (0 unclassified) |

## Batch Classification

All 9 dirty entries classified by `report-worktree-batches.ps1 -FailOnUnclassified`:

| Batch | Count | Name |
|---|---|---|
| 0 | 0 | Local output hygiene |
| 1 | 0 | Status and release docs |
| 2 | 0 | Governance and architecture docs |
| 3 | 3 | Ancient source and tests |
| 4 | 0 | Ascension source and tests |
| 5 | 1 | Scripts, CI, and validation tests |
| 6-7 | 0 | Other |
| 8 | 5 | Stray docs and audits |
| -1 | 0 | Unclassified |

## Per-file Reconciliation

### Batch 3: Ancient source and tests (3 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 1 | `docs/features/sts1-events/source-research/sts2-act-event-registration.md` | Doc update | StS1 act-event registration research |
| 2 | `docs/features/sts1-events/wiki-event-catalog.md` | Doc update | Wiki event catalog with corrected counts |
| 3 | `docs/features/sts1-events/multiplayer-is-shared-matrix.md` | **NEW (untracked)** | Per-event IsShared matrix |

### Batch 5: Scripts, CI, and validation tests (1 file)

| # | File | Change Type | Description |
|---|---|---|---|
| 4 | `EZMicroBalance/localization/zhs/sts1_events.json` | Localization update | ZHS localization for Sts1Events |

### Batch 8: Stray docs and audits (5 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 5 | `docs/goals/debug.md` | Stale numbers | References "354 passed", "32 dirty files" — stale vs current 361/9 |
| 6 | `docs/goals/event.md` | New content | StS1 Event Port audit v8 + June spec + overnight run gates. Trailing whitespace fixed. |
| 7 | `docs/goals/migration.md` | Stale numbers | Review doc with stale "0 warnings" / "324 passed" — fixed |
| 8 | `docs/goals/overnight-diff-ledger.md` | Updated | Diff ledger updated from Revision E to Revision F |
| 9 | `docs/goals/overnight-run-status.md` | Updated | Status updated with verified replay numbers |

## Classification Decision

All 3 files are `docs/goals/` review/audit documents. They are batch 8 (stray docs and audits). No code, test, or resource changes. No Sts1Events governance impact.

## Comparison: Revision E vs Revision F

| Metric | Revision E | Revision F |
|---|---|---|
| Dirty files | 17 (16 modified + 1 untracked) | 9 (8 modified + 1 untracked) |
| Unclassified | 0 | 0 |
| Batch distribution | batch 1 (2), 2 (1), 3 (10), 5 (4) | batch 3 (3), 5 (1), 8 (5) |
| Code changes present | Yes (6 IsShared + FeatureRegistry + tests) | Localization JSON only |
| Untracked files | 1 (UrdaStateCodecGuardTests.cs) | 1 (multiplayer-is-shared-matrix.md) |

## Build/Test Impact

| Check | Result |
|---|---|
| Clean build | 0 errors, 87 warnings (all Sts1Events nullable) |
| Tests | 361 passed, 0 failed, 21 skipped (382 total) |
| Format | Clean |
| Whitespace | Clean (after event.md trailing whitespace fix) |
| Batch classification | 0 unclassified |
