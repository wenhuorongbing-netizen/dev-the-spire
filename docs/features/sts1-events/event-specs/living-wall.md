# Living Wall — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Forget | Remove a card from your deck. |
| Change | Transform a card in your deck. |
| Trade | Upgrade a card in your deck. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1LivingWall`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1LivingWall>()`.
- **Layout:** Default

### Localization Keys
```
STS1_LIVING_WALL.title
STS1_LIVING_WALL.pages.INITIAL.description
STS1_LIVING_WALL.pages.INITIAL.options.FORGET.title / .description
STS1_LIVING_WALL.pages.INITIAL.options.CHANGE.title / .description
STS1_LIVING_WALL.pages.INITIAL.options.TRADE.title / .description
STS1_LIVING_WALL.pages.FORGET.description
STS1_LIVING_WALL.pages.CHANGE.description
STS1_LIVING_WALL.pages.TRADE.description
```

### Dependencies
- Card remove/transform/upgrade UI
