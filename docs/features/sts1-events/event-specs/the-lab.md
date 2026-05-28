# The Lab — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Open | Obtain 3 random potions. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TheLab`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default

### Localization Keys
```
STS1_THE_LAB.title
STS1_THE_LAB.pages.INITIAL.description
STS1_THE_LAB.pages.INITIAL.options.OPEN.title / .description
STS1_THE_LAB.pages.INITIAL.options.LEAVE.title / .description
STS1_THE_LAB.pages.OPEN.description
```

### Dependencies
- Random potion reward helper (×3)
