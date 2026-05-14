Implemented the narrow Lotha bugfix.

Files changed:
- [LothaRunHook.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs:909)
- [LothaPolishGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs:113)
- [work-log.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/work-log.md:3)

Exact bug fixed:
- Removed first-Power-in-hand gating for Mirror Hall Echo, Deferred Verdict, and Single Sentence.
- After-play Power replacement now rechecks the actual played Power directly, so it no longer depends on `PowerReplacementCardPendingBenefit` being set by cost preview.
- Mirror Rebuttal remains tied to the selected marked Rebuttal card only.
- Power cards still are not extra-played; autoplay, clones, and non-first executions remain excluded.
- Added guards against reintroducing `FirstOrDefault(IsPowerCard)`, `IsCurrentEligiblePowerInHand`, or pending-marker-only after-play gating.

Validation:
- `dotnet build EZMicroBalance.sln --no-restore`: exact command failed on known local `Godot.NET.Sdk/4.5.1` resolver/cache issue.
- `$env:NUGET_PACKAGES='C:\Users\Jack\.nuget\packages'; dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: exact command ran tests successfully but exited 1 afterward on the same SDK resolver issue.
- `$env:NUGET_PACKAGES='C:\Users\Jack\.nuget\packages'; dotnet test EZMicroBalance.sln --no-build`: passed, 126 passed, 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed, workspace warning only.
- `git diff --check`: passed, CRLF normalization warnings only.

No art touched. No publish/package run. No live game, save/load, death-path, clicked UI, or co-op testing performed.