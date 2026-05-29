# Overnight Run Status — M3 Week 1 Commit Readiness Gate

Date: 2026-05-29
Run started: 2026-05-29T07:57:57+02:00 (Revision D)
Revision E completed: 2026-05-29T09:50:00+02:00
Revision F replay: 2026-05-29T11:45:00+02:00
M3 Week 1 validation: 2026-05-29T16:14:00+02:00

## Branch State

| Field | Value |
|---|---|
| Branch | `main` |
| HEAD | `d290598c` ("debugging") |
| Stash list | Empty |
| Dirty tracked files | 12 (unstaged modifications) |
| Untracked entries | 3 (2 files + 1 directory) |
| **Total entries** | **15** |

## Evidence Hardening Pass (Revision F)

Three strict-audit evidence issues fixed:

1. **api-command-matrix.md**: Removed phantom `CardCmd.UpgradeCard` (does not exist). Fixed 6 additional phantom/wrong-signature methods. All signatures verified against `source code/src/Core/Commands/`.
2. **wiki-event-catalog.md**: Corrected counts to 52/46/52/4. Removed stale `act_bucket_memberships=57`.
3. **patch-boundaries.md**: Added StS1Events row to High-Risk Owners 5-column table and manual evidence map.

Additional fixes:
- 6 combat event files now declare `public override bool IsShared => true` (Sts1DeadAdventurer, Sts1ScorpionNest, Sts1TreasureOoze, Sts1MaskedBandits, Sts1MindBloom, Sts1MysteriousSphere)
- Source manifest updated with `EZMicroBalanceCode/Core/Architecture/` files
- New `ArchitectureSkeletonGuardTests.cs` covers CardPlayContext and RewardPipeline

## Terminal Validation Results (M3 Week 1 Replay)

| Command | Exit Code | Result |
|---|---|---|
| `dotnet clean .\EZMicroBalance.csproj` | 0 | Clean |
| `dotnet build .\EZMicroBalance.csproj` | 0 | 0 errors, 92 warnings (all Sts1Events nullable) |
| `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` | 0 | 387 passed, 0 failed, 21 skipped (408 total) |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | 0 | Clean |
| `git diff --check` | 0 | Clean |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | 0 | 10 dirty (script output), 0 unclassified |

## Stop Condition

**GREEN** — All terminal validation commands passed with exit code 0.

## Files Changed During Evidence Hardening (Revision F)

| Category | Files | Change |
|---|---|---|
| Doc fixes | `api-command-matrix.md`, `wiki-event-catalog.md`, `patch-boundaries.md` | Phantom APIs removed, counts corrected, StS1Events row added |
| Combat events | 6 files in `Sts1Events/Models/` | Added `IsShared => true` |
| Source manifest | `ActiveSourceManifestGuardTests.cs` | Added Core/Architecture files |
| Tests | `ArchitectureSkeletonGuardTests.cs` (new) | Covers CardPlayContext + RewardPipeline |
| Docs | `overnight-run-status.md`, `issues.md`, `refactor.md`, `patch-inventory.md` | Updated counts and status |

## Current Stop Condition

```text
Status: GREEN — all terminal validation commands passed
M3 Week 1: Terminal validations replayed at d290598c
Build: 0 errors, 92 warnings (all Sts1Events nullable)
Test: 387 passed, 0 failed, 21 skipped (408 total)
Dirty: 12 tracked + 3 untracked = 15 total entries
Commit slices: 6 slices prepared (revision-f-commit-slices.md)
Stale docs: 55 count locations identified, fixes in progress
Sts1Events: Staging-only (24 guard tests, 92 warnings, runtime unverified)
RitsuLib: Attempted/runtime-unverified (no error handling, no runtime proof)
Debug: Accept-scaffold (default-off, Warn unconditional, LogPreview dead)
Release-ready: No
Required next: Fix stale doc counts, owner decision on commit slices
```
