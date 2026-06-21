# BUGFIX_REPORT.md

Archived 2026-06-20: historical Seedbed draw re-entrancy bugfix report for the beta.83/beta.84 package line. Current Spire Plus package/runtime state is tracked in `PROJECT_STATE.md` and current release docs.

## Root Cause
The historic crash in the provided log is a draw-state re-entrancy issue.

During an active draw, Hook.AfterCardChangedPiles -> UrdaRunHook.AfterCardChangedPiles -> UrdaBlessingService.TryPlantSeedbedCardFromHand could execute the immediate branch
while draw depth was still active. That allowed synchronous pile mutation from PlantSeedbedCard inside an awaited draw flow and produced a NullReferenceException.

The immediate mutation was possible because that helper lacked the draw-in-progress gate, even though other paths were already queue-based.

## Fix Summary
- In UrdaBlessingService.SeedbedCombat.TryPlantSeedbedCardFromHand, added a draw-state gate:
  - if IsSeedbedDrawInProgress(player) is true, call QueueSeedbedPlantFromHand.
  - otherwise preserve existing immediate behavior.
- No functional behavior changed for non-draw contexts.
- Existing guards stayed in place:
  - UrdaSeedbedCardPileDrawPatch draw-depth begin/end with finally.
  - UrdaSeedbedAfterCardDrawnPatch planted-card short-circuit.
  - Queue drain and marker cleanup in UrdaBlessingService.SeedbedState.

## Regression Coverage Added
- tests/EZMicroBalance.Tests/RuntimeCrashRegressionGuardTests.cs
  - verifies queue usage from UrdaRunHook hand-entry path.
  - verifies TryPlantSeedbedCardFromHand routes to queue when draw is active.
  - verifies draw-patch finally-unwind and AfterCardDrawn planted-card short-circuit.
- tests/EZMicroBalance.Tests/ReleaseArtifactParityGuardTests.cs
  - verifies stack-only EZMicroBalance stack traces are treated as error lines.
  - verifies manifest parsing and version-parity test path exists.

## Verification Commands and Results
- dotnet build -> pass
- dotnet publish -> pass
- dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~RuntimeCrashRegressionGuardTests" -> pass (3 passed)
- dotnet test -> pass (303 passed, 21 skipped)
- dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~ReleaseArtifactParityGuardTests" -> pass/skipped as expected without release flags (5 passed, 7 skipped)
- SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~ReleaseArtifactParityGuardTests"
  - fail due environment drift only: runtime-vs-manifest version mismatch and hash drift in local install/staging artifacts.
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/audit-godot-log.ps1 -Path "C:/Users/Jack/AppData/Roaming/SlayTheSpire2/logs/godot2026-05-27T02.37.23.log" -> parsed historical crash context
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore -> pass

## Remaining Risk
- Exact runtime reproduction cannot be claimed against the current workspace runtime due version/hash drift.
- Queue processing still depends on global hook timing and external mods if present; this fix protects draw re-entrancy but does not alter those upstream dependencies.
- Release gates are currently blocked by stale local artifacts and must be refreshed before release signoff.

## Follow-up Recommendations
- Reinstall cleanly from current publish output (v0.1.0-private-beta.84), then rerun:
  - dotnet test with SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1,
  - live runtime smoke run with only previous framework + Spire Plus active.
- Keep runtime evidence and docs aligned before any release/publish handoff.
- Preserve the invariant by extending draw-depth queue gating to any future seedbed-like hand-entry mutation paths.
