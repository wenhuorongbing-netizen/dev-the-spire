# Vampires — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Accept | Remove all Strikes from your deck. Obtain 5 Bites. Lose 30% max HP. |
| Refuse | Nothing happens. |

Bite is a 1-cost Attack: Deal 7 damage. Heal 2 HP.

### Ascension Differences
- A15+: Lose 40% max HP instead of 30%.

## StS2 Implementation

### Class: `Sts1Vampires`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1Vampires>()`.
- **Layout:** Default

### Localization Keys
```
STS1_VAMPIRES.title
STS1_VAMPIRES.pages.INITIAL.description
STS1_VAMPIRES.pages.INITIAL.options.ACCEPT.title / .description
STS1_VAMPIRES.pages.INITIAL.options.REFUSE.title / .description
STS1_VAMPIRES.pages.ACCEPT.description
```

### Dependencies
- Bite card model (×5)
- Remove all Strikes
- Max HP loss (30% normal, 40% A15+)
