# StS1 Events — Multiplayer IsShared Matrix

Created: 2026-05-29 | Status: source/code-verified only; live two-client co-op proof pending

This matrix records source-level `IsShared` intent and expected multiplayer shape. It is not live co-op proof: no Sts1Events two-client session, save/load pass, screenshots, or desync-free traversal evidence is recorded yet. `CanaryOnly` and `AdditiveBatch1` registration remain controlled by `SPIREPLUS_STS1_EVENT_MODE`; there is no separate Sts1Events network-mode gate.

Revision L correction, 2026-06-10: this is an `IsShared`/co-op behavior matrix, not an Act-bucket parity matrix. Big Fish and Golden Idol now source-register to the StS2 Act 1 buckets while retaining `IsShared=true` co-op voting behavior; current runtime bucket and two-client proof remain pending.

## Legend

| Symbol | Meaning |
|--------|---------|
| `IsShared = true` | All players vote / same outcome / shared RNG. The host owns the event state and all players see the same result. |
| `IsShared = false` | Each player chooses independently. Default for `EventModel.IsShared`. Per-player state and RNG. |
| `[COMBAT]` | Combat event — MUST be `IsShared = true` because `EnterCombatWithoutExitingEvent` requires shared event context. |
| `[EXCLUDED]` | Compile-excluded model (`Sts1Duplicator.cs` removed via `<Compile Remove>` in `.csproj`). |

## Per-Event Matrix

### IsShared Event Models (18 models — all IsShared = true)

| # | Event ID | Model Class | Act | IsShared | Reason | Co-op Behavior | RNG Owner | Save/Load | Test Evidence |
|---|----------|-------------|-----|----------|--------|----------------|-----------|-----------|---------------|
| 1 | `sts1_big_fish` | `Sts1BigFish` | Act 1 | `true` | Act 1 event with shared vote behavior | All players vote Banana/Donut/Box; each player's heal is computed from own max HP | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue`; `BigFishUsesBoxOptionName` |
| 2 | `sts1_the_cleric` | `Sts1TheCleric` | Act 1 | `true` | Act 1 event with shared co-op vote behavior; eligible only when every player has at least 35 gold | All players vote Heal/Purify/Leave; each player spends own gold and selects own card; Purify costs 75 at A15+ | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue`; `TheClericUsesA15PurifyCostAndGoldEligibility`; `ActMappingUsesOvergrowthAndUnderdocksForAct1` |
| 3 | `sts1_golden_idol` | `Sts1GoldenIdol` | Act 1 | `true` | Act 1 event with shared vote behavior | All players vote Take/Leave then Outrun/Smash/Hide; damage and max HP loss computed per-player | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 4 | `sts1_golden_wing` | `Sts1GoldenWing` | All | `true` | Shared-act event; vote determines option for all | All players vote Accept/Decline; each player gets own rare card from own card pool | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 5 | `sts1_living_wall` | `Sts1LivingWall` | All | `true` | Shared-act event; vote determines option for all | All players vote Forget/Change/Trade; each player selects own card for removal/transform/upgrade | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 6 | `sts1_old_beggar` | `Sts1OldBeggar` | All | `true` | Shared-act event; vote determines option for all | All players vote Offer/Leave; Offer is source-gated at 75+ gold, then each player spends own 75g and removes own card | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 7 | `sts1_purifier` | `Sts1Purifier` | All | `true` | Shared-act event; vote determines option for all | All players vote Purify/Leave; each player removes own card | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 8 | `sts1_golden_shrine` | `Sts1GoldenShrine` | All | `true` | Shared-act event; vote determines option for all | All players vote Pray/Desecrate/Leave; each player gains own gold; Desecrate also adds Regret to each player's own deck | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue`; `GoldenShrineUsesWikiGoldAndRegretOptions` |
| 9 | `sts1_the_woman_in_blue` | `Sts1TheWomanInBlue` | All | `true` | Shared-act event; vote determines option for all | All players vote Buy1/Buy2/Buy3/Leave; each player spends own gold and gets own potion | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 10 | `sts1_bonfire_spirits` | `Sts1BonfireSpirits` | All | `true` | Shared-act event; vote determines option for all | All players vote Offer/Leave; each player removes own card and heals to own max HP | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 11 | `sts1_divine_fountain` | `Sts1DivineFountain` | All | `true` | Shared-act event; vote determines option for all | Eligible only when every player has at least one curse; all players vote Pray/Leave; each player's curses removed from own deck | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue`; `DivineFountainRequiresEveryPlayerToHaveACurse` |
| 12 | `sts1_duplicator` | `Sts1Duplicator` | All | `true` | Shared-act event; vote determines option for all `[EXCLUDED]` | All players vote Duplicate/Leave; each player selects own card to duplicate | Host | Shared state on host | `Sts1DuplicatorExcludedFromCompilation` |
| 13 | `sts1_face_trader` | `Sts1FaceTrader` | All | `true` | Shared-act event; vote determines option for all | All players vote Trade/Leave; each player loses own max HP and gets own relic | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 14 | `sts1_fountain_of_cleansing` | `Sts1FountainOfCleansing` | All | `true` | Shared-act event; vote determines option for all | All players vote Drink/Leave; each player's curses removed and max HP lost independently | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 15 | `sts1_the_mausoleum` | `Sts1TheMausoleum` | All | `true` | Shared-act event; shared RNG roll | All players vote Open/Leave; 50/50 roll is shared — all get relic or all get curse | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 16 | `sts1_wheel_of_change` | `Sts1WheelOfChange` | All | `true` | Shared-act event; shared RNG spin | All players spin together; same outcome (gold/damage/relic/curse/heal/remove) for all, but effects applied per-player | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 17 | `sts1_designer` | `Sts1Designer` | All | `true` | Shared-act event; vote determines option for all | All players vote Upgrade/Remove/Transform/Leave; each player acts on own cards | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |
| 18 | `sts1_the_lab` | `Sts1TheLab` | All | `true` | Shared-act event; vote determines option for all | All players vote Open/Leave; each player receives 3 own random potions | Host | Shared state on host | `AllSharedEventModelsDeclareIsSharedTrue` |

