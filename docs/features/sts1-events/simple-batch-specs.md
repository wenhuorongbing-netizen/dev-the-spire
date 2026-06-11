# O17: Simple Batch Event Exact Specs

Date: 2026-05-29
Gate: 6 simple events with exact specs

## Summary

| # | Event | Model Exists? | File | Implementation Status |
|---|---|---|---|---|
| 1 | Purifier | YES | `Models/Shared/Sts1Purifier.cs` | Implemented; runtime unverified |
| 2 | Upgrade Shrine | YES | `Models/Act3/Sts1UpgradeShrine.cs` | Implemented |
| 3 | Golden Shrine | YES | `Models/Shared/Sts1GoldenShrine.cs` | Implemented; runtime unverified |
| 4 | The Cleric | YES | `Models/Shared/Sts1TheCleric.cs` | Implemented |
| 5 | Old Beggar | YES | `Models/Shared/Sts1OldBeggar.cs` | Implemented |
| 6 | Shining Light | YES | `Models/Act1/Sts1ShiningLight.cs` | Implemented |

All 6 simple-batch models exist and are in the AdditiveBatch1 source scope. Runtime gameplay, EN/ZHS render, image render/license, and save/load proof remain unverified.

AdditiveBatch1 exact source scope is 10 event types total (4 canary + these 6 simple events) through 14 registration calls because Big Fish, Golden Idol, The Cleric, and Shining Light register to both Overgrowth and Underdocks.

---

## 1. Purifier

**StS1 Behavior**: Free card removal event. Player chooses a card to remove from their deck. No cost.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `true` - shared-act event vote, with per-player card selection after the shared choice

**Options**:
- **Purify**: Open card removal UI -> remove 1 card from deck. Free, no gold cost.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1Purifier.cs`):
- `Purify()`: calls `Sts1EventHelpers.OpenCardRemoval(Owner)`.
- Leave: null handler.

**APIs Used**:
- `Sts1EventHelpers.OpenCardRemoval(Owner)` - opens `CardSelectCmd.FromDeckForRemoval`.

**DynamicVars**: None

**A15 Behavior**: None (no A15 variant in StS1)

**Parity Notes**: Source implementation uses existing APIs; runtime proof remains pending.

---

## 2. Upgrade Shrine

**StS1 Behavior**: Player chooses a card to upgrade.

**Wiki Classification**: Simple

**Act Bucket**: Glory (Act 3) - registered as `ActEvent<Glory, Sts1UpgradeShrine>`

**IsShared**: `false` (default) - card selection is per-player

**Options**:
- **Pray**: Open card upgrade UI -> upgrade 1 card.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Act3/Sts1UpgradeShrine.cs`):
- `Pray()`: calls `Sts1EventHelpers.OpenCardUpgrade(Owner)`.
- Leave: null handler.

**APIs Used**: `Sts1EventHelpers.OpenCardUpgrade(Owner)` - source verified.

**DynamicVars**: None

**A15 Behavior**: None

**Parity Notes**: Exact StS1 parity. Runtime proof remains pending.

---

## 3. Golden Shrine

**StS1 Behavior**: Three options - Pray for gold, Desecrate for more gold plus Regret, or Leave.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `true` - shared-act event vote, with per-player gold gain/curse application after the shared choice

**Options**:
- **Pray**: Gain 100 gold. (StS1 A15+: 50 gold)
- **Desecrate**: Gain 275 gold and obtain Regret. Always available.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1GoldenShrine.cs`):
- `Pray()`: calls `PlayerCmd.GainGold(PrayGoldAmount, Owner)`.
- `Desecrate()`: calls `PlayerCmd.GainGold(275, Owner)` and `CardPileCmd.AddCursesToDeck(new[] { ModelDb.Card<Regret>() }, Owner)`.
- Leave: null handler.

**APIs Used**:
- `PlayerCmd.GainGold(amount, Owner)` - for gold gain.
- `CardPileCmd.AddCursesToDeck(...)` - adds Regret to the deck.

**DynamicVars**: `GoldVar(100)` for the Pray branch.

**A15 Behavior**: Pray gold reduced from 100 to 50. Desecrate remains 275 gold plus Regret.

**Parity Notes**: Source/localization are now aligned to the StS1 option shape and guarded by `GoldenShrineUsesWikiGoldAndRegretOptions`; runtime UI/result/save-load proof remains pending.

---

## 4. The Cleric

**StS1 Behavior**: Pay gold for healing or card removal. The event requires at least 35 gold to appear.

**Wiki Classification**: Simple

**Act Bucket**: Overgrowth + Underdocks (StS1 Act 1)

**IsShared**: `true` - Act 1 event with shared co-op vote behavior and per-player gold/card operations after the shared choice

**Options**:
- **Heal**: Pay 35 gold -> heal 25% of max HP.
- **Purify**: Pay 50 gold -> remove 1 card from deck. (StS1 A15+: 75 gold)
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1TheCleric.cs`):
- `IsAllowed(IRunState)`: requires every player to have at least 35 gold before the shared event can appear.
- `Heal()`: `PlayerCmd.LoseGold(35, Owner, GoldLossType.Spent)` + `CreatureCmd.Heal(Owner.Creature, 25% maxHP)`.
- `Purify()`: `PlayerCmd.LoseGold(PurifyCost, Owner, GoldLossType.Spent)` + `Sts1EventHelpers.OpenCardRemoval(Owner)`.
- Leave: null handler.

