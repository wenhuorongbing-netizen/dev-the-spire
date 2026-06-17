# Winding Halls — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Embrace Madness | Obtain 2 Madness cards. Lose 5% max HP. |
| Retreat | Take 20% of max HP as damage. |
| Continue On | Lose 10% max HP. |

### Ascension Differences
- A15+: All outcomes are worse: Embrace Madness gives 3 Madness cards and loses 10% max HP. Retreat deals 30% max HP damage. Continue On loses 15% max HP.

## StS2 Implementation

### Class: `Sts1WindingHalls`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1WindingHalls>()`.
- **Layout:** Default

### Localization Keys
```
STS1_WINDING_HALLS.title
STS1_WINDING_HALLS.pages.INITIAL.description
STS1_WINDING_HALLS.pages.INITIAL.options.EMBRACE.title / .description
STS1_WINDING_HALLS.pages.INITIAL.options.RETREAT.title / .description
STS1_WINDING_HALLS.pages.INITIAL.options.CONTINUE.title / .description
```

### Dependencies
- Madness card model (×2 normal, ×3 A15+)
- Max HP loss
- HP damage