### Act 1 Events (7 models)

| # | Event ID | Model Class | Act | IsShared | Reason | Co-op Behavior | RNG Owner | Save/Load | Test Evidence |
|---|----------|-------------|-----|----------|--------|----------------|-----------|-----------|---------------|
| 17 | `sts1_joust` | `Sts1Joust` | 1 | `false` | Per-player gambling choice; each player bets independently | Each player independently bets on Self/Opponent; own gold spent, own 50/50 roll | Each player | Per-player state | — |
| 18 | `sts1_the_ssssserpent` | `Sts1TheSsssserpent` | 1 | `false` | Per-player accept/refuse decision | Each player independently accepts (150g + Doubt curses) or refuses | Each player | Per-player state | — |
| 19 | `sts1_shining_light` | `Sts1ShiningLight` | 1 | `false` | Per-player enter/leave decision; per-player random card upgrades | Each player independently enters (takes damage, upgrades 2 random own upgradable deck cards) or leaves | Each player | Per-player state | — |
| 20 | `sts1_dead_adventurer` | `Sts1DeadAdventurer` | 1 | `true` | `[COMBAT]` — may enter elite combat via `EnterCombatWithoutExitingEvent` | All players search together; shared RNG roll determines gold/relic/combat; combat is shared | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |
| 21 | `sts1_mushrooms` | `Sts1Mushrooms` | 1 | `false` | Per-player eat/leave decision; per-player outcome | Each player independently eats (50/50 max HP gain/loss + potion) or leaves | Each player | Per-player state | — |
| 22 | `sts1_scorpion_nest` | `Sts1ScorpionNest` | 1 | `true` | `[COMBAT]` — fights 3 Louses via `EnterCombatWithoutExitingEvent` | All players investigate together; shared combat encounter; relic reward per-player after combat | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |
| 23 | `sts1_treasure_ooze` | `Sts1TreasureOoze` | 1 | `true` | `[COMBAT]` — fight option uses `EnterCombatWithoutExitingEvent` | All players vote Offer/Fight/Leave; combat path is shared; offer path is per-player gold/relic | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |

