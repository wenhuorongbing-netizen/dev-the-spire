# StS1 Event Specs

This directory contains per-event behavior notes for the StS1 event-port prototype. These pages are implementation notes, not the registration authority.

## Current Registration Authority

Use `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.cs` as the source of truth for current registration. The current RitsuLib API shape is:

```csharp
content.SharedEvent<Sts1TheLab>();
content.ActEvent<Overgrowth, Sts1BigFish>();
content.ActEvent<Underdocks, Sts1BigFish>();
content.ActEvent<Hive, Sts1Altar>();
content.ActEvent<Glory, Sts1UpgradeShrine>();
```

Per-event registration notes were refreshed on 2026-06-11 to reference `Sts1EventRegistrationService` and the current RitsuLib `content.SharedEvent<TEvent>()` / `content.ActEvent<TAct,TEvent>()` APIs. Static coverage is 50 per-event spec files and 50 registration or explicit non-registration notes; `README.md` is the directory guide and is not counted as an event spec.

Reproduce the per-spec static coverage check with:

```powershell
.\scripts\check-sts1-event-spec-registration-notes.ps1 -FailOnMismatch
```

## Current Bucket Map

| Bucket | Registration | Event types |
|--------|--------------|-------------|
| Shared | `content.SharedEvent<TEvent>()` | Golden Wing, Living Wall, Old Beggar, Purifier, Golden Shrine, Bonfire Spirits, Divine Fountain, Fountain of Cleansing, The Lab, Face Trader, The Mausoleum, Designer, The Woman in Blue, Wheel of Change |
| StS1 Act 1 | `content.ActEvent<Overgrowth,TEvent>()` and `content.ActEvent<Underdocks,TEvent>()` | Big Fish, Golden Idol, The Cleric, Shining Light, Mushrooms, Dead Adventurer, Scorpion Nest, Treasure Ooze, Joust, The Ssssserpent |
| StS1 Act 2 | `content.ActEvent<Hive,TEvent>()` | Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Masked Bandits, Forgotten Altar, The Ghost, N'loth |
| StS1 Act 3 | `content.ActEvent<Glory,TEvent>()` | Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, Falling, Mind Bloom, Mysterious Sphere, Tomb of Lord Red Mask, Winding Halls |
| Compile-excluded | none | Duplicator, pending duplicate-selection API support |
| Special stubs | no unknown-room event model | Neow, Combat Start |

## Mode Counts

| Mode | Current source count | Notes |
|------|----------------------|-------|
| Off | 0 calls | Default path. Current beta.85 Off loader proof is clean, but this proves only disabled-mode startup and patch application. |
| CanaryOnly | 6 calls / 4 event types | Big Fish and Golden Idol register to both StS2 Act 1 buckets; The Lab and Divine Fountain are shared. Current `v0.107.0` enabled-mode smoke is still pending. |
| AdditiveBatch1 | 14 calls / 10 event types | Canary set plus Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, and Shining Light. Current `v0.107.0` enabled-mode smoke and gameplay proof are still pending. |
| AdditiveAllDraft | 57 calls / 47 compiling event types | Unsafe/dev-only mode. Does not include compile-excluded Duplicator. |

## Non-Claims

This directory does not prove runtime event spawning, encounter screenshots, EN/ZHS render behavior, save/load, image/license parity, replacement-pool behavior, multiplayer behavior, or full StS1 parity.
