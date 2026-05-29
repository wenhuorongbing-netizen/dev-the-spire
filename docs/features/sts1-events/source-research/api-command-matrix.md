# StS2 API Command Matrix — StS1 Event Port

Created: 2026-05-29 | Status: source-verified

All APIs listed are from `source code/src/Core/` unless otherwise noted. RitsuLib helper methods are from the STS2RitsuLib package.

## HP / Damage / Max HP

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Heal | `CreatureCmd.Heal` | `Commands/CreatureCmd.cs` | `async Task Heal(CreatureState creature, int amount)` |
| Set current HP | `CreatureCmd.SetCurrentHp` | `Commands/CreatureCmd.cs` | `async Task SetCurrentHp(CreatureState creature, int hp)` |
| Gain max HP | `CreatureCmd.GainMaxHp` | `Commands/CreatureCmd.cs` | `async Task GainMaxHp(CreatureState creature, int amount)` |
| Lose max HP | `CreatureCmd.LoseMaxHp` | `Commands/CreatureCmd.cs` | `async Task LoseMaxHp(CreatureState creature, int amount)` |
| Damage | `CreatureCmd.Damage` | `Commands/CreatureCmd.cs` | `async Task Damage(CreatureState source, CreatureState target, int amount, DamageType? type, DamageInfo? info)` (10 overloads) |
| Kill | `CreatureCmd.Kill` | `Commands/CreatureCmd.cs` | `async Task Kill(CreatureState creature)` |

### Usage in Canary Events

- **Big Fish Banana**: `CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp / 3)` — floor division
- **Big Fish Donut**: `CreatureCmd.GainMaxHp(Owner.Creature, 5)`
- **Golden Idol Jump**: `CreatureCmd.Damage(null, Owner.Creature, (int)(CurrentHp * pct), null, null)` — 25%/35% current HP
- **Golden Idol Destroy**: `CreatureCmd.LoseMaxHp(Owner.Creature, (int)(MaxHp * pct))` — 10%/15% max HP

## Cards / Curses

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Add curse to deck | `CardPileCmd.AddCursesToDeck` | `Commands/CardPileCmd.cs` | `async Task AddCursesToDeck(IReadOnlyList<CardModel> curses, Player player)` |
| Remove card | `CardCmd.RemoveCard` | `Commands/CardCmd.cs` | `async Task RemoveCard(CardModel card, Player player)` |
| Upgrade card | `CardCmd.UpgradeCard` | `Commands/CardCmd.cs` | `async Task UpgradeCard(CardModel card)` |
| Transform card | `CardCmd.TransformCard` | `Commands/CardCmd.cs` | `async Task TransformCard(CardModel card, Player player)` |
| Exhaust card | `CardCmd.Exhaust` | `Commands/CardCmd.cs` | `async Task Exhaust(CardModel card, Player player)` |
| Add card to deck | `CardPileCmd.AddCardToDeck` | `Commands/CardPileCmd.cs` | `async Task AddCardToDeck(CardModel card, Player player)` |
| Select card from deck | `CardSelectCmd.FromDeck` | `Commands/CardSelectCmd.cs` | `async Task<CardModel> FromDeck(Player player, ...)` |
| Select for transformation | `CardSelectCmd.FromDeckForTransformation` | `Commands/CardSelectCmd.cs` | `async Task<CardModel> FromDeckForTransformation(Player player, ...)` |

### Curse Card Lookups

| Curse | StS1 Name | StS2 Equivalent | Status |
| --- | --- | --- | --- |
| Regret | Regret | `ModelDb.Card<Regret>()` | StS2 has native Regret |
| Injury | Injury | `ModelDb.Card<Injury>()` | **Not confirmed** — may need custom model |
| Parasite | Parasite | StS2 has Parasite | **Substitute only** — StS1 Parasite differs |
| Doubt | Doubt | StS2 has Doubt | **Substitute only** |
| Normality | Normality | StS2 has Normality | **Substitute only** |
| Decay | Decay | StS2 has Decay | **Substitute only** |
| Writhe | Writhe | StS2 has Writhe | **Substitute only** |
| Pain | Pain | — | **Custom model required** |
| Nemesis | Nemesis | — | **Custom model required** |

