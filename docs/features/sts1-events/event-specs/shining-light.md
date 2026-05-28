# Shining Light — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Enter | Take 30% of max HP as damage. Upgrade 2 random cards. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Take 40% of max HP as damage instead of 30%.

## StS2 Implementation

### Class: `Sts1ShiningLight`
- **Registration:** `[RegisterActEvent(typeof(Act1Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_SHINING_LIGHT.title
STS1_SHINING_LIGHT.pages.INITIAL.description
STS1_SHINING_LIGHT.pages.INITIAL.options.ENTER.title / .description
STS1_SHINING_LIGHT.pages.INITIAL.options.LEAVE.title / .description
STS1_SHINING_LIGHT.pages.ENTER.description
```

### Dependencies
- Upgrade random cards command
- Damage (30% normal, 40% A15+)
