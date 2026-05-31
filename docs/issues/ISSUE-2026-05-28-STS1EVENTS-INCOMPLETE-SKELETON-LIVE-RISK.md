# ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK

## Status

**Open — governance hardened, content incomplete.** Default Off is safe; CanaryOnly and AdditiveBatch1 are controlled source-test modes; AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.

## Summary

StS1 event port model files (`Sts1Events/Models/`) are compiled into the Spire Plus assembly. The feature module (`Sts1EventsFeatureModule`) is registered in `SpirePlusFeatureRegistry` and gated to Off by default via environment variable `SPIREPLUS_STS1_EVENT_MODE`.

## Current State (as of 2026-05-29 overnight run)

### Compile status

- 52 C# files under `EZMicroBalanceCode/Sts1Events/`
- 51 compiled, 1 compile-excluded (`Sts1Duplicator.cs`)
- `Sts1EventRegistrationService.cs` IS compiled (not compile-excluded)

### Feature registration

- `SpirePlusFeatureRegistry` registers `Sts1EventsFeatureModule`
- Feature gate defaults to Off when env var `SPIREPLUS_STS1_EVENT_MODE` is unset
- Zero events registered at runtime unless env var is set

### Mode Safety Matrix

| Mode | Env var value | Registration count | Risk level | Use case |
|------|--------------|-------------------|------------|----------|
| Off | unset / empty / invalid | 0 | **Safe** | Default — production |
| CanaryOnly | `CanaryOnly` | 4 registrations / 4 event types (Big Fish, Golden Idol, Lab, Divine Fountain) | **Controlled** | Canary test harness |
| AdditiveBatch1 | `AdditiveBatch1` | 11 registrations / 10 event types (4 canary + 6 simple; Shining Light registers to two Act 1 buckets) | **Controlled** | Verified-scope prototype testing only |
| AdditiveAllDraft | `AdditiveAllDraft` | 54 registration calls (47 unique event types) | **Unsafe / dev-only** | Includes TODO/BLOCKED events |
| ReplaceUnknownEventsPrototype | `ReplaceUnknownEventsPrototype` | 0 unless `REPLACEMENT_PROTOTYPE_ENABLED` is compiled; then 54 registration calls (47 unique event types) | **Unsafe / debug-only** | Debug-only replacement prototype |

### AdditiveAllDraft Risk Table

Events with TODO/BLOCKED/partial status in AdditiveAllDraft mode:

| Event ID | Display Name | Status | Missing API | Risk |
|----------|-------------|--------|-------------|------|
| `sts1_dead_adventurer` | Dead Adventurer | TODO — combat path is no-op stub | Encounter model for random elite | **HIGH** |
| `sts1_scorpion_nest` | Scorpion Nest | TODO — combat path is no-op stub | Encounter model for 3 Louses | **HIGH** |
| `sts1_treasure_ooze` | Treasure Ooze | TODO — combat path is no-op stub | Encounter model for large slime | **HIGH** |
| `sts1_masked_bandits` | Masked Bandits | TODO — FIGHT path is no-op stub | Encounter model for 3 bandits | **HIGH** |
| `sts1_mind_bloom` | Mind Bloom | BLOCKED — WAR option is no-op stub | Encounter model for Act 1 boss | **HIGH** |
| `sts1_mysterious_sphere` | Mysterious Sphere | TODO — combat path is no-op stub | Encounter model for 2 Orb Walkers | **HIGH** |
| `sts1_nloth` | N'loth | BLOCKED — OFFER option is no-op stub | RelicSelectCmd API | **HIGH** |
| `sts1_vampires` | Vampires | Partial — removes Strikes but no Bite cards | Custom Bite card model | **MEDIUM** |

7 HIGH-risk events (6 combat stubs + 1 BLOCKED relic-select), 1 MEDIUM-risk (partial Vampires).

### Guard tests

- Dedicated tests in `Sts1EventFeatureGuardTests.cs`
- Tests verify gate defaults, canary events, AdditiveBatch1 verified scope, act mapping, registry presence, registration counts, patch-boundaries row, mode safety
- Safe/controlled modes (Off, CanaryOnly, AdditiveBatch1) are verified by guard tests
- CanaryOnly events are hardcoded — no TODO/BLOCKED events can enter safe modes

### Why this is safe

1. The feature gate defaults to Off, so no events are registered at runtime unless the environment variable is explicitly set.
2. CanaryOnly registers exactly 4 hardcoded shared events — all in `spec-drafted` status, none TODO/BLOCKED.
3. AdditiveBatch1 registers only the current verified prototype scope and is separate from AdditiveAllDraft.
4. AdditiveAllDraft and ReplaceUnknownEventsPrototype require explicit env var setting and are documented as dev-only; ReplacementPrototype also fails closed unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED`.
5. Guard tests verify Off=0, CanaryOnly=4, AdditiveBatch1 exact scope, and that the registration service is compiled.
5. `Sts1Duplicator` is compile-excluded (needs `CardSelectCmd`/`CardPileCmd` APIs not yet available).

### What remains incomplete

- Runtime gameplay verification (requires STS2-RitsuLib installation + game launch)
- ZHS localization (38 placeholder entries)
- Combat encounter models for 7 blocked events
- Event images
- Replacement pool has structure/file guard only, not functional proof
- No canary save/load, screenshot, unknown room extraction, or event pool replacement proof

## Acceptance Criteria for Closing

1. Runtime smoke passes with Off mode (0 events registered, verified in godot.log)
2. Runtime smoke passes with CanaryOnly mode (4 events registered, verified in godot.log)
3. At least 4 canary events are debug-spawned and manually verified
4. Save/load works after canary event completion
5. EN/ZHS render verified for canary events
