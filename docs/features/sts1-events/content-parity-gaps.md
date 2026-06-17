# StS1 Events — Content Parity Gap Analysis

> **Generated:** 2026-05-29
> **Source:** Historical 46-model gap audit plus `Sts1EventHelpers.cs`. Current count reconciliation is `48` model files / `47` compiling models in `registry-reconciliation.md`; this table is a dependency backlog, not current completeness proof.
> **Scope:** Identifies game-object dependencies that prevent full StS1 parity in event models.

## Classification Key

| Tag | Meaning |
|-----|---------|
| `native-equivalent` | StS2 has this game object; no gap |
| `temporary-substitute` | Workaround in place; not parity-accurate |
| `blocked` | Cannot implement with current StS2 APIs |
| `custom-required` | Needs a custom Spire Plus model |

---

## 1. Summary Table

| # | Event | Act | Status | Gap Type | Dependency |
|---|-------|-----|--------|----------|------------|
| 1 | BigFish | Act1 | **OK** | `native-equivalent` | Regret curse, random relic |
| 2 | GoldenIdol | Act1 | **GAP** | `temporary-substitute` | Golden Idol relic model/effect is missing; Take currently grants a random relic. Injury curse is native-equivalent. |
| 3 | LivingWall | Shared | **OK** | `native-equivalent` | Card removal/transform/upgrade |
| 4 | Duplicator | Shared | **GAP** | `blocked` | Compile-excluded; card duplication selection APIs not available |
| 5 | DivineFountain | Shared | **OK** | `native-equivalent` | Curse removal; curse prerequisite now source-guarded, runtime selection proof pending |
| 6 | FountainOfCleansing | Shared | **OK** | `native-equivalent` | Curse removal + max HP loss |
| 7 | TheCleric | Act1 | **OK** | `native-equivalent` | Gold, heal, card removal |
| 8 | OldBeggar | Shared | **OK** | `native-equivalent` | Gold, card removal |
| 9 | Designer | Shared | **OK** | `native-equivalent` | Card upgrade/remove/transform |
| 10 | TheMausoleum | Shared | **OK** | `native-equivalent` | Wound curse, random relic |
| 11 | WheelOfChange | Shared | **OK** | `native-equivalent` | Gold, damage, relic, Decay curse, heal, removal |
| 12 | **FaceTrader** | Shared | **GAP** | `temporary-substitute` | StS1 face relics → random relic |
| 13 | GoldenWing | Shared | **OK** | `native-equivalent` | Random rare card |
| 14 | BonfireSpirits | Shared | **OK** | `native-equivalent` | Card removal, full heal |
| 15 | TheWomanInBlue | Shared | **OK** | `native-equivalent` | Gold, random potions |
| 16 | TheLab | Shared | **OK** | `native-equivalent` | Random potions |
| 17 | TreasureOoze | Act1 | **GAP** | `blocked` | Combat encounter (large slime) |
| 18 | ScorpionNest | Act1 | **GAP** | `blocked` | Combat encounter (3 Louses) |
| 19 | DeadAdventurer | Act1 | **GAP** | `blocked` | Combat encounter (random elite) |
| 20 | Mushrooms | Act1 | **OK** | `native-equivalent` | Max HP, random potion |
| 21 | Joust | Act1 | **OK** | `native-equivalent` | Gold gambling |
| 22 | ShiningLight | Act1 | **OK** | `native-equivalent` | Damage, card upgrade |
| 23 | TheSsssserpent | Act1 | **OK** | `native-equivalent` | Gold, Doubt curse |
| 24 | MaskedBandits | Act2 | **GAP** | `blocked` | Combat encounter (3 bandits) |
| 25 | TheLibrary | Act2 | **OK** | `native-equivalent` | Card selection grid |
| 26 | **Nloth** | Act2 | **GAP** | `blocked` | Relic selection UI (no API) |
| 27 | **Vampires** | Act2 | **GAP** | `custom-required` | Bite card (missing from StS2) |
| 28 | CursedTome | Act2 | **OK** | `native-equivalent` | Damage, random rare relic |
| 29 | KnowingSkull | Act2 | **OK** | `native-equivalent` | Damage, random rare card |
| 30 | TheGhost | Act2 | **OK** | `native-equivalent` | Random rare card |
| 31 | DrugDealer | Act2 | **OK** | `native-equivalent` | Gold, random potions |
| 32 | ForgottenAltar | Act2 | **OK** | `native-equivalent` | Max HP, Doubt curse, gold, relic |
| 33 | **Nest** | Act2 | **GAP** | `temporary-substitute` | Parasite curse → Clumsy |
| 34 | Altar | Act2 | **OK** | `native-equivalent` | Card upgrade/removal, relic |
| 35 | Augmenter | Act2 | **OK** | `native-equivalent` | Card transform/upgrade |
| 36 | CouncilOfGhosts | Act2 | **OK** | `native-equivalent` | Apparition card, max HP loss |
| 37 | AncientWriting | Act2 | **OK** | `native-equivalent` | Card upgrade/removal |
| 38 | **MindBloom** | Act3 | **GAP** | `blocked` (partial) | War option: Act 1 boss combat |
| 39 | MysteriousSphere | Act3 | **GAP** | `blocked` | Combat encounter (2 Orb Walkers) |
| 40 | TombOfLordRedMask | Act3 | **OK** | `native-equivalent` | Gold, random relic |
| 41 | SensoryStone | Act3 | **OK** | `native-equivalent` | Rare card selection |
| 42 | UpgradeShrine | Act3 | **OK** | `native-equivalent` | Card upgrade |
| 43 | Transmogrifier | Act3 | **OK** | `native-equivalent` | Card transform |
| 44 | Falling | Act3 | **OK** | `native-equivalent` | Card removal/damage/transform |
| 45 | MoaiHead | Act3 | **OK** | `native-equivalent` | Gold, max HP |
| 46 | **WindingHalls** | Act3 | **GAP** | `temporary-substitute` | Madness curse → Debt |
| 47 | Purifier | Shared | **OK** | `native-equivalent` | Free card removal |
| 48 | GoldenShrine | Shared | **OK** | `native-equivalent` | Gold, Regret curse |

