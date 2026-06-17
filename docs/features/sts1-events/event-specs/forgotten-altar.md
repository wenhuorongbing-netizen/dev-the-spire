# Forgotten Altar — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pray | Gain 3 max HP. Obtain a Curse (Doubt). |
| Offer Gold (50 gold) | Pay 50 gold. Gain 5 max HP. |
| Desecrate | Gain a random relic. Lose 10% max HP. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Pray gives +1 max HP instead of +3. Offer Gold gives +3 max HP instead of +5.

## StS2 Implementation

### Class: `Sts1ForgottenAltar`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1ForgottenAltar>()`.
- **Layout:** Default

### Localization Keys
```
STS1_FORGOTTEN_ALTAR.title
STS1_FORGOTTEN_ALTAR.pages.INITIAL.description
STS1_FORGOTTEN_ALTAR.pages.INITIAL.options.PRAY.title / .description
STS1_FORGOTTEN_ALTAR.pages.INITIAL.options.OFFER.title / .description
STS1_FORGOTTEN_ALTAR.pages.INITIAL.options.DESECRATE.title / .description
STS1_FORGOTTEN_ALTAR.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Doubt curse card model
- Random relic reward
- Max HP gain/loss
