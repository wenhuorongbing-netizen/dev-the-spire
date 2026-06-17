# Joust — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Bet on Yourself (50 gold) | Pay 50 gold. 50% chance: Gain 200 gold. 50% chance: Lose gold, take damage. |
| Bet on Opponent (50 gold) | Pay 50 gold. 50% chance: Gain 200 gold. 50% chance: Lose gold. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Lose 100 gold on failure instead of just the 50 bet.

## StS2 Implementation

### Class: `Sts1Joust`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 1 event into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1Joust>()` and `content.ActEvent<Underdocks, Sts1Joust>()`.
- **Layout:** Default

### Localization Keys
```
STS1_JOUST.title
STS1_JOUST.pages.INITIAL.description
STS1_JOUST.pages.INITIAL.options.BET_SELF.title / .description
STS1_JOUST.pages.INITIAL.options.BET_OPPONENT.title / .description
STS1_JOUST.pages.INITIAL.options.LEAVE.title / .description
STS1_JOUST.pages.WIN.description
STS1_JOUST.pages.LOSE.description
```

### Dependencies
- Gold reward/loss
- RNG for 50/50
