# Mysterious Sphere — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Open the Sphere | Fight 2 Orb Walkers. Reward: random relic. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Orb Walkers have +1 Strength.

## StS2 Implementation

### Class: `Sts1MysteriousSphere`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1MysteriousSphere>()`.
- **Layout:** Combat

### Localization Keys
```
STS1_MYSTERIOUS_SPHERE.title
STS1_MYSTERIOUS_SPHERE.pages.INITIAL.description
STS1_MYSTERIOUS_SPHERE.pages.INITIAL.options.OPEN.title / .description
STS1_MYSTERIOUS_SPHERE.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Orb Walker encounter model (×2)
- Random relic reward
