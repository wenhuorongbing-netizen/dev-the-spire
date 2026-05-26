# Spire Plus API Discovery

Last updated: 2026-05-26 19:30:00 +02:00

## Reference Check

Live pages rechecked on 2026-05-05:

- RitsuLib Ancient tutorial: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/04-ritsulib/04-07-add-ancient/`
- BaseLib Ancient tutorial: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/03-baselib/03-07-add-ancient/`

Tutorial mismatch:

- RitsuLib guidance uses `ModAncientEventTemplate`, registration attributes such as `RegisterActAncient` / `RegisterSharedAncient`, `CreateModRelicOption<T>()`, `AllPossibleOptions`, and `GenerateInitialOptions()`.
- The active `EZMicroBalance.csproj` references `Alchyr.Sts2.BaseLib` `3.1.4`; no RitsuLib package is referenced. Earlier discovery work started from the legacy `EzDailyContent.csproj`, which is now archived outside the active solution.
- The BaseLib tutorial aligns with the current project dependency and shows `CustomAncientModel`, `OptionPools`, `MakePool(...)`, and `AncientOption<T>()` for custom Ancients.
- Phase 1 does not add a custom Ancient. It patches an existing game Ancient relic reward, so the tutorial pages are context for Ancient option structure rather than the direct implementation API.

## Local Compile-time Evidence

Evidence source:

- Local game assembly: `D:\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`
- Runtime target in `docs/dev-environment.md`: public beta `v0.106.0`, source-refreshed locally on `2026-05-22`
- Local project package: `Alchyr.Sts2.BaseLib` `3.1.4`
- Tooling used for API inspection: local ignored `.tools/ilspy` install of `ilspycmd` `8.2.0.7535`

Findings:

- 2026-05-22 v0.106.0 source refresh: `BrightestFlame` still exposes canonical `CardsVar` draw text and `CardKeyword.Exhaust` can be surfaced through canonical keyword patching. EZMB adjusts Quality Flame by adding the Exhaust keyword and increasing the existing dynamic draw var by 1, so vanilla upgrade scaling remains dynamic instead of adding a second post-play draw command.
- `MegaCrit.Sts2.Core.Models.Events.Pael` is the Pael Ancient event model.
- Pael's first option pool includes `MegaCrit.Sts2.Core.Models.Relics.PaelsHorn`.
- `MegaCrit.Sts2.Core.Events.EventOption` stores the option `TextKey`, optional `Relic`, and private `OnChosen` callback. `Chosen()` invokes the callback after any `BeforeChosen` hook.
- Existing no-op probe `EzDailyContentCode/AncientRewardNoopProbe.cs` patches `AncientEventModel.GenerateInitialOptionsWrapper` and logs generated option count, text key, option type, and relic summary. It was useful during API discovery. The active `EZMicroBalance` release project does not compile this probe; the legacy probe is also gated behind explicit environment variable `EZ_MICRO_BALANCE_DEBUG_PROBES=1`.
- `MegaCrit.Sts2.Core.Models.Relics.PaelsHorn.AfterObtained()` is the narrowest phase 1 hook. Local evidence shows the vanilla method creates two `Relax` card instances and adds them to the deck.
- Existing command APIs for this phase:
  - `owner.RunState.CreateCard<Relax>(owner)` creates an owned `Relax` instance.
  - `CardCmd.Upgrade(card)` upgrades a card instance.
  - `CardPileCmd.Add(card, PileType.Deck)` adds the card to the deck and records deck gain history.
  - `CardCmd.PreviewCardPileAdd(results, 2f)` previews added cards.

Path note:

- Early discovery entries refer to `EzDailyContentCode/Ancients/` because the work started in the original scaffold. The active release implementation now lives under `EZMicroBalanceCode/Ancients/`; use `docs/PROJECT_MAP.md` and `docs/architecture-ez-micro-balance.md` for current project boundaries.

## Phase 1 Patch Point

Patch:

- `HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))`
- Prefix sets `__result` to the modded async task and returns `false`, skipping the vanilla method.

Reason:

