# Drug Dealer — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Buy a Potion | Pay varying gold for a potion. |
| Buy all Potions | Pay gold for all available potions. |
| Leave | Nothing happens. |

The dealer offers 3 potions at varying prices (25-75 gold each).

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1DrugDealer`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1DrugDealer>()`.
- **Layout:** Default

### Localization Keys
```
STS1_DRUG_DEALER.title
STS1_DRUG_DEALER.pages.INITIAL.description
STS1_DRUG_DEALER.pages.INITIAL.options.BUY_1.title / .description
STS1_DRUG_DEALER.pages.INITIAL.options.BUY_ALL.title / .description
STS1_DRUG_DEALER.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Random potion rewards (×3)
