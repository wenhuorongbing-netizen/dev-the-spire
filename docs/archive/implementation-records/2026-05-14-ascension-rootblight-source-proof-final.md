Code, tests, localization, and docs were changed. The repo already had unrelated dirty changes; I preserved them.

**Files Changed**
- [AscensionMapService.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ascension/Map/AscensionMapService.cs)
- [AscensionMapGenerationPatches.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ascension/Patches/AscensionMapGenerationPatches.cs)
- [RootDeckService.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs)
- [ascension.json](D:/Game/FOTN/dev-the-spire/EZMicroBalance/localization/eng/ascension.json)
- [ascension.json](D:/Game/FOTN/dev-the-spire/EZMicroBalance/localization/zhs/ascension.json)
- [cards.json](D:/Game/FOTN/dev-the-spire/EZMicroBalance/localization/eng/cards.json)
- [cards.json](D:/Game/FOTN/dev-the-spire/EZMicroBalance/localization/zhs/cards.json)
- [AscensionFeatureGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs)
- [AscensionV2MilestoneGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/AscensionV2MilestoneGuardTests.cs)
- [ReleaseCoverageGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs)
- [api-research.md](D:/Game/FOTN/dev-the-spire/docs/features/ascension-11-20/api-research.md)
- [implementation-plan.md](D:/Game/FOTN/dev-the-spire/docs/features/ascension-11-20/implementation-plan.md)
- [development-checklist-v2.md](D:/Game/FOTN/dev-the-spire/docs/features/ascension-11-20/development-checklist-v2.md)
- [manual-test-checklist.md](D:/Game/FOTN/dev-the-spire/docs/features/ascension-11-20/manual-test-checklist.md)
- [work-log.md](D:/Game/FOTN/dev-the-spire/docs/features/ascension-11-20/work-log.md)
- [issues.md](D:/Game/FOTN/dev-the-spire/docs/issues.md)
- [PROJECT_STATE.md](D:/Game/FOTN/dev-the-spire/PROJECT_STATE.md)
- [README.md](D:/Game/FOTN/dev-the-spire/README.md)

**A11 Conclusion**
A11 is now source-patched at the earlier `ActModel.CreateMap` boundary, plus the existing generated-map path remains guarded. The audit confirmed vanilla map UI reads actual `ActMap` row/column data, not metadata. Manual proof is still needed in a current build to confirm visible Act 1/2/3 row counts and natural traversal in real play.

**Rootblight v2.2 Conclusion**
Rootblight/Blight Sprout v2.2 is implemented in source and guarded by tests: four-root cap, seed seen/unplayed behavior, discarded/exhausted sprout handling, I/II growth, III one-time split with no IV, played downgrade/removal, new roots not growing same combat, and rest removing one highest-stage root. Runtime save/load and multiplayer ownership remain manual/pending.

**Validation**
- `git status --short --branch`: run at start and end; worktree remains dirty with unrelated pre-existing changes.
- `dotnet build EZMicroBalance.sln --no-restore`: initial exact run failed due sandbox NuGet path missing `Godot.NET.Sdk`; rerun with `NUGET_PACKAGES=C:\Users\Jack\.nuget\packages` passed.
- `dotnet test EZMicroBalance.sln --no-build`: passed, `125` passed, `18` skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed; only existing line-ending warnings were printed.
- `dotnet publish EZMicroBalance.sln --no-restore`: attempted because localization/resources changed. Real Steam mods publish was blocked by sandbox access denied; redirected workspace-local publish with `ModsPath`, `APPDATA`, and `LOCALAPPDATA` under `.tools/` passed.

No live gameplay, save-load, or co-op testing was done.