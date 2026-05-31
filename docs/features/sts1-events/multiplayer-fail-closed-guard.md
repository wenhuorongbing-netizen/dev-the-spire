# Multiplayer Fail-Closed Guard

> Created: 2026-05-29

## Design

The StS1 event system is **fail-closed** by default:

1. **Feature gate defaults to Off** — `Sts1EventFeatureGate.ResolveMode()` returns `Sts1EventRegistrationMode.Off` when:
   - Environment variable `SPIREPLUS_STS1_EVENT_MODE` is unset
   - Environment variable is empty or whitespace
   - Environment variable contains an unrecognized value

2. **Off mode = zero registrations** — `Sts1EventRegistrationService.RegisterGated()` returns immediately without calling any `content.SharedEvent<T>()` or `content.ActEvent<TAct, TEvent>()`.

3. **No events registered = no events in pool** — RitsuLib's `ModContentPackBuilder.Apply()` is never called, so no StS1 events enter the event pool.

4. **Multiplayer implication** — If a co-op session has one player with StS1 events enabled and another without, the event registration is host-side only. The host's RitsuLib configuration determines which events are available. Clients don't need to know about StS1 events — they receive event state via the game's multiplayer synchronization.

## Fail-Closed Behavior

| Scenario | Mode | Registrations | Events in Pool |
|----------|------|---------------|----------------|
| No env var (default) | Off | 0 | 0 (native StS2 only) |
| Empty env var | Off | 0 | 0 |
| Unknown value | Off | 0 | 0 |
| `canaryonly` | CanaryOnly | 4 | 4 Shared events |
| `additivebatch1` | AdditiveBatch1 | 11 calls / 10 event types | Controlled prototype batch |
| `additivealldraft` | AdditiveAllDraft | 54 calls / 47 event types | All compiling draft events (unsafe/dev-only) |
| `replaceunknowneventsprototype` | ReplaceUnknownEventsPrototype | 0 unless `REPLACEMENT_PROTOTYPE_ENABLED` is defined | Debug-only replacement prototype |

## IsShared / Co-op Behavior

- **Shared events (IsShared=true)**: All players see the same event, vote on options, same outcome. Used for events where a single decision affects the whole party (e.g., Big Fish HP trade, Divine Fountain curse removal).
- **Combat events (IsShared=true)**: Required for `EnterCombatWithoutExitingEvent`. All players enter the same combat.
- **Non-shared events (IsShared=false, default)**: Each player gets an independent clone. Used for per-player choices (card selection, gold spending, etc.).

## Guard Tests

| Test | What It Verifies |
|------|------------------|
| `FeatureGateDefaultsToOffWhenEnvVarIsUnset` | Default is Off when env var missing |
| `FeatureGateEvaluatesAllModes` | All 5 modes have correct gate results |
| `OffModeReturnsImmediatelyWithZeroRegistrations` | Off mode returns without registering |
| `RegisterCanaryOnlyRegistersExactlyFourSharedEvents` | CanaryOnly = exactly 4 |
| `RegisterAdditiveBatch1RegistersOnlyVerifiedScope` | AdditiveBatch1 = 11 calls / 10 event types |
| `CombatEventsDeclareIsSharedTrue` | All 6 combat events have IsShared=true |
| `AllSharedEventModelsDeclareIsSharedTrue` | At least 17 shared models have IsShared=true |

## Runtime Verification Status

**UNVERIFIED** — requires game launch with:
1. `SPIREPLUS_STS1_EVENT_MODE=Off` (default) — verify no StS1 events appear
2. `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` — verify exactly 4 events appear
3. `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` — verify the bounded prototype batch only in a controlled runtime smoke
4. Multiplayer session — verify host controls event registration

## Verdict

**Source-level fail-closed: VERIFIED by guard tests.**
**Runtime-level fail-closed: HARD BLOCKED until STS2-RitsuLib is installed and Off/CanaryOnly `godot.log` evidence is captured.**
