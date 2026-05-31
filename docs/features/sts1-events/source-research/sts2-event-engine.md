# StS2 Event Engine Research

## EventModel Base Class

`MegaCrit.Sts2.Core.Models.EventModel` extends `AbstractModel` and provides:

### Key Properties
- `Title` — localized from `{Entry}.title`
- `InitialDescription` — localized from `{Entry}.pages.INITIAL.description`
- `CurrentOptions` — list of `EventOption` for current page
- `IsFinished` — true when event has no more options
- `Owner` — the `Player` who entered the event
- `Rng` — deterministic RNG seeded from run + event
- `IsShared` — whether event appears in all acts
- `LayoutType` — Default, Combat, Ancient, or Custom
- `LocTable` — localization table name (default: "events")

### Key Methods
- `GenerateInitialOptions()` — abstract, returns initial page options
- `SetEventState(description, options)` — set current page
- `SetEventFinished(description)` — end the event
- `CalculateVars()` — compute dynamic variables
- `IsAllowed(IRunState)` — gate event availability
- `L10NLookup(entryName)` — lookup localization string
- `RelicOption<T>(onChosen)` — helper for relic-granting options
- `EnterCombatWithoutExitingEvent<T>(rewards, resume)` — enter combat from event

### Localization Pattern
```
{EVENT_ENTRY}.title
{EVENT_ENTRY}.pages.INITIAL.description
{EVENT_ENTRY}.pages.INITIAL.options.{OPTION_NAME}.title
{EVENT_ENTRY}.pages.INITIAL.options.{OPTION_NAME}.description
{EVENT_ENTRY}.pages.{PAGE_NAME}.description
```

### EventOption
`MegaCrit.Sts2.Core.Events.EventOption` represents a player choice:
- `Title` / `Description` — localized text
- `OnChosen` — async callback when chosen
- `IsLocked` — true if `onChosen` is null (no action). **CONFIRMED via IL decompilation**: Constructor #2 IL at offset 0x44 sets `IsLocked = (OnChosen == null)`. To create a disabled/locked option (e.g. when the player doesn't meet a condition), pass `null` as the `onChosen` handler: `new EventOption(this, condition ? ActionHandler : null, textKey)`. Do NOT use `condition ? null : null` — both branches produce `null` and the ternary is a no-op.
- `IsProceed` — "continue" button
- `HoverTips` — tooltip hints for the option
- `WillKillPlayer` — predicate for death warning
- `ThatDoesDamage(damage)` — marks option as damaging
- `ThatDecreasesMaxHp(value)` — marks option as max HP reducing

### Dynamic Variables
- `GoldVar` — gold amount with variance
- `DamageVar` — damage with properties
- `HealVar` — heal amount
- `MaxHpVar` — max HP change
- `StringVar` — arbitrary string (card name, relic name, etc.)

## RitsuLib Event Registration

### ModEventTemplate
Base class for RitsuLib mod events. Provides:
- `AssetProfile` — scene paths for layout/background
- `InitialOptionKey(name)` — namespaced option key helper
- `ModOptionKey(page, name)` — multi-page option key helper
- `CreateModRelicOption<T>()` — relic option helper

### Registration Attributes
```csharp
[RegisterSharedEvent]           // Shared across all acts
[RegisterActEvent(typeof(Act))] // Act-specific
```

### Content Pack Style
```csharp
RitsuLibFramework.CreateContentPack("EZMicroBalance")
    .ActEvent<Act1Model, Sts1BigFish>()
    .SharedEvent<Sts1DivineFountain>()
    .Apply();
```

## Event Room System

Events appear in "Unknown" rooms on the map. The `ActModel.GenerateRooms`
method creates `RoomSet.events` which contains event rooms. `PullNextEvent`
validates the next event, then `Hook.ModifyNextEvent` allows mods to
modify the selection.

### Room Types
- `RoomType.Event` — standard unknown/event room
- `RoomType.Boss` — boss room (Neow appears here)

### Event Pool
Events are filtered by:
1. Act (act-specific events only in their act)
2. `IsAllowed(IRunState)` — custom availability check
3. Deduplication (events don't repeat within a run)

## Image Loading

Event portraits are loaded from:
```
ImageHelper.GetImagePath("events/{entry}.png")
```

For mod events, this resolves to:
```
res://EZMicroBalance/images/events/{entry}.png
```

Phobia mode portraits use `_phobia_mode` suffix.

## Source References

- `source code/src/Core/Models/EventModel.cs` — base event class
- `source code/src/Core/Events/EventOption.cs` — option class
- `source code/src/Core/Models/AncientEventModel.cs` — ancient event base
- `source code/src/Core/Models/events/*.cs` — StS2 event implementations
