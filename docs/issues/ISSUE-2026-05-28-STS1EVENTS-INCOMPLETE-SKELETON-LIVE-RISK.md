# ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK

## Status

**Open — governance hardened, content incomplete, runtime proof blocked.** Default Off is source-safe; CanaryOnly and AdditiveBatch1 are controlled source-test modes; AdditiveAllDraft and ReplaceUnknownEventsPrototype are disabled unless an explicit unsafe/debug override is set, and remain dev-only/unsafe.

2026-06-20 correction: historical loader proof exists for Off, CanaryOnly, and AdditiveBatch1. Retained beta.85 `v0.107.0` proof covers default-Off plus CanaryOnly loader registration only as previous-package/game-version context, while current beta.92 RitsuLib-only proof covers Off plus AdditiveBatch1 loader/registration shape only. Event gameplay, save-load, screenshot, replacement functional proof, and two-client co-op/fail-closed proof remain pending or blocked.

## Summary

StS1 event port model files (`Sts1Events/Models/`) are compiled into the Spire Plus assembly. The feature module (`Sts1EventsFeatureModule`) is registered in `SpirePlusFeatureRegistry` and gated to Off by default via environment variable `SPIREPLUS_STS1_EVENT_MODE`.

## Current State (updated 2026-06-21 from beta.92 RitsuLib-only loader-gate evidence)

### Compile status

- 52 C# files under `EZMicroBalanceCode/Sts1Events/`
- 51 compiled, 1 compile-excluded (`Sts1Duplicator.cs`)
- `Sts1EventRegistrationService.cs` IS compiled (not compile-excluded)

### Feature registration

- `SpirePlusFeatureRegistry` registers `Sts1EventsFeatureModule`
- Feature gate defaults to Off when env var `SPIREPLUS_STS1_EVENT_MODE` is unset
- Unsafe all-draft/debug modes also require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; `ReplaceUnknownEventsPrototype` remains compile-symbol gated
- Zero events registered at runtime unless env var is set
- Sts1Events has no separate network-mode gate; `CanaryOnly` and `AdditiveBatch1` are enabled wherever the mode env var is set. Do not treat controlled registration as co-op safety proof.

### Mode Safety Matrix

| Mode | Env var value | Additional gate | Registration count | Risk level | Use case |
|------|--------------|-----------------|-------------------|------------|----------|
| Off | unset / empty / invalid | none | 0 | **Safe** | Default - production |
| CanaryOnly | `CanaryOnly` | none | 4 registrations / 4 event types (Big Fish, Golden Idol, Lab, Divine Fountain) | **Controlled** | Canary test harness |
| AdditiveBatch1 | `AdditiveBatch1` | none | 14 registrations / 10 event types (4 canary + 6 simple; Big Fish, Golden Idol, The Cleric, and Shining Light register to two Act 1 buckets) | **Controlled** | Verified-scope prototype testing only |
| AdditiveAllDraft | `AdditiveAllDraft` | `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | 57 registration calls (47 compiling event types) | **Unsafe / dev-only** | Includes TODO/BLOCKED events |
| ReplaceUnknownEventsPrototype | `ReplaceUnknownEventsPrototype` | `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | 0 in normal builds; 57 registration calls (47 compiling event types) only in explicit debug builds | **Unsafe / debug-only** | Debug-only replacement prototype |

### AdditiveBatch1 Risk Table

AdditiveBatch1 is source-guarded for exact registration scope. Historical loader proof covered the older 11-call source shape; beta.85 proof covers default-Off and CanaryOnly loader registration only, retained beta.87 `v0.107.0` proof covers AdditiveBatch1 loader/registration only, and current `v0.107.1` AdditiveBatch1 enabled-mode plus gameplay/co-op evidence remain pending or blocked. It may be used only for prototype runtime smoke after RitsuLib is installed; it is not a tester-facing gameplay claim.

