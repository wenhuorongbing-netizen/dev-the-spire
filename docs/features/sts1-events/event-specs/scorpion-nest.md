# Scorpion Nest — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Investigate | Fight 3 Louses (random types). Reward: random relic. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Louses have +1 Strength.

## StS2 Implementation

### Class: `Sts1ScorpionNest`
- **Registration:** `[RegisterActEvent(typeof(Act1Model))]`
- **Layout:** Combat

### Localization Keys
```
STS1_SCORPION_NEST.title
STS1_SCORPION_NEST.pages.INITIAL.description
STS1_SCORPION_NEST.pages.INITIAL.options.INVESTIGATE.title / .description
STS1_SCORPION_NEST.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Louse encounter model (3 Louses)
- Random relic reward
