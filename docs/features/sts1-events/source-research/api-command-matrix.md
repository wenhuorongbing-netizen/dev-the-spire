# StS2 API Command Matrix — StS1 Event Port

Created: 2026-05-29 | Status: source-verified (signatures from `source code/src/Core/Commands/`)

All APIs listed are from `source code/src/Core/` unless otherwise noted. Every signature below was verified against the actual source file.

## HP / Damage / Max HP

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Heal | `CreatureCmd.Heal` | `Commands/CreatureCmd.cs` | `async Task Heal(Creature creature, decimal amount, bool playAnim = true)` |
| Set current HP | `CreatureCmd.SetCurrentHp` | `Commands/CreatureCmd.cs` | `async Task SetCurrentHp(Creature creature, decimal amount)` |
| Gain max HP | `CreatureCmd.GainMaxHp` | `Commands/CreatureCmd.cs` | `async Task GainMaxHp(Creature creature, decimal amount)` |
| Lose max HP | `CreatureCmd.LoseMaxHp` | `Commands/CreatureCmd.cs` | `async Task LoseMaxHp(PlayerChoiceContext choiceContext, Creature creature, decimal amount, bool isFromCard)` |
| Damage | `CreatureCmd.Damage` | `Commands/CreatureCmd.cs` | 10 overloads; e.g. `async Task<IEnumerable<DamageResult>> Damage(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)` |
| Kill | `CreatureCmd.Kill` | `Commands/CreatureCmd.cs` | `async Task Kill(Creature creature, bool force = false)` |

### Usage in Canary Events (signatures approximate — verify against compiled code)

- **Big Fish Banana**: `CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp / 3m)` — floor division
- **Big Fish Donut**: `CreatureCmd.GainMaxHp(Owner.Creature, 5m)`
- **Golden Idol Smash**: `CreatureCmd.Damage(choiceContext, Owner.Creature, MaxHp * pct, ...)` — 25%/35% max HP as HP damage
- **Golden Idol Hide**: `CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, MaxHp * pct, isFromCard: false)` — 8%/10% max HP

## Cards / Curses

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Upgrade card | `CardCmd.Upgrade` | `Commands/CardCmd.cs` | `void Upgrade(CardModel card, CardPreviewStyle style = CardPreviewStyle.HorizontalLayout)` — synchronous, no `await` |
| Upgrade multiple | `CardCmd.Upgrade` | `Commands/CardCmd.cs` | `void Upgrade(IEnumerable<CardModel> cards, CardPreviewStyle style)` — list overload |
| Transform card | `CardCmd.Transform` | `Commands/CardCmd.cs` | `async Task<CardPileAddResult?> Transform(CardModel original, CardModel replacement, CardPreviewStyle style = ...)` |
| Transform to random | `CardCmd.TransformToRandom` | `Commands/CardCmd.cs` | `async Task<CardPileAddResult> TransformToRandom(CardModel original, Rng rng, CardPreviewStyle style = ...)` |
| Exhaust card | `CardCmd.Exhaust` | `Commands/CardCmd.cs` | `async Task Exhaust(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal = false, bool skipVisuals = false)` |
| Remove card from deck | `CardPileCmd.RemoveFromDeck` | `Commands/CardPileCmd.cs` | `async Task RemoveFromDeck(CardModel card, bool showPreview = true)` |
| Add card to pile | `CardPileCmd.Add` | `Commands/CardPileCmd.cs` | `async Task<CardPileAddResult> Add(CardModel card, PileType newPileType, CardPilePosition position = ..., ...)` |
| Add curse to deck | `CardPileCmd.AddCursesToDeck` | `Commands/CardPileCmd.cs` | `async Task<IEnumerable<CardPileAddResult>> AddCursesToDeck(IEnumerable<CardModel> curses, Player owner)` |
| Add single curse | `CardPileCmd.AddCurseToDeck<T>` | `Commands/CardPileCmd.cs` | `async Task<CardModel?> AddCurseToDeck<T>(Player owner) where T : CardModel` |
| Select from deck (generic) | `CardSelectCmd.FromDeckGeneric` | `Commands/CardSelectCmd.cs` | `async Task<IEnumerable<CardModel>> FromDeckGeneric(Player player, CardSelectorPrefs prefs, ...)` |
| Select for upgrade | `CardSelectCmd.FromDeckForUpgrade` | `Commands/CardSelectCmd.cs` | `async Task<IEnumerable<CardModel>> FromDeckForUpgrade(Player player, CardSelectorPrefs prefs)` |
| Select for removal | `CardSelectCmd.FromDeckForRemoval` | `Commands/CardSelectCmd.cs` | `Task<IEnumerable<CardModel>> FromDeckForRemoval(Player player, CardSelectorPrefs prefs, ...)` |
| Select for transformation | `CardSelectCmd.FromDeckForTransformation` | `Commands/CardSelectCmd.cs` | `async Task<IEnumerable<CardModel>> FromDeckForTransformation(Player player, CardSelectorPrefs prefs, ...)` |
| Select from hand | `CardSelectCmd.FromHand` | `Commands/CardSelectCmd.cs` | `async Task<IEnumerable<CardModel>> FromHand(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, ...)` |
| Simple grid select | `CardSelectCmd.FromSimpleGrid` | `Commands/CardSelectCmd.cs` | `async Task<IEnumerable<CardModel>> FromSimpleGrid(PlayerChoiceContext context, IReadOnlyList<CardModel> cardsIn, Player player, CardSelectorPrefs prefs)` |