**Dependency table totals:** 35 native-equivalent / 4 direct game-object substitutes / 7 blocked / 1 custom-required, plus Mind Bloom as a partial blocked row. Current release-gate blocker handling is authoritative in `status-board.md`, `registry-reconciliation.md`, and the static parity checker.

**Current release-gate non-parity rows:** `status-board.md` intentionally groups six non-parity rows under Temporary Substitutes: Golden Idol, Face Trader, Nest, Vampires, Mind Bloom, and Winding Halls. Vampires still needs a Bite card for parity; Mind Bloom still needs War combat proof. Do not call any of these parity-complete until their missing model or encounter proof is closed.

Static reproduction:

```powershell
.\scripts\check-sts1-event-parity-blockers.ps1 -FailOnMismatch
```

---

## 2. Per-Event Gap Analysis

### 2.0 GoldenIdol (Act 1) - `temporary-substitute`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs`

**StS1 behavior:** Take obtains the Golden Idol relic, then the trap branch offers Outrun, Smash, and Hide.

**Gap:** The trap branch source/localization now uses Outrun / Smash / Hide, with Smash dealing 25%/35% max HP as HP damage and Hide losing 8%/10% max HP. The Take branch still grants `RelicFactory.PullNextRelicFromFront(owner)` instead of a Golden Idol relic model/effect.

**Impact:** Medium-high. The event grants a generic relic instead of the named Golden Idol reward.

**Resolution path:** Create a source-safe `Sts1GoldenIdolRelic` only after the current StS2 gold-reward hook surface is proven. Do not add a marker-only relic and call it parity.

---

### 2.1 FaceTrader (Shared) — `temporary-substitute`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1FaceTrader.cs:43-44`

**StS1 behavior:** Trade 10% max HP (15% at A15) for one of five specific face relics (Face of Cleric, Face of Guardian, Face of Healer, Face of Navigator, Face of Soldier).

**Gap:** None of the five StS1 face relics exist in StS2. The event currently grants a random relic via `Sts1EventHelpers.GrantRandomRelic()`.

**Impact:** Player gets a generic relic instead of a thematically specific face relic. Power level is roughly comparable but not thematically faithful.

**Resolution path:** Could create 5 custom face relic models (`custom-required`), or accept the random relic substitute as "close enough" for private beta.

---

### 2.2 Vampires (Act 2) — `custom-required`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1Vampires.cs:43-45`

