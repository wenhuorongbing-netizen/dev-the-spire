Implemented the A11 map geometry source-hardening goal.

Fixed a source-proof gap: A11 target dimensions no longer count as good enough unless the inserted column has a reachable optional route choice and there is still a start-to-boss route that avoids that inserted column. Chokepoint-only “widening” is now rejected by deterministic tests.

Changed files include:
- `EZMicroBalanceCode/Ascension/Map/A11MapGeometryProof.cs`
- `EZMicroBalanceCode/Ascension/Map/AscensionMapService.cs`
- `tests/EZMicroBalance.Tests/A11MapGeometryProofTests.cs`
- `tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj`
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`
- Current docs/status files: `PROJECT_STATE.md`, `docs/issues.md`, release/dev/handoff audits, and the active Ascension docs under `docs/features/ascension-11-20/`

Core source evidence consulted:
`ActModel.CreateMap`, `StandardActMap`, `RunManager.GenerateMap`, `SavedActMap`, `SerializableActMap`, `SerializableMapPoint`, `NMapScreen.SetMap`, `MapTravel`, `ActMap`, `MapPoint`, and `MapCoord`.

Validation:
- `git status --short --branch`: ran; worktree was already broadly dirty.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings / 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 147 passed / 18 skipped / 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed.
- `scripts\package-spire-plus.ps1`: passed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 165 passed / 0 skipped / 0 failed.

Refreshed package artifacts:
- Zip: `8856F4ADEF942CEE207D7F5C6074C03943311B446D5B5B4B1A5BC416A249A7D2`
- DLL: `216C98859E0BC2E9FD99CE61C7C100A9BA8EBBAFD5820D6E873C44C841D1D306`
- PCK: `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live game testing was run. No natural route-click traversal, gameplay, save-load, clicked UI, death/failure-path, or co-op testing was run.