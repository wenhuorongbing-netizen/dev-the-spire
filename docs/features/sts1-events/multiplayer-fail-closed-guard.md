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

4. **Multiplayer implication** — Host-authoritative event registration is the design assumption, but this is runtime-unverified. Co-op needs either fail-closed proof or two-client behavior proof before any gameplay claim.

## Fail-Closed Behavior

| Scenario | Mode | Registrations | Events in Pool |
|----------|------|---------------|----------------|
| No env var (default) | Off | 0 | 0 (native StS2 only) |
| Empty env var | Off | 0 | 0 |
| Unknown value | Off | 0 | 0 |
| `canaryonly` | CanaryOnly | 6 calls / 4 event types | Big Fish and Golden Idol register to both Act 1 buckets; Lab and Divine Fountain are shared |
| `additivebatch1` | AdditiveBatch1 | 13 calls / 10 event types | Controlled prototype batch |
| `additivealldraft` | AdditiveAllDraft | 0 unless `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; then 56 calls / 47 event types | All compiling draft events (unsafe/dev-only) |
| `replaceunknowneventsprototype` | ReplaceUnknownEventsPrototype | 0 unless `REPLACEMENT_PROTOTYPE_ENABLED` and `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` are both present | Debug-only replacement prototype |

## IsShared / Co-op Behavior

- **Shared events (IsShared=true)**: All players see the same event, vote on options, same outcome. Used for events where a single decision affects the whole party (e.g., Big Fish HP trade, Divine Fountain curse removal).
- **Combat events (IsShared=true)**: Required for `EnterCombatWithoutExitingEvent`. All players enter the same combat.
- **Non-shared events (IsShared=false, default)**: Each player gets an independent clone. Used for per-player choices (card selection, gold spending, etc.).

## Guard Tests

| Test | What It Verifies |
|------|------------------|
| `FeatureGateDefaultsToOffWhenEnvVarIsUnset` | Default is Off when env var missing |
| `FeatureGateEvaluatesAllModes` | All 5 modes route through the correct gate paths |
| `UnsafeModesRequireExplicitUnsafeOverride` | Unsafe all-draft/debug modes require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` |
| `ReplacementPrototypeGateFailsClosedWithoutCompileSymbol` | Replacement prototype reports disabled in normal builds without `REPLACEMENT_PROTOTYPE_ENABLED` |
| `OffModeReturnsImmediatelyWithZeroRegistrations` | Off mode returns without registering |
| `RegisterCanaryOnlyRegistersExactlyFourCanaryEventTypes` | CanaryOnly = 6 calls / 4 event types |
| `RegisterAdditiveBatch1RegistersOnlyVerifiedScope` | AdditiveBatch1 = 13 calls / 10 event types |
| `CombatEventsDeclareIsSharedTrue` | All 6 combat events have IsShared=true |
| `AllSharedEventModelsDeclareIsSharedTrue` | Shared-capable models have IsShared=true |

## Runtime Verification Status

Current status split: historical Off, CanaryOnly, and AdditiveBatch1 loader-gate evidence exists for the recorded `v0.106.1` smokes. Installed beta.85 package parity is restored, and the current `v0.107.0` Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510` is clean for loader proof; current multiplayer evidence remains pending until co-op is directly proven or explicitly blocked.

**UNVERIFIED** — requires game launch with:
1. `SPIREPLUS_STS1_EVENT_MODE=Off` (default) — verify no StS1 events appear
2. `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` — verify exactly 4 events appear
3. `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` — verify the bounded prototype batch only in a controlled runtime smoke
4. Multiplayer session — capture fail-closed proof or two-client behavior proof before making co-op claims

## Verdict

**Source-level fail-closed: VERIFIED by guard tests.**
**Historical loader-level fail-closed: VERIFIED only for the recorded `v0.106.1` diagnostic smokes.**
**Current runtime/co-op fail-closed: PENDING until the non-clean `v0.107.0` Off smoke is fixed, a clean current smoke exists, and multiplayer evidence exists.**
