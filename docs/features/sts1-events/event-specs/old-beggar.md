# Old Beggar — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer Gold (75 gold) | Pay 75 gold. Remove a card from your deck. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1OldBeggar`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default

### Localization Keys
```
STS1_OLD_BEGGAR.title
STS1_OLD_BEGGAR.pages.INITIAL.description
STS1_OLD_BEGGAR.pages.INITIAL.options.OFFER_GOLD.title / .description
STS1_OLD_BEGGAR.pages.INITIAL.options.LEAVE.title / .description
STS1_OLD_BEGGAR.pages.OFFER_GOLD.description
```

### Dependencies
- Card removal UI

### Dynamic Variables
| Variable | Type | Value |
|----------|------|-------|
| GoldCost | GoldVar | 75 |
