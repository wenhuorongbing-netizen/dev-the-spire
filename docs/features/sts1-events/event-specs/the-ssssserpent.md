# The Ssssserpent — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Accept the Deal | Gain 150 gold. Obtain 2 Curses (Doubt). |
| Refuse | Nothing happens. |

### Ascension Differences
- A15+: Gain 3 Curses (Doubt) instead of 2.

## StS2 Implementation

### Class: `Sts1TheSsssserpent`
- **Registration:** `[RegisterActEvent(typeof(Act1Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_THE_SSSSSERPENT.title
STS1_THE_SSSSSERPENT.pages.INITIAL.description
STS1_THE_SSSSSERPENT.pages.INITIAL.options.ACCEPT.title / .description
STS1_THE_SSSSSERPENT.pages.INITIAL.options.REFUSE.title / .description
STS1_THE_SSSSSERPENT.pages.ACCEPT.description
```

### Dependencies
- Doubt curse card model (×2 normal, ×3 A15+)
- Gold reward (150)
