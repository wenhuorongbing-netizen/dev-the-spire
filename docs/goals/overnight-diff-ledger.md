# Overnight Diff Ledger — M3 Week 1

Date: 2026-05-29
Verified: `git status --short --porcelain` at M3 Week 1 validation (16:14 CEST)
HEAD: `d290598c` ("debugging")

## Summary

| Category | Count | Description |
|---|---|---|
| Modified (tracked) | 12 | Unstaged modifications to existing tracked files |
| Untracked | 3 | 2 files + 1 directory |
| **Total dirty** | **15** | All classified (0 unclassified) |

## Batch Classification Discrepancy

`report-worktree-batches.ps1` reports 10 dirty entries. Actual `git status` shows 12 dirty + 3 untracked = 15 entries. The script missed:
- `docs/goals/event.md` (batch 8)
- `tests/.../ArchitectureSkeletonGuardTests.cs` (batch 5)
- All 3 untracked entries

All entries are classified. 0 unclassified.

## Per-file Reconciliation

### Batch 1: Status and release docs (1 file)

| # | File | Change Type | Description |
|---|---|---|---|
| 1 | `docs/issues.md` | Doc update | Test count updated 361→387 in REFACTOR-PHASE0-1-VALIDATION |

### Batch 3: Ancient source and tests (1 file)

| # | File | Change Type | Description |
|---|---|---|---|
| 2 | `tests/.../UrdaStateCodecGuardTests.cs` | Test update | 15 new behavioral tests added (28-field constructor) |

### Batch 5: Scripts, CI, and validation tests (6 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 3 | `docs/features/ritsulib-migration/monthly-dev-spec.md` | Doc update | Test/guard counts updated |
| 4 | `docs/features/ritsulib-migration/next-overnight-run.md` | Doc update | Pre-run checklist test count updated |
| 5 | `scripts/report-worktree-batches.ps1` | Script update | .csproj added to ignore list |
| 6 | `tests/.../ActiveSourceManifestGuardTests.cs` | Test update | Expected string updated |
| 7 | `tests/.../ArchitectureSkeletonGuardTests.cs` | Test update | Assertion changes |
| 8 | `tests/.../EZMicroBalance.Tests.csproj` | Test config | Compile links for UrdaStateCodec, UrdaBlessingService.StateSchema, RewardPipeline, CardPlayContext |
| 9 | `tests/.../EngineeringGovernanceGuardTests.cs` | Test update | Assertion split |

### Batch 8: Stray docs and audits (3 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 10 | `docs/goals/debug.md` | Doc rewrite | Major M3 rewrite — still has stale counts |
| 11 | `docs/goals/event.md` | Doc update | StS1 Event Port audit content |
| 12 | `docs/goals/migration.md` | Doc update | Review doc with stale counts |

### Untracked entries (3 entries)

| # | File | Type | Description |
|---|---|---|---|
| 13 | `EZMicroBalanceCode/Core/Architecture/DeathProtectionService.cs` | Source stub | Death protection service skeleton |
| 14 | `EZMicroBalanceCode/Core/Architecture/MultiplayerPolicy.cs` | Source stub | Multiplayer policy skeleton |
| 15 | `tests/EZMicroBalance.Tests/Stubs/DiagnosticsNamespaceStub.cs` | Test stub | Diagnostics namespace test stub |

## Classification Decision

All entries are docs, tests, scripts, test config, or source/test stubs. No production behavior changes. Build safety verified (0 errors, 92 warnings).

## Comparison: Revision E → Revision F → M3 Week 1

| Metric | Revision E | Revision F | M3 Week 1 |
|---|---|---|---|
| Dirty tracked | 16 | 8 | 12 |
| Untracked | 1 | 1 | 3 |
| Total | 17 | 9 | 15 |
| Unclassified | 0 | 0 | 0 |
| Build errors | 0 | 0 | 0 |
| Build warnings | 87 | 87 | 92 |
| Tests passed | 361 | 361 | 387 |
| Tests failed | 0 | 0 | 0 |
| Tests skipped | 21 | 21 | 21 |
| Tests total | 382 | 382 | 408 |

## Build/Test Impact

| Check | Result |
|---|---|
| Clean build | 0 errors, 92 warnings (all Sts1Events nullable) |
| Tests | 387 passed, 0 failed, 21 skipped (408 total) |
| Format | Clean |
| Whitespace | Clean |
| Batch classification | 0 unclassified (script reports 10, actual 15) |
