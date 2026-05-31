# Warning Ledger — Revision I Current State

Date: 2026-05-31
Verified: Revision I project clean/build replay (`dotnet clean .\EZMicroBalance.csproj` + `dotnet build .\EZMicroBalance.csproj`) at HEAD `87820303`

## Summary

| Metric | Value |
|---|---|
| Total CS warnings | **89** in 2026-05-31 clean solution build |
| CS8604 (possible null reference argument) | **54** |
| CS8602 (dereference of possibly null reference) | **34** |
| CS8625 (cannot convert null literal) | **1** |
| Warnings outside Sts1Events/ | **0** |
| MSBuild locked-file/env artifact warnings | **0** in Revision I replay; earlier non-code artifacts remain ignored |

## All warnings are in `EZMicroBalanceCode/Sts1Events/Models/`

Every CS warning in the Revision I replay is a nullable reference type warning in the Sts1Events staging code.
No warnings exist in core Spire Plus code, RitsuLib integration, preview tools, or tests.

Current replay command: `dotnet clean .\EZMicroBalance.csproj` followed by `dotnet build .\EZMicroBalance.csproj` at HEAD `87820303` on 2026-05-31.

## Per-file Breakdown (Historical Revision E — 87 warnings)

**Note**: Current clean solution build produces 89 warnings. The per-file breakdown below is historical Revision E detail and is not the canonical current count; the current canonical count is the summary above and `docs/reviews/current-validation.md`.

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

All 89 current warnings depend on the Sts1Events governance decision:

- **If Sts1Events goes formal**: Fix all nullable annotations in Sts1Events models.
- **If Sts1Events goes staging-only**: Warnings are acceptable in dormant code behind feature gate.
- **If Sts1Events is removed**: Warnings disappear with the code.

## History

| Date | Clean build warnings | Notes |
|---|---|---|
| 2026-05-29 (Rev D) | 69 | First clean-build count |
| 2026-05-29 (Rev E) | 87 | Updated after new IsShared property additions in 6 files |
| 2026-05-29 (M3 Week 1) | 92 | Previous report after UrdaStateCodec/Sts1Events model changes at d290598c |
| 2026-05-31 (Revision H replay) | 89 | Historical clean solution build; all warnings in Sts1Events model staging code |
| 2026-05-31 (Revision I replay) | 89 | Current project clean/build replay at `87820303`; all warnings in Sts1Events model staging code |
