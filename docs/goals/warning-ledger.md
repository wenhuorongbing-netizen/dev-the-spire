# Warning Ledger - Revision J Current State

Date: 2026-05-31
HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`

## Summary

| Metric | Value |
|---|---:|
| Total CS warnings | 89 |
| CS8604 possible null reference argument | 54 |
| CS8602 dereference of possibly null reference | 34 |
| CS8625 cannot convert null literal | 1 |
| Warnings outside `EZMicroBalanceCode/Sts1Events/Models/` | 0 |
| MSBuild locked-file/env artifact warnings | 0 in the current warning classification |

All warnings are owned by Sts1Events staging model code. They remain accepted only while Sts1Events stays default-Off/prototype-gated and are not acceptable as formal runtime-ready code.

## Current Per-File Breakdown

| File | CS8602 | CS8604 | CS8625 | Total | Owner |
|---|---:|---:|---:|---:|---|
| `Act1/Sts1DeadAdventurer.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act1/Sts1Joust.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Act1/Sts1Mushrooms.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Act1/Sts1ShiningLight.cs` | 1 | 0 | 0 | 1 | Sts1Events |
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
| `Act3/Sts1UpgradeShrine.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Act3/Sts1WindingHalls.cs` | 3 | 1 | 0 | 4 | Sts1Events |
| `Shared/Sts1BigFish.cs` | 2 | 1 | 0 | 3 | Sts1Events |
| `Shared/Sts1BonfireSpirits.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Shared/Sts1Designer.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1DivineFountain.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Shared/Sts1FaceTrader.cs` | 1 | 1 | 0 | 2 | Sts1Events |
| `Shared/Sts1FountainOfCleansing.cs` | 2 | 0 | 0 | 2 | Sts1Events |
| `Shared/Sts1GoldenIdol.cs` | 3 | 1 | 1 | 5 | Sts1Events |
| `Shared/Sts1GoldenShrine.cs` | 1 | 2 | 0 | 3 | Sts1Events |
| `Shared/Sts1GoldenWing.cs` | 1 | 0 | 0 | 1 | Sts1Events |
| `Shared/Sts1LivingWall.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1OldBeggar.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Shared/Sts1Purifier.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Shared/Sts1TheCleric.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Shared/Sts1TheLab.cs` | 0 | 1 | 0 | 1 | Sts1Events |
| `Shared/Sts1TheMausoleum.cs` | 0 | 2 | 0 | 2 | Sts1Events |
| `Shared/Sts1TheWomanInBlue.cs` | 0 | 3 | 0 | 3 | Sts1Events |
| `Shared/Sts1WheelOfChange.cs` | 2 | 4 | 0 | 6 | Sts1Events |
| **Total** | **34** | **54** | **1** | **89** | **Sts1Events** |

## Governance Decision

- Sts1Events recommendation: staging-only.
- Formalization blocker: fix or explicitly risk-accept the 89 nullable warnings, then prove Off=0 and CanaryOnly=4 in live `godot.log` evidence.
- Removal option: would clear warning debt but would discard intentionally staged prototype infrastructure; requires owner decision.

## History

| Date | Clean build warnings | Notes |
|---|---:|---|
| 2026-05-29 Rev D | 69 | Historical first clean-build count |
| 2026-05-29 Rev E | 87 | Historical count after early `IsShared` updates |
| 2026-05-29 M3 Week 1 | 92 | Historical count after UrdaStateCodec/Sts1Events changes |
| 2026-05-31 Revision I/J | 89 | Current count; all warnings in Sts1Events model staging code |
