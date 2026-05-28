# Skipped Files

Files checked with no real bugs found.

| File | Round | Date | Reason |
| ---- | ----- | ---- | ------ |
| EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs | 1 | 2026-05-28 | Static field key constants. 2 unused constants but no real bug. Well-tested by guard tests. |
| EZMicroBalanceCode/Ancients/Common/AncientSelectionEvidenceLog.cs | 2 | 2026-05-28 | Simple logging utility. Defensive try-catch. Well-used across all Ancient features. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatEnd.cs | 3 | 2026-05-28 | Combat end handler. Two-pass logic correct. Null-conditional on RoundNumber. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatStart.cs | 4 | 2026-05-28 | Combat start handler. All helper methods exist. DebugOnlyGetState is standard API. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatState.cs | 5 | 2026-05-28 | Simple state class. ConditionalWeakTable for per-player state. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.CombatStateReset.cs | 6 | 2026-05-28 | Two reset methods. All 18 properties covered. Turn state correctly preserves combat-persistent state. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaCombatHook.cs | 7 | 2026-05-28 | Thin hook class. Consistent co-op guard pattern. All methods delegate. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRewardSelectionService.cs | 8 | 2026-05-28 | Reward selection. All methods exist. Localization present. Proper null handling. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs | 9 | 2026-05-28 | Run lifecycle hook. Two-level co-op guard. All methods delegate. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingService.CombatLifecycle.cs | 10 | 2026-05-28 | Combat lifecycle. All methods exist. ResetCombatState chains to ResetTurnState. No real bugs. |
| EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingService.RunLifecycle.cs | 14 | 2026-05-28 | Run lifecycle. Complex act-gating logic. Correct. No real bugs. |
