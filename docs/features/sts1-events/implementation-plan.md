# StS1 Events Implementation Plan

## Phase 0: Infrastructure (Current)

- [x] Create documentation structure under `docs/features/sts1-events/`
- [x] Create code scaffolding under `EZMicroBalanceCode/Sts1Events/`
- [x] Create localization files under `EZMicroBalance/localization/`
- [x] Create asset extraction scripts
- [x] Create event manifests
- [ ] Verify `dotnet build` compiles with new event files

## Phase 1: Canary Events

Implement Big Fish and Golden Idol to prove the full pipeline works.

### Big Fish (Act 1 Shared)

**Wiki behavior:**
- Option 1: Heal 1/3 max HP
- Option 2: +5 max HP
- Option 3: Obtain a random relic + Regret curse

**Implementation:**
- Extend `ModEventTemplate`
- Register with `[RegisterSharedEvent]` (shared across acts)
- Localization: `BIG_FISH.pages.INITIAL.description`, options
- Dynamic vars: `HealAmount` (1/3 max HP), `MaxHpGain` (5)

### Golden Idol (Act 1 Shared)

**Wiki behavior:**
- Initial: Take Golden Idol (relic) or Leave
- If take: trap triggers →
  - Option A: Obtain Injury curse
  - Option B: Lose 25% current HP (A15: 35%)
  - Option C: Lose 10% max HP (A15: 15%)

**Implementation:**
- Extend `ModEventTemplate`
- Register with `[RegisterSharedEvent]`
- Multi-page event: INITIAL → TRAP → done
- Requires Golden Idol relic model (or use existing if StS2 has one)

## Phase 2: Simple Batch

Events with straightforward heal/damage/gold/card rewards:

The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits,
Divine Fountain, Duplicator, Fountain of Cleansing, The Lab, Shining Light,
Mushrooms, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter,
Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine

## Phase 3: Card Service Batch

Events that add/remove/transform cards:

Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome,
Knowing Skull, Nest, Vampires, Falling, Mind Bloom

## Phase 4: Combat Batch

Events that trigger combat encounters:

Dead Adventurer, Masked Bandits, Mysterious Sphere, Scorpion Nest,
Treasure Ooze, Joust, The Ssssserpent

## Phase 5: Custom UI Batch

Events requiring minigame layouts — use simplified option-based fallbacks:

Wheel of Change, Designer, The Woman in Blue, The Ghost, Nloth,
Tomb of Lord Red Mask, Winding Halls, Forgotten Altar

## Phase 6: Pool Replacement

Implement `ReplaceUnknownEvents` to create an StS1-only event pool that
replaces StS2 events in Unknown rooms. This requires patching the event
selection system.

## Dependencies Per Phase

### Phase 1 Dependencies
- Regret curse card model
- Injury curse card model
- Random relic reward helper
- HP/max HP command helpers (already exist in game API)

### Phase 2 Dependencies
- Gold reward helper
- Card reward helper
- Potion reward helper

### Phase 4 Dependencies
- Combat encounter models for each event's monsters
- Combat reward setup

### Phase 6 Dependencies
- Event pool replacement patch
- Act-specific event filtering

## File Structure

```
EZMicroBalanceCode/Sts1Events/
  Runtime/
    Sts1EventRegistry.cs          # Event registration with RitsuLib
    Sts1EventPoolPatch.cs         # Phase 6: pool replacement
  Models/
    Shared/
      Sts1BigFish.cs              # Phase 1 canary
      Sts1GoldenIdol.cs           # Phase 1 canary
      Sts1TheCleric.cs            # Phase 2
      Sts1DivineFountain.cs       # Phase 2
      Sts1BonfireSpirits.cs       # Phase 2
      Sts1LivingWall.cs           # Phase 2
      Sts1OldBeggar.cs            # Phase 2
      Sts1GoldenWing.cs           # Phase 2
      Sts1Duplicator.cs           # Phase 2
      Sts1FountainOfCleansing.cs  # Phase 2
      Sts1FaceTrader.cs           # Phase 3
      Sts1TheMausoleum.cs         # Phase 3
      Sts1WheelOfChange.cs        # Phase 5
      ...
    Act1/
      Sts1ShiningLight.cs         # Phase 2
      Sts1DeadAdventurer.cs       # Phase 4
      Sts1Mushrooms.cs            # Phase 2
      Sts1ScorpionNest.cs         # Phase 4
      Sts1TreasureOoze.cs         # Phase 4
      Sts1Joust.cs                # Phase 4
      Sts1TheSsssserpent.cs       # Phase 4
    Act2/
      Sts1Altar.cs                # Phase 2
      Sts1CouncilOfGhosts.cs      # Phase 3
      Sts1CursedTome.cs           # Phase 3
      Sts1DrugDealer.cs           # Phase 2
      Sts1ForgottenAltar.cs       # Phase 5
      Sts1KnowingSkull.cs         # Phase 3
      Sts1Nest.cs                 # Phase 3
      Sts1TheLibrary.cs           # Phase 2
      Sts1MaskedBandits.cs        # Phase 4
      Sts1Nloth.cs                # Phase 5
      Sts1Vampires.cs             # Phase 3
      Sts1AncientWriting.cs       # Phase 2
      Sts1Augmenter.cs            # Phase 2
    Act3/
      Sts1SensoryStone.cs         # Phase 2
      Sts1Falling.cs              # Phase 3
      Sts1MindBloom.cs            # Phase 3
      Sts1MoaiHead.cs             # Phase 2
      Sts1MysteriousSphere.cs     # Phase 4
      Sts1TombOfLordRedMask.cs    # Phase 5
      Sts1WindingHalls.cs         # Phase 5
      Sts1Transmogrifier.cs       # Phase 2
      Sts1UpgradeShrine.cs        # Phase 2

EZMicroBalance/localization/
  eng/sts1_events.json
  zhs/sts1_events.json

scripts/
  extract-sts1-event-assets.ps1
  validate-sts1-event-assets.ps1

manifests/
  sts1_events_manifest.csv
  sts1_events_manifest.json
  asset_manifest.csv
```
