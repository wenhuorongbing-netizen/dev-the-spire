# Fountain of Cleansing — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Drink | Remove all Curses from your deck. Lose 10% max HP. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Lose 15% max HP instead of 10%.

## StS2 Implementation

### Class: `Sts1FountainOfCleansing`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default

### Localization Keys
```
STS1_FOUNTAIN_OF_CLEANSING.title
STS1_FOUNTAIN_OF_CLEANSING.pages.INITIAL.description
STS1_FOUNTAIN_OF_CLEANSING.pages.INITIAL.options.DRINK.title / .description
STS1_FOUNTAIN_OF_CLEANSING.pages.INITIAL.options.LEAVE.title / .description
STS1_FOUNTAIN_OF_CLEANSING.pages.DRINK.description
```

### Dependencies
- Remove all curses command
- Max HP loss (10% normal, 15% A15+)