- It changes only `PaelsHorn` reward application.
- It avoids rewriting Pael option generation or unrelated Ancient pools.
- It uses the same card creation, deck-add, and preview APIs as the vanilla method, with the only behavioral difference being `CardCmd.Upgrade()` on the second `Relax` instance before it is added.

Historical phase-1 limits from before the finish batches:

- The exact Pael's Horn in-game `TextKey` and rendered option text originally needed runtime confirmation via `[AncientRewardNoopProbe]` log lines. Current source guards and localization overrides now cover the active `EZMicroBalance` implementation; manual gameplay verification is still pending.
- Manual verification should still confirm the deck receives exactly one `Relax` and one `Relax+` after choosing Pael's Horn.
- The phase-1 pass did not update localization text. The later active localization pass added English and Simplified Chinese overrides for the changed Ancient behavior.

## Batch 2 API Evidence

Current authoritative source is the refreshed local public beta `v0.106.0` assembly/source noted above. The original Batch 2 inspection was performed against `v0.104.0` (`2026.04.23`) plus the live RitsuLib/BaseLib tutorial references; keep those older notes as historical context only and revalidate against `v0.106.0` before treating an old Batch 2 detail as current API authority.

Implemented with narrow Harmony patches. The original batch was developed in the legacy `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`; the active release implementation now lives in grouped files under `EZMicroBalanceCode/Ancients/Patches/`:

- `RelicModel.AfterObtained()` is used only for relics that do not override pickup behavior locally.
- `BlackStar` is patched at `RelicCmd.Obtain(RelicModel, Player, int)` instead of the base pickup hook so the act-3 compensation runs after the relic is actually obtained from rewards/events. Local `RunState.CurrentActIndex` is zero-based; act 3+ is `>= 2`. `RelicFactory.PullNextRelicFromFront(player).ToMutable()` plus `RelicCmd.Obtain(...)` grants the immediate compensation relic.
  - `WarHammer`: `CardSelectCmd.FromDeckForUpgrade(...)` provides the deck upgrade UI; `CardCmd.Upgrade(...)` applies the two chosen upgrades.
  - `Sozu`: `PotionFactory.CreateRandomPotionOutOfCombat(...)` and `PotionCmd.TryToProcure(...)` fill open potion slots. A temporary gate patch lets this initial fill bypass Sozu's own future-potion block.
  - `Ectoplasm`: `PlayerCmd.GainGold(250, owner)` is used for initial gold. A temporary gate patch lets this pickup grant bypass Ectoplasm's own future-gold block.
  - `SealOfGold`: pickup adds two `Debt` cards with `RunState.CreateCard<Debt>()` and `CardPileCmd.Add(...)`.
- Existing override patches:
  - `JewelryBox.AfterObtained()` creates `Apotheosis`, removes `Innate` with instance-level keyword mutation, then adds it to the deck.
  - `PreservedFog.AfterObtained()` changes removal count from 3 to 4 and creates `Folly` without `Ethereal` or `Retain`.
  - `BeautifulBracelet.AfterObtained()` uses the existing deck enchantment UI and applies `Swift` amount 2.
  - `MusicBox` is reimplemented around the same attack-copy trigger, but the generated attack copy gets temporary cost -1 plus `Ethereal` and `Exhaust`.
  - `WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate(...)` now auto-plays only one currently playable highest-cost hand card during rounds 1-3.
  - `PumpkinCandle` is no longer patched by EZMB. Current source keeps vanilla Pumpkin Candle behavior and has no `PumpkinCandlePatch`, `ExtinguishedSentinel`, or active `PUMPKIN_CANDLE.description` override.
- Dynamic variable getter patches:
  - `IronClub` `CardsVar(5)` changes draw cadence to every 5 cards.
  - `BrilliantScarf` `CardsVar(6)` changes the free-card trigger to the 6th card each turn.
  - `BeautifulBracelet` reports `Swift` amount 2.
