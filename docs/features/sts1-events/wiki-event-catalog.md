# StS1 Wiki Event Catalog — Canonical Matrix

Created: 2026-05-29 | Updated: 2026-06-11 | Status: source-verified

## 52 / 54 / 50 / 48 / 47 / 57 / 14 Count Reconciliation

The counts refer to different dimensions:

| Dimension | Count | Explanation |
| --- | --- | --- |
| `public_wiki_baseline` | **52** | Public unknown-room target from the StS1 Wiki category counts: 16 shared, 12 Act 1, 16 Act 2, 8 Act 3. |
| `canonical_rows` | **54** | Internal audit rows: public baseline plus local special rows used for source/registry governance. |
| `registry_entries` | **50** | Unique identities in `Sts1EventRegistry.cs`, including 2 special stubs and excluding 4 duplicate wiki rows. |
| `runtime_event_models` | **48** | C# model files in `EZMicroBalanceCode/Sts1Events/Models/`. Excludes Neow and Combat Start (start-of-run only), plus 4 entries that share a model with their parent (Golden Wing Act1, The Cleric Act1, The Mausoleum Act2, The Woman in Blue Act2). Includes `Sts1Duplicator.cs` (compile-excluded). Includes Purifier and Golden Shrine (newly added). |
| `compiling_models` | **47** | Model files minus compile-excluded `Sts1Duplicator.cs`. |
| `register_all_calls` | **57** | Total RitsuLib registration calls in `RegisterAll` mode: 14 shared x 1 + 10 Act1 x 2 (Overgrowth + Underdocks) + 14 Act2 x 1 + 9 Act3 x 1 = 57. Verified against `Sts1EventRegistrationService.RegisterAll()`. |
| `canary_only_calls` | **6** | Subset used in `CanaryOnly` mode: Big Fish and Golden Idol register to both Act 1 buckets, while The Lab and Divine Fountain remain shared events. |
| `additive_batch1_calls` | **14** | 10 verified-scope event types; Big Fish, Golden Idol, The Cleric, and Shining Light register to both StS2 Act 1 buckets. |

`canary_only_calls` is **not** additive with `register_all_calls`. Canary mode replaces RegisterAll; it does not add to it.

### Why 54 Canonical Rows but Only 48 Models?

- **Neow** and **Combat Start** are start-of-run special events. They have no unknown-room pool entry and no `ModEventTemplate` model. Neow is handled by the base game's `Neow` class; Combat Start is a tutorial flow.
- **Golden Wing** appears in both Shared and Act 1 on the wiki. The wiki lists it twice (#4 Shared, #23 Act1 exclusive), but it's one model (`Sts1GoldenWing.cs`).
- **The Cleric** appears in both Shared and Act 1 on the wiki (#2 Shared, #26 Act1 exclusive). One model (`Sts1TheCleric.cs`).
- **The Mausoleum** appears in both Shared and Act 2 (#13 Shared, #38 Act2 exclusive). One model (`Sts1TheMausoleum.cs`).
- **The Woman in Blue** appears in both Shared and Act 2 (#7 Shared, #40 Act2 exclusive). One model (`Sts1TheWomanInBlue.cs`).
- **Purifier** and **Golden Shrine** are StS1 shrine events added to Spire Plus (entries #53, #54).

So: 54 canonical rows - 2 special (Neow, Combat Start) - 4 duplicates (shared/act split) = **48 model files**, of which **47 compile** because `Sts1Duplicator.cs` is excluded.

### Why 57 Registration Calls?

Each Act 1 event registers twice (once for `Overgrowth`, once for `Underdocks`):
- 14 shared events x 1 call each = 14
- 10 Act 1 events x 2 acts = 20
- 14 Act 2 events × 1 act = 14
- 9 Act 3 events × 1 act = 9
- Total = 57 (in `RegisterAll` mode)

Canary mode uses 6 registration calls for 4 event types: Big Fish and Golden Idol are Act 1 bucket registrations, and The Lab plus Divine Fountain are shared registrations.

AdditiveBatch1 uses 14 registration calls for 10 event types because Big Fish, Golden Idol, The Cleric, and Shining Light are each registered into both StS2 Act 1 buckets.

## Canonical Matrix

| # | Wiki Entry | Runtime Model | Shared? | Act Bucket | Phase | Status |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Big Fish | `Sts1BigFish.cs` | Yes | Act 1 (Overgrowth + Underdocks) | Canary | implemented; runtime proof pending |
| 2 | The Cleric | `Sts1TheCleric.cs` | Yes | Act 1 (Overgrowth + Underdocks) | Simple | implemented; runtime proof pending |
| 3 | Golden Idol | `Sts1GoldenIdol.cs` | Yes | Act 1 (Overgrowth + Underdocks) | Canary | implemented; Golden Idol relic parity/runtime proof pending |
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
| 53 | Purifier | `Sts1Purifier.cs` | Yes | All acts (shared) | Simple | compiled |
| 54 | Golden Shrine | `Sts1GoldenShrine.cs` | Yes | All acts (shared) | Simple | compiled |
| 17 | — | — | — | — | — | Neow: start-of-run only, no model |
| 18 | — | — | — | — | — | Combat Start: tutorial only, no model |
| 19 | Joust | `Sts1Joust.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 20 | The Ssssserpent | `Sts1TheSsssserpent.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 21 | Shining Light | `Sts1ShiningLight.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 22 | Dead Adventurer | `Sts1DeadAdventurer.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 23 | (see #4 Golden Wing) | — | — | — | — | Duplicate wiki entry |
| 24 | Mushrooms | `Sts1Mushrooms.cs` | No | Overgrowth + Underdocks | Simple | spec-drafted |
| 25 | Scorpion Nest | `Sts1ScorpionNest.cs` | No | Overgrowth + Underdocks | Combat | spec-drafted |
| 26 | (see #2 The Cleric) | — | — | — | — | Duplicate wiki entry; source uses Act 1 registration |
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

### Simple (23 events)
Big Fish, The Cleric, Golden Wing, Living Wall, Old Beggar, Purifier, Golden Shrine, Bonfire Spirits,
Divine Fountain, Fountain of Cleansing, The Lab, Shining Light, Mushrooms,
The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter,
Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine

### Card Service (10 events)
Golden Idol, Face Trader, The Mausoleum, Duplicator, Council of Ghosts,
Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom

### Combat (5 events + 1 blocked combat option)
Dead Adventurer, Masked Bandits, Mysterious Sphere, Scorpion Nest, Treasure Ooze.
Mind Bloom has a blocked combat option. Joust and The Ssssserpent are source-classified as non-combat events.

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
