# ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK

## Status

**Stale — needs rewrite.** Current HEAD state differs from claims below.
See Sts1Events governance status in overnight run ledger for accurate state.

## Summary

StS1 event port model files (`Sts1Events/Models/`) are compiled into the
Spire Plus assembly. The feature module (`Sts1EventsFeatureModule`) is
registered in `SpirePlusFeatureRegistry` and gated to Off by default
via environment variable `SPIREPLUS_STS1_EVENT_MODE`.

## Current State (as of 2026-05-29 overnight run)

### Compile status

- 52 C# files under `EZMicroBalanceCode/Sts1Events/`
- 51 compiled, 1 compile-excluded (`Sts1Duplicator.cs`)
- `Sts1EventRegistrationService.cs` IS compiled (not compile-excluded)

### Feature registration

- `SpirePlusFeatureRegistry` registers `Sts1EventsFeatureModule`
- Feature gate defaults to Off when env var `SPIREPLUS_STS1_EVENT_MODE` is unset
- Zero events registered at runtime unless env var is set

### Guard tests

- 13+ dedicated tests in `Sts1EventFeatureGuardTests.cs`
- Tests verify gate defaults, canary events, act mapping, registry presence

### Why this is safe

The feature gate defaults to Off, so no events are registered at runtime
unless the environment variable is explicitly set. Guard tests verify this
behavior. The model files compile but are never instantiated by the game's
event system in the default configuration.

## Previous claims (stale — kept for history)

The following claims were accurate at time of writing (2026-05-28) but are
now stale:

1. "Registration service is compile-excluded" — now compiled
2. "Feature registry does not register Sts1EventsFeatureModule" — now registered
3. "Draft feature module archived" — now promoted to live source
