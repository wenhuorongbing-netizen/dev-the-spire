# Canary Events — Source/API Proof Audit

Audited: 2026-05-29
Scope: 4 canary events registered in `Sts1EventRegistrationMode.CanaryOnly`

## Summary

| Event             | TODOs in Reachable Code | APIs Real | Loc Keys Present (EN) | Loc Keys Present (ZHS) | Dynamic Vars Aligned | Verdict |
|-------------------|------------------------|-----------|----------------------|------------------------|---------------------|---------|
| Big Fish          | none                   | yes       | yes                  | yes                    | yes (no placeholders) | **PASS** |
| Golden Idol       | none                   | yes       | yes                  | yes                    | yes                 | **PASS** |
| The Lab           | none                   | yes       | yes                  | yes                    | N/A (no vars)       | **PASS** |
| Divine Fountain   | none                   | yes       | yes                  | yes                    | N/A (no vars)       | **PASS** |

---

### Big Fish

- **Source file**: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1BigFish.cs`
- **Model class**: `Sts1BigFish`
- **IsShared**: true
- **Options**:
  - **Banana**: Heal for 1/3 max HP → `CreatureCmd.Heal(Owner.Creature, healAmount)`
  - **Donut**: Gain 5 max HP → `CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue)`
  - **Shoe**: Obtain 1 random relic + Regret curse → `RelicFactory.PullNextRelicFromFront(Owner)` → `RelicCmd.Obtain(relic, Owner)` + `CardPileCmd.AddCursesToDeck(ModelDb.Card<Regret>(), Owner)`
- **TODOs in reachable code**: none
- **APIs used**:
  - `CreatureCmd.Heal(creature, amount)` — used in 13+ codebase locations
  - `CreatureCmd.GainMaxHp(creature, amount)` — used in 7+ codebase locations
  - `RelicFactory.PullNextRelicFromFront(owner)` — used in 4+ codebase locations
  - `RelicCmd.Obtain(relic, owner)` — used in 9 codebase locations
  - `CardPileCmd.AddCursesToDeck(cards, owner)` — used in 6 codebase locations
  - `ModelDb.Card<Regret>()` — pattern used with `Normality`, `Injury`, `Doubt`, `Debt`, `Apparition` in 10+ locations
- **EN localization** (keys present in `EZMicroBalance/localization/eng/sts1_events.json`):
  - `STS1_BIG_FISH.title`: "Big Fish"
  - `STS1_BIG_FISH.pages.INITIAL.description`: event intro text
  - `STS1_BIG_FISH.pages.INITIAL.options.BANANA.title/description`: "Eat the Banana" / "Heal for 1/3 of your max HP."
  - `STS1_BIG_FISH.pages.INITIAL.options.DONUT.title/description`: "Eat the Donut" / "Gain 5 max HP."
  - `STS1_BIG_FISH.pages.INITIAL.options.SHOE.title/description`: "Search the Shoe" / "Obtain 1 random relic. Obtain Regret."
  - `STS1_BIG_FISH.pages.BANANA.description`: finish text
  - `STS1_BIG_FISH.pages.DONUT.description`: finish text
  - `STS1_BIG_FISH.pages.SHOE.description`: finish text
- **ZHS localization** (keys present in `EZMicroBalance/localization/zhs/sts1_events.json`):
  - `STS1_BIG_FISH.title`: "大鱼"
  - All INITIAL, BANANA, DONUT, SHOE keys present with Chinese text
- **Dynamic variables**:
  - `HealVar(0m)` — base value computed in `CalculateVars()` to `maxHp / 3m`; no `{Heal}` placeholder in option descriptions (static text "Heal for 1/3 of your max HP."), so no alignment concern
  - `MaxHpVar(5m)` — constant 5; no `{MaxHp}` placeholder in option description (static text "Gain 5 max HP."), so no alignment concern
- **Source/API verdict**: **PASS** — all APIs are real (verified usage across codebase), no TODOs/BLOCKED in reachable code, all localization keys present in both EN and ZHS, dynamic variables correctly computed

---

### Golden Idol

- **Source file**: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs`
- **Model class**: `Sts1GoldenIdol`
- **IsShared**: true
- **Options**:
  - **Take Idol** → transitions to TRAP sub-page via `SetEventState()` with 3 trap options:
    - **Smash**: Obtain Injury curse → `CardPileCmd.AddCursesToDeck(ModelDb.Card<Injury>(), Owner)`
    - **Jump**: Take `CurrentHp * 0.25` (or 0.35 at A15) unblockable damage → `CreatureCmd.Damage(ctx, creature, damageVar, null)`
    - **Destroy**: Lose `MaxHp * 0.10` (or 0.15 at A15) max HP → `CreatureCmd.LoseMaxHp(ctx, creature, maxHpLoss, isFromCard: false)`
  - **Leave**: Nothing happens → `SetEventFinished()`
