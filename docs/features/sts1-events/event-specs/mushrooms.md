# Mushrooms — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Eat the Mushrooms | 50% chance: Gain 5 max HP. 50% chance: Lose 5 max HP and obtain a random potion. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Always lose 5 max HP (no positive outcome).

## StS2 Implementation

### Class: `Sts1Mushrooms`
- **Registration:** `[RegisterActEvent(typeof(Act1Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_MUSHROOMS.title
STS1_MUSHROOMS.pages.INITIAL.description
STS1_MUSHROOMS.pages.INITIAL.options.EAT.title / .description
STS1_MUSHROOMS.pages.INITIAL.options.LEAVE.title / .description
STS1_MUSHROOMS.pages.EAT_GOOD.description
STS1_MUSHROOMS.pages.EAT_BAD.description
```

### Dependencies
- Max HP gain/loss (5)
- Random potion reward
