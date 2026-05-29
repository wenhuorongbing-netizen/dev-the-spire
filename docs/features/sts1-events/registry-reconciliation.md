# Registry Reconciliation — 48 vs 52

> Created: 2026-05-29

## Three Counts Explained

| Dimension | Count | What It Is |
|-----------|-------|------------|
| Wiki event entries | **52** | All events listed on the StS1 Wiki event page |
| Runtime registry entries | **48** | Entries in `Sts1EventRegistry.cs` `Events` list |
| Registration calls (RegisterAll) | **52** | Calls to `content.SharedEvent<T>()` / `content.ActEvent<TAct, TEvent>()` in `RegisterAll()` |

## Why 52 Wiki → 48 Registry

4 wiki entries have no dedicated registry entry:

| Wiki Entry | Reason | Handling |
|------------|--------|----------|
| Neow | Start-of-run special event; no unknown-room model | 1 registry entry as Special stub, no model file |
| Combat Start | Tutorial flow; no unknown-room model | 1 registry entry as Special stub, no model file |
| Golden Wing (Act1 duplicate) | Same event as Shared Golden Wing | 1 model serves both wiki rows |
| The Cleric (Act1 duplicate) | Same event as Shared The Cleric | 1 model serves both wiki rows |
| The Mausoleum (Act2 duplicate) | Same event as Shared The Mausoleum | 1 model serves both wiki rows |
| The Woman in Blue (Act2 duplicate) | Same event as Shared The Woman in Blue | 1 model serves both wiki rows |

Math: 52 wiki − 2 special stubs (counted once each in registry) − 2 duplicate pairs (4 entries → 2 counted) = 48 unique registry entries.

Wait, let me recount:
- 52 wiki entries
- Neow and Combat Start DO have registry entries (they're in the Special section)
- The 4 duplicates (Golden Wing Act1, Cleric Act1, Mausoleum Act2, Woman in Blue Act2) do NOT have registry entries
- So: 52 − 4 duplicates = 48 registry entries ✓

The 2 Special stubs (Neow, Combat Start) have registry entries but no model files.

## Why 52 Registration Calls

Each Act 1 event registers in 2 acts (Overgrowth + Underdocks):

| Bucket | Events | Acts per Event | Calls |
|--------|--------|----------------|-------|
| Shared | 15 | 1 (shared registry) | 15 |
| Act 1 | 7 | 2 (Overgrowth + Underdocks) | 14 |
| Act 2 | 14 | 1 (Hive) | 14 |
| Act 3 | 9 | 1 (Glory) | 9 |
| **Total** | **45 unique** | — | **52** |

Note: 45 unique models (not 46) because Sts1Duplicator is compile-excluded and not registered.

## Why 46 Model Files

```
Shared models:  16 (including Sts1Duplicator.cs which is compile-excluded)
Act 1 models:    7
Act 2 models:   14
Act 3 models:    9
Total:          46
```

46 models − 1 compile-excluded = 45 compiling models.

## Registry Entry Breakdown (48)

| Phase | Count | Entries |
|-------|-------|---------|
| Canary | 2 | Big Fish, Golden Idol |
| Simple | 20 | The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Divine Fountain, Duplicator, Fountain of Cleansing, The Lab, Shining Light, Mushrooms, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine |
| CardService | 9 | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom |
| Combat | 7 | Dead Adventurer, Scorpion Nest, Treasure Ooze, Joust, The Ssssserpent, Masked Bandits, Mysterious Sphere |
| CustomUi | 8 | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls |
| Special | 2 | Neow, Combat Start |
| **Total** | **48** | — |

## Guard Test

`RegistryEntryCountIs48` in `Sts1EventFeatureGuardTests.cs` verifies exactly 48 entries using `CountOccurrences(entriesBlock, "new(\"")`.

## Key Insight

48 registry entries is **correct** — it represents the 48 unique event identities. The 52 registration calls reflect the act-mapping strategy (Act1 events dual-registered). Neither number alone tells the full story; the canonical matrix tracks all 52 wiki entries with proper reconciliation.
