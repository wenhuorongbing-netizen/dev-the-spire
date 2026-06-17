# StS2 Event Engine Research

## 2026-06-11 Revision M Current Boundary

This source-research note documents event-engine APIs and source patterns for StS1 event implementation. It is not current `v0.107.0` runtime proof. Beta.85 proves default-Off loader behavior only; CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, handoff, and release-ready proof still require fresh current evidence.

## EventModel Base Class

`MegaCrit.Sts2.Core.Models.EventModel` extends `AbstractModel` and provides:

### Key Properties

- `Title` - localized from `{Entry}.title`
- `InitialDescription` - localized from `{Entry}.pages.INITIAL.description`
- `CurrentOptions` - list of `EventOption` for current page
- `IsFinished` - true when event has no more options
- `Owner` - the `Player` who entered the event
- `Rng` - deterministic RNG seeded from run + event
- `IsShared` - shared co-op event behavior; act scope comes from registration
- `LayoutType` - Default, Combat, Ancient, or Custom
- `LocTable` - localization table name, defaulting to `events`

### Key Methods

- `GenerateInitialOptions()` - abstract, returns initial page options
- `SetEventState(description, options)` - set current page
- `SetEventFinished(description)` - end the event
- `CalculateVars()` - compute dynamic variables
- `IsAllowed(IRunState)` - gate event availability
- `L10NLookup(entryName)` - lookup localization string
- `RelicOption<T>(onChosen)` - helper for relic-granting options
- `EnterCombatWithoutExitingEvent<T>(rewards, resume)` - enter combat from event

### Localization Pattern

```text
{EVENT_ENTRY}.title
{EVENT_ENTRY}.pages.INITIAL.description
{EVENT_ENTRY}.pages.INITIAL.options.{OPTION_NAME}.title
{EVENT_ENTRY}.pages.INITIAL.options.{OPTION_NAME}.description
{EVENT_ENTRY}.pages.{PAGE_NAME}.description
```

### EventOption

`MegaCrit.Sts2.Core.Events.EventOption` represents a player choice:

- `Title` / `Description` - localized text
- `OnChosen` - async callback when chosen
- `IsLocked` - true if `onChosen` is null. Confirmed via IL decompilation: constructor IL sets `IsLocked = (OnChosen == null)`. To create a disabled/locked option, pass `null` as the `onChosen` handler.
- `IsProceed` - continue button
- `HoverTips` - tooltip hints for the option
- `WillKillPlayer` - predicate for death warning
- `ThatDoesDamage(damage)` - marks option as damaging
- `ThatDecreasesMaxHp(value)` - marks option as max HP reducing

### Dynamic Variables

- `GoldVar` - gold amount with variance
- `DamageVar` - damage with properties
- `HealVar` - heal amount
- `MaxHpVar` - max HP change
- `StringVar` - arbitrary string such as card or relic name

## RitsuLib Event Registration

Current StS1 event models inherit `EventModel` directly and are registered centrally by `Sts1EventRegistrationService` through RitsuLib content-builder APIs. Per-model registration attributes and `ModEventTemplate` are not the current source pattern for this prototype.

```csharp
RitsuLibFramework.CreateContentPack("EZMicroBalance")
    .ActEvent<Overgrowth, Sts1BigFish>()
    .ActEvent<Underdocks, Sts1BigFish>()
    .SharedEvent<Sts1DivineFountain>()
    .Apply();
```

## Event Room System

Events appear in Unknown rooms on the map. The `ActModel.GenerateRooms` method creates `RoomSet.events`, which contains event rooms. `PullNextEvent` validates the next event, then `Hook.ModifyNextEvent` allows mods to modify the selection.

### Room Types

- `RoomType.Event` - standard unknown/event room
- `RoomType.Boss` - boss room

### Event Pool

Events are filtered by:

1. Act registration.
2. `IsAllowed(IRunState)` custom availability checks.
3. Deduplication so events do not repeat within a run until the pool allows it.

## Image Loading

Event portraits are loaded from:

```text
ImageHelper.GetImagePath("events/{entry}.png")
```

For mod events, this resolves to:

```text
res://EZMicroBalance/images/events/{entry}.png
```

Phobia mode portraits use `_phobia_mode` suffix.

## Source References

- `source code/src/Core/Models/EventModel.cs` - base event class
- `source code/src/Core/Events/EventOption.cs` - option class
- `source code/src/Core/Models/AncientEventModel.cs` - ancient event base
- `source code/src/Core/Models/events/*.cs` - StS2 event implementations
