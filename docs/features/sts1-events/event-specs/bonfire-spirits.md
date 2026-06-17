# Bonfire Spirits — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer a Card | Remove a card from your deck. Heal to full HP. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1BonfireSpirits`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1BonfireSpirits>()`.
- **Layout:** Default

### Localization Keys
```
STS1_BONFIRE_SPIRITS.title
STS1_BONFIRE_SPIRITS.pages.INITIAL.description
STS1_BONFIRE_SPIRITS.pages.INITIAL.options.OFFER.title / .description
STS1_BONFIRE_SPIRITS.pages.INITIAL.options.LEAVE.title / .description
STS1_BONFIRE_SPIRITS.pages.OFFER.description
```

### Dependencies
- Card removal UI
- Full heal command
