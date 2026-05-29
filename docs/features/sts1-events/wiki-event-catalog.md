# StS1 Wiki Event Catalog — Canonical Matrix

Created: 2026-05-29 | Status: source-verified

## 46/52 Count Mismatch — Resolution

The counts refer to different dimensions:

| Dimension | Count | Explanation |
| --- | --- | --- |
| `wiki_event_entries` | **52** | Total entries on the Slay the Spire Wiki event page. Includes Neow and Combat Start (special start-of-run events with no unknown-room equivalent). |
| `runtime_event_models` | **46** | C# model files in `EZMicroBalanceCode/Sts1Events/Models/`. Excludes Neow and Combat Start (start-of-run only), plus 4 entries that share a model with their parent (Golden Wing Act1, The Cleric Act1, The Mausoleum Act2, The Woman in Blue Act2). Includes `Sts1Duplicator.cs` (compile-excluded). |
| `register_all_calls` | **52** | Total RitsuLib registration calls in `RegisterAll` mode: 15 shared × 1 + 7 Act1 × 2 (Overgrowth + Underdocks) + 14 Act2 × 1 + 9 Act3 × 1 = 52. Verified against `Sts1EventRegistrationService.RegisterAll()`. |
| `canary_only_calls` | **4** | Subset used in `CanaryOnly` mode: Big Fish, Golden Idol, The Lab, Divine Fountain (all shared events). A strict subset of the 15 shared calls in `RegisterAll`. |

`canary_only_calls` is **not** additive with `register_all_calls`. Canary mode replaces RegisterAll; it does not add to it.

### Why 52 Wiki Entries but Only 46 Models?