### Act 2 Events (13 models)

| # | Event ID | Model Class | Act | IsShared | Reason | Co-op Behavior | RNG Owner | Save/Load | Test Evidence |
|---|----------|-------------|-----|----------|--------|----------------|-----------|-----------|---------------|
| 24 | `sts1_altar` | `Sts1Altar` | 2 | `false` | Per-player pray/sacrifice choice | Each player independently prays (upgrade 3 cards) or sacrifices (remove card + relic) | Each player | Per-player state | — |
| 25 | `sts1_council_of_ghosts` | `Sts1CouncilOfGhosts` | 2 | `false` | Per-player accept/refuse decision | Each player independently accepts Apparitions (5 or 3 at A15) + loses 50% max HP | Each player | Per-player state | — |
| 26 | `sts1_cursed_tome` | `Sts1CursedTome` | 2 | `false` | Per-player read/leave decision | Each player independently reads (takes HP damage, gets rare relic) or leaves | Each player | Per-player state | — |
| 27 | `sts1_drug_dealer` | `Sts1DrugDealer` | 2 | `false` | Per-player buy/leave decision | Each player independently buys 3 potions for 60g | Each player | Per-player state | — |
| 28 | `sts1_forgotten_altar` | `Sts1ForgottenAltar` | 2 | `false` | Per-player choice among 4 options | Each player independently chooses Pray/Offer/Desecrate/Leave with own gold/HP consequences | Each player | Per-player state | — |
| 29 | `sts1_the_ghost` | `Sts1TheGhost` | 2 | `false` | Per-player accept/refuse decision | Each player independently accepts (random rare card) or refuses | Each player | Per-player state | — |
| 30 | `sts1_knowing_skull` | `Sts1KnowingSkull` | 2 | `false` | Per-player question selection; per-player HP cost | Each player independently asks questions; pays own HP, gets own rare cards | Each player | Per-player state | — |
| 31 | `sts1_nest` | `Sts1Nest` | 2 | `false` | Per-player search/leave decision | Each player independently searches (relic + Parasite/Clumsy curses) | Each player | Per-player state | — |
| 32 | `sts1_the_library` | `Sts1TheLibrary` | 2 | `false` | Per-player card selection | Each player independently reads (chooses 1 of 20 cards) or rests (heal 1/3 HP) | Each player | Per-player state | — |
| 33 | `sts1_masked_bandits` | `Sts1MaskedBandits` | 2 | `true` | `[COMBAT]` — fight option uses `EnterCombatWithoutExitingEvent` | All players vote Pay/Fight; combat path is shared; pay path is per-player gold | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |
| 34 | `sts1_nloth` | `Sts1Nloth` | 2 | `false` | Per-player relic trade decision (BLOCKED: no RelicSelectCmd) | Each player independently offers a relic for a random relic (when implemented) | Each player | Per-player state | — |
| 35 | `sts1_vampires` | `Sts1Vampires` | 2 | `false` | Per-player accept/refuse decision | Each player independently accepts (remove Strikes + lose max HP) or refuses | Each player | Per-player state | — |
| 36 | `sts1_ancient_writing` | `Sts1AncientWriting` | 2 | `false` | Per-player elegance/simplicity choice | Each player independently chooses Elegance (upgrade card) or Simplicity (remove card) | Each player | Per-player state | — |
| 37 | `sts1_augmenter` | `Sts1Augmenter` | 2 | `false` | Per-player transform/mutate choice | Each player independently chooses Transform (2 cards), Mutate (upgrade), or Reject | Each player | Per-player state | — |

### Act 3 Events (9 models)