- Card patches:
  - `Debt` is configured as 1-cost, playable, `Exhaust`; `CardCmd.Exhaust(...)` loses up to 5 gold for `Debt`.
  - `Enthralled` gains 10 block on play while preserving the existing forced-priority behavior.
  - `BrightestFlame` is the current game class behind Quality Flame. Vanilla source has `CardsVar(2)` and upgrades both `Energy` and `Cards` by 1. EZMB patches `BrightestFlame.CanonicalVars` so draw is vanilla +1 at the dynamic-var source, giving 3 cards unupgraded and 4 upgraded, and patches `CanonicalKeywords` so Exhaust is visible before play. The active localization override uses `BRIGHTEST_FLAME.title` / `BRIGHTEST_FLAME.description` with `{Cards:diff()}` instead of fixed draw text.

Historical limits after batch 2, superseded by later finish evidence below:

- In-game localization/resources were not rewritten in that batch. The current active resource pass now provides English and Simplified Chinese overrides for changed relic/card/rest-site text.
- `SereTalon`, `Crossbow`, `Fiddle`, `JeweledMask`, `ChoicesParadox`, `PrismaticGem`, `PaelsTooth`, rest-site changes, and reward-slot rewrites were later implemented with the local API evidence documented below. `Claws` (`Tanx Claws` in player text) is the separate Tanx Maul-transform relic and is not the Vakuu curse/Wish relic. They remain manual-runtime pending, not source-deferred.
- `Debt` now loses gold on `CardCmd.Exhaust(...)`; manual testing should confirm all expected exhaust paths use that command.

## Finish Batch API Evidence

Timestamp: 2026-05-05 17:14:30 +02:00.

Additional local APIs inspected:

- `SereTalon` now uses a scoped Spire Plus pickup patch because the rebalance requires a four-Curse choice screen. Use `CardSelectCmd.FromSimpleGrid` because the three-card choose screen cannot show four Curses.
- Add-to-deck feedback should not rely on `CardPileCmd.Add(...)` alone. Local Core reward/shop flows animate existing UI cards separately, so Spire Plus direct-gain paths use `SpirePlusFeedback.PreviewDeckAdds(...)`: it keeps `CardCmd.PreviewCardPileAdd(...)`, adds the vanilla deck-movement SFX, and uses a very weak short screen shake as a small confirmation cue.
- One-use Ancient reward rerolls should also give a small confirmation cue. `SpirePlusFeedback.ConfirmChoiceRefresh()` uses the same relic-activation SFX plus a very weak short screen shake after Urda, Morvi, or Lotha replaces its initial reward options. It does not change choice history, RNG rules, or reward contents.
- Relic payoff feedback should sit beside, not replace, the underlying command feedback. Core `CreatureCmd.Heal(...)` already plays the heal SFX/VFX path, so Urda Elite Root uses `SpirePlusFeedback.ConfirmRelicPayoff(...)` only to flash/play the source relic cue and add a very weak shake before the 10 HP elite-victory heal resolves.
- `CardSelectCmd.FromChooseACardScreen(PlayerChoiceContext, IReadOnlyList<CardModel>, Player, bool canSkip)` accepts one card and a skip option. This supports `Crossbow` and `ToastyMittens` lightweight accept/skip prompts.
- `AbstractModel.BeforeSideTurnStart(PlayerChoiceContext, CombatSide, IReadOnlyList<Creature>, ICombatState)` is inherited by `Crossbow`, while `Crossbow.AfterSideTurnStart(...)` contains the vanilla unconditional generated-attack add. The finish batch patches the inherited before-turn hook for the offer and no-ops the vanilla after-turn add.
- `Fiddle.ModifyHandDrawLate(...)` and `Fiddle.ShouldDraw(...)` are narrow relic hooks for replacing the vanilla draw-to-7 behavior and removing the old blanket draw prevention. `CardPileCmd.Draw(...)` is the command-level draw path; prefixing it allows a player-turn non-hand-draw cap of 7 while leaving draw effects callable.
- `JeweledMask.BeforeHandDraw(...)` is the combat-start pull hook. A custom `CustomEnchantmentModel` can persist on a selected power through the game's `SerializableEnchantment` path, and `ModelDb.AllAbstractModelSubtypes` includes mod `AbstractModel` subtypes discovered by reflection.
- `CardCmd.Enchant<T>(CardModel, decimal)` is save-compatible for persistent card markers but rejects already-enchanted cards. The implementation filters Jeweled Mask pickup choices to unenchanted powers rather than replacing existing enchantments.
- `ChoicesParadox.AfterPlayerTurnStart(...)` already uses `CardFactory.GetDistinctForCombat(...)`, `CardCmd.ApplyKeyword(..., Retain)`, `CardSelectCmd.FromSimpleGrid(...)`, and `CardPileCmd.AddGeneratedCardToCombat(...)`. The finish batch reuses that flow with a rare-card filter over all character pools plus colorless.
- `ToastyMittens.BeforeHandDraw(...)` is the local API name for the source-design "Baking Gloves" behavior. It already uses top-of-draw-pile exhaust plus `PowerCmd.Apply<StrengthPower>(...)`, so the safe rework is to insert the one-card skip prompt before exhausting.
- `MeatCleaver.TryModifyRestSiteOptions(...)` adds the built-in `CookRestSiteOption`; player-facing text is overridden to `Cleaver` / `切肉`. `CookRestSiteOption.OnSelect()` is patchable, and `CreatureCmd.SetCurrentHp(...)` is the available command API for a current-HP loss that fires the current-HP hook. `RestSiteOption.Owner` is protected, so the patch reads it through Harmony reflection.
- `LocManager` loads localization files as flat `Dictionary<string,string>` tables and merges mod `localization/<language>/<table>.json` files into matching base tables. This supports the active resource pass in `EZMicroBalance/localization/eng/*.json` and `EZMicroBalance/localization/zhs/*.json`.
- `LocManager` maps Simplified Chinese Weblate language `zh_Hans` to game language folder `zhs`; `Languages` includes `eng` and `zhs`, and `CultureInfoFromThreeLetterCode("zhs")` maps to `zh-hans`.

