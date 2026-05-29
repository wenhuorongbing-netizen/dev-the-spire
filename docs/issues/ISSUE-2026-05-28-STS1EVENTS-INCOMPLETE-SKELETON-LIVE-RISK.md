# ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK

## Status

**Closed as safe — inactive source skeleton.**

## Summary

StS1 event port model files (`Sts1Events/Models/`) are compiled into the
Spire Plus assembly but are **dead code**: the registration service that would
register them with the game's event system is compile-excluded and has no
call site in the live initialization path.

## Evidence

### Compile exclusion

`EZMicroBalance.csproj` excludes the registration service:

```xml
<Compile Remove="EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.cs" />
```

The only file that calls `content.SharedEvent<>()` / `content.ActEvent<>()` is
`Sts1EventRegistrationService.cs`, which is never compiled.

### No call site in MainFile.cs

`MainFile.Initialize()` does not call `Sts1EventRegistrationService.RegisterAll()`.
The feature registry (`SpirePlusFeatureRegistry.CreateDefault()`) does not register
a `Sts1EventsFeatureModule`. The committed feature orders do not include `Sts1Events`.

### Draft feature module archived

A local draft `Sts1EventsFeatureModule.cs` and `Sts1EventFeatureGate.cs` existed
in the working tree but were never committed. They reference the compile-excluded
`Sts1EventRegistrationService` and would cause a build error if compiled. They have
been archived to `docs/archive/sts1-events-feature-module-draft/`.

### TODO branches in model files

1. **Sts1DeadAdventurer** (Act 1): Line 37 has `// TODO: Enter combat with random elite`.
   When the elite branch is rolled, the event ends without entering combat.

2. **Sts1Joust** (Act 1): No gold-sufficiency check. `BetSelf()` and `BetOpponent()`
   call `PlayerCmd.GainGold(-50, Owner)` without checking if the player has ≥50 gold.
   On A15 failure, an additional 100g is deducted unconditionally.

### Why this is safe

Both TODO branches are unreachable in live gameplay because:
- The registration service is compile-excluded
- No code path in `MainFile.Initialize()` or the feature registry triggers registration
- The model files are compiled but never instantiated by the game's event system

## Guard

`RitsuLibMigrationGuardTests.Sts1EventRegistrationServiceIsCompileExcluded()` verifies
that `Sts1EventRegistrationService.cs` remains in the `<Compile Remove>` list.

## Resolution criteria

Before Sts1Events can go live:
- [ ] Complete Sts1DeadAdventurer elite combat branch
- [ ] Add gold-sufficiency check to Sts1Joust
- [ ] Add negative-gold protection
- [ ] Implement and register `Sts1EventsFeatureModule` with proper feature gate
- [ ] Add runtime smoke test for event registration

## Created

2026-05-28
