# BUGFIX_REPORT.md

## Root Cause
`UrdaSeedbedAfterCardDrawnPatch` short-circuited `Hook.AfterCardDrawn` for planted cards but, in the older behavior shown by the provided log, did not assign a non-null `Task` to `ref Task __result`.
`CardPileCmd.Draw` then awaited a `null` task as part of its draw loop, producing:
`System.NullReferenceException` at `CardPileCmd+<Draw>d__16.MoveNext_Patch1`.

## Fix Summary
1. Ensure the after-card-drawn skip path is contract-safe:
   - `UrdaSeedbedAfterCardDrawnPatch` now assigns `Task.CompletedTask` before returning `false`.
2. Keep draw-depth management on the draw patch side in a completion-safe wrapper (`finally`) so queue processing only resumes after draw completion.
3. Added/kept regression coverage by source assertions for the task-completion short-circuit and draw-patch structure.

## Regression Coverage Added
- `tests/EZMicroBalance.Tests/RuntimeCrashRegressionGuardTests.cs`
  - Added assertions that `UrdaSeedbedAfterCardDrawnPatch` contains:
    - `HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))`
    - `__result = Task.CompletedTask`
    - explicit short-circuit `return false;`

## Verification Commands and Results
- `dotnet build`
  - `dotnet build EZMicroBalance.sln` ✅
- `dotnet test`
  - `dotnet test EZMicroBalance.sln --no-build` ✅ (301 passed, 20 skipped)

## Remaining Risk
- If another mod or future in-repo patch also short-circuits `Hook.AfterCardDrawn` for any path without returning a valid `Task`, this same class of crash can reappear.
- No adjacent feature regressions were observed in the changed paths; existing adjacent Urda/Seedbed logic remains unchanged.

## Follow-up Recommendations
- Add a shared test helper/guideline for Harmony patches on `Task`-returning methods: every skip-path must set `__result` explicitly to a completed/failed task.
- When touching draw-path hooks, prefer explicit runtime smoke checks that exercise `CardPileCmd.Draw` with seeded planted-card sequences.
