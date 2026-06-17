# Dead Adventurer — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Search the Body | 50% chance: Gain 30-50 gold. 25% chance: Find a random relic. 25% chance: Fight an Elite. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Elite encounter chance increases to 50% (gold/relic split the other 50%).

## StS2 Implementation

### Class: `Sts1DeadAdventurer`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 1 event into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1DeadAdventurer>()` and `content.ActEvent<Underdocks, Sts1DeadAdventurer>()`.
- **Layout:** Default

### Localization Keys
```
STS1_DEAD_ADVENTURER.title
STS1_DEAD_ADVENTURER.pages.INITIAL.description
STS1_DEAD_ADVENTURER.pages.INITIAL.options.SEARCH.title / .description
STS1_DEAD_ADVENTURER.pages.INITIAL.options.LEAVE.title / .description
STS1_DEAD_ADVENTURER.pages.GOLD.description
STS1_DEAD_ADVENTURER.pages.RELIC.description
STS1_DEAD_ADVENTURER.pages.FIGHT.description
```

### Dependencies
- Random elite encounter
- Random relic reward
- Gold reward (30-50)
