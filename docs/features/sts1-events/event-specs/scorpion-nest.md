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
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 1 event into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1ScorpionNest>()` and `content.ActEvent<Underdocks, Sts1ScorpionNest>()`.
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