Implemented in finish batch:

- `SereTalon`: source behavior added two random Curses and 3 `Wish`; Spire Plus patches pickup to offer four Curses, add the selected Curse, add 2 `Wish`, and add 1 upgraded `Wish+`. Do not patch `Claws`; that relic belongs to Tanx and transforms cards into `Maul`.
- `Crossbow`: each owner turn offers one generated attack; accept adds it to hand with temporary cost -1, `Ethereal`, and `Exhaust`; skip removes the generated combat card.
- `Fiddle`: start-of-turn hand draw targets 7, vanilla blanket draw prevention is disabled, and non-hand-draw player-turn draw is capped at the remaining room up to 7.
- `JeweledMask`: on pickup, select an unenchanted deck power or draft one generated character power; enchant it with a persistent custom marker that permanently sets energy cost to 0; combat start pulls the marked power from draw pile to hand.
- `ChoicesParadox`: on combat round 1, offers five generated usable rare cards from all character card pools plus colorless, applies `Retain`, and adds the chosen generated card to hand only for the combat.
- `ToastyMittens` / source-design Baking Gloves: before hand draw, offers the top draw-pile card; accepting exhausts it and grants 1 Strength, skipping keeps it.
- `MeatCleaver`: built-in Cook rest option is shown as `Cleaver` / `切肉`, removes exactly two cards, and loses 5 current HP instead of gaining max HP; it is disabled when the player has fewer than two removable cards or current HP is not above 5.
- `Folly`: canonical keywords are now `Unplayable`, `Innate`, and `Eternal`; `Ethereal` is removed globally for newly created/loaded canonical Folly instances.
- `Debt`: end-of-turn in-hand gold loss is disabled; only `CardCmd.Exhaust(...)` loses up to 5 gold.
- Localization/resources: flat English table overrides were added for changed relic/card/rest-site text plus the custom Jeweled Mask enchantment localization through `ILocalizationProvider`.

## Remaining Blocker Finish Evidence

Timestamp: 2026-05-05 18:10:54 +02:00.

Additional local APIs inspected:

