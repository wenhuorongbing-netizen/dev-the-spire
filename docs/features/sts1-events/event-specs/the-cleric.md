# The Cleric — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Heal (35 gold) | Pay 35 gold. Heal 25% of max HP. |
| Purify (50 gold) | Pay 50 gold. Remove a card from your deck. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TheCleric`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default

### Localization Keys
```
STS1_THE_CLERIC.title
STS1_THE_CLERIC.pages.INITIAL.description
STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title / .description
STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title / .description
STS1_THE_CLERIC.pages.INITIAL.options.LEAVE.title / .description
STS1_THE_CLERIC.pages.HEAL.description
STS1_THE_CLERIC.pages.PURIFY.description
```

### Dependencies
- Card removal UI (or use game command API)

### Dynamic Variables
| Variable | Type | Value |
|----------|------|-------|
| HealCost | GoldVar | 35 |
| PurifyCost | GoldVar | 50 |
| HealPct | HealVar | 25% max HP |
