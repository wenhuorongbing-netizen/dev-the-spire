# Masked Bandits — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pay Them (75 gold) | Pay 75 gold. Nothing else happens. |
| Fight | Fight 3 bandits. Reward: gold + random relic. |

### Ascension Differences
- A15+: Bandits have +1 Strength.

## StS2 Implementation

### Class: `Sts1MaskedBandits`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1MaskedBandits>()`.
- **Layout:** Default / Combat hybrid

### Localization Keys
```
STS1_MASKED_BANDITS.title
STS1_MASKED_BANDITS.pages.INITIAL.description
STS1_MASKED_BANDITS.pages.INITIAL.options.PAY.title / .description
STS1_MASKED_BANDITS.pages.INITIAL.options.FIGHT.title / .description
```

### Dependencies
- Bandit encounter model (3 bandits)
- Random relic reward