- **Neow** and **Combat Start** are start-of-run special events. They have no unknown-room pool entry and no `ModEventTemplate` model. Neow is handled by the base game's `Neow` class; Combat Start is a tutorial flow.
- **Golden Wing** appears in both Shared and Act 1 on the wiki. The wiki lists it twice (#4 Shared, #23 Act1 exclusive), but it's one model (`Sts1GoldenWing.cs`).
- **The Cleric** appears in both Shared and Act 1 on the wiki (#2 Shared, #26 Act1 exclusive). One model (`Sts1TheCleric.cs`).
- **The Mausoleum** appears in both Shared and Act 2 (#13 Shared, #38 Act2 exclusive). One model (`Sts1TheMausoleum.cs`).
- **The Woman in Blue** appears in both Shared and Act 2 (#7 Shared, #40 Act2 exclusive). One model (`Sts1TheWomanInBlue.cs`).

So: 52 wiki entries − 2 special (Neow, Combat Start) − 4 duplicates (shared/act split) = **46 unique models**.

### Why 57 Registration Calls?

Each Act 1 event registers twice (once for `Overgrowth`, once for `Underdocks`):
- 15 shared events × 1 call each = 15
- 7 Act 1 events × 2 acts = 14
- 13 Act 2 events × 1 act = 13
- 9 Act 3 events × 1 act = 9
- Total = 51 (in `RegisterAll` mode)

Canary mode adds 4 shared event registrations (subset of the 15 shared).

## Canonical Matrix

| # | Wiki Entry | Runtime Model | Shared? | Act Bucket | Phase | Status |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Big Fish | `Sts1BigFish.cs` | Yes | All acts (shared) | Canary | spec-drafted |
| 2 | The Cleric | `Sts1TheCleric.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 3 | Golden Idol | `Sts1GoldenIdol.cs` | Yes | All acts (shared) | Canary | spec-drafted |
| 4 | Golden Wing | `Sts1GoldenWing.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 5 | Living Wall | `Sts1LivingWall.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 6 | Old Beggar | `Sts1OldBeggar.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 7 | The Woman in Blue | `Sts1TheWomanInBlue.cs` | Yes | All acts (shared) | CustomUI | spec-drafted |
| 8 | Bonfire Spirits | `Sts1BonfireSpirits.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 9 | Divine Fountain | `Sts1DivineFountain.cs` | Yes | All acts (shared) | Canary | spec-drafted |
| 10 | Duplicator | `Sts1Duplicator.cs` | Yes | All acts (shared) | CardService | **excluded** — needs `CardSelectCmd`/`CardPileCmd` |
| 11 | Face Trader | `Sts1FaceTrader.cs` | Yes | All acts (shared) | CardService | spec-drafted |
| 12 | Fountain of Cleansing | `Sts1FountainOfCleansing.cs` | Yes | All acts (shared) | Simple | spec-drafted |
| 13 | The Mausoleum | `Sts1TheMausoleum.cs` | Yes | All acts (shared) | CardService | spec-drafted |
| 14 | Wheel of Change | `Sts1WheelOfChange.cs` | Yes | All acts (shared) | CustomUI | spec-drafted |
| 15 | Designer | `Sts1Designer.cs` | Yes | All acts (shared) | CustomUI | spec-drafted |
| 16 | The Lab | `Sts1TheLab.cs` | Yes | All acts (shared) | Canary | spec-drafted |
| 17 | — | — | — | — | — | Neow: start-of-run only, no model |
| 18 | — | — | — | — | — | Combat Start: tutorial only, no model |
| 19 | Joust | `Sts1Joust.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 20 | The Ssssserpent | `Sts1TheSsssserpent.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 21 | Shining Light | `Sts1ShiningLight.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 22 | Dead Adventurer | `Sts1DeadAdventurer.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 23 | (see #4 Golden Wing) | — | — | — | — | Duplicate wiki entry |
| 24 | Mushrooms | `Sts1Mushrooms.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 25 | Scorpion Nest | `Sts1ScorpionNest.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 26 | (see #2 The Cleric) | — | — | — | — | Duplicate wiki entry |
| 27 | Treasure Ooze | `Sts1TreasureOoze.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 28 | Altar | `Sts1Altar.cs` | No | Hive | Simple | spec-drafted |
| 29 | Council of Ghosts | `Sts1CouncilOfGhosts.cs` | No | Hive | CardService | spec-drafted |
| 30 | Cursed Tome | `Sts1CursedTome.cs` | No | Hive | CardService | spec-drafted |
| 31 | Drug Dealer | `Sts1DrugDealer.cs` | No | Hive | Simple | spec-drafted |
| 32 | Forgotten Altar | `Sts1ForgottenAltar.cs` | No | Hive | CustomUI | spec-drafted |
| 33 | The Ghost | `Sts1TheGhost.cs` | No | Hive | CustomUI | spec-drafted |
| 34 | Knowing Skull | `Sts1KnowingSkull.cs` | No | Hive | CardService | spec-drafted |
| 35 | Nest | `Sts1Nest.cs` | No | Hive | CardService | spec-drafted |
| 36 | The Library | `Sts1TheLibrary.cs` | No | Hive | Simple | spec-drafted |
| 37 | Masked Bandits | `Sts1MaskedBandits.cs` | No | Hive | Combat | spec-drafted |
| 38 | (see #13 The Mausoleum) | — | — | — | — | Duplicate wiki entry |
| 39 | Nloth | `Sts1Nloth.cs` | No | Hive | CustomUI | spec-drafted |
| 40 | (see #7 The Woman in Blue) | — | — | — | — | Duplicate wiki entry |
| 41 | Vampires | `Sts1Vampires.cs` | No | Hive | CardService | spec-drafted |
| 42 | Ancient Writing | `Sts1AncientWriting.cs` | No | Hive | Simple | spec-drafted |
| 43 | Augmenter | `Sts1Augmenter.cs` | No | Hive | Simple | spec-drafted |
| 44 | Sensory Stone | `Sts1SensoryStone.cs` | No | Glory | Simple | spec-drafted |
| 45 | Falling | `Sts1Falling.cs` | No | Glory | CardService | spec-drafted |
| 46 | Mind Bloom | `Sts1MindBloom.cs` | No | Glory | CardService | spec-drafted |
| 47 | Moai Head | `Sts1MoaiHead.cs` | No | Glory | Simple | spec-drafted |
| 48 | Mysterious Sphere | `Sts1MysteriousSphere.cs` | No | Glory | Combat | spec-drafted |
| 49 | Tomb of Lord Red Mask | `Sts1TombOfLordRedMask.cs` | No | Glory | CustomUI | spec-drafted |
| 50 | Winding Halls | `Sts1WindingHalls.cs` | No | Glory | CustomUI | spec-drafted |
| 51 | Transmogrifier | `Sts1Transmogrifier.cs` | No | Glory | CardService | spec-drafted |
| 52 | Upgrade Shrine | `Sts1UpgradeShrine.cs` | No | Glory | Simple | spec-drafted |

## Event Classification by Complexity

### Simple (21 events)
Big Fish, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits,
Divine Fountain, Fountain of Cleansing, The Lab, Shining Light, Mushrooms,
The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter,
Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine

### Card Service (10 events)
Golden Idol, Face Trader, The Mausoleum, Duplicator, Council of Ghosts,
Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom

### Combat (7 events)
Dead Adventurer, Masked Bandits, Mysterious Sphere, Scorpion Nest,
Treasure Ooze, Joust, The Ssssserpent

### Custom UI (8 events)
Wheel of Change, Designer, The Woman in Blue, The Ghost, Nloth,
Tomb of Lord Red Mask, Winding Halls, Forgotten Altar

### Special (2 events — no runtime model)
Neow (start-of-run ancient), Combat Start (tutorial)

## Allowed Statuses

Events use only these statuses (no generic "Done"):

```
planned
spec-drafted
wiki-verified
api-verified
dependency-ready
implemented
asset-verified
loc-render-verified
manual-verified
save-load-verified
blocked
```
