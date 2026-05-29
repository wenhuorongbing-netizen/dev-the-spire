# Warning Ledger — M3 Week 1

Date: 2026-05-29
Verified: Clean build (`dotnet clean` + `dotnet build .\EZMicroBalance.csproj`) at HEAD `d290598c`

## Summary

| Metric | Value |
|---|---|
| Total CS warnings | **92** |
| CS8604 (possible null reference argument) | **TBD (needs recount)** |
| CS8602 (dereference of possibly null reference) | **TBD (needs recount)** |
| CS8625 (cannot convert null literal) | **TBD (needs recount)** |
| Warnings outside Sts1Events/ | **0** |
| MSB3026 (env artifact, not code) | **10** (non-blocking, ignored) |

## All warnings are in `EZMicroBalanceCode/Sts1Events/Models/`

Every CS warning is a nullable reference type warning in the Sts1Events staging code.
No warnings exist in core Spire Plus code, RitsuLib integration, preview tools, or tests.

## Per-file Breakdown (Revision E — 87 warnings)

**Note**: Current clean build produces 92 warnings (+5 from Revision E). The per-file breakdown below is from Revision E and needs recount. All new warnings are in `EZMicroBalanceCode/Sts1Events/Models/`.

| File | CS8604 | CS8602 | CS8625 | Total |
|---|---|---|---|---|
| `Shared/Sts1WheelOfChange.cs` | 4 | 2 | 0 | 6 |
| `Shared/Sts1BigFish.cs` | 1 | 3 | 0 | 4 |
| `Shared/Sts1GoldenIdol.cs` | 1 | 3 | 1 | 5 |
| `Shared/Sts1LivingWall.cs` | 3 | 0 | 0 | 3 |
| `Shared/Sts1TheCleric.cs` | 4 | 0 | 0 | 4 |
| `Shared/Sts1TheWomanInBlue.cs` | 3 | 0 | 0 | 3 |
| `Shared/Sts1Designer.cs` | 3 | 0 | 0 | 3 |
| `Shared/Sts1OldBeggar.cs` | 1 | 0 | 0 | 1 |
| `Shared/Sts1BonfireSpirits.cs` | 1 | 1 | 0 | 2 |
| `Shared/Sts1FaceTrader.cs` | 1 | 1 | 0 | 2 |
| `Shared/Sts1FountainOfCleansing.cs` | 0 | 2 | 0 | 2 |
| `Shared/Sts1GoldenWing.cs` | 0 | 1 | 0 | 1 |
| `Shared/Sts1TheMausoleum.cs` | 2 | 0 | 0 | 2 |
| `Shared/Sts1TheLab.cs` | 1 | 0 | 0 | 1 |
| `Shared/Sts1DivineFountain.cs` | 0 | 1 | 0 | 1 |
| `Act1/Sts1DeadAdventurer.cs` | 2 | 0 | 0 | 2 |
| `Act1/Sts1ShiningLight.cs` | 0 | 1 | 0 | 1 |
| `Act1/Sts1Mushrooms.cs` | 0 | 2 | 0 | 2 |
| `Act1/Sts1Joust.cs` | 2 | 0 | 0 | 2 |
| `Act1/Sts1TheSsssserpent.cs` | 1 | 0 | 0 | 1 |
| `Act1/Sts1TreasureOoze.cs` | 1 | 0 | 0 | 1 |
| `Act2/Sts1Vampires.cs` | 1 | 1 | 0 | 2 |
| `Act2/Sts1TheLibrary.cs` | 0 | 2 | 0 | 2 |
| `Act2/Sts1Altar.cs` | 2 | 0 | 0 | 2 |
| `Act2/Sts1AncientWriting.cs` | 2 | 0 | 0 | 2 |
| `Act2/Sts1Augmenter.cs` | 2 | 0 | 0 | 2 |
| `Act2/Sts1ForgottenAltar.cs` | 2 | 2 | 0 | 4 |
| `Act2/Sts1MaskedBandits.cs` | 1 | 0 | 0 | 1 |
| `Act2/Sts1Nest.cs` | 1 | 0 | 0 | 1 |
| `Act2/Sts1KnowingSkull.cs` | 0 | 2 | 0 | 2 |
| `Act2/Sts1TheGhost.cs` | 0 | 1 | 0 | 1 |
| `Act2/Sts1DrugDealer.cs` | 1 | 0 | 0 | 1 |
| `Act2/Sts1CouncilOfGhosts.cs` | 0 | 1 | 0 | 1 |
| `Act2/Sts1CursedTome.cs` | 0 | 1 | 0 | 1 |
| `Act3/Sts1WindingHalls.cs` | 1 | 3 | 0 | 4 |
| `Act3/Sts1MindBloom.cs` | 1 | 1 | 0 | 2 |
| `Act3/Sts1Falling.cs` | 2 | 1 | 0 | 3 |
| `Act3/Sts1MoaiHead.cs` | 1 | 1 | 0 | 2 |
| `Act3/Sts1SensoryStone.cs` | 0 | 1 | 0 | 1 |
| `Act3/Sts1UpgradeShrine.cs` | 1 | 0 | 0 | 1 |
| `Act3/Sts1Transmogrifier.cs` | 1 | 0 | 0 | 1 |
| `Act3/Sts1TombOfLordRedMask.cs` | 3 | 0 | 0 | 3 |
| **Total** | **53** | **33** | **1** | **87** |

## Dependency

All 87 warnings depend on the Week 2 Sts1Events governance decision:

- **If Sts1Events goes formal**: Fix all nullable annotations in Sts1Events models.
- **If Sts1Events goes staging-only**: Warnings are acceptable in dormant code behind feature gate.
- **If Sts1Events is removed**: Warnings disappear with the code.

## History

| Date | Clean build warnings | Notes |
|---|---|---|
| 2026-05-29 (Rev D) | 69 | First clean-build count |
| 2026-05-29 (Rev E) | 87 | Updated after new IsShared property additions in 6 files |
| 2026-05-29 (M3 Week 1) | 92 | Updated after UrdaStateCodec/Sts1Events model changes at d290598c |
