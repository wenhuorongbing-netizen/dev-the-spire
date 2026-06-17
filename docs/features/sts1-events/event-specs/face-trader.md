# Face Trader — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Trade | Choose a face relic. Obtain it and lose 10% max HP. |
| Leave | Nothing happens. |

Face relics available:
- **Face of Cleric**: +1 max HP at the start of each turn
- **Face of the Guardian**: At the start of combat, gain 5 Block
- **Face of the Healer**: +7 max HP
- **Face of the Navigator**: Draw 1 extra card each turn
- **Face of the Soldier**: At the start of combat, gain 1 Strength

### Ascension Differences
- A15+: Lose 15% max HP instead of 10%.

## StS2 Implementation

### Class: `Sts1FaceTrader`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1FaceTrader>()`.
- **Layout:** Default

### Localization Keys
```
STS1_FACE_TRADER.title
STS1_FACE_TRADER.pages.INITIAL.description
STS1_FACE_TRADER.pages.INITIAL.options.TRADE.title / .description
STS1_FACE_TRADER.pages.INITIAL.options.LEAVE.title / .description
STS1_FACE_TRADER.pages.TRADE.description
```

### Dependencies
- Face relic models (Face of Cleric, Guardian, Healer, Navigator, Soldier)
- Max HP loss (10% normal, 15% A15+)
