# The Ghost — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Accept the Gift | Obtain 1 random rare card. |
| Refuse | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TheGhost`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1TheGhost>()`.
- **Layout:** Default

### Localization Keys
```
STS1_THE_GHOST.title
STS1_THE_GHOST.pages.INITIAL.description
STS1_THE_GHOST.pages.INITIAL.options.ACCEPT.title / .description
STS1_THE_GHOST.pages.INITIAL.options.REFUSE.title / .description
STS1_THE_GHOST.pages.ACCEPT.description
```

### Dependencies
- Random rare card reward
