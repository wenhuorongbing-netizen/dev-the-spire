# Warning Ledger - Current State

Date: 2026-06-10
Baseline HEAD: `f32c6767 (HEAD -> main, origin/main, origin/HEAD)`
Worktree: dirty, owner-review pending.

Revision M note, 2026-06-11: beta.85 validation keeps the nullable warning blocker closed, but this ledger remains a no-game warning ledger only. Do not use 0 warnings as current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, or release-ready proof.

Current supersession, 2026-06-21: beta.93 has RitsuLib-only Off and AdditiveBatch1 loader/registration proof on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `0.4.31`; gameplay, UI, save-load, replacement, co-op, QA, and release-ready proof remain pending. Use `PROJECT_STATE.md` and `docs/test-ready-development-goal.md` for current proof claims.

## Summary

| Metric | Value |
|---|---:|
| Unique CS warnings | 0 |
| CS8604 possible null reference argument | 0 |
| CS8602 dereference of possibly null reference | 0 |
| CS8625 cannot convert null literal | 0 |
| Warnings outside `EZMicroBalanceCode/Sts1Events/Models/` | 0 |
| MSBuild locked-file/env artifact warnings | 0 in the current warning classification |

Current build evidence: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` was rerun on 2026-06-10 during migration reconciliation and reported `0 Warning(s)` / `0 Error(s)`. The previous 70-warning snapshot is preserved below as historical burn-down context only; it is not the active warning budget.

## Historical 70-Warning Snapshot

| File | CS8602 | CS8604 | CS8625 | Total | Owner |
|---|---:|---:|---:|---:|---|
| `Act1/Sts1DeadAdventurer.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act1/Sts1Joust.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act1/Sts1Mushrooms.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Act1/Sts1TheSsssserpent.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act1/Sts1TreasureOoze.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act2/Sts1Altar.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act2/Sts1AncientWriting.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act2/Sts1Augmenter.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act2/Sts1CouncilOfGhosts.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Act2/Sts1CursedTome.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Act2/Sts1DrugDealer.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act2/Sts1ForgottenAltar.cs` | 2 | 2 | 0 | 4 | Sts1Events |
| `Act2/Sts1KnowingSkull.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Act2/Sts1MaskedBandits.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act2/Sts1Nest.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act2/Sts1TheGhost.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Act2/Sts1TheLibrary.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Act2/Sts1Vampires.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Act3/Sts1Falling.cs` | 1 | 2 | 0 | 3 | Sts1Events |
| `Act3/Sts1MindBloom.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Act3/Sts1MoaiHead.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Act3/Sts1SensoryStone.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Act3/Sts1TombOfLordRedMask.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Act3/Sts1Transmogrifier.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act3/Sts1WindingHalls.cs` | 3 | 1 | 0 | 4 | Sts1Events |
| `Shared/Sts1BonfireSpirits.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Shared/Sts1Designer.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1FaceTrader.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Shared/Sts1FountainOfCleansing.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Shared/Sts1GoldenWing.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Shared/Sts1LivingWall.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1TheMausoleum.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Shared/Sts1TheWomanInBlue.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1WheelOfChange.cs` | 2 | 4 | 0 | 6 | Sts1Events |
| **Total** | **26** | **44** | **0** | **70** | **Sts1Events** |

## Cleared Rows

The CanaryOnly files and the current AdditiveBatch1 files are warning-clean after explicit `Owner` guards:

- `Shared/Sts1BigFish.cs`
- `Shared/Sts1GoldenIdol.cs`
- `Shared/Sts1TheLab.cs`
- `Shared/Sts1DivineFountain.cs`
- `Act1/Sts1ShiningLight.cs`
- `Act3/Sts1UpgradeShrine.cs`
- `Shared/Sts1GoldenShrine.cs`
- `Shared/Sts1OldBeggar.cs`
- `Shared/Sts1Purifier.cs`
- `Shared/Sts1TheCleric.cs`

## Governance Decision

- Sts1Events recommendation: staging-only until current-runtime and gameplay evidence exists.
- Runtime loader proof: historical Off=0, CanaryOnly=4, and AdditiveBatch1=10 event types / 11 registration-call evidence remains diagnostic only. Current beta.93 RitsuLib-only `v0.107.1` Off and AdditiveBatch1 loader/registration proof is clean; current source expects AdditiveBatch1=10 event types / 14 registration calls, and beta.93 verifier evidence matches that shape. Retained beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence only.
- Nullable warning blocker: closed for the beta.85 runtime-fix validation lane recorded in `PROJECT_STATE.md`.
- Remaining formalization blockers: recapture any needed current-version CanaryOnly loader proof before Canary gameplay claims, then prove gameplay, EN/ZHS render, save-load, image/render behavior, replacement behavior, multiplayer fail-closed behavior, independent QA, and handoff.
- Removal option: would clear warning debt but would discard intentionally staged prototype infrastructure; requires owner decision.

## History

| Date | Clean build warnings | Notes |
|---|---:|---|
| 2026-05-29 Rev D | 69 | Historical first clean-build count |
| 2026-05-29 Rev E | 87 | Historical count after early `IsShared` updates |
| 2026-05-29 M3 Week 1 | 92 | Historical count after UrdaStateCodec/Sts1Events changes |
| 2026-05-31 Revision I/J | 89 | Count after sprint2; all warnings in Sts1Events model staging code |
| 2026-06-02 sprint4 | 79 | Reduced by 10 after CanaryOnly null-safety fixes |
| 2026-06-10 Revision L | 70 | Reduced by AdditiveBatch1 owner guards; remaining warnings are draft/deferred Sts1Events rows |
| 2026-06-10 Revision L zero-warning build | 0 | Expanded owner guards cleared the remaining nullable warnings; reverified by `dotnet build EZMicroBalance.sln -m:1 --no-incremental` |
