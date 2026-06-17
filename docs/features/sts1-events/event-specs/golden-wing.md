# Golden Wing — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Accept | Obtain the card offered (a random rare card). |
| Decline | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1GoldenWing`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1GoldenWing>()`.
- **Layout:** Default

### Localization Keys
```
STS1_GOLDEN_WING.title
STS1_GOLDEN_WING.pages.INITIAL.description
STS1_GOLDEN_WING.pages.INITIAL.options.ACCEPT.title / .description
STS1_GOLDEN_WING.pages.INITIAL.options.DECLINE.title / .description
STS1_GOLDEN_WING.pages.ACCEPT.description
```

### Dependencies
- Random rare card reward helper

### Notes
- Offers a random rare card from the player's class
