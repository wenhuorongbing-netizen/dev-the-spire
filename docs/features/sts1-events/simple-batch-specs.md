# O17: Simple Batch Event Exact Specs

Date: 2026-05-29
Gate: 6 simple events with exact specs

## Summary

| # | Event | Model Exists? | File | Implementation Status |
|---|---|---|---|---|
| 1 | Purifier | **NO** | — | Needs creation |
| 2 | Upgrade Shrine | YES | `Models/Act3/Sts1UpgradeShrine.cs` | Implemented |
| 3 | Golden Shrine | **NO** | — | Needs creation |
| 4 | The Cleric | YES | `Models/Shared/Sts1TheCleric.cs` | Implemented |
| 5 | Old Beggar | YES | `Models/Shared/Sts1OldBeggar.cs` | Implemented |
| 6 | Shining Light | YES | `Models/Act1/Sts1ShiningLight.cs` | Implemented |

4 of 6 already implemented. 2 need creation: Purifier, Golden Shrine.

---

## 1. Purifier

**StS1 Behavior**: Free card removal event. Player chooses a card to remove from their deck. No cost.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `false` — card selection is per-player

**Options**:
- **Purify**: Open card removal UI → remove 1 card from deck. Free, no gold cost.
- **Leave**: Do nothing.

**APIs Needed**:
- `Sts1EventHelpers.OpenCardRemoval(Owner)` — already exists, opens `CardSelectCmd.FromDeckForRemoval`

**DynamicVars**: None

**A15 Behavior**: None (no A15 variant in StS1)

**Parity Notes**: Exact StS1 parity achievable with existing APIs.

---

## 2. Upgrade Shrine

**StS1 Behavior**: Player chooses a card to upgrade.

**Wiki Classification**: Simple

**Act Bucket**: Glory (Act 3) — registered as `ActEvent<Glory, Sts1UpgradeShrine>`

**IsShared**: `false` (default) — card selection is per-player

**Options**:
- **Pray**: Open card upgrade UI → upgrade 1 card.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Act3/Sts1UpgradeShrine.cs`):
- `Pray()`: calls `Sts1EventHelpers.OpenCardUpgrade(Owner)` ✓
- Leave: null handler ✓

**APIs Used**: `Sts1EventHelpers.OpenCardUpgrade(Owner)` — verified working

**DynamicVars**: None

**A15 Behavior**: None

**Parity Notes**: Exact StS1 parity. **Already implemented.**

---

## 3. Golden Shrine

**StS1 Behavior**: Two options — gain 250 gold, or remove all Curses from deck (if cursed).

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `false` — gold gain and curse removal are per-player

**Options**:
- **Take Gold**: Gain 250 gold. (StS1 value: 250g on non-A15, 100g on A15)
- **Desecrate**: Remove all Curses from deck. Only available if player has curses.
- **Leave**: Do nothing.

**APIs Needed**:
- `PlayerCmd.GainGold(amount, Owner)` — for gold gain
- `Sts1EventHelpers.RemoveAllCurses(Owner)` — already exists, removes all curses from deck

**DynamicVars**: `GoldVar(250)` (or 100 on A15)

**A15 Behavior**: Gold reduced from 250 to 100.

**Parity Notes**: Exact StS1 parity achievable with existing APIs. The "Desecrate" option should be conditionally shown only when the player has curses in deck.

---

## 4. The Cleric

**StS1 Behavior**: Pay gold for healing or card removal.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `false` — gold/card operations are per-player

**Options**:
- **Heal**: Pay 35 gold → heal 25% of max HP.
- **Purify**: Pay 50 gold → remove 1 card from deck.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1TheCleric.cs`):
- `Heal()`: `PlayerCmd.LoseGold(35, Owner, GoldLossType.Spent)` + `CreatureCmd.Heal(Owner.Creature, 25% maxHP)` ✓
- `Purify()`: `PlayerCmd.LoseGold(50, Owner, GoldLossType.Spent)` + `Sts1EventHelpers.OpenCardRemoval(Owner)` ✓
- Leave: null handler ✓

**APIs Used**: `PlayerCmd.LoseGold`, `CreatureCmd.Heal`, `Sts1EventHelpers.OpenCardRemoval` — all verified

**DynamicVars**: `GoldVar(35)`, `HealVar(0m)` computed as 25% max HP

**A15 Behavior**: None (costs unchanged in StS1 A15)

**Parity Notes**: Exact StS1 parity. **Already implemented.**

---

## 5. Old Beggar (Pleading Vagrant)

**StS1 Behavior**: Pay 75 gold to remove a card from your deck.

**Wiki Classification**: Simple

**Act Bucket**: Shared (all acts)

**IsShared**: `false` — gold/card operations are per-player

**Options**:
- **Offer Gold**: Pay 75 gold → remove 1 card from deck.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Shared/Sts1OldBeggar.cs`):
- `OfferGold()`: `PlayerCmd.LoseGold(75, Owner, GoldLossType.Spent)` + `Sts1EventHelpers.OpenCardRemoval(Owner)` ✓
- Leave: null handler ✓

**APIs Used**: `PlayerCmd.LoseGold`, `Sts1EventHelpers.OpenCardRemoval` — all verified

**DynamicVars**: `GoldVar(75)`

**A15 Behavior**: None (cost unchanged in StS1 A15)

**Parity Notes**: Exact StS1 parity. **Already implemented.**

---

## 6. Shining Light

**StS1 Behavior**: Take damage equal to 30% of max HP (40% at A15) to upgrade 2 random cards.

**Wiki Classification**: Simple

**Act Bucket**: Overgrowth + Underdocks (StS1 Act 1)

**IsShared**: `false` (default) — damage and upgrades are per-player

**Options**:
- **Enter**: Take 30%/40% max HP unblockable damage → upgrade 2 random cards.
- **Leave**: Do nothing.

**Current Implementation** (`Models/Act1/Sts1ShiningLight.cs`):
- `Enter()`: `CreatureCmd.Damage(...)` with `Unblockable | Unpowered` + `Sts1EventHelpers.OpenCardUpgrade(Owner, count: 2)` ✓
- A15: `DamagePct` switches from 0.30 to 0.40 ✓
- Leave: null handler ✓

**APIs Used**: `CreatureCmd.Damage`, `Sts1EventHelpers.OpenCardUpgrade` — all verified

**DynamicVars**: `DamageVar(30%)` (or 40% at A15)

**A15 Behavior**: Damage increased from 30% to 40% max HP.

**Parity Notes**: Exact StS1 parity. **Already implemented.**

---

## Implementation Gap Summary

| Event | Gap | Effort |
|---|---|---|
| Purifier | Model file does not exist | Low — use `OpenCardRemoval` |
| Golden Shrine | Model file does not exist | Low — use `GainGold` + `RemoveAllCurses` |
| Upgrade Shrine | None | Already done |
| The Cleric | None | Already done |
| Old Beggar | None | Already done |
| Shining Light | None | Already done |

## Required New Files

1. `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1Purifier.cs`
2. `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenShrine.cs`

## Registration Changes Needed

Both new events are Shared, so they need:
- `content.SharedEvent<Sts1Purifier>()` in `RegisterAll()`
- `content.SharedEvent<Sts1GoldenShrine>()` in `RegisterAll()`
- Optionally add to `RegisterCanaryOnly()` if desired (not required for simple batch)
