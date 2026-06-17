# Treasure Ooze — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer Gold | Give 50 gold. Obtain a random relic. |
| Fight | Fight a large slime. Reward: gold + relic. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: The slime has +1 Strength.

## StS2 Implementation

### Class: `Sts1TreasureOoze`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 1 event into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1TreasureOoze>()` and `content.ActEvent<Underdocks, Sts1TreasureOoze>()`.
- **Layout:** Default / Combat hybrid

### Localization Keys
```
STS1_TREASURE_OOZE.title
STS1_TREASURE_OOZE.pages.INITIAL.description
STS1_TREASURE_OOZE.pages.INITIAL.options.OFFER.title / .description
STS1_TREASURE_OOZE.pages.INITIAL.options.FIGHT.title / .description
STS1_TREASURE_OOZE.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Large slime encounter model
- Random relic reward
