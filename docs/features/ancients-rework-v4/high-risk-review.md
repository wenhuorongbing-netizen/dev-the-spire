# High-risk Patch Review

Review date: 2026-05-06

Scope: private beta hardening pass for implemented Spire Plus patches under technical id `EZMicroBalance`. v4.3 is current.

## Reviewed Surfaces

| Surface | Status | Evidence / action |
|---|---|---|
| Prismatic Gem reward replacement | Hardened for v4.3 | Screen-scoped state prevents reroll counter drift, and the trigger now makes every visible standard reward option off-color. Normal reward exclusions remain in `PrismaticGemPatches.cs`; hover and reward-banner hints are patched for the v4.3 UI count behavior, with `UI/Banner` fallback lookup and log diagnostics if the private banner field is unavailable. Manual runtime verification still required. |
| Velvet Choker soft limit | Reviewed | Hard six-card blocking is bypassed. The counter increments only for non-autoplay first manual card-play series from hand, resets on player-turn/combat boundaries, and applies +1 cost through energy-cost hooks after other modifiers. Manual cost/X-cost verification still required. |
| Distinguished Cape max-HP cost | Reviewed for v4.3 | Pickup cost uses `max(ceil(currentMaxHp * 0.30), 18)`, replaces an unaffordable Vakuu Cape roll with a payable Pool 2 option instead of shrinking choices, clamps current HP without damage, then loses max HP and adds three `Apparition` cards. Manual low-HP option-pool verification still required. |
| Pael's Tooth stored-card return | Hardened | Added a null guard around reflected `UpdateCardList` display refresh. Manual save/load and act-boss clear verification still required. |
| Debt gold-loss behavior | Hardened | Gold loss now wraps the original `CardCmd.Exhaust` task and awaits it before losing gold, avoiding a synchronous `.GetAwaiter().GetResult()` in a Harmony hook. |
| Jeweled Mask permanent free power | Hardened | Enchantment now uses `SetCustomBaseCost(0)` instead of offsetting current cost by a negative upgrade amount. Save/load verification still required. |
| Meat Cleaver Cleaver option | Reviewed | The rest-site Cleaver / 切肉 option remains gated by current HP and removable-card count, removes exactly two cards, then loses 5 current HP through `CreatureCmd.SetCurrentHp`. Manual rest-site verification still required. |
| Music Box generated attack copy | Hardened | Removed an accidental `[HarmonyPatch]` attribute from the state-tracker helper class; copy behavior still uses generated combat add and per-turn state reset. |
| Crossbow generated attack offer | Reviewed | Skip path removes the generated combat card; accept path adds the generated temporary attack with cost reduction, Ethereal, and Exhaust. Manual skip/accept verification still required. |
| Fiddle draw cap | Reviewed | Draw cap applies only during the owner side's combat turn and excludes hand-draw setup. Manual draw-effect verification still required. |
| Whispering Earring auto-play | Reviewed | Local API evidence shows `CardCmd.AutoPlay` does not spend resources itself; the patch spends resources before auto-playing and uses normal auto-play wrapper flow. Manual targeting/cost verification still required. |

## Build Result

`dotnet build EZMicroBalance.sln` passed after the v4.3 code changes during the implementation pass. Final validation results are recorded in `completion-audit.md`.

v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.

## Remaining Risk

This review is compile-time and code-path review only. Runtime behavior must still be verified in the manual matrix before private beta readiness is claimed.