- BaseLib `SavedSpireField<TKey,TVal>` subclasses `SpireField<TKey,TVal>` and exports/imports through `SavedProperties`. `SavedSpireFieldPatch` patches `SavedProperties.FromInternal(...)` and `FillInternal(...)`, and BaseLib post-mod initialization registers static `SavedSpireField<,>` fields before saved-property net IDs are finalized. Supported saved value types include `int` and `List<SerializableCard>`, so save-backed counters on vanilla relic instances are safe here.
- `CardReward` stores reward slots as `List<CardCreationResult>`, calls `CardFactory.CreateForReward(...)`, then applies `Hook.TryModifyCardRewardOptions(...)`. `CardCreationResult.ModifyCard(CardModel, RelicModel)` replaces a single populated reward result while recording the modifying relic.
- `CardFactory.CreateForReward(Player, int, CardCreationOptions)` sets `CardCreationFlags.IsCardReward` for reward cards and calls `Hook.TryModifyCardRewardOptions(...)` after all reward cards are created. `CardCreationOptions.ForRoom(...)` marks Monster/Elite/Boss as `CardCreationSource.Encounter`; only normal monster rewards use `CardRarityOddsType.RegularEncounter`.
- `CardCreationFlags` includes `NoCardPoolModifications`, `NoCardModelModifications`, `NoModifyHooks`, and `IsCardReward`. The Prismatic Gem patch skips no-pool/no-model modification rewards, custom pools, filtered pools, colorless pools, non-encounter sources, and non-regular encounter odds.
- Vanilla `PrismaticGem.ModifyCardRewardCreationOptions(...)` broadens the whole card pool before card creation. The v4.3 rework no-ops that method and inserts screen-scoped all-slot replacement inside `Hook.TryModifyCardRewardOptions(...)` after all early reward modifiers and before late reward modifiers. Early slot adders such as `LastingCandy` appear before Prismatic replacement; late modifiers such as Eggs and reward enchantment relics apply to the final off-color cards.
- Vanilla `VelvetChoker.ShouldPlay(...)` enforces the hard six-card cap. The retained v4.2 patch no-ops that cap, uses a `CardEnergyCost.GetWithModifiers(...)` postfix so the +1 soft-limit tax is applied after local and global cost changes, and wraps X-cost resource spending so the extra energy does not increase the captured X value.
- Vanilla `DistinguishedCape.AfterObtained()` loses a fixed 9 max HP through `CreatureCmd.LoseMaxHp(...)` and adds three `Apparition` cards. The v4.3 patch computes `max(ceil(currentMaxHp * 0.30), 18)`, patches `Vakuu.GenerateInitialOptions(...)` so an unaffordable Cape roll is replaced by a payable Pool 2 option instead of shrinking Vakuu's three visible options, keeps a localized locked Cape fallback if no replacement exists, lowers current HP with `CreatureCmd.SetCurrentHp(...)` before max-HP loss when needed, then calls `CreatureCmd.LoseMaxHp(...)` without routing the cost through damage.
- `RelicModel.HoverTips` can be patched safely to append a count hover tip for Prismatic Gem. `NCardRewardSelectionScreen.RefreshOptions(...)` receives the visible `IReadOnlyList<CardCreationResult>` and owns a private `_banner`; the private `_banner` field type is runtime-guarded against the installed game API, the implementation falls back to the public `UI/Banner` node lookup when the field is missing, wrong-typed, null, or throws, and one-time diagnostics record the active path. Runtime visual placement still requires manual gameplay verification.
- Vanilla `PaelsTooth` already has `[SavedProperty] public List<SerializableCard> SerializableCards`, removes 5 upgradable cards on pickup through `CardSelectCmd.FromDeckForRemoval(...)` and `CardPileCmd.RemoveFromDeck(...)`, and randomly returns one saved card after every combat. The rework preserves the saved removed-card list and replaces only combat-end return behavior.
- `CombatRoom.RoomType` exposes `RoomType.Boss`, which supports clearing Pael's Tooth's remaining stored cards after act boss combat. `AbstractModel.AfterActEntered()` is also available and is patched as a defensive act-transition clear if any stored cards remain.
- `CardSelectCmd.FromChooseABundleScreen(Player, IReadOnlyList<IReadOnlyList<CardModel>>)` supports Pael's Tooth's no-context post-combat choice among stored one-card bundles.
- Local Tezcatara option pools contain `VeryHotCocoa`, `YummyCookie`, `BiiigHug`, `Storybook`, `SealOfGold`, `ToastyMittens`, `GoldenCompass`, `PumpkinCandle`, `ToyBox`, and `NutritiousSoup`; no direct relic/card named `QualityBlade` exists. `RefineBlade` is a permanent common skill that calls `ForgeCmd.Forge(...)`. `ForgeCmd.Forge(...)` creates generated token attack `SovereignBlade`, sets `SovereignBlade.CreatedThroughForge = true`, and adds it to combat hand with `CardPileCmd.AddGeneratedCardToCombat(...)`.