| Scope | Event IDs | Risk |
|----------|-------------|------|
| Canary events | `sts1_big_fish`, `sts1_golden_idol`, `sts1_the_lab`, `sts1_divine_fountain` | Controlled; current source registers Big Fish and Golden Idol to both Act 1 buckets while The Lab and Divine Fountain remain shared; current enabled-mode loader proof exists, but event encounter gameplay and save/load proof remain pending |
| Simple batch events | `sts1_purifier`, `sts1_upgrade_shrine`, `sts1_golden_shrine`, `sts1_the_cleric`, `sts1_old_beggar`, `sts1_shining_light` | Controlled source-test scope; still needs live event flow, EN/ZHS render, image, and save/load proof |
| Act duplicate registrations | `sts1_big_fish`, `sts1_golden_idol`, `sts1_the_cleric`, and `sts1_shining_light` each register to both Overgrowth and Underdocks | Count drift risk if registration calls are treated as unique event types |

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

### ReplaceUnknownEventsPrototype Risk Table

| Surface | Status | Risk |
|----------|--------|------|
| Compile gate | Requires `REPLACEMENT_PROTOTYPE_ENABLED` and `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Fail-closed by default and disabled in normal builds |
| Unknown room replacement | Not runtime-verified | Could alter event pool, act buckets, event bag, or save/load behavior |
| Registration scope | Uses the all-draft registration family when compiled | Inherits AdditiveAllDraft blocked/TODO event risks |
| Release disposition | Debug-only | Must not be enabled in tester package or release path |

### Guard tests

- Dedicated tests in `Sts1EventFeatureGuardTests.cs` cover the current focused guard set
- Tests verify gate defaults, canary events, AdditiveBatch1 verified scope, act mapping, registry presence, registration counts, patch-boundaries row, mode safety
- Safe/controlled modes (Off, CanaryOnly, AdditiveBatch1) are source-verified by guard tests; current loader proof is captured for default-Off and CanaryOnly with installed STS2-RitsuLib, while current AdditiveBatch1 enabled-mode launch, event gameplay, and co-op proof remain pending or blocked
- CanaryOnly events are hardcoded — no TODO/BLOCKED events can enter safe modes

### Why this is safe

1. The feature gate defaults to Off, so source flow registers no events unless the environment variable is explicitly set.
2. CanaryOnly registers exactly 4 hardcoded shared events — all in `spec-drafted` status, none TODO/BLOCKED.
3. AdditiveBatch1 registers only the current verified prototype scope and is separate from AdditiveAllDraft.
4. AdditiveAllDraft and ReplaceUnknownEventsPrototype require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` in addition to the mode selector and are documented as dev-only; the replacement prototype also fails closed unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED`.
5. Guard tests verify Off=0, CanaryOnly=4, AdditiveBatch1 exact scope, and that the registration service is compiled; current `godot.log` loader proof exists for default-Off and CanaryOnly loader registration only, so current AdditiveBatch1 enabled-mode launch, event encounter, save/load, screenshot, and co-op proof are still required.
6. `Sts1Duplicator` is compile-excluded (needs `CardSelectCmd`/`CardPileCmd` APIs not yet available).

### What remains incomplete

- Runtime gameplay verification of event encounter flow, beyond main-menu registration
- Two-client co-op/fail-closed proof for Sts1Events modes
- ZHS localization has source-file placeholder guards, but live EN/ZHS render proof is still pending
- Combat encounter models for 5 fully blocked combat events plus the Mind Bloom War option
- Event images
- Replacement pool has structure/file guard only, not functional proof
- No canary save/load, screenshot, unknown room extraction, or event pool replacement proof

## Acceptance Criteria for Closing

1. Runtime smoke passes with Off mode (0 events registered, verified in godot.log) [loader-gate satisfied]
2. Runtime smoke passes with CanaryOnly mode (4 events registered, verified in godot.log) [loader-gate satisfied]
3. At least 4 canary events are debug-spawned and manually verified
4. Save/load works after canary event completion
5. EN/ZHS render verified for canary events
