# Transmogrifier — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Transform | Choose a card to transform. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Transmogrifier`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1Transmogrifier>()`.
- **Layout:** Default

### Localization Keys
```
STS1_TRANSMOGRIFIER.title
STS1_TRANSMOGRIFIER.pages.INITIAL.description
STS1_TRANSMOGRIFIER.pages.INITIAL.options.TRANSFORM.title / .description
STS1_TRANSMOGRIFIER.pages.INITIAL.options.LEAVE.title / .description
STS1_TRANSMOGRIFIER.pages.TRANSFORM.description
```

### Dependencies
- Card transform UI
