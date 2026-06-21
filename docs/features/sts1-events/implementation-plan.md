# StS1 Events Implementation Plan

Current boundary, Revision P / beta.93: this roadmap is source planning, not gameplay proof. Retained beta.85 `v0.107.0` loader evidence covers default-Off startup/patch application and CanaryOnly 4 event types / 6 registration calls as previous-package context; retained beta.87 `v0.107.0` AdditiveBatch1 proof covers 10 event types / 14 registration calls as previous-game-version context; beta.88 covers previous package `v0.107.1` AdditiveBatch1 loader/registration context. Previous beta.93 proof under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` covers `v0.107.1` RitsuLib-only Off/AdditiveBatch1 loader/registration only with STS2-RitsuLib `v0.4.31`, 25/25 patches, clean audit, retained enabled-mode verifier 31 / 0, and AdditiveBatch1 runtime packet verifier 61 / 0. Gameplay, game-native AutoSlay/monkey batch stability, save-load, replacement, multiplayer, image/render, QA, and handoff proof remain pending. Historical `v0.106.1` enabled-mode loader logs remain history only, and the beta.85 AdditiveBatch1 13/14 verifier mismatch plus beta.87 `v0.107.1` previous package `v3.2.1` failure remain root-cause history only.

## Phase 0: Infrastructure (Current)

- [x] Create documentation structure under `docs/features/sts1-events/`
- [x] Create code scaffolding under `EZMicroBalanceCode/Sts1Events/`
- [x] Create localization files under `EZMicroBalance/localization/`
- [x] Create asset extraction scripts
- [x] Create event manifests
- [x] Verify source builds with the current StS1 event files; runtime proof remains separate

## Phase 1: Canary Events

Implement Big Fish, Golden Idol, The Lab, and Divine Fountain to prove the bounded canary pipeline before broader runtime claims.

### Big Fish (Act 1, IsShared)

**Wiki behavior:**
- Option 1: Heal 1/3 max HP
- Option 2: +5 max HP
- Option 3: Obtain a random relic + Regret curse

**Implementation:**
- Extend `EventModel`
- Register to the StS2 Act 1 buckets (`Overgrowth` and `Underdocks`); keep `IsShared=true` for co-op voting semantics.
- Localization: `BIG_FISH.pages.INITIAL.description`, options
- Dynamic vars: `HealAmount` (1/3 max HP), `MaxHpGain` (5)

### Golden Idol (Act 1, IsShared)

**Wiki behavior:**
- Initial: Take Golden Idol (relic) or Leave
- If take: trap triggers:
  - Outrun: Obtain Injury curse
  - Smash: Take damage equal to 25% max HP (A15: 35%)
  - Hide: Lose 8% max HP (A15: 10%)

**Implementation:**
- Extend `EventModel`
- Register to the StS2 Act 1 buckets (`Overgrowth` and `Underdocks`); keep `IsShared=true` for co-op voting semantics.
- Multi-page event: INITIAL -> TRAP -> done
- Golden Idol relic model remains pending; current source uses a random relic substitute and must stay marked non-parity until the relic exists.

## Phase 2: Simple Batch

Current AdditiveBatch1 verified-scope prototype events with straightforward heal/damage/gold/card rewards:

Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar / Pleading Vagrant, and Shining Light. AdditiveBatch1 registers 10 event types total when the 4 canaries are included, through 14 registration calls because Big Fish, Golden Idol, The Cleric, and Shining Light are registered for both StS2 Act 1 buckets.

## Phase 3: Card Service Batch

Events that add/remove/transform cards:

Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome,
Knowing Skull, Nest, Vampires, Falling, Mind Bloom

## Phase 4: Combat Batch

Events that trigger combat encounters:

Dead Adventurer, Masked Bandits, Mysterious Sphere, Scorpion Nest, and Treasure Ooze. Mind Bloom has a blocked combat option. Joust and The Ssssserpent are source-classified as non-combat events.

## Phase 5: Custom UI Batch

Events requiring minigame layouts - use simplified option-based fallbacks:

Wheel of Change, Designer, The Woman in Blue, The Ghost, Nloth,
Tomb of Lord Red Mask, Winding Halls, Forgotten Altar

## Phase 6: Pool Replacement

Prototype `ReplaceUnknownEvents` only in debug builds to create an StS1-only event pool that replaces StS2 events in Unknown rooms. Normal builds fail closed; the prototype requires `REPLACEMENT_PROTOTYPE_ENABLED`, `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype`, and `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` before runtime testing.

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
