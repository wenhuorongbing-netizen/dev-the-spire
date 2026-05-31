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
| HEAD | `aed2a498` ("debug") |
| Stash list | Empty |
| Dirty tracked files | 11 (unstaged modifications) |
| Untracked entries | 0 |
| **Total entries** | **11** |

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
| `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` | 0 | 444 passed, 0 failed, 21 skipped (465 total) |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | 0 | Clean |
| `git diff --check` | 0 | Clean |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | 0 | 11 dirty (script output), 0 unclassified |

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
Status: BLOCKED — Revision H validation replay failed on current dirty state
M3 Revision H: Terminal validations replayed at 85a38dd1
Build: 0 errors, 89 warnings (all Sts1Events nullable: CS8604=54, CS8602=34, CS8625=1)
Test: failed/aborted (13 failed, 427 passed, 21 skipped, 461 total before host crash)
Dirty: batch classifier reports 39 dirty entries after validation/build outputs, 0 unclassified
Commit slices: Revision H plan prepared for owner review only; no commit authorized
Stale docs: active docs still mix Revision G/H state and validation failures
Sts1Events: Staging-only (warnings/runtime unverified; ZHS sts1_events key parity currently complete)
RitsuLib: Attempted/runtime-unverified (no error handling, no runtime proof)
Debug: Accept-scaffold (default-off, Warn unconditional, LogPreview dead)
Release-ready: No
Required next: owner decision on guarded doc deletions, untracked ArchitectureCanaryBootstrap, Sts1Events enum intent, package hash/site refresh, and parallel commit acceptance notation
```