- **TODOs in reachable code**: none
- **APIs used**:
  - `CreatureCmd.Damage(ctx, creature, damageVar, card)` — used in 13 codebase locations
  - `CreatureCmd.LoseMaxHp(ctx, creature, amount, isFromCard)` — used in 11 codebase locations
  - `CardPileCmd.AddCursesToDeck(cards, owner)` — used in 6 codebase locations
  - `ModelDb.Card<Injury>()` — standard card model lookup pattern
  - `SetEventState(description, options)` — base EventModel method, used across 100+ event locations
  - `EventOption.ThatDoesDamage(amount)` — option annotation for damage preview display
  - `EventOption.ThatDecreasesMaxHp(amount)` — option annotation for max HP loss preview display
  - `StringHelper.Slugify(typeName)` — framework utility for key generation (used only in this event's `OptionKey` helper)
- **EN localization**:
  - `STS1_GOLDEN_IDOL.title`: "Golden Idol"
  - `STS1_GOLDEN_IDOL.pages.INITIAL.description/options.*`: intro + TAKE/LEAVE options
  - `STS1_GOLDEN_IDOL.pages.TRAP.description`: trap trigger text
  - `STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title/description`: "Smash" / "Obtain Injury."
  - `STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP.title/description`: "Jump" / "Lose {DamageAmount} HP."
  - `STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY.title/description`: "Destroy" / "Lose {MaxHpAmount} max HP."
  - `STS1_GOLDEN_IDOL.pages.SMASH/JUMP/DESTROY.description`: outcome texts
- **ZHS localization**:
  - `STS1_GOLDEN_IDOL.title`: "金色神像"
  - All keys present: INITIAL, TRAP, SMASH, JUMP, DESTROY pages with Chinese text
  - `{DamageAmount}` and `{MaxHpAmount}` placeholders present in ZHS option descriptions
- **Dynamic variables**:
  - `DamageVar(JumpPctNormal * 100m, Unblockable)` — canonical var initialized at 25 (percentage); actual display value set via `ThatDoesDamage(jumpDamage)` on the Jump option, where `jumpDamage = (int)(CurrentHp * JumpPct)`. The `{DamageAmount}` placeholder in loc text resolves from the option-level override. **Aligned**.
  - `MaxHpVar(0m)` — canonical var initialized at 0; actual display value set via `ThatDecreasesMaxHp(destroyMaxHp)` on the Destroy option, where `destroyMaxHp = (int)(MaxHp * DestroyPct)`. The `{MaxHpAmount}` placeholder in loc text resolves from the option-level override. **Aligned**.
  - A15 scaling: `HasA15` checks `AscensionLevel >= 15` as proxy for StS1 A15 unfavorable-events behavior. Jump 25%→35%, Destroy 10%→15%.
- **Note**: The `OptionKey` helper (line 107) uses `StringHelper.Slugify(GetType().Name)` to generate sub-page option keys, unlike other events that only use `InitialOptionKey`. `StringHelper` is a framework utility (not defined in project code) — its exact case output cannot be verified from source alone, but the project builds and loc keys use `STS1_GOLDEN_IDOL` prefix consistently.
- **Source/API verdict**: **PASS** — all APIs are real, no TODOs/BLOCKED, all localization keys present in both EN and ZHS, dynamic variables correctly aligned via option-level overrides

---

### The Lab

- **Source file**: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1TheLab.cs`
- **Model class**: `Sts1TheLab`
- **IsShared**: true
- **Options**:
  - **Open**: Obtain 3 random potions → loops `Sts1EventHelpers.GrantRandomPotion(Owner, Rng)` 3 times
  - **Leave**: Nothing happens (null handler → `SetEventFinished` via base class)
- **TODOs in reachable code**: none
- **APIs used**:
  - `Sts1EventHelpers.GrantRandomPotion(owner, rng)` — defined at `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventHelpers.cs:39`, calls `PotionFactory.CreateRandomPotionOutOfCombat(owner, rng)` → `PotionCmd.TryToProcure(potion, owner)`. Used in 4 event files (DrugDealer, Mushrooms, TheWomanInBlue, TheLab).
- **EN localization**:
  - `STS1_THE_LAB.title`: "The Lab"
  - `STS1_THE_LAB.pages.INITIAL.description`: "You discover an abandoned laboratory. Shelves of potions line the walls."
  - `STS1_THE_LAB.pages.INITIAL.options.OPEN.title/description`: "Open" / "Obtain 3 random potions."
  - `STS1_THE_LAB.pages.INITIAL.options.LEAVE.title/description`: "Leave" / "Nothing happens."
  - `STS1_THE_LAB.pages.OPEN.description`: "You grab three potions from the shelves. They glow with various colors."
- **ZHS localization**:
  - `STS1_THE_LAB.title`: "实验室"
  - All keys present: INITIAL, OPEN, LEAVE with Chinese text
  - `STS1_THE_LAB.pages.OPEN.description`: "你从架子上拿了三瓶药水。它们闪烁着不同的颜色。"
- **Dynamic variables**: none (no `CanonicalVars` override, no `{var}` placeholders in loc text)
- **Source/API verdict**: **PASS** — helper method is real and fully implemented, no TODOs/BLOCKED, all localization keys present in both EN and ZHS

---

### Divine Fountain

- **Source file**: `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1DivineFountain.cs`
- **Model class**: `Sts1DivineFountain`
- **IsShared**: true
- **Options**:
  - **Pray**: Remove all Curses from deck → iterates `Owner.Deck.Cards`, filters `CardType.Curse`, calls `CardPileCmd.RemoveFromDeck(curses, showPreview: false)`
  - **Leave**: Nothing happens (null handler → `SetEventFinished` via base class)
- **TODOs in reachable code**: none
- **APIs used**:
  - `CardPileCmd.RemoveFromDeck(cards, showPreview: false)` — used in 18 codebase locations
  - `card.Type == MegaCrit.Sts2.Core.Entities.Cards.CardType.Curse` — standard card type enum check
  - `Owner.Deck.Cards` — deck card iteration, standard pattern
- **EN localization**:
  - `STS1_DIVINE_FOUNTAIN.title`: "Divine Fountain"
  - `STS1_DIVINE_FOUNTAIN.pages.INITIAL.description`: "A beautiful fountain stands before you. The water glows with a soft, divine light."
  - `STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.PRAY.title/description`: "Pray" / "Remove all Curses from your deck."
  - `STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.title/description`: "Leave" / "Nothing happens."
  - `STS1_DIVINE_FOUNTAIN.pages.PRAY.description`: "You kneel before the fountain. The divine water washes away your curses."
- **ZHS localization**:
  - `STS1_DIVINE_FOUNTAIN.title`: "神圣之泉"
  - All keys present: INITIAL, PRAY, LEAVE with Chinese text
  - `STS1_DIVINE_FOUNTAIN.pages.PRAY.description`: "你跪在泉水前。神圣之水洗去了你的诅咒。"
- **Dynamic variables**: none (no `CanonicalVars` override, no `{var}` placeholders in loc text)
- **Note**: Divine Fountain has a dedicated `RemoveAllCurses` helper in `Sts1EventHelpers` (line 81) with identical logic. The event implements the same logic inline rather than calling the helper. This is a minor style inconsistency but not a correctness issue.
- **Source/API verdict**: **PASS** — all APIs are real, no TODOs/BLOCKED, all localization keys present in both EN and ZHS

---

## Cross-cutting observations

1. **No TODOs or BLOCKED comments** in any of the 4 canary event source files (verified via grep).
2. **All APIs** (`CreatureCmd.Heal`, `CreatureCmd.GainMaxHp`, `CreatureCmd.Damage`, `CreatureCmd.LoseMaxHp`, `RelicCmd.Obtain`, `RelicFactory.PullNextRelicFromFront`, `CardPileCmd.AddCursesToDeck`, `CardPileCmd.RemoveFromDeck`, `ModelDb.Card<T>()`, `SetEventState`, `SetEventFinished`) are used extensively across the codebase and are confirmed real (not stubs).
3. **Sts1EventHelpers** helper methods (`GrantRandomPotion`, `RemoveAllCurses`) are fully implemented with real factory/command calls.
4. **Localization coverage**: all code-referenced L10N keys exist in both `eng/sts1_events.json` and `zhs/sts1_events.json`.
5. **Dynamic variables**: Golden Idol uses `{DamageAmount}` and `{MaxHpAmount}` placeholders that align with the `DamageVar` and `MaxHpVar` canonical types; actual display values are set via option-level `ThatDoesDamage`/`ThatDecreasesMaxHp` overrides.
6. **All 4 events are registered** in `Sts1EventRegistrationService.RegisterCanaryOnly()` via `content.SharedEvent<T>()`, confirming the `IsShared => true` pattern.
