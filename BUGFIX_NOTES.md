# BUGFIX_NOTES.md

## Bug Summary
Combat can crash with:

`System.NullReferenceException: Object reference not set to an instance of an object.`
at `MegaCrit.Sts2.Core.Commands.CardPileCmd+<Draw>d__16.MoveNext_Patch1`

during Urda Seedbed sequencing when a card is planted from hand and becomes part of draw handling.

## Reproduction Status
Reproduced from provided log:
`C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log`
lines around 1023–1030.

The crash occurs after:
`[EZMicroBalance] [Spire Plus] Urda Seedbed skipped AfterCardDrawn hooks for planted card ...`

## Expected Behavior
When a patch skips `Hook.AfterCardDrawn` for a planted card, it must still return a valid `Task` to the Harmony hook pipeline so the call site in `CardPileCmd.Draw` can safely `await` it.

## Actual Behavior
`CardPileCmd.Draw` awaited a `null` task during the draw await chain, which propagated as `NullReferenceException` in the patched draw async state machine (`MoveNext_Patch1`).

## Investigation Steps
- Inspected runtime log and isolated the exception to `CardPileCmd.Draw`.
- Followed `CardPileCmd.Draw` implementation in local source:
  - `await Hook.AfterCardDrawn(combatState, choiceContext, card, fromHandDraw);`
- Traced Urda-specific overrides:
  - `UrdaSeedbedAfterCardDrawnPatch` (`HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))`)
  - `UrdaSeedbedCardPileDrawPatch` depth-tracking wrapper for draw calls
- Cross-checked patch source and call flow in:
  - `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedAfterCardDrawnPatch.cs`
  - `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedCardPileDrawPatch.cs`
- Verified this is a historical failure shape in the log package (`v0.1.0-private-beta.82`) that is not a valid source marker for current working tree version.

## Root Cause
`UrdaSeedbedAfterCardDrawnPatch.Prefix` skipped the hook by returning `false` without assigning `ref Task __result` to a completed task in an older code state, so `CardPileCmd.Draw` resumed with `null` and crashed when awaiting it.

## Affected Files
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedAfterCardDrawnPatch.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbedCardPileDrawPatch.cs`

## Fix Strategy
1. Keep the draw-depth tracking patch (`UrdaSeedbedCardPileDrawPatch`) wrapping the draw task and ending depth in a `finally`.
2. Make `UrdaSeedbedAfterCardDrawnPatch` assign `Task.CompletedTask` when short-circuiting hook execution:
   - include `ref Task __result`
   - set `__result = Task.CompletedTask`
   - return `false`

## Regression Test Plan
- Source-guard test to assert:
  - the AfterCardDrawn prefix remains task-return-aware
  - `__result` is explicitly completed when returning `false`
- General build/test command set:
  - `dotnet build EZMicroBalance.sln`
  - `dotnet test EZMicroBalance.sln --no-build`

## Verification Commands
- Executed during this pass:
  - `dotnet build` ✅
  - `dotnet test` ✅ (`dotnet test EZMicroBalance.sln --no-build`)
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` ✅
