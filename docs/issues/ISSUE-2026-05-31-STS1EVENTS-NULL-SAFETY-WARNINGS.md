# ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS

## Status

**Open — tracked warning debt.** Current clean solution build records 89 nullable warnings, all inside `EZMicroBalanceCode/Sts1Events/Models/` staging code.

## Warning Matrix

| Code | Count | Scope | Risk | Fix batch |
| --- | ---: | --- | --- | --- |
| `CS8604` | 54 | Possible null argument passed to player/card/relic helper APIs | Runtime null risk if Sts1Events modes are enabled without source hardening | Sts1Events null-guard batch 1 |
| `CS8602` | 34 | Possible null dereference in event option handlers | Runtime null risk if event state/player references are absent | Sts1Events null-guard batch 2 |
| `CS8625` | 1 | Null literal passed to a non-nullable reference | Runtime null risk in one Golden Idol code path | Sts1Events null-guard batch 1 |

## Per-File Triage

Owner for every row: Sts1Events implementation pass.

| File | `CS8602` | `CS8604` | `CS8625` | Disposition |
| --- | ---: | ---: | ---: | --- |
| `Models/Act1/Sts1DeadAdventurer.cs` | 0 | 2 | 0 | Defer; unsafe draft combat event |
| `Models/Act1/Sts1Joust.cs` | 0 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act1/Sts1Mushrooms.cs` | 2 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act1/Sts1ShiningLight.cs` | 1 | 0 | 0 | Defer; AdditiveBatch1 prototype guard needed before runtime use |
| `Models/Act1/Sts1TheSsssserpent.cs` | 0 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act1/Sts1TreasureOoze.cs` | 0 | 1 | 0 | Defer; unsafe draft combat event |
| `Models/Act2/Sts1Altar.cs` | 0 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1AncientWriting.cs` | 0 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1Augmenter.cs` | 0 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1CouncilOfGhosts.cs` | 1 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1CursedTome.cs` | 1 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1DrugDealer.cs` | 0 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1ForgottenAltar.cs` | 2 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1KnowingSkull.cs` | 2 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1MaskedBandits.cs` | 0 | 1 | 0 | Defer; unsafe draft combat event |
| `Models/Act2/Sts1Nest.cs` | 0 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1TheGhost.cs` | 1 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1TheLibrary.cs` | 2 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act2/Sts1Vampires.cs` | 1 | 1 | 0 | Defer; partial event, not release-safe |
| `Models/Act3/Sts1Falling.cs` | 1 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1MindBloom.cs` | 1 | 1 | 0 | Defer; blocked combat option |
| `Models/Act3/Sts1MoaiHead.cs` | 1 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1SensoryStone.cs` | 1 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1TombOfLordRedMask.cs` | 0 | 3 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1Transmogrifier.cs` | 0 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1UpgradeShrine.cs` | 0 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Act3/Sts1WindingHalls.cs` | 3 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1BigFish.cs` | 2 | 1 | 0 | Fix before CanaryOnly live canary proof |
| `Models/Shared/Sts1BonfireSpirits.cs` | 1 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1Designer.cs` | 0 | 3 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1DivineFountain.cs` | 1 | 0 | 0 | Fix before CanaryOnly live canary proof |
| `Models/Shared/Sts1FaceTrader.cs` | 1 | 1 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1FountainOfCleansing.cs` | 2 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1GoldenIdol.cs` | 3 | 1 | 1 | Fix before CanaryOnly live canary proof |
| `Models/Shared/Sts1GoldenShrine.cs` | 1 | 2 | 0 | Defer; AdditiveBatch1 prototype guard needed before runtime use |
| `Models/Shared/Sts1GoldenWing.cs` | 1 | 0 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1LivingWall.cs` | 0 | 3 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1OldBeggar.cs` | 0 | 1 | 0 | Defer; AdditiveBatch1 prototype guard needed before runtime use |
| `Models/Shared/Sts1Purifier.cs` | 0 | 1 | 0 | Defer; AdditiveBatch1 prototype guard needed before runtime use |
| `Models/Shared/Sts1TheCleric.cs` | 0 | 2 | 0 | Defer; AdditiveBatch1 prototype guard needed before runtime use |
| `Models/Shared/Sts1TheLab.cs` | 0 | 1 | 0 | Fix before CanaryOnly live canary proof |
| `Models/Shared/Sts1TheMausoleum.cs` | 0 | 2 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1TheWomanInBlue.cs` | 0 | 3 | 0 | Defer; prototype event option guards needed |
| `Models/Shared/Sts1WheelOfChange.cs` | 2 | 4 | 0 | Defer; prototype event option guards needed |

## Decision

- Accepted only while Sts1Events remains default Off and runtime-unverified.
- Must be fixed before any Sts1Events mode is promoted beyond source-test/runtime-smoke scopes.
- CanaryOnly warnings in Big Fish, Golden Idol, The Lab, and Divine Fountain are first priority before live canary expansion.
- AdditiveBatch1 warnings are not release-safe and stay prototype-only until runtime smoke and null hardening pass.

## Cleanup Plan

1. Add explicit player/run/event-state guards in CanaryOnly event option handlers.
2. Prefer fail-closed event option behavior over null-forgiving operators.
3. Recount warnings only from `dotnet clean EZMicroBalance.sln` followed by `dotnet build EZMicroBalance.sln` raw output.
4. Keep `docs/reviews/current-validation.md` and this issue as the current warning count sources.