| # | Event ID | Model Class | Act | IsShared | Reason | Co-op Behavior | RNG Owner | Save/Load | Test Evidence |
|---|----------|-------------|-----|----------|--------|----------------|-----------|-----------|---------------|
| 38 | `sts1_sensory_stone` | `Sts1SensoryStone` | 3 | `false` | Per-player card selection | Each player independently touches (chooses 1 of 3 rare cards) or leaves | Each player | Per-player state | — |
| 39 | `sts1_falling` | `Sts1Falling` | 3 | `false` | Per-player let-go/hold-on/fly choice | Each player independently chooses Let Go (remove), Hold On (take damage), or Fly (transform) | Each player | Per-player state | — |
| 40 | `sts1_mind_bloom` | `Sts1MindBloom` | 3 | `true` | `[COMBAT]` — War option uses `EnterCombatWithoutExitingEvent` | All players vote War/Awake/Rich/Leave; combat path is shared; Awake/Rich are per-player effects | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |
| 41 | `sts1_moai_head` | `Sts1MoaiHead` | 3 | `false` | Per-player worship/offer choice | Each player independently chooses Worship (+1 max HP), Offer (50g for +3 max HP), or Leave | Each player | Per-player state | — |
| 42 | `sts1_mysterious_sphere` | `Sts1MysteriousSphere` | 3 | `true` | `[COMBAT]` — fights 2 Orb Walkers via `EnterCombatWithoutExitingEvent` | All players open together; shared combat encounter; relic reward per-player after combat | Host | Shared state on host | `CombatEventsDeclareIsSharedTrue` |
| 43 | `sts1_tomb_of_lord_red_mask` | `Sts1TombOfLordRedMask` | 3 | `false` | Per-player offer decision | Each player independently offers 50g, all gold, or leaves; gets own relic | Each player | Per-player state | — |
| 44 | `sts1_winding_halls` | `Sts1WindingHalls` | 3 | `false` | Per-player embrace/retreat/continue choice | Each player independently chooses Embrace (Madness + max HP loss), Retreat (damage), or Continue (max HP loss) | Each player | Per-player state | — |
| 45 | `sts1_transmogrifier` | `Sts1Transmogrifier` | 3 | `false` | Per-player card transform | Each player independently transforms own card or leaves | Each player | Per-player state | — |
| 46 | `sts1_upgrade_shrine` | `Sts1UpgradeShrine` | 3 | `false` | Per-player card upgrade | Each player independently upgrades own card or leaves | Each player | Per-player state | — |

## Summary

### Counts

| Category | Count |
|----------|-------|
| Total event models in this matrix | 46 |
| Compiling event models in this matrix | 45 |
| Compile-excluded models | 1 (`Sts1Duplicator.cs`) |
| **IsShared = true** | **24** (18 shared-behavior event models + 6 combat) |
| **IsShared = false** | **23** (4 Act1 + 12 Act2 + 7 Act3 non-combat) |

### Combat Events with IsShared = true (6 events)

All guarded by `CombatEventsDeclareIsSharedTrue` test:

| Event | Model File | Combat Trigger |
|-------|-----------|----------------|
| Dead Adventurer | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1DeadAdventurer.cs` | Elite combat (TODO: encounter model) |
| Scorpion Nest | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1ScorpionNest.cs` | 3 Louse combat (TODO: encounter model) |
| Treasure Ooze | `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1TreasureOoze.cs` | Large Slime combat (TODO: encounter model) |
| Masked Bandits | `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1MaskedBandits.cs` | 3 Bandit combat (TODO: encounter model) |
| Mind Bloom | `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MindBloom.cs` | Act 1 boss combat (TODO: encounter model) |
| Mysterious Sphere | `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MysteriousSphere.cs` | 2 Orb Walker combat (TODO: encounter model) |

### IsShared Event Models with IsShared = true (18 models)

All guarded by `AllSharedEventModelsDeclareIsSharedTrue` test (verifies every `.cs` in `Models/Shared/`):

Big Fish, The Cleric, Golden Idol, Golden Wing, Living Wall, Old Beggar, Purifier, Golden Shrine, The Woman in Blue, Bonfire Spirits, Divine Fountain, Duplicator (excluded), Face Trader, Fountain of Cleansing, The Mausoleum, Wheel of Change, Designer, The Lab.

### Events Where IsShared Decision Is Debatable

