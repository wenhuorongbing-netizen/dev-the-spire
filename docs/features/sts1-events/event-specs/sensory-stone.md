# Sensory Stone — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Touch the Stone | Obtain 1 of 3 random rare cards. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1SensoryStone`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1SensoryStone>()`.
- **Layout:** Default

### Localization Keys
```
STS1_SENSORY_STONE.title
STS1_SENSORY_STONE.pages.INITIAL.description
STS1_SENSORY_STONE.pages.INITIAL.options.TOUCH.title / .description
STS1_SENSORY_STONE.pages.INITIAL.options.LEAVE.title / .description
STS1_SENSORY_STONE.pages.TOUCH.description
```

### Dependencies
- Card selection UI (choose 1 of 3 rare cards)