Implemented after blocker finish:

- `PrismaticGem`: save-backed standard card reward counter using `SavedSpireField<PrismaticGem,int>`; Every second standard card reward contains only off-color cards, preserving each slot's original type and rarity when available. Fallbacks relax rarity first, then type, then both before failing. If no replacement set can be built, the saved counter is restored to its pre-trigger value. The vanilla pool broadening is skipped.
- `VelvetChoker`: no hard six-card cap; every player turn counts non-autoplay first manual card-play series from hand, and the seventh and later from-hand plays cost +1 after other cost changes. X-cost cards require the extra energy without increasing captured X.
- `DistinguishedCape`: pickup uses `lose 30% of current Max HP, at least 18`; current max HP must be greater than the calculated cost before the trade can be selected. When the player cannot pay, an otherwise rolled Cape is replaced by a payable Vakuu Pool 2 option, with a locked localized Cape fallback if replacement ever fails. It then loses max HP and adds three `Apparition` cards. The current-HP clamp is not implemented as damage.
- `PaelsTooth`: save-backed non-boss combat counter using `SavedSpireField<PaelsTooth,int>`; pickup still uses the vanilla saved removed-card list. Every second non-boss combat offers the stored removed cards, returns the chosen card upgraded through command APIs, and removes that saved entry. Boss combat and act transition clear remaining saved cards.
- `Quality Blade` / name-TBD: resolved locally as generated `SovereignBlade` from `ForgeCmd.Forge(...)`, not permanent `RefineBlade`. Forged temporary `SovereignBlade` cards with `CreatedThroughForge` now gain `Exhaust`; permanent `RefineBlade` and non-forged copies are not altered.
- Chinese localization: Simplified Chinese `zhs` flat-table overrides were added for changed relics, cards, Prismatic Gem count/reward hints, and rest-site Cleaver / 切肉 UI. English relic text was also updated for `PrismaticGem`, `PaelsTooth`, `BloodSoakedRose`, `DistinguishedCape`, and `VelvetChoker`. v4.3 zhs player-facing text removes spaces between Chinese text, numbers, and units.

No source-design item remains deferred for lack of local compile-time API evidence after this pass.

## Prismatic Gem Reroll Fix Evidence

Timestamp: 2026-05-05 18:58:36 +02:00.

Additional local APIs inspected:

- `CardReward.Populate()` chooses `Options` before reroll and `RerollOptions` after `_hasBeenRerolled` is set. It returns early if `_cards` already exists, but `CardReward.Reroll()` clears `_cards` and calls `Populate()` again.
- `CardReward.Reroll()` records the old cards as skipped, sets `_hasBeenRerolled = true`, clears the same reward object's `_cards`, then repopulates the same `CardReward` instance.
- `CardFactory.CreateForReward(...)` calls `Hook.TryModifyCardRewardOptions(...)` every time `Populate()` creates a fresh card list, so a local reroll re-enters the Prismatic Gem `TryModifyCardRewardOptions` patch.

Chosen state strategy:

- Keep `SavedSpireField<PrismaticGem,int>` as the long-lived normal reward counter.
- Add a per-screen state keyed by the active `CardReward` instance with `ConditionalWeakTable<CardReward, RewardScreenState>`.
- Patch `CardReward.Populate()` only to expose the active reward screen through a thread-local stack while `CardFactory.CreateForReward(...)` and `Hook.TryModifyCardRewardOptions(...)` run.
- The first Prismatic Gem evaluation for a `CardReward` decides the screen: eligible normal rewards increment the saved counter once and store whether this screen should replace all slots; ineligible rewards store a non-trigger decision and do not increment.
- Rerolls reuse the same `CardReward` state. Trigger screens regenerate all-slot off-color replacements on every reroll; non-trigger screens stay non-trigger and do not increment the saved counter. Replacement runs after early reward modifiers and before late reward modifiers, so added slots are included and late modifiers apply to the final visible cards.
- If a Prismatic Gem reward modification runs without a `CardReward.Populate()` context, it does not increment or replace. This prevents non-screen card generation from consuming the saved reward counter.