| Event | Wiki Category | Current | Analysis |
|-------|---------------|---------|----------|
| Joust | Combat | `false` | Wiki labels this "Combat" but the model has no `EnterCombatWithoutExitingEvent` call. It is a gambling/betting event with independent per-player gold outcomes. `false` is correct. |
| The Ssssserpent | Combat | `false` | Wiki labels this "Combat" but the model has no `EnterCombatWithoutExitingEvent` call. It is a gold+curse trade event with no combat encounter. `false` is correct. |
| Big Fish | Shared | `true` | Per-player heal computed from own max HP. In `IsShared=true`, all players vote on the same option but each receives individually computed rewards. Correct for shared decision. |
| The Mausoleum | Shared | `true` | 50/50 RNG roll is shared — all players get the same result (relic or curse). The `Rng.NextInt` call uses the host-owned seed. Intentional design. |
| Wheel of Change | Shared | `true` | 6-way RNG spin is shared — all players get the same outcome category. Effects (gold, damage, etc.) are applied per-player with individual values. Intentional design. |
| Mind Bloom | Combat | `true` | Only the "War" option triggers combat. "Awake" and "Rich" are per-player effects. The `IsShared=true` is required because one option path enters combat. All players must vote on the same option. |

## Co-op Risk List

Events where multiplayer behavior might be surprising, buggy, or require special attention during testing:

The mitigations below are source/design dispositions until a real two-client session proves them.

### High Risk

| Event | Risk | Mitigation |
|-------|------|------------|
| **Wheel of Change** | Shared RNG spin means all players get the same outcome. If one outcome is bad (damage, curse), ALL players suffer. This may feel punishing in co-op. | Documented shared behavior. No code change needed — this matches StS1 design intent. |
| **The Mausoleum** | Shared 50/50 means all players get relic or all get curse. At A15 (always curse), all players are cursed. | Matches StS1 design. A15 check uses per-player ascension level — verify all players have same ascension or handle mismatch. |
| **Council of Ghosts** | `IsShared=false` means each player independently accepts Apparitions. In a 2-player game, one player might accept (losing 50% max HP) while the other refuses. This creates asymmetric deck states. | Correct behavior for per-player choice. No mitigation needed. |
| **Dead Adventurer / Scorpion Nest / Treasure Ooze / Masked Bandits / Mysterious Sphere / Mind Bloom** | Combat encounters are TODO. The `EnterCombatWithoutExitingEvent` call is not yet implemented. Until combat is wired, these events finish immediately on the combat option. | Implement combat encounter models before private beta. Guard tests verify `IsShared=true` but not combat functionality. |

### Medium Risk

| Event | Risk | Mitigation |
|-------|------|------------|
| **Big Fish / Golden Idol / Face Trader / Fountain of Cleansing** | Per-player HP/max HP calculations use `Owner.Creature.MaxHp` which varies by player. In `IsShared=true`, all players vote together but receive individually scaled damage/healing. | Correct behavior. Test with players at different HP levels. |
| **Living Wall / Old Beggar / Bonfire Spirits / Designer** | Card selection UI (`OpenCardRemoval`, `OpenCardUpgrade`, `OpenCardTransform`) must be opened for each player individually after the shared vote. Verify the game handles sequential per-player card selection in a shared event. | Test that card selection UI appears for each player after the shared vote resolves. |
| **The Woman in Blue / The Lab / Drug Dealer** | Potion grants are per-player. In `IsShared=true`, all players buy/receive potions together. Verify potion slots are checked per-player (a player with full potion slots may not receive potions). | Test with players at different potion slot states. |
| **Vampires** | Partially implemented — removes Strikes but cannot add Bite cards (no Bite model in StS2). `IsShared=false` means each player independently decides, which is correct. But the incomplete implementation means accepting players only lose Strikes + max HP without gaining Bites. | Document as `temporary-substitute`. Create Bite card model before release. |

### Low Risk

| Event | Risk | Mitigation |
|-------|------|------------|
| **Duplicator** | Compile-excluded. Not registered in any mode. No runtime risk. | Re-enable when `CardSelectCmd`/`CardPileCmd` APIs are verified. |
| **N'loth** | BLOCKED — no `RelicSelectCmd` API. Offer option is a no-op stub. `IsShared=false` is correct for future per-player relic selection. | Implement when relic selection UI API is available. |
| **Joust / The Ssssserpent** | Labeled "Combat" in wiki complexity but have no `EnterCombatWithoutExitingEvent` call. `IsShared=false` is correct. No actual combat risk. | No action needed. Complexity label refers to StS1 wiki classification, not runtime combat. |

