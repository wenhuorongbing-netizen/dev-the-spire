# Ancients Rework v4.3 Source Design

Status: v4.3 is current. This file is the compact active source-design summary for the implemented Spire Plus Ancient reward rebalance. The older corrupted long draft was preserved at `docs/archive/feature-inputs/ancients-rework-v4/source-design-mojibake-pre-slim-20260518.md`; the cleaner v4.3 adjustment input remains at `reference-inputs/sts2_ancients_rework_v4_3_adjustment_plan.md`.

This design is source-complete for automated test handoff. Runtime gameplay, reward-screen visuals, save/load, and co-op verification are still pending.

## Current Rules

| Reward | Current behavior | Runtime proof |
| --- | --- | --- |
| Velvet Choker | Gain 1 Energy. Cards can still be played after six manual from-hand plays. The 7th and later manual from-hand card plays each player turn cost 1 more after other cost changes. Copied, autoplayed, and repeated plays do not advance the counter. | Pending manual combat test. |
| Distinguished Cape | Lose 30% of current Max HP, at least 18, then add 3 Apparition cards. If the player cannot pay, Vakuu should keep three visible reward choices through same-pool Vakuu replacement; a locked `EventOption` fallback exists only as a defensive last resort. | Pending low-HP Vakuu option and pickup test. |
| Prismatic Gem | Gain 1 Energy. Every second standard card reward contains only off-color cards. The trigger is scoped to the current `CardReward` screen, so reroll keeps the same trigger state and does not advance the saved counter again. | Pending reward-screen and reroll test. |
| Jewelry Box | Adds a non-Innate Apotheosis and preserves that marker through save/load. | Pending live deck/save-load test. |
| Preserved Fog | Removes up to four cards and adds Folly without Ethereal or Retain. Folly exposes Unplayable, Eternal, and Innate. | Pending live deck test. |
| Vakuu's Sere Talon / 瓦库原初之爪 | Vakuu relic. Adds two random Curses and three Wish. | Pending live reward test. |
| Tanx Claws / 坦克斯利爪 | Tanx relic. Transforms selected cards into upgraded Maul+ / 撕咬+. | Pending live pickup test. |
| Choices Paradox | On the first combat turn, offers five rare temporary retained choices and removes unselected generated cards. | Pending combat test. |
| Jeweled Mask | Marks one Power card. It permanently costs 0 and starts in hand each combat. | Pending live draft/save-load/combat test. |
| Prismatic Gem count hint | Relic hover should show the saved reward count, such as `棱彩计数：0/2` or `棱彩计数：1/2`. | Pending hover visual test. |
| Prismatic Gem reward hint | A triggered reward screen should show `棱彩奖励：本次只出现异色牌。` when the banner can be reached. The private `_banner` field type is runtime-guarded, and `UI/Banner fallback` is used when the private field is unavailable. Runtime visual placement still requires manual gameplay verification. | Pending visual test. |

## Superseded Rules

- v4.2 rightmost-slot Prismatic Gem is historical only.
- v4.2 Distinguished Cape 40% min15 is historical only.
- Pumpkin Candle is restored to vanilla behavior and is no longer an active EZMB override.

## Implementation Evidence

- Reward patches live under `EZMicroBalanceCode/Ancients/Patches/`.
- Current implementation notes live in `api-discovery.md`, `implementation-plan.md`, `high-risk-review.md`, `completion-audit.md`, and `manual-verification-matrix.md`.
- Localization checks live in `localization-validation.md`.
- Manual proof should be recorded in `manual-verification-matrix.md`; do not claim release readiness from source review alone.

## Player Text Commitments

- Chinese visible text uses compact wording and no-space number formatting, such as `获得1点能量`, `第7张`, `至少18点`, `迅速2`, and `棱彩计数：0/2`.
- Tooltips should describe the player-visible effect, cost, and timing.
- Technical terms such as hook, source-safe, backend, candidate, debug, and fallback must stay out of player-facing text. Internal docs may mention implementation fallbacks only when they explain test or runtime evidence.
