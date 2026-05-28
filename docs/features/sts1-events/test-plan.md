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
5. Verify all options work correctly
6. Verify localization displays properly

### Big Fish Test Matrix

| Option | Expected Result | Pass/Fail |
|--------|----------------|-----------|
| Banana | Heal 1/3 max HP | |
| Donut | +5 max HP | |
| Shoe | Random relic + Regret curse | |

### Golden Idol Test Matrix

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Take → Smash | Injury curse | Same | |
| Take → Jump | Lose 25% HP | Lose 35% HP | |
| Take → Destroy | Lose 10% max HP | Lose 15% max HP | |
| Leave | Nothing | Same | |

### Localization Test

1. Switch game language to English → verify all text displays
2. Switch game language to Chinese → verify all text displays
3. Verify no missing/placeholder text

## Test File

`tests/EZMicroBalance.Tests/Sts1EventManifestTests.cs`

Tests:
- All manifest events have source files
- All manifest events have localization entries
- Localization key format is valid
- No duplicate event entries