### Verified Upgrade Usage

`CardCmd.Upgrade` is synchronous (void return, no `await`). Default `CardPreviewStyle` is `HorizontalLayout`.

| File | Usage | Notes |
| --- | --- | --- |
| `Sts1EventHelpers.cs:64` | `CardCmd.Upgrade(card)` | Single-card upgrade after `CardSelectCmd.FromDeckForUpgrade` |
| `TanxClawsMaulTuningPatches.cs:54` | `CardCmd.Upgrade(maul, CardPreviewStyle.None)` | Per-card loop |
| `SereTalonPickupPatches.cs:121` | `CardCmd.Upgrade(wish, CardPreviewStyle.None)` | Per-card loop |
| `PaelsHornPhase1Patch.cs:26` | `CardCmd.Upgrade(upgradedRelax)` | Single card, uses default preview |
| `PrismaticGemReplacementPatches.cs:50` | `CardCmd.Upgrade(replacement)` | Single card |
| `ForgeTokenService.cs:47` | `CardCmd.Upgrade(upgradeTarget, CardPreviewStyle.HorizontalLayout)` | Rest heal upgrade |
| `PickupRewardService.cs:9` | `CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)` | List overload (multiple cards) |
| `Sts1MindBloom.cs:51` | `CardCmd.Upgrade(card)` | Per-card loop in `Awake()` — upgrade all |

No `CardCmd.UpgradeCard` method exists anywhere in the codebase.

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

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Obtain specific relic | `RelicCmd.Obtain` | `Commands/RelicCmd.cs` | `async Task<RelicModel> Obtain(RelicModel relic, Player player, int index = -1)` |
| Obtain by type | `RelicCmd.Obtain<T>` | `Commands/RelicCmd.cs` | `async Task<T> Obtain<T>(Player player) where T : RelicModel` |
| Remove relic | `RelicCmd.Remove` | `Commands/RelicCmd.cs` | `async Task Remove(RelicModel relic)` — no `Player` parameter |
| Replace relic | `RelicCmd.Replace` | `Commands/RelicCmd.cs` | `async Task<RelicModel> Replace(RelicModel original, RelicModel replace)` — no `Player` parameter |
| Melt relic | `RelicCmd.Melt` | `Commands/RelicCmd.cs` | `async Task Melt(RelicModel relic)` |

**Note:** No `RelicCmd.ObtainRandom` method exists. Random relic obtain must use other mechanisms (e.g., reward system).