## Guard Test Reference

The following tests in `tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs` verify IsShared declarations:

| Test Name | What It Verifies |
|-----------|------------------|
| `CombatEventsDeclareIsSharedTrue` | All 6 combat event model files contain `public override bool IsShared => true;` |
| `AllSharedEventModelsDeclareIsSharedTrue` | All `.cs` files in `Models/Shared/` (≥15) contain `public override bool IsShared => true;` |
| `RegisterAllSharedEventCountIs14` | `RegisterAll` registers exactly 14 `SharedEvent<>` calls after Big Fish, Golden Idol, and The Cleric moved to Act 1 bucket registrations |
| `RegistryEntryCountIs50` | Total registry entries: 47 compiling + 1 excluded + 2 special stubs = 50 |
| `Sts1DuplicatorExcludedFromCompilation` | `Sts1Duplicator.cs` is in `<Compile Remove>` in `.csproj` |

## File Paths Reference

All model files are under `EZMicroBalanceCode/Sts1Events/Models/`:

```
Models/Shared/          (16 files — all IsShared=true)
  Sts1BigFish.cs
  Sts1BonfireSpirits.cs
  Sts1Designer.cs
  Sts1DivineFountain.cs
  Sts1Duplicator.cs        [EXCLUDED]
  Sts1FaceTrader.cs
  Sts1FountainOfCleansing.cs
  Sts1GoldenIdol.cs
  Sts1GoldenWing.cs
  Sts1LivingWall.cs
  Sts1OldBeggar.cs
  Sts1TheCleric.cs
  Sts1TheLab.cs
  Sts1TheMausoleum.cs
  Sts1TheWomanInBlue.cs
  Sts1WheelOfChange.cs

Models/Act1/            (7 files — 3 IsShared=true, 4 IsShared=false)
  Sts1DeadAdventurer.cs    IsShared=true  [COMBAT]
  Sts1Joust.cs             IsShared=false
  Sts1Mushrooms.cs         IsShared=false
  Sts1ScorpionNest.cs      IsShared=true  [COMBAT]
  Sts1ShiningLight.cs      IsShared=false
  Sts1TheSsssserpent.cs    IsShared=false
  Sts1TreasureOoze.cs      IsShared=true  [COMBAT]

Models/Act2/            (13 files — 1 IsShared=true, 12 IsShared=false)
  Sts1Altar.cs             IsShared=false
  Sts1AncientWriting.cs    IsShared=false
  Sts1Augmenter.cs         IsShared=false
  Sts1CouncilOfGhosts.cs   IsShared=false
  Sts1CursedTome.cs        IsShared=false
  Sts1DrugDealer.cs        IsShared=false
  Sts1ForgottenAltar.cs    IsShared=false
  Sts1KnowingSkull.cs      IsShared=false
  Sts1MaskedBandits.cs     IsShared=true  [COMBAT]
  Sts1Nest.cs              IsShared=false
  Sts1Nloth.cs             IsShared=false
  Sts1TheGhost.cs          IsShared=false
  Sts1TheLibrary.cs        IsShared=false
  Sts1Vampires.cs          IsShared=false

Models/Act3/            (9 files — 2 IsShared=true, 7 IsShared=false)
  Sts1Falling.cs           IsShared=false
  Sts1MindBloom.cs         IsShared=true  [COMBAT]
  Sts1MoaiHead.cs          IsShared=false
  Sts1MysteriousSphere.cs  IsShared=true  [COMBAT]
  Sts1SensoryStone.cs      IsShared=false
  Sts1TombOfLordRedMask.cs IsShared=false
  Sts1Transmogrifier.cs    IsShared=false
  Sts1UpgradeShrine.cs     IsShared=false
  Sts1WindingHalls.cs      IsShared=false
```
