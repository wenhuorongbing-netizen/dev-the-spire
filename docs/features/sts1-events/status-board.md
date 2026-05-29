# StS1 Events Status Board

> Last updated: 2026-05-29

## Overall Progress

| Phase | Status | Events | Notes |
|-------|--------|--------|-------|
| 0: Infrastructure | **Done** | — | Feature gate, registry, feature module, registration service |
| 1: Canary | **Code Done** | 4 | Big Fish, Golden Idol, Lab, Divine Fountain |
| 2: Simple Batch | **Code Done** | 17 | All compile, EN localized |
| 3: Card Service | **Code Done** | 9 | All compile, EN localized |
| 4: Combat | **Code Done** | 7 | All compile, EN localized. Combat TODOs remain. |
| 5: Custom UI | **Code Done** | 8 | All compile, EN localized. UI simplified to option-based. |
| 6: Pool Replacement | **Prototype** | — | `Sts1ReplacementPrototype.cs` exists, gated `#if REPLACEMENT_PROTOTYPE_ENABLED` |

## Registration Summary

| Metric | Count |
|--------|-------|
| Registry entries (Sts1EventRegistry) | 48 |
| Model files (C#) | 46 |
| Model files compiling | 45 (Duplicator excluded via csproj) |
| Registration calls in RegisterAll | 52 |
| SharedEvent calls | 15 |
| ActEvent calls (Act1: 7×2, Act2: 14, Act3: 9) | 37 |
| EN localization keys | 380 |
| ZHS localization keys | 380 (342 translated, 38 placeholder "待翻译") |
| Event images | 0 (directory only has Ancient portraits) |

## Build & Test Status

- `dotnet build`: **0 errors**, 87 nullable warnings (CS8602/CS8604 only)
- `dotnet test`: **361 passed, 0 failed, 21 skipped** (382 total)
  - 21 skipped are release artifact tests requiring `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`
- 20 Sts1EventFeatureGuardTests all pass

## Multiplayer IsShared Status

All 22 event models with `IsShared => true`:

| Category | Events | IsShared |
|----------|--------|----------|
| Shared (16 models) | Big Fish, Golden Idol, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Divine Fountain, Duplicator*, Fountain of Cleansing, The Lab, Face Trader, The Mausoleum, Designer, The Woman in Blue, Wheel of Change | `true` |
| Combat (6 models) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere, Mind Bloom | `true` (required for EnterCombatWithoutExitingEvent) |

*Sts1Duplicator is compile-excluded.

## What's Implemented (per event)

Every compiling event has:
- C# model class with options, effects, A15/A15+ logic
- EN localization (380 keys complete)
- Registry entry and registration call
- `IsShared` override where required

## TODOs Still Present in Code

| Event | TODO | Severity |
|-------|------|----------|
| Dead Adventurer | Enter combat with random elite | Blocked (needs encounter model) |
| Scorpion Nest | Enter combat with 3 Louses | Blocked (needs encounter model) |
| Treasure Ooze | Enter combat with large slime | Blocked (needs encounter model) |
| Joust | Enter combat with Lagavulin | Blocked (needs encounter model) |
| The Ssssserpent | Enter combat with 3 Ssssents | Blocked (needs encounter model) |
| Masked Bandits | Enter combat with 3 bandits | Blocked (needs encounter model) |
| Mysterious Sphere | Enter combat with 2 Orb Walkers | Blocked (needs encounter model) |
| Mind Bloom (War) | Enter combat with random Act 1 boss | Blocked (needs encounter model) |

## Active Blockers

1. **Combat encounter models**: 7 events need encounter definitions for combat phases
2. **Sts1Duplicator**: `CardSelectCmd.FromDeckForRewards` and `CardSelectorPrefs.DuplicateSelectionPrompt` don't exist in RitsuLib 0.3.2
3. **Sts1Nloth**: No `RelicSelectCmd` API for relic selection UI
4. **ZHS localization**: 38 entries are "待翻译" placeholder
5. **Event images**: 0 images exist
6. **Runtime gameplay verification**: Requires game launch with `SPIREPLUS_STS1_EVENT_MODE` env var

## Feature Gate

Default: **Off** — zero StS1 event registrations unless explicitly enabled.

| Mode | Env Var Value | Behavior |
|------|---------------|----------|
| Off | (default/unset) | No registrations |
| CanaryOnly | `canaryonly` | 4 SharedEvent registrations only |
| AdditiveAllDraft | `additivealldraft` | All 52 registrations, adds to native pool |
| ReplaceUnknownEventsPrototype | `replaceunknowneventsprototype` | All 52 + Harmony postfix to filter event pool |

## Next Steps

1. Runtime gameplay verification of canary events (requires game launch)
2. ZHS localization: translate 38 placeholder entries
3. Combat encounter models for 7 blocked events
4. Event images for all 46 events
5. StS2 source audit of event selection system
6. Wiki parity check against wiki-event-catalog.md
