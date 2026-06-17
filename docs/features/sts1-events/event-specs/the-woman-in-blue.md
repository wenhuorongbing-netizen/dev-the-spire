# The Woman in Blue — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Buy Potion (20 gold) | Pay 20 gold. Obtain a random potion. |
| Buy Potion (30 gold) | Pay 30 gold. Obtain a random potion. |
| Buy Potion (40 gold) | Pay 40 gold. Obtain a random potion. |
| Leave | Nothing happens. |

Each potion purchase option appears based on how many potions you can hold. The prices increase.

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TheWomanInBlue`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1TheWomanInBlue>()`.
- **Layout:** Default (simplified — no custom UI needed)

### Localization Keys
```
STS1_THE_WOMAN_IN_BLUE.title
STS1_THE_WOMAN_IN_BLUE.pages.INITIAL.description
STS1_THE_WOMAN_IN_BLUE.pages.INITIAL.options.BUY_1.title / .description
STS1_THE_WOMAN_IN_BLUE.pages.INITIAL.options.BUY_2.title / .description
STS1_THE_WOMAN_IN_BLUE.pages.INITIAL.options.BUY_3.title / .description
STS1_THE_WOMAN_IN_BLUE.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Random potion reward helper
