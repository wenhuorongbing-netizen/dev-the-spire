# N'loth — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer a Relic | Give up 1 relic. Obtain a random relic. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Nloth`
- **Registration:** `[RegisterActEvent(typeof(Act2Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_NLOTH.title
STS1_NLOTH.pages.INITIAL.description
STS1_NLOTH.pages.INITIAL.options.OFFER.title / .description
STS1_NLOTH.pages.INITIAL.options.LEAVE.title / .description
STS1_NLOTH.pages.OFFER.description
```

### Dependencies
- Relic selection UI (choose a relic to give up)
- Random relic reward
