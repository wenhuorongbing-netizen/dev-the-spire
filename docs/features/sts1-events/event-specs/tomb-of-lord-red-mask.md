# Tomb of Lord Red Mask — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer Gold (50 gold) | Pay 50 gold. Gain a random relic. |
| Offer all Gold | Give all gold. Gain a random relic. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TombOfLordRedMask`
- **Registration:** `[RegisterActEvent(typeof(Act3Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_TOMB_OF_LORD_RED_MASK.title
STS1_TOMB_OF_LORD_RED_MASK.pages.INITIAL.description
STS1_TOMB_OF_LORD_RED_MASK.pages.INITIAL.options.OFFER_50.title / .description
STS1_TOMB_OF_LORD_RED_MASK.pages.INITIAL.options.OFFER_ALL.title / .description
STS1_TOMB_OF_LORD_RED_MASK.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Random relic reward