## Relics

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Obtain specific relic | `RelicCmd.Obtain` | `Commands/RelicCmd.cs` | `async Task Obtain(RelicModel relic, Player player)` |
| Obtain random relic | `RelicCmd.ObtainRandom` | `Commands/RelicCmd.cs` | `async Task ObtainRandom(Player player)` (multiple overloads for pool filtering) |
| Remove relic | `RelicCmd.Remove` | `Commands/RelicCmd.cs` | `async Task Remove(RelicModel relic, Player player)` |
| Replace relic | `RelicCmd.Replace` | `Commands/RelicCmd.cs` | `async Task Replace(RelicModel old, RelicModel new, Player player)` |

### Relic Lookups for Canary Events

| Relic | Status | Notes |
| --- | --- | --- |
| Golden Idol (StS1) | **Custom model required** | StS2 may have a different Golden Idol relic; verify with `ModelDb.HasRelic<Golden Idol>()` |
| Bloody Idol (StS1) | **Custom model required** | Not in StS2 base |

## Potions

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Procure potion | `PotionCmd.TryToProcure` | `Commands/PotionCmd.cs` | `async Task TryToProcure(Player player, PotionModel? potion = null)` |
| Discard potion | `PotionCmd.Discard` | `Commands/PotionCmd.cs` | `async Task Discard(PotionModel potion, Player player)` |

### Usage in Canary Events

- **The Lab**: `PotionCmd.TryToProcure(Owner)` ×3 for random potions

## Gold

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Gain gold | `PlayerCmd.GainGold` | `Commands/PlayerCmd.cs` | `async Task GainGold(Player player, int amount)` |
| Lose gold | `PlayerCmd.LoseGold` | `Commands/PlayerCmd.cs` | `async Task LoseGold(Player player, int amount)` |

## Event Flow

| Operation | API | File | Signature |
| --- | --- | --- | --- |
| Set event finished | `SetEventFinished` | `EventModel` | `void SetEventFinished(string description)` |
| Go to page | `GoToPage` | `EventModel` | `Task GoToPage(string pageName)` |
| Set event state | `SetEventState` | `EventModel` | `void SetEventState(string description, IReadOnlyList<EventOption> options)` |
| Enter combat | `EnterCombatWithoutExitingEvent<T>` | `EventModel` | `Task EnterCombatWithoutExitingEvent<T>(...)` |
| Check ascension | `HasAscension` | `EventModel` | `bool HasAscension(int level)` |
| Localization lookup | `L10NLookup` | `EventModel` | `string L10NLookup(string entryName)` |

## Save/Load

| Operation | API | Notes |
| --- | --- | --- |
| Event state persistence | `EventModel` serialization | Event state (current page, choices made) is serialized with the room. |
| Custom saved fields | `SavedSpireFields` | Spire Plus uses `SavedSpireFields` for custom persistence. StS1 events can use the same mechanism. |
| Event bag state | Not yet implemented | StS1 event bag (no-repeat, per-act pools) requires custom `Sts1EventPoolService` — **blocked on Week 4**. |

## Missing APIs / Blockers

| Dependency | Status | Blocker |
| --- | --- | --- |
| `RelicCmd.ObtainRandom` with pool filter | Available in RitsuLib | Verify overload signature matches usage |
| Regret curse card | StS2 has native Regret | Confirm `ModelDb.Card<Regret>()` compiles |
| Injury curse card | Unconfirmed | May need custom `Sts1Injury : CardModel` |
| Golden Idol relic | Unconfirmed | May need custom `Sts1GoldenIdolRelic : RelicModel` |
| Random potion ×3 | `PotionCmd.TryToProcure` available | Confirm no side-effect conflicts |
| Card select/remove/upgrade UI | `CardSelectCmd` available | `Sts1Duplicator` excluded — needs `CardSelectCmd` + `CardPileCmd` |
| Combat encounter models | Not started | Dead Adventurer, Masked Bandits, etc. need combat encounter models |
| Event pool replacement | Not started | `ReplaceUnknownEventsPrototype` is design-only |
