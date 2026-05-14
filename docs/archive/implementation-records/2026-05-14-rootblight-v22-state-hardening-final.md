Executed the goal file exactly and completed the Rootblight v2.2 state-hardening pass.

**Root-Cause Fixes**
- Preserved Rootblight split lineage through `III -> II -> I` downgrades by carrying `splitState` directly instead of tying it to max level.
- Normalized existing/restored Blight Sprout rounds before seeding new sprouts, so boss sprouts use rounds `3/4` and elite sprouts use round `3`.
- Hardened A11 map geometry diagnostics to fail closed if map evidence extraction throws.

**Files Changed**
- `EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs`
- `EZMicroBalanceCode/Ascension/Combat/RootBudCombatHook.cs`
- `EZMicroBalanceCode/Ascension/Map/AscensionMapService.cs`
- `tests/EZMicroBalance.Tests/AscensionV2MilestoneGuardTests.cs`
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `PROJECT_STATE.md`
- `docs/issues.md`
- `docs/features/ascension-11-20/work-log.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/development-checklist-v2.md`
- Current hash/reference docs: `docs/dev-environment.md`, `docs/release-checklist.md`, `docs/test-ready-completion-audit.md`, `docs/features/ancients-rework-v4/completion-audit.md`, `docs/private-beta-verification-handoff.md`, `docs/private-beta-release-completion-audit.md`

The worktree was already broadly dirty before this pass; I only changed the files above plus refreshed generated package artifacts.

**Tests And Validation**
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings, 0 errors
- `dotnet test EZMicroBalance.sln --no-build`: passed, 147 passed, 18 skipped
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed
- `git diff --check`: passed, only CRLF normalization warnings
- `dotnet publish EZMicroBalance.sln --no-restore`: passed
- `scripts\package-spire-plus.ps1`: passed
- Post-package `dotnet build EZMicroBalance.sln --no-restore`: passed
- Release artifact tests with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`: passed, 165 passed, 0 skipped

**Refreshed Package Hashes**
- ZIP: `595160E11BAE06407EAC67601C4884FDBD593F2FE2F88B91DE412DCCB9EC3046`
- DLL: `DF83EA7A7D0DAACAF2DC33416FEC63A04436D9A36070561DEB00F0C40DA1AF21`
- PCK: `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README_INSTALL: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live game, Steam startup, save/load, gameplay, route-click traversal, death/failure-path, co-op, or Image API art generation was run.