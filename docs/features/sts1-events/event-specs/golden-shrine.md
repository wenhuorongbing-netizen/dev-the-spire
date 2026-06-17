# Golden Shrine - Event Specification

## StS1 Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pray | Gain 100 gold. |
| Desecrate | Gain 275 gold. Obtain Regret. |
| Leave | Nothing happens. |

### Ascension Differences

- A15+: Pray gives 50 gold instead of 100.

## StS2 Implementation

### Class: `Sts1GoldenShrine`

- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1GoldenShrine>()`; included in `RegisterAll()` and `RegisterAdditiveBatch1()`.
- **Layout:** Default
- **Source behavior:** `Pray()` grants `PrayGoldAmount`, which is 100 normally and 50 at A15+. `Desecrate()` grants 275 gold and adds `Regret` with `CardPileCmd.AddCursesToDeck(new[] { ModelDb.Card<Regret>() }, owner)`.
- **Runtime proof:** Pending. AdditiveBatch1 encounter UI, result log, EN/ZHS render, and save-load proof still need live evidence.

### Localization Keys

```text
STS1_GOLDEN_SHRINE.title
STS1_GOLDEN_SHRINE.pages.INITIAL.description
STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.title / .description
STS1_GOLDEN_SHRINE.pages.INITIAL.options.DESECRATE.title / .description
STS1_GOLDEN_SHRINE.pages.INITIAL.options.LEAVE.title / .description
STS1_GOLDEN_SHRINE.pages.PRAY.description
STS1_GOLDEN_SHRINE.pages.DESECRATE.description
STS1_GOLDEN_SHRINE.pages.LEAVE.description
```

### Dependencies

- `PlayerCmd.GainGold`
- `CardPileCmd.AddCursesToDeck`
- `ModelDb.Card<Regret>()`

### Dynamic Variables

The source keeps `GoldVar(100)` for the Pray branch, while current localization uses static 100/50 and 275 text.