**StS1 behavior:** Remove all Strikes from deck, add 5 Bite cards, lose 30% max HP (40% at A15).

**Gap:** The Bite card does not exist in StS2. The event currently removes all Strikes and applies max HP loss, but **cannot add Bite cards**. The accept option is mechanically incomplete — the player loses Strikes with no replacement.

**Impact:** High. The core reward of the event (Bite cards that heal on hit) is entirely missing. The event is a pure penalty with no upside.

**Resolution path:** Requires a custom Bite card model. This is the single highest-priority `custom-required` item.

---

### 2.3 Nest (Act 2) — `temporary-substitute`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1Nest.cs:33`

**StS1 behavior:** Search for a relic but gain 2 Parasite curses (3 at A15). Parasite is a curse that, when removed, causes 3 max HP loss.

**Gap:** Parasite curse does not exist in StS2. Uses Clumsy as a substitute (unplayable 0-cost curse, retains on draw).

**Impact:** Medium. The "remove risk" mechanic of Parasite (losing max HP on removal) is lost. Clumsy is a different kind of downside (draws a dead card but no removal penalty).

**Resolution path:** Could create a custom Parasite card model (`custom-required`), or accept Clumsy as a thematic approximation.

---

### 2.4 WindingHalls (Act 3) — `temporary-substitute`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1WindingHalls.cs:40-44`

**StS1 behavior:** Embrace Madness option grants 2 Madness curses (3 at A15) + 5% max HP loss (10% at A15). Madness is a curse that costs 1 mana to play (exhausts).

**Gap:** Madness curse does not exist in StS2. Uses Debt as a substitute.

**Impact:** Low-medium. Debt is thematically different (costs gold on draw vs. costing mana to play) but serves a similar "junk curse" role. The mana cost mechanic of Madness is lost.

**Resolution path:** Could create a custom Madness card model (`custom-required`), or accept Debt as a thematic approximation.

---

### 2.5 N'loth (Act 2) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1Nloth.cs:11,27-28`

**StS1 behavior:** Give up one of your relics to receive a random relic.

**Gap:** No `RelicSelectCmd` API exists in StS2. The event cannot present a relic selection UI to the player. The Offer option is a no-op stub.

**Impact:** High. The entire event mechanic (sacrifice a relic for a new one) cannot function.

**Resolution path:** Blocked until StS2/BaseLib exposes a relic selection command, or a custom relic selection screen is implemented.

---

### 2.6 TreasureOoze (Act 1) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1TreasureOoze.cs:40`

**StS1 behavior:** Pay 50g for a relic, fight a large slime for relic + gold, or leave.

**Gap:** The Fight option has a TODO stub — no combat encounter integration. Offer (50g for relic) and Leave work.

**Impact:** Medium. One of three options is non-functional. The "free relic via combat" path is unavailable.

**Resolution path:** Blocked on combat encounter system (`EnterCombatWithoutExitingEvent`).

---

### 2.7 ScorpionNest (Act 1) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1ScorpionNest.cs:27`

**StS1 behavior:** Fight 3 Louses for a random relic, or leave.

**Gap:** The Investigate option has a TODO stub — no combat encounter. Leave works.

**Impact:** Medium. The entire "investigate" branch is non-functional. Event is effectively leave-only.

**Resolution path:** Blocked on combat encounter system.

---

### 2.8 DeadAdventurer (Act 1) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1DeadAdventurer.cs:40`

**StS1 behavior:** Search for gold/relic with a chance of fighting a random elite.

**Gap:** The elite combat branch has a TODO stub. Gold and relic search branches work.

**Impact:** Medium. The risk/reward element (elite fight chance) is missing. Searching always succeeds without combat risk.

**Resolution path:** Blocked on combat encounter system.

---

### 2.9 MaskedBandits (Act 2) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act2/Sts1MaskedBandits.cs:37`

**StS1 behavior:** Pay 75g to avoid combat, or fight 3 bandits for gold + relic.

**Gap:** The Fight option has a TODO stub — no combat encounter. Pay works.

**Impact:** Medium. The "fight for profit" path is unavailable. Event is effectively pay-or-nothing.

**Resolution path:** Blocked on combat encounter system.

---