Runtime-risk notes:

- `JeweledMaskFreePower` is compile-verified and uses BaseLib's custom model registration path, but it still needs manual runtime verification that BaseLib prefixes and registers the custom enchantment before a Jeweled Mask pickup save/load cycle.
- `Crossbow`, `ToastyMittens`, and `ChoicesParadox` use generated combat cards and selection screens; manual testing should verify skipped generated cards do not linger in combat state.
- `MeatCleaver` patches the built-in `CookRestSiteOption`; manual testing should verify no other source creates the rest-site Cleaver option without Meat Cleaver.
- `PrismaticGem` should be manually tested across two normal monster card rewards and a non-normal reward (elite, boss, event, or colorless-only) to confirm the saved counter only affects the intended reward type.
- `PaelsTooth` should be manually tested across pickup, one non-boss combat, two non-boss combats, and act boss completion to verify the saved counter, choice UI, upgraded return, and stored-card clear.

## v4.3 Validation Refresh

Timestamp: 2026-05-06.

v4.3 is current. The all-slot Prismatic Gem behavior is retained, with hook ordering hardened through the between-early-and-late Hook insertion. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.

No new public API surface was added beyond the hover/banner hooks documented above. The active v4.3 implementation uses the previously documented local APIs:

- `VelvetChoker.ShouldPlay`, `CardEnergyCost.GetWithModifiers`, `PlayerCombatState.HasEnoughResourcesFor`, `CardModel.SpendResources`, and the relic turn/combat hooks for the no-hard-cap soft-limit implementation.
- `Vakuu.GenerateInitialOptions`, same-pool replacement plus a locked `EventOption` fallback for unaffordable Cape choices, `DistinguishedCape.AfterObtained`, `CreatureCmd.SetCurrentHp`, and `CreatureCmd.LoseMaxHp` for pay-gated proportional max-HP loss without routing the cost through damage.
- `CardReward.Populate`, `CardReward.Reroll`, `Hook.TryModifyCardRewardOptions`, `CardCreationResult.ModifyCard`, and `SavedSpireField<PrismaticGem,int>` for screen-scoped Prismatic Gem all-slot replacement.
- `RelicModel.HoverTips`, `RelicModel.HoverTipsExcludingRelic`, and `NCardRewardSelectionScreen.RefreshOptions` for the Prismatic Gem count hover and reward-screen banner hint. The `_banner` contract is guarded by source and installed-API tests; runtime rejects missing, wrong-typed, null, detached, or throwing private-banner paths, falls back to the public `UI/Banner` node lookup plus log diagnostics, and treats visible all-off-color cards plus the relic hover count as fallback evidence if no banner can be updated. Runtime visual placement still requires manual gameplay verification.

The archived v4.2 next-plan file at `../../archive/feature-inputs/ancients-rework-v4/sts2_ancients_rework_v4_2_next_plan.md` is byte-for-byte identical to `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_2_next_plan.md` as of that preservation pass.
The v4.3 adjustment plan file at `reference-inputs/sts2_ancients_rework_v4_3_adjustment_plan.md` originated from `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_3_adjustment_plan.md` and now includes repository-local implementation-status annotations clarifying that the code is implemented while runtime/gameplay verification remains pending.

## Feedback / Juiciness Notes

Current rule for direct Ancient payoffs: use real game feedback primitives, not custom noisy overlays. Card additions should preview the exact cards, attach the source relic to the preview metadata, play the deck-movement cue, show the source relic pulse, and use a short weak screen shake. One-use Ancient reward rerolls and relic payoffs use the relic activation cue plus the same weak confirmation shake. This keeps player feedback legible without changing reward content, RNG, or save state.
