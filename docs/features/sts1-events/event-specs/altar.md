# Altar — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pray | Upgrade a random card 3 times. |
| Sacrifice | Remove a card from your deck. Gain a random relic. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Altar`
- **Registration:** `[RegisterActEvent(typeof(Act2Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_ALTAR.title
STS1_ALTAR.pages.INITIAL.description
STS1_ALTAR.pages.INITIAL.options.PRAY.title / .description
STS1_ALTAR.pages.INITIAL.options.SACRIFICE.title / .description
STS1_ALTAR.pages.INITIAL.options.LEAVE.title / .description
STS1_ALTAR.pages.PRAY.description
STS1_ALTAR.pages.SACRIFICE.description
```

### Dependencies
- Upgrade card command (×3 random)
- Card removal UI
- Random relic reward