### 2.10 MysteriousSphere (Act 3) — `blocked`

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MysteriousSphere.cs:27`

**StS1 behavior:** Fight 2 Orb Walkers for a random relic, or leave.

**Gap:** The Open option has a TODO stub — no combat encounter. Leave works.

**Impact:** Medium. The entire event is effectively leave-only.

**Resolution path:** Blocked on combat encounter system.

---

### 2.11 MindBloom (Act 3) — `blocked` (partial)

**Source:** `EZMicroBalanceCode/Sts1Events/Models/Act3/Sts1MindBloom.cs:40-41`

**StS1 behavior:** Three options — fight Act 1 boss, upgrade all cards, or gain 999g + Normality curses.

**Gap:** The War option (fight Act 1 boss) has a BLOCKED/TODO stub. Awake (upgrade all) and Rich (999g + Normality) work.

**Impact:** Low-medium. Two of three options work. The combat option is the most mechanically complex but the other two are fully functional.

**Resolution path:** Blocked on combat encounter system.

---

## 3. Temporary Substitutes (Red-Flagged)

These events are **functionally complete** but use **wrong game objects** as substitutes. Each is a parity inaccuracy that should be resolved before a parity-accurate release.

| Event | StS1 Object | StS2 Substitute | Parity Impact |
|-------|-------------|-----------------|---------------|
| **FaceTrader** | 5 face relics (Cleric/Guardian/Healer/Navigator/Soldier) | `GrantRandomRelic` | Thematic loss; power level roughly equivalent |
| **Nest** | Parasite curse (removal causes 3 max HP loss) | Clumsy curse (dead draw, no removal penalty) | Mechanical loss; removal-risk mechanic missing |
| **WindingHalls** | Madness curse (1 mana to exhaust) | Debt curse (gold cost on draw) | Mechanical loss; mana-cost mechanic missing |

**Common pattern:** All three substitute a thematically specific StS1 game object with a generic StS2 equivalent. The event flow works, but the specific risk/reward profile diverges from StS1.

---

## 4. Custom-Required List

These events need **new Spire Plus models** to achieve parity. No StS2 native equivalent exists.

| Event | Custom Model Needed | Complexity | Priority |
|-------|-------------------|------------|----------|
| **Vampires** | Bite card (0-cost attack, 6 damage, heals 2 HP) | Medium — new card model + art | **High** — event is mechanically incomplete without it |
| **Nest** | Parasite curse (unplayable, removal causes 3 max HP loss) | Low — curse card model | Low — Clumsy is an acceptable substitute |
| **WindingHalls** | Madness curse (1-cost, exhausts, no effect) | Low — curse card model | Low — Debt is an acceptable substitute |
| **FaceTrader** | 5 face relics (various passive effects) | High — 5 relic models + art | Low — random relic is acceptable for private beta |

**Recommendation for private beta:** Implement Bite card only. Accept Clumsy/Debt/random-relic substitutes for the other three. The combat encounter gaps (5 events + MindBloom War) are the larger parity blocker and should be prioritized after the Bite card.

---

## Appendix: Curse Card Dependency Map

| Curse | StS1 Behavior | StS2 Status | Used By |
|-------|--------------|-------------|---------|
| Regret | Unplayable; lose 1 HP per card played | `native-equivalent` | BigFish |
| Injury | Unplayable; no effect | `native-equivalent` | GoldenIdol |
| Doubt | Unplayable; gain 1 Weak per turn | `native-equivalent` | Ssssserpent, ForgottenAltar |
| Wound | Unplayable; no effect | `native-equivalent` | TheMausoleum |
| Decay | Unplayable; take 2 HP damage per turn | `native-equivalent` | WheelOfChange |
| Normality | Unplayable; can only play 3 cards per turn | `native-equivalent` | MindBloom |
| Clumsy | Unplayable; retain | `native-equivalent` (substitute for Parasite) | Nest |
| Debt | Unplayable; costs gold on draw | `native-equivalent` (substitute for Madness) | WindingHalls |
| Apparition | 1-cost skill; 1 Intangible, Ethereal, exhausts | `native-equivalent` | CouncilOfGhosts |
| **Parasite** | Unplayable; removal causes 3 max HP loss | **MISSING** | Nest (uses Clumsy) |
| **Madness** | 1-cost; exhausts, no effect | **MISSING** | WindingHalls (uses Debt) |
| **Bite** | 0-cost attack; 6 dmg, heal 2 HP | **MISSING** | Vampires (not added) |
