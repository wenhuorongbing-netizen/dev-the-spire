# Divine Fountain — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pray | Remove all Curses from your deck. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1DivineFountain`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default

### Localization Keys
```
STS1_DIVINE_FOUNTAIN.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.PRAY.title / .description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.title / .description
STS1_DIVINE_FOUNTAIN.pages.PRAY.description
```

### Dependencies
- Remove all curses command
