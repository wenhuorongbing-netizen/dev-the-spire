# StS1 Events Test Plan

## Automated Tests

### 1. Manifest Integrity Test
Verify that all events in the manifest have corresponding:
- Source file in `EZMicroBalanceCode/Sts1Events/`
- Localization entries in both `eng/sts1_events.json` and `zhs/sts1_events.json`
- Documentation in `event-specs/`

### 2. Localization Completeness Test
Verify that every event class has all required localization keys:
- `{ENTRY}.title`
- `{ENTRY}.pages.INITIAL.description`
- At least one `{ENTRY}.pages.INITIAL.options.*.title`

### 3. Build Verification
`dotnet build` must succeed with all event files included.

## Manual Testing

### Debug Spawn Test (Canary Events)

1. Build and publish: `dotnet build && dotnet publish`
2. Install to `<GameRoot>/mods/EZMicroBalance/`
3. Start a new run
4. Use debug console to spawn events:
   - `event sts1_big_fish`
   - `event sts1_golden_idol`
   - `event sts1_the_lab`
   - `event sts1_divine_fountain`
5. Verify all options work correctly
6. Verify localization displays properly

### Big Fish Test Matrix

| Option | Expected Result | Pass/Fail |
|--------|----------------|-----------|
| Banana | Heal 1/3 max HP | |
| Donut | +5 max HP | |
| Box | Random relic + Regret curse | |

### Golden Idol Test Matrix

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Take → Outrun | Injury curse | Same | |
| Take → Smash | Lose 25% max HP as HP damage | Lose 35% max HP as HP damage | |
| Take → Hide | Lose 8% max HP | Lose 10% max HP | |
| Leave | Nothing | Same | |

### Golden Shrine AdditiveBatch1 Check

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Pray | Gain 100 gold | Gain 50 gold | |
| Desecrate | Gain 275 gold and obtain Regret | Same | |
| Leave | Nothing | Same | |

### The Cleric AdditiveBatch1 Check

| State | Option | Expected Result | A15+ | Pass/Fail |
|-------|--------|----------------|------|-----------|
| Player has 35+ gold | Heal | Spend 35 gold, heal 25% max HP | Same | |
| Player has fewer than 35 gold | Encounter eligibility | Event should not appear from the random pool | Same | |
| Normal, player has 50+ gold | Purify | Spend 50 gold, then remove 1 card | N/A | |
| A15+, player has 75+ gold | Purify | N/A | Spend 75 gold, then remove 1 card | |
| Player has less than Purify cost | Purify | Option is unavailable; no card-removal UI opens | Same | |
| Any | Leave | Nothing | Same | |

### The Lab Test Matrix

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Open | Obtain 3 random potions | Obtain 2 random potions | |

### Old Beggar AdditiveBatch1 Check

| State | Option | Expected Result | Pass/Fail |
|-------|--------|----------------|-----------|
| Player has 75+ gold | Offer Gold | Spend 75 gold, then remove 1 card | |
| Player has fewer than 75 gold | Offer Gold | Option is unavailable; no card-removal UI opens | |

### Shining Light AdditiveBatch1 Check

| State | Option | Expected Result | Pass/Fail |
|-------|--------|----------------|-----------|
| Normal | Enter | Lose 30% max HP as unblockable damage, then 2 random upgradable deck cards upgrade without opening the card picker | |
| Ascension 15+ | Enter | Lose 40% max HP as unblockable damage, then 2 random upgradable deck cards upgrade without opening the card picker | |
| Fewer than 2 upgradable cards | Enter | Upgrade every available upgradable deck card, up to 2 | |

### Localization Test

1. Switch game language to English → verify all text displays
2. Switch game language to Chinese → verify all text displays
3. Verify no missing/placeholder text

## Test File

`tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs`

Tests:
- All manifest events have source files
- All manifest events have localization entries
- Localization key format is valid
- No duplicate event entries
