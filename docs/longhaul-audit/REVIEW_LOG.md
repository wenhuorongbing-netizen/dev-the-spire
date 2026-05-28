# Review Log

One entry per file audited.

| Round | File | Category | Decision | Summary | Date |
| ----- | ---- | -------- | -------- | ------- | ---- |
| 1 | EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs | cs-high-risk | skipped | Static field key constants. 2 unused (VakuuFightSelectedAncientsKey, EvilMode) but no real bug. Well-tested by guard tests. | 2026-05-28 |
| 2 | EZMicroBalanceCode/Ancients/Common/AncientSelectionEvidenceLog.cs | cs-high-risk | skipped | Simple logging utility. Defensive PlayerSlot try-catch. Well-used across all Ancient features. No real bugs. | 2026-05-28 |
| 3 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatEnd.cs | cs-high-risk | skipped | Combat end handler. Two-pass logic correct. Null-conditional on RoundNumber. ConditionalWeakTable cleanup. No real bugs. | 2026-05-28 |
| 4 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatStart.cs | cs-high-risk | skipped | Combat start handler. All helper methods exist. DebugOnlyGetState is standard API. SingleSentence power correctly applied. No real bugs. | 2026-05-28 |
| 5 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatState.cs | cs-high-risk | skipped | Simple state class. ConditionalWeakTable for per-player state. 18 properties tracking blessing mechanics. No real bugs. | 2026-05-28 |
| 6 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatStateReset.cs | cs-high-risk | skipped | Two reset methods. ResetCombatState covers all 18 properties. ResetTurnState correctly preserves combat-persistent state. No real bugs. | 2026-05-28 |
| 7 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaCombatHook.cs | cs-high-risk | skipped | Thin hook class. Consistent co-op guard pattern. All methods delegate to LothaBlessingService. No real bugs. | 2026-05-28 |
| 8 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRewardSelectionService.cs | cs-high-risk | skipped | Reward selection. All methods exist. Localization present in eng/zhs. Proper null handling for card selection. No real bugs. | 2026-05-28 |
| 9 | EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs | cs-high-risk | skipped | Run lifecycle hook. Two-level co-op guard (combat vs gameplay). All methods delegate. SyncPersistentState handles null. No real bugs. | 2026-05-28 |
| 10 | EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingService.CombatLifecycle.cs | cs-high-risk | skipped | Combat lifecycle. All referenced methods exist. ResetCombatState chains to ResetTurnState. Proper IsActiveForHooks checks. No real bugs. | 2026-05-28 |
