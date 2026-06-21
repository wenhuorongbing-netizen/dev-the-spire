# Canary Events - Source/API Proof Audit

Audited: 2026-06-11
Scope: 4 canary events registered in `Sts1EventRegistrationMode.CanaryOnly`

This file is source/API evidence only. It does not prove current `v0.107.1` encounter gameplay, save-load, EN/ZHS rendering, image/license status, replacement-pool behavior, multiplayer disposition, or full StS1 wiki parity. Previous beta.93 AdditiveBatch1 loader/registration proof covers these event types only as part of the RitsuLib-only batch; current CanaryOnly runtime/gameplay proof still requires fresh evidence.

Current canary source state:

- Big Fish and Golden Idol register to StS2 Act 1 buckets (`Overgrowth`, `Underdocks`) in source. Runtime bucket proof remains pending.
- Big Fish source/localization use the wiki-aligned Box option identity.
- Golden Idol source/localization use Take Idol followed by Outrun / Smash / Hide trap options.
- Golden Idol currently grants a random relic substitute before the trap branch because no Golden Idol relic model exists. Golden Idol relic parity remains pending.
- Divine Fountain has a curse prerequisite and Drink option identity guarded in source/localization.

## Summary

| Event | TODOs in Reachable Code | APIs Real | Loc Keys Present (EN) | Loc Keys Present (ZHS) | Dynamic Vars Aligned | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Big Fish | none | yes | yes | yes | yes (no placeholders) | SOURCE/API PASS; PARITY PENDING |
| Golden Idol | none | yes | yes | yes | yes | SOURCE/API PASS; PARITY PENDING |
| The Lab | none | yes | yes | yes | N/A (no vars) | SOURCE/API PASS; GAMEPLAY PENDING |
| Divine Fountain | none | yes | yes | yes | N/A (no vars) | SOURCE/API PASS; PARITY PENDING |

## Big Fish

- Source file: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1BigFish.cs`
- Model class: `Sts1BigFish`
- `IsShared`: true
- Availability: no event-specific `IsAllowed(IRunState)` override; source registration currently targets the StS2 Act 1 buckets (`Overgrowth`, `Underdocks`). Runtime bucket proof remains pending.
- Options:
  - Banana: heal for 1/3 max HP through `CreatureCmd.Heal(Owner.Creature, healAmount)`.
  - Donut: gain 5 max HP through `CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue)`.
  - Box: obtain 1 random relic plus Regret through `RelicFactory.PullNextRelicFromFront(Owner)`, `RelicCmd.Obtain(relic, Owner)`, and `CardPileCmd.AddCursesToDeck(ModelDb.Card<Regret>(), Owner)`.
- TODOs in reachable code: none.
- APIs used:
  - `CreatureCmd.Heal(creature, amount)`
  - `CreatureCmd.GainMaxHp(creature, amount)`
  - `RelicFactory.PullNextRelicFromFront(owner)`
  - `RelicCmd.Obtain(relic, owner)`
  - `CardPileCmd.AddCursesToDeck(cards, owner)`
  - `ModelDb.Card<Regret>()`
- Localization: all code-referenced EN and ZHS keys are present for `INITIAL`, `BANANA`, `DONUT`, and `BOX`.
- Dynamic variables:
  - `HealVar(0m)` is computed from `maxHp / 3m`; option text uses static "1/3" wording and does not require a placeholder.
  - `MaxHpVar(5m)` is constant; option text uses static "5 max HP" wording and does not require a placeholder.
- Source/API verdict: PASS for current substitute implementation. Runtime UI/result proof and full parity proof remain pending.

## Golden Idol

- Source file: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs`
- Model class: `Sts1GoldenIdol`
- `IsShared`: true
- Options:
  - Take Idol: obtains a random relic substitute through `RelicFactory.PullNextRelicFromFront(owner).ToMutable()` and `RelicCmd.Obtain(relic, owner)`, then transitions to the `TRAP` sub-page through `SetEventState()`.
  - Outrun: obtain Injury through `CardPileCmd.AddCursesToDeck(ModelDb.Card<Injury>(), Owner)`.
  - Smash: take `MaxHp * 0.25` unblockable damage, or `MaxHp * 0.35` at A15+, through `CreatureCmd.Damage(ctx, creature, damageVar, null)`.
  - Hide: lose `MaxHp * 0.08` max HP, or `MaxHp * 0.10` at A15+, through `CreatureCmd.LoseMaxHp(ctx, creature, maxHpLoss, isFromCard: false)`.
  - Leave: finishes the event.
