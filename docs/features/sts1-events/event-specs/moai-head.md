# Moai Head — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Worship | Gain 1 max HP. |
| Offer Gold (50 gold) | Pay 50 gold. Gain 3 max HP. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1MoaiHead`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1MoaiHead>()`.
- **Layout:** Default

### Localization Keys
```
STS1_MOAI_HEAD.title
STS1_MOAI_HEAD.pages.INITIAL.description
STS1_MOAI_HEAD.pages.INITIAL.options.WORSHIP.title / .description
STS1_MOAI_HEAD.pages.INITIAL.options.OFFER.title / .description
STS1_MOAI_HEAD.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Max HP gain (1 or 3)
