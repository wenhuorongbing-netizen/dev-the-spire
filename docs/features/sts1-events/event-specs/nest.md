# Nest — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Search the Nest | Obtain a random relic. Gain 2 Curses (Parasite). |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Gain 3 Curses (Parasite) instead of 2.

## StS2 Implementation

### Class: `Sts1Nest`
- **Registration:** `[RegisterActEvent(typeof(Act2Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_NEST.title
STS1_NEST.pages.INITIAL.description
STS1_NEST.pages.INITIAL.options.SEARCH.title / .description
STS1_NEST.pages.INITIAL.options.LEAVE.title / .description
STS1_NEST.pages.SEARCH.description
```

### Dependencies
- Parasite curse card model (×2 normal, ×3 A15+)
- Random relic reward
