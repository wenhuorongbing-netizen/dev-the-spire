# BUGFIX_NOTES.md

## Bug Summary
System.NullReferenceException in CardPileCmd.Draw was caused by Urda Seedbed planting while draw processing was still active.

## Reproduction Status
Reproduction evidence is in the provided historical log:
C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log

I did not reproduce the crash in a fresh run with current source in this workspace. The available runtime evidence in godot.log reports EZMicroBalance version v0.1.0-private-beta.83 while source manifest is v0.1.0-private-beta.84, so exact same runtime conditions are currently unavailable.

Closest local reproduction evidence remains the historical stack:
CardPileCmd.Draw -> Hook.AfterCardChangedPiles -> UrdaRunHook.AfterCardChangedPiles ->
UrdaBlessingService.TryPlantSeedbedCardFromHand -> UrdaBlessingService.PlantSeedbedCard
followed by System.NullReferenceException.

## Expected Behavior
When drawn-card flow triggers hand-entry seedbed logic, no immediate combat-pile mutation should occur inside the draw-hook chain.
Hand-entry requests during draw should be deferred until draw completion.

## Actual Behavior
UrdaRunHook.AfterCardChangedPiles can route a hand-entry card into
UrdaBlessingService.TryPlantSeedbedCardFromHand.
That helper could still take its immediate branch and call PlantSeedbedCard even when CardPileCmd.Draw draw-depth was active,
creating re-entrant pile mutation and null-state crashes.

## Investigation Steps
1. Parsed the provided log and mapped stack progression from draw execution to seedbed planting.
2. Confirmed CardPileCmd.Draw draw completion and hook sequencing.
3. Compared source callsites for seedbed hand-entry:
   - queueing path in UrdaRunHook (QueueSeedbedPlantFromHand)
   - direct helper path in TryPlantSeedbedCardFromHand.
4. Identified the direct helper path was missing a draw-in-progress guard.
5. Validated existing invariants already present:
   - UrdaSeedbedCardPileDrawPatch draw-depth tracking with finally unwind,
   - UrdaSeedbedAfterCardDrawnPatch skip for planted cards,
   - queue processor wait loop in UrdaBlessingService.SeedbedState.
6. Added parser-level smoke-log validation to classify stack-only EZMicroBalance traces as errors.

## Root Cause
The state-transition invariant was inconsistent across entrypoints:
not all seedbed hand-entry entrypoints enforced deferred execution while draw depth was active.

TryPlantSeedbedCardFromHand could execute PlantSeedbedCard synchronously during draw, causing hook-driven mutation re-entry.

## Affected Files
- EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingService.SeedbedCombat.cs
- tests/EZMicroBalance.Tests/RuntimeCrashRegressionGuardTests.cs
- tests/EZMicroBalance.Tests/ReleaseArtifactParityGuardTests.cs

## Fix Strategy
1. Preserve normal non-draw behavior.
2. If draw is in progress, force TryPlantSeedbedCardFromHand to defer via QueueSeedbedPlantFromHand.
3. Keep the existing queue/depth model intact so this is a boundary-fix, not a behavioral rewrite.

## Regression Test Plan
- Unit/source-level checks:
  - assert draw-safe branch and queue routing are present in TryPlantSeedbedCardFromHand.
  - assert seeded-husk AfterCardDrawn short-circuit and draw patch finally-unwind are present.
- Regression/parity checks:
  - classify stack-only EZMicroBalance lines as runtime errors.
  - verify manifest/runtime parity when release artifact tests are enabled.
- Manual verification (not possible with this exact historical artifact):
  - clean reinstall current v0.1.0-private-beta.84 package,
  - run a Seedbed-in-draw scenario and confirm no draw-time NullReferenceException.

## Verification Commands
- dotnet build -> pass
- dotnet publish -> pass
- dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~RuntimeCrashRegressionGuardTests" -> pass (3 passed)
- dotnet test -> pass (303 passed, 21 skipped)
- dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~ReleaseArtifactParityGuardTests" -> pass/skipped as expected (5 passed, 7 skipped without release flags)
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/audit-godot-log.ps1 -Path "C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log" -> parses historical crash and reports Clean=false
- SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~ReleaseArtifactParityGuardTests" -> fails due environment drift (runtime v0.1.0-private-beta.83 vs manifest v0.1.0-private-beta.84) and stale packaged hash docs, not crash-path logic.
