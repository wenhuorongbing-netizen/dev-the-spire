# Registry Reconciliation - 52 / 54 / 48 / 50 / 57 / 397

> Updated: 2026-06-11 Revision M Act 1 simple-batch registration correction

This file reconciles the StS1 event-port source counts. These counts are source/doc counts only; they do not prove runtime gameplay, save/load, image rendering, replacement-pool behavior, or parity.

Revision L correction, 2026-06-10: Big Fish and Golden Idol now register to the StS2 Act 1 buckets (`Overgrowth` and `Underdocks`) in CanaryOnly, AdditiveBatch1, and RegisterAll. Their models remain `IsShared=true` for co-op voting semantics; Act bucket parity is source-aligned but runtime bucket proof is still pending.

Revision M correction, 2026-06-11: The Cleric now registers to the StS2 Act 1 buckets (`Overgrowth` and `Underdocks`) in AdditiveBatch1 and RegisterAll. Its model remains `IsShared=true` for co-op voting semantics; Act bucket parity is source-aligned but runtime bucket proof is still pending.

## Current Counts

| Dimension | Count | What It Is | Evidence |
|-----------|-------|------------|----------|
| Public wiki baseline | 52 | External unknown-room target from the v13 goal (`16 shared + 12 Act 1 + 16 Act 2 + 8 Act 3`) | `docs/goals/event.md` |
| Canonical matrix rows | 54 | Public/wiki-derived rows plus local special/debug rows tracked for audit | `canonical-event-matrix.csv` |
| Runtime registry entries | 50 | Unique identities in `Sts1EventRegistry.cs`, including 2 special stubs and excluding 4 duplicate wiki rows | `RegistryEntryCountIs50` |
| Model files | 48 | C# event model files under `EZMicroBalanceCode/Sts1Events/Models/` | source tree |
| Compiling models | 47 | 48 model files minus compile-excluded `Sts1Duplicator.cs` | `Sts1DuplicatorExcludedFromCompilation` |
| RegisterAll calls | 57 | `content.SharedEvent<T>()` plus `content.ActEvent<TAct,TEvent>()` calls in all-draft mode | `RegisterAllTotalRegistrationCallsIs57` |
| AdditiveBatch1 calls | 14 | 10 verified-scope event types; Big Fish, Golden Idol, The Cleric, and Shining Light register to both StS2 Act 1 buckets | `RegisterAdditiveBatch1RegistersOnlyVerifiedScope` |

## Why 54 Rows Become 50 Registry Entries

The public target remains 52 unknown-room wiki entries. The active audit matrix carries 54 rows because it also tracks two local special stubs, `sts1_neow` and `sts1_combat_start`, so source and registry guards can keep start/tutorial surfaces explicit without treating them as unknown-room parity proof.

Four canonical matrix rows are duplicate wiki/bucket memberships and intentionally do not receive separate registry identities:

| Duplicate row | Canonical handling |
|---------------|--------------------|
| `golden_wing_act1` | Shares `sts1_golden_wing` |
| `the_cleric_act1` | Duplicate of `sts1_the_cleric` Act 1 registration |
| `the_mausoleum_act2` | Shares `sts1_the_mausoleum` |
| `the_woman_in_blue_act2` | Shares `sts1_the_woman_in_blue` |

Math: 54 canonical rows - 4 duplicate rows = 50 registry identities.

`sts1_neow` and `sts1_combat_start` are special registry stubs. They count as registry identities but have no unknown-room model files.

## Why RegisterAll Has 57 Calls

| Bucket | Event types | Calls per event | Calls |
|--------|-------------|-----------------|-------|
| Shared | 14 registered shared event types | 1 | 14 |
| StS1 Act 1 | 10 event types | 2 (`Overgrowth` + `Underdocks`) | 20 |
| StS1 Act 2 | 14 event types | 1 (`Hive`) | 14 |
| StS1 Act 3 | 9 event types | 1 (`Glory`) | 9 |
| **Total** | **47 compiling registered event types** | - | **57** |

`Sts1Duplicator` is compile-excluded and is not registered by `RegisterAll`.

## Registry Entry Breakdown

| Phase | Count | Entries |
|-------|-------|---------|
| Canary | 4 | Big Fish, Golden Idol, Divine Fountain, The Lab |
| Simple | 22 | The Cleric, Golden Wing, Living Wall, Old Beggar, Purifier, Golden Shrine, Bonfire Spirits, Duplicator, Fountain of Cleansing, Shining Light, Mushrooms, Joust, The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine |
| CardService | 9 | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom |
| Combat | 5 | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere |
| CustomUi | 8 | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls |
| Special | 2 | Neow, Combat Start |
| **Total** | **50** | - |

One Simple entry, Duplicator, is compile-excluded and not registered by `RegisterAll`.

## Guard Tests

- `RegistryEntryCountIs50` guards the registry count.
- `RegisterAllTotalRegistrationCallsIs57` guards all-draft registration calls.
- `RegisterAllSharedEventCountIs14` guards shared registration calls.
- `RegisterAdditiveBatch1RegistersOnlyVerifiedScope` guards Batch1 identity and count.
- `RegistryCanaryPhaseMatchesCanaryEventIds` guards canary phase metadata.

## Non-Claims

These counts do not prove:

- runtime event spawning;
- unknown room replacement behavior;
- save/load event bag persistence;
- EN/ZHS render behavior;
- image rendering or license parity;
- co-op behavior;
- StS1 full event parity.
