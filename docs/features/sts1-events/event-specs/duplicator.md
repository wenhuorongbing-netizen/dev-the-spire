# Duplicator — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Duplicate | Choose a card in your deck. Obtain a copy of it. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Duplicator`
- **Registration:** Compile-excluded and not registered by `Sts1EventRegistrationService` until duplicate-card selection API support is available.
- **Layout:** Default

### Localization Keys
```
STS1_DUPLICATOR.title
STS1_DUPLICATOR.pages.INITIAL.description
STS1_DUPLICATOR.pages.INITIAL.options.DUPLICATE.title / .description
STS1_DUPLICATOR.pages.INITIAL.options.LEAVE.title / .description
STS1_DUPLICATOR.pages.DUPLICATE.description
```

### Dependencies
- Card selection UI (choose a card to copy)
