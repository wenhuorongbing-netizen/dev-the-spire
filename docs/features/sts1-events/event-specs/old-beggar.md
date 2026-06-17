# Old Beggar — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Offer Gold (75 gold) | Pay 75 gold. Remove a card from your deck. Disabled when the player has fewer than 75 gold. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1OldBeggar`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1OldBeggar>()`.
- **Layout:** Default
- **Source guard:** `GenerateInitialOptions()` sets the Offer Gold handler to `null` unless `(Owner?.Gold ?? 0) >= GoldCost`, preventing underfunded card removal through `PlayerCmd.LoseGold` clamping.

### Localization Keys
```
STS1_OLD_BEGGAR.title
STS1_OLD_BEGGAR.pages.INITIAL.description
STS1_OLD_BEGGAR.pages.INITIAL.options.OFFER_GOLD.title / .description
STS1_OLD_BEGGAR.pages.INITIAL.options.LEAVE.title / .description
STS1_OLD_BEGGAR.pages.OFFER_GOLD.description
```

### Dependencies
- Card removal UI

### Dynamic Variables

None. The current localization uses static `75` text and the source keeps `GoldCost = 75`.
