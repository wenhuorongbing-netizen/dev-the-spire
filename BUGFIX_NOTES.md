# BUGFIX_NOTES.md

## Bug Summary
A crash in combat (`System.NullReferenceException` at `MegaCrit.Sts2.Core.Commands.CardPileCmd+<Draw>d__16.MoveNext_Patch1`) occurred during card drawing under Urda's Seedbed mechanics when an eligible card (e.g. `SLIMED`) was drawn and planted.

## Reproduction Status
Confirmed by the provided runtime log `C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log` at lines 1021-1024.

## Expected Behavior
1. Active drawing (`CardPileCmd.Draw`) should track draw depth correctly and prevent queued hand planting from running concurrently during the draw operation.
2. If `Hook.AfterCardDrawn` is skipped for planted cards, the patch must return a completed `Task` to avoid throwing a `NullReferenceException` when the caller awaits the return value of `Hook.AfterCardDrawn`.

## Actual Behavior
1. `UrdaSeedbedCardPileDrawPatch` patched `CardPileCmd.Draw` using wrapper Prefix/Finalizer. Because async methods return the task immediately at the first `await`, the finalizer ran prematurely, decrementing draw depth and allowing concurrent planting.
2. `UrdaSeedbedAfterCardDrawnPatch.Prefix` skipped `Hook.AfterCardDrawn` by returning `false` but did not assign a `Task` value to `ref Task __result`. Harmony returned `default(Task)` (`null`), causing a `NullReferenceException` when `CardPileCmd.Draw` awaited it.

## Investigation Steps
1. Inspected `C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log` to identify the stack trace.
2. Traced the source of `CardPileCmd.Draw` and `Hook.AfterCardDrawn` in the game source code.
3. Analyzed `UrdaSeedbedAfterCardDrawnPatch.cs` and `UrdaSeedbedCardPileDrawPatch.cs` to identify Harmony task-return and wrapper return behaviors.

## Root Cause
1. **Wrapper-Level Async Patching**: `CardPileCmd.Draw` returns a `Task` immediately when it yields, so the `Finalizer` ran immediately, resetting the draw depth.
2. **Missing Async Result Assignment**: Returning `false` in a prefix patching a `Task`-returning method without assigning `ref Task __result = Task.CompletedTask` causes the patch to return a `null` `Task`, throwing `NullReferenceException` upon await.

## Affected Files
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedAfterCardDrawnPatch.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedCardPileDrawPatch.cs`

## Fix Strategy
1. **Assign CompletedTask**: Modify `UrdaSeedbedAfterCardDrawnPatch.Prefix` to accept `ref Task __result` and assign `Task.CompletedTask` when returning `false`.
2. **Async Task Postfix Wrapping**: Modify `UrdaSeedbedCardPileDrawPatch` to use a `Postfix` that wraps the returned `Task<IEnumerable<CardModel>>` and decrements draw depth only when the task completes.

## Regression Test Plan
- Run `dotnet test` to ensure existing regression tests pass.
- Verify that `dotnet build` succeeds with zero errors.

## Verification Commands
- `dotnet build`
- `dotnet test`