- TODOs in reachable code: none.
- APIs used:
  - `RelicFactory.PullNextRelicFromFront(owner)`
  - `RelicCmd.Obtain(relic, owner)`
  - `CreatureCmd.Damage(ctx, creature, damageVar, card)`
  - `CreatureCmd.LoseMaxHp(ctx, creature, amount, isFromCard)`
  - `CardPileCmd.AddCursesToDeck(cards, owner)`
  - `ModelDb.Card<Injury>()`
  - `SetEventState(description, options)`
  - `EventOption.ThatDoesDamage(amount)`
  - `EventOption.ThatDecreasesMaxHp(amount)`
  - `StringHelper.Slugify(typeName)`
- Localization: all code-referenced EN and ZHS keys are present for `INITIAL`, `TRAP`, `OUTRUN`, `SMASH`, `HIDE`, and `LEAVE`.
- Dynamic variables:
  - `DamageVar(SmashDamagePctNormal * 100m, Unblockable)` defines the canonical damage var type. The actual Smash option display value is set through `.ThatDoesDamage(smashDamage)`, where `smashDamage = (int)(MaxHp * SmashDamagePct)`.
  - `MaxHpVar(0m)` defines the canonical max-HP var type. The actual Hide option display value is set through `.ThatDecreasesMaxHp(hideMaxHpLoss)`, where `hideMaxHpLoss = (int)(MaxHp * HideMaxHpPct)`.
  - EN and ZHS option descriptions include `{DamageAmount}` and `{MaxHpAmount}` placeholders for those option-level values.
  - A15 scaling uses `AscensionLevel >= 15` as the proxy for StS1 unfavorable-event behavior.
- Source/API verdict: PASS for the current random relic substitute and trap implementation. Runtime UI/result proof and Golden Idol relic parity remain pending.

## The Lab

- Source file: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1TheLab.cs`
- Model class: `Sts1TheLab`
- `IsShared`: true
- Options:
  - Open: obtain 3 random potions, or 2 at A15+, by calling `Sts1EventHelpers.GrantRandomPotion(Owner, Rng)` in a loop.
- TODOs in reachable code: none.
- APIs used:
  - `Sts1EventHelpers.GrantRandomPotion(owner, rng)`, implemented in `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventHelpers.cs`.
  - The helper calls `PotionFactory.CreateRandomPotionOutOfCombat(owner, rng)` and `PotionCmd.TryToProcure(potion, owner)`.
- Localization: all code-referenced EN and ZHS keys are present for `INITIAL` and `OPEN`.
- Dynamic variables: none.
- Source/API verdict: PASS for source/API coverage. Runtime UI/result proof remains pending.

## Divine Fountain

- Source file: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1DivineFountain.cs`
- Model class: `Sts1DivineFountain`
- `IsShared`: true
- Availability:
  - `IsAllowed(IRunState)` requires each player in the run to have at least one curse and requires `runState.Players.Count > 0`.
- Options:
  - Drink: remove all curses from the current owner deck by filtering `Owner.Deck.Cards` for `CardType.Curse` and calling `CardPileCmd.RemoveFromDeck(curses, showPreview: false)`.
  - Leave: null handler, finishing through base event behavior.
- TODOs in reachable code: none.
- APIs used:
  - `CardPileCmd.RemoveFromDeck(cards, showPreview: false)`
  - `card.Type == CardType.Curse`
  - `Owner.Deck.Cards`
  - `EventModel.IsAllowed(IRunState)`
- Localization: all code-referenced EN and ZHS keys are present for `INITIAL`, `DRINK`, and `LEAVE`.
- Dynamic variables: none.
- Source/API verdict: PASS for source/API coverage. Runtime event-selection and result proof remain pending.

## Cross-Cutting Observations

1. No TODO or BLOCKED comments appear in reachable source for the 4 canary event model files.
2. The canary implementations use real game/mod APIs already used elsewhere in the codebase: `CreatureCmd.Heal`, `CreatureCmd.GainMaxHp`, `CreatureCmd.Damage`, `CreatureCmd.LoseMaxHp`, `RelicCmd.Obtain`, `RelicFactory.PullNextRelicFromFront`, `CardPileCmd.AddCursesToDeck`, `CardPileCmd.RemoveFromDeck`, `ModelDb.Card<T>()`, `SetEventState`, and `SetEventFinished`.
3. `Sts1EventHelpers.GrantRandomPotion` is implemented with real potion factory/command calls.
4. All code-referenced localization keys for these 4 events exist in both `EZMicroBalance/localization/eng/sts1_events.json` and `EZMicroBalance/localization/zhs/sts1_events.json`.
5. Golden Idol uses `{DamageAmount}` and `{MaxHpAmount}` placeholders that align with option-level `ThatDoesDamage` and `ThatDecreasesMaxHp` overrides.
6. All 4 event types are registered in `Sts1EventRegistrationService.RegisterCanaryOnly()`: Big Fish and Golden Idol use Act 1 bucket registrations, while The Lab and Divine Fountain remain shared registrations.
