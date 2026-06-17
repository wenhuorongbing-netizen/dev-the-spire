# The Cleric - Event Specification

## StS1 Wiki Behavior

**Acts:** 1 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Heal (35 gold) | Pay 35 gold. Heal 25% of max HP. |
| Purify (50/75 gold) | Pay 50 gold, or 75 at A15+. Remove a card from your deck. |
| Leave | Nothing happens. |

### Ascension Differences

- A15+: Purify costs 75 gold instead of 50.

### Eligibility

- The event requires at least 35 gold to appear. In the shared StS2 model, every run participant must have at least 35 gold.

## StS2 Implementation

### Class: `Sts1TheCleric`

- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 1 event into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1TheCleric>()` and `content.ActEvent<Underdocks, Sts1TheCleric>()`; included in `RegisterAll()` and `RegisterAdditiveBatch1()`.
- **Layout:** Default
- **Source behavior:** `IsShared=true` preserves co-op vote semantics while Act registration restricts the event to StS2 Act 1 buckets. `IsAllowed(IRunState)` requires every player to have at least 35 gold. `Heal()` spends 35 gold and heals 25% max HP. `Purify()` spends `PurifyCost`, which is 50 normally and 75 at A15+, then opens card removal.
- **Runtime proof:** Pending. AdditiveBatch1 encounter UI, option lock behavior, result log, EN/ZHS render, and save-load proof still need live evidence.

### Localization Keys

```text
STS1_THE_CLERIC.title
STS1_THE_CLERIC.pages.INITIAL.description
STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title / .description
STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title / .description
STS1_THE_CLERIC.pages.INITIAL.options.LEAVE.title / .description
STS1_THE_CLERIC.pages.HEAL.description
STS1_THE_CLERIC.pages.PURIFY.description
```

### Dependencies

- `PlayerCmd.LoseGold`
- `CreatureCmd.Heal`
- `Sts1EventHelpers.OpenCardRemoval`

### Dynamic Variables

| Variable | Type | Value |
|----------|------|-------|
| HealCost | `GoldVar` | 35 |
| HealPct | `HealVar` | 25% max HP |
