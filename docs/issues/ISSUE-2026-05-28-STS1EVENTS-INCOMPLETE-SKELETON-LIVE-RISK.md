# ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK

## Status

**Open — governance hardened, content incomplete.** Default Off is safe; CanaryOnly is controlled; AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.

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
| CanaryOnly | `CanaryOnly` | 4 (Big Fish, Golden Idol, Lab, Divine Fountain) | **Controlled** | Test harness |
| AdditiveAllDraft | `AdditiveAllDraft` | 52 (all drafted events) | **Unsafe / dev-only** | Includes TODO/BLOCKED events |
| ReplaceUnknownEventsPrototype | `ReplaceUnknownEventsPrototype` | 52 (all drafted events) | **Unsafe / dev-only** | Debug-only replacement |

### Guard tests

- 15+ dedicated tests in `Sts1EventFeatureGuardTests.cs`
- Tests verify gate defaults, canary events, act mapping, registry presence, registration counts, patch-boundaries row
- Safe modes (Off, CanaryOnly) are verified by guard tests
- CanaryOnly events are hardcoded — no TODO/BLOCKED events can enter safe modes

### Why this is safe

1. The feature gate defaults to Off, so no events are registered at runtime unless the environment variable is explicitly set.
2. CanaryOnly registers exactly 4 hardcoded shared events — all in `spec-drafted` status, none TODO/BLOCKED.
3. AdditiveAllDraft and ReplaceUnknownEventsPrototype require explicit env var setting and are documented as dev-only.
4. Guard tests verify Off=0, CanaryOnly=4, and that the registration service is compiled.
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