**APIs Used**: `PlayerCmd.LoseGold`, `CreatureCmd.Heal`, `Sts1EventHelpers.OpenCardRemoval` - source verified.

**DynamicVars**: `GoldVar(35)`, `HealVar(0m)` computed as 25% max HP

**A15 Behavior**: Purify cost increases from 50 to 75. Heal remains 35.

**Parity Notes**: Source/localization now guard the A15 Purify cost, 35+ gold event eligibility, and Act 1 bucket registration; runtime UI/result/bucket/save-load proof remains pending.

---

## 5. Old Beggar (Pleading Vagrant)

**StS1 Behavior**: Pay 75 gold to remove a card from your deck.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `true` - shared-act event vote, with per-player gold/card operations after the shared choice

**Options**:
- **Offer Gold**: Pay 75 gold -> remove 1 card from deck. Disabled when the player has fewer than 75 gold.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1OldBeggar.cs`):
- `GenerateInitialOptions()`: gates Offer Gold with `(Owner?.Gold ?? 0) >= GoldCost` so underfunded players cannot buy card removal for less than 75 gold.
- `OfferGold()`: `PlayerCmd.LoseGold(75, Owner, GoldLossType.Spent)` + `Sts1EventHelpers.OpenCardRemoval(Owner)`.
- Leave: null handler.

**APIs Used**: `PlayerCmd.LoseGold`, `Sts1EventHelpers.OpenCardRemoval` - source verified.

**DynamicVars**: `GoldVar(75)`

**A15 Behavior**: None (cost unchanged in StS1 A15)

**Parity Notes**: Source now gates the paid removal option; runtime proof remains pending.

---

## 6. Shining Light

**StS1 Behavior**: Take damage equal to 30% of max HP (40% at A15) to upgrade 2 random cards.

**Wiki Classification**: Simple

**Act Bucket**: Overgrowth + Underdocks (StS1 Act 1)

**IsShared**: `false` (default) - damage and upgrades are per-player

**Options**:
- **Enter**: Take 30%/40% max HP unblockable damage -> upgrade 2 random cards.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Act1/Sts1ShiningLight.cs`):
- `Enter()`: `CreatureCmd.Damage(...)` with `Unblockable | Unpowered` + `Sts1EventHelpers.UpgradeRandomCards(Owner, Rng, count: 2)`.
- A15: `DamagePct` switches from 0.30 to 0.40.
- Leave: null handler.

**APIs Used**: `CreatureCmd.Damage`, `Sts1EventHelpers.UpgradeRandomCards` - source verified; runtime proof pending.

**DynamicVars**: `DamageVar(30%)` (or 40% at A15)

**A15 Behavior**: Damage increased from 30% to 40% max HP.

**Parity Notes**: Exact StS1 parity. Runtime proof remains pending.

---

## Implementation Gap Summary

| Event | Gap | Effort |
|---|---|---|
| Purifier | Runtime gameplay/render/save-load proof missing | Owner/game launch |
| Golden Shrine | Runtime gameplay/render/save-load proof missing | Owner/game launch |
| Upgrade Shrine | Runtime gameplay/render/save-load proof missing | Owner/game launch |
| The Cleric | Runtime gameplay/render/save-load proof missing | Owner/game launch |
| Old Beggar | Runtime gameplay/render/save-load proof missing | Owner/game launch |
| Shining Light | Runtime gameplay/render/save-load proof missing | Owner/game launch |

## Registration Status

All six simple-batch events are included in `RegisterAdditiveBatch1()` for bounded prototype runtime smoke. Purifier, Golden Shrine, and Old Beggar are shared all-act events. The Cleric and Shining Light are registered in both StS2 Act 1 buckets. Upgrade Shrine is registered in Act 3.
