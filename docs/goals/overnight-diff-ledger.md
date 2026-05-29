# Overnight Diff Ledger — M2 Revision E

Date: 2026-05-29
Verified: `git status --short` at Revision E

## Summary

| Category | Count | Description |
|---|---|---|
| Modified (tracked) | 16 | Staged/unstaged changes to existing tracked files |
| Untracked | 1 | New test file |
| **Total dirty** | **17** | All classified by batch script (0 unclassified) |

## Batch Classification

All 17 dirty entries classified by `report-worktree-batches.ps1 -FailOnUnclassified`:

| Batch | Count | Name |
|---|---|---|
| 0 | 0 | Local output hygiene |
| 1 | 2 | Status and release docs |
| 2 | 1 | Governance and architecture docs |
| 3 | 10 | Ancient source and tests |
| 4 | 0 | Ascension source and tests |
| 5 | 4 | Scripts, CI, and validation tests |
| 6-8 | 0 | Other |
| -1 | 0 | Unclassified |

## Per-file Reconciliation

### Batch 1: Status and release docs (2 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 1 | `docs/issues.md` | Stale count fix | Updated test counts (324→354) and warning note |
| 2 | `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` | Stale doc | Previously updated from Revision D; still dirty |

### Batch 2: Governance and architecture docs (1 file)

| # | File | Change Type | Description |
|---|---|---|---|
| 3 | `docs/architecture/patch-boundaries.md` | Doc update | Phase 2 adapter checklist added, StS1Events row |

### Batch 3: Ancient source and tests (10 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 4 | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1DeadAdventurer.cs` | `IsShared` addition | Combat event: `IsShared = true` class-level property |
| 5 | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1ScorpionNest.cs` | `IsShared` addition | Combat event: `IsShared = true` class-level property |
| 6 | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1TreasureOoze.cs` | `IsShared` addition | Combat event: `IsShared = true` class-level property |
| 7 | `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1MaskedBandits.cs` | `IsShared` addition | Combat event: `IsShared = true` class-level property |
| 8 | `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MindBloom.cs` | `IsShared` + `CardCmd.Upgrade` fix | Combat event IsShared + CS0117 fix |
| 9 | `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MysteriousSphere.cs` | `IsShared` addition | Combat event: `IsShared = true` class-level property |
| 10 | `docs/features/sts1-events/source-research/api-command-matrix.md` | Doc update | CardCmd.Upgrade correction, verified API matrix |
| 11 | `docs/features/sts1-events/status-board.md` | Doc update | Per-event evidence matrix, IsShared decisions |
| 12 | `docs/features/sts1-events/wiki-event-catalog.md` | Doc update | 46/48/52 mismatch policy |
| 13 | `tests/EZMicroBalance.Tests/UrdaStateCodecGuardTests.cs` | **NEW (untracked)** | New guard test file |

### Batch 5: Scripts, CI, and validation tests (4 files)

| # | File | Change Type | Description |
|---|---|---|---|
| 14 | `EZMicroBalanceCode/Core/Features/FeatureRegistry.cs` | Dirty | Feature registry changes |
| 15 | `EZMicroBalanceCode/Core/Features/IFeatureModule.cs` | Dirty | Feature module interface changes |
| 16 | `tests/EZMicroBalance.Tests/EngineeringGovernanceGuardTests.cs` | Dirty | Governance guard test updates |
| 17 | `tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs` | Dirty | Sts1Events guard test updates |

## Classification Decision

Per AGENTS.md rules: Sts1Events is **tracked** (not untracked/unrelated). It is compiled, feature-gated, dormant-by-default with active guard tests. The governance classification is **formal/staging** — not "unrelated."

The 6 `IsShared` property additions are valid C# at class level (confirmed by clean build with 0 errors). They add multiplayer state classification to combat events.

## Build/Test Impact

| Check | Result |
|---|---|
| Clean build | 0 errors, 87 warnings (all Sts1Events nullable) |
| Tests | 354 passed, 0 failed, 21 skipped |
| Format | Clean |
| Whitespace | Clean (trailing whitespace in status-board.md fixed) |
| Batch classification | 0 unclassified |
