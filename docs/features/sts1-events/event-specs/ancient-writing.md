# Ancient Writing — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Elegance | Upgrade a card. |
| Simplicity | Remove a card from your deck. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1AncientWriting`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1AncientWriting>()`.
- **Layout:** Default

### Localization Keys
```
STS1_ANCIENT_WRITING.title
STS1_ANCIENT_WRITING.pages.INITIAL.description
STS1_ANCIENT_WRITING.pages.INITIAL.options.ELEGANCE.title / .description
STS1_ANCIENT_WRITING.pages.INITIAL.options.SIMPLICITY.title / .description
STS1_ANCIENT_WRITING.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Card upgrade UI
- Card removal UI