### Relic Lookups for Canary Events

| Relic | Status | Notes |
| --- | --- | --- |
| Golden Idol (StS1) | **Custom model required** | StS2 may have a different Golden Idol relic; verify with `ModelDb.HasRelic<Golden Idol>()` |
| Bloody Idol (StS1) | **Custom model required** | Not in StS2 base |

## Potions

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Procure specific potion | `PotionCmd.TryToProcure` | `Commands/PotionCmd.cs` | `async Task<PotionProcureResult> TryToProcure(PotionModel potion, Player player, int slotIndex = -1)` |
| Procure by type | `PotionCmd.TryToProcure<T>` | `Commands/PotionCmd.cs` | `async Task<PotionProcureResult> TryToProcure<T>(Player player) where T : PotionModel` |
| Discard potion | `PotionCmd.Discard` | `Commands/PotionCmd.cs` | `async Task Discard(PotionModel potion)` — no `Player` parameter |

### Usage in Canary Events

- **The Lab**: `PotionCmd.TryToProcure<T>(Owner)` ×3 for random potions (generic overload)

## Gold

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Gain gold | `PlayerCmd.GainGold` | `Commands/PlayerCmd.cs` | `async Task GainGold(decimal amount, Player player, bool wasStolenBack = false)` |
| Lose gold | `PlayerCmd.LoseGold` | `Commands/PlayerCmd.cs` | `Task LoseGold(decimal amount, Player player, GoldLossType goldLossType = GoldLossType.Lost)` |

**Note:** Parameter order is `(decimal amount, Player player)`, not `(Player, int)`.

## Event Flow

| Operation | API | File | Signature (source-verified) |
| --- | --- | --- | --- |
| Set event finished | `SetEventFinished` | `EventModel` | `void SetEventFinished(LocString description)` — takes `LocString`, not `string` |
| Set event state | `SetEventState` | `EventModel` | `void SetEventState(LocString description, IEnumerable<EventOption> options)` |
| Localization lookup | `L10NLookup` | `EventModel` | `LocString L10NLookup(string entryName)` — returns `LocString`, not `string` |
| Check ascension | `HasAscension` | `EventModel` | `bool HasAscension(int ascensionLevel)` |
| Enter combat | `EnterCombatWithoutExitingEvent<T>` | `EventModel` | `void EnterCombatWithoutExitingEvent<T>(...)` — returns `void`, not `Task` |

**Note:** `GoToPage` does not exist on `EventModel`. Event page navigation uses `SetEventState` with page-specific options.

## Save/Load

| Operation | API | Notes |
| --- | --- | --- |
| Event state persistence | `EventModel` serialization | Event state (current page, choices made) is serialized with the room. |
| Custom saved fields | `SavedAttachedState` | Current Spire Plus beta.93 uses RitsuLib `SavedAttachedState` for custom persistence. Historical `SavedSpireFields` rows are BaseLib-era evidence only. |
| Event bag state | Not yet implemented | StS1 event bag (no-repeat, per-act pools) requires custom `Sts1EventPoolService` — **blocked on Week 4**. |

## Missing APIs / Blockers

| Dependency | Status | Blocker |
| --- | --- | --- |
| Regret curse card | StS2 has native Regret | Confirm `ModelDb.Card<Regret>()` compiles |
| Injury curse card | Unconfirmed | May need custom `Sts1Injury : CardModel` |
| Golden Idol relic | Unconfirmed | May need custom `Sts1GoldenIdolRelic : RelicModel` |
| Random potion ×3 | `PotionCmd.TryToProcure<T>` available | Confirm generic overload resolves correctly |
| Card select/remove/upgrade UI | `CardSelectCmd` available | `Sts1Duplicator` excluded — needs `CardSelectCmd` + `CardPileCmd` |
| Combat encounter models | Not started | Dead Adventurer, Masked Bandits, etc. need combat encounter models |
| Event pool replacement | Not started | `ReplaceUnknownEventsPrototype` is design-only |
