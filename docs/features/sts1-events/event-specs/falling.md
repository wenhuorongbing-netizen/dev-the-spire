# Falling — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Let Go | Remove a card from your deck. |
| Hold On | Take 30% of max HP as damage. |
| Fly | Transform a card in your deck. |

### Ascension Differences
- A15+: Hold On deals 40% max HP instead of 30%.

## StS2 Implementation

### Class: `Sts1Falling`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1Falling>()`.
- **Layout:** Default

### Localization Keys
```
STS1_FALLING.title
STS1_FALLING.pages.INITIAL.description
STS1_FALLING.pages.INITIAL.options.LET_GO.title / .description
STS1_FALLING.pages.INITIAL.options.HOLD_ON.title / .description
STS1_FALLING.pages.INITIAL.options.FLY.title / .description
```

### Dependencies
- Card removal UI
- Card transform UI
- Damage (30% normal, 40% A15+)
