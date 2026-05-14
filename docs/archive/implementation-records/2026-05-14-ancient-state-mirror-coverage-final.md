Implemented the Ancient state mirror coverage pass. Runtime source did not change; this was tests/docs only.

Changed files:
- `tests/EZMicroBalance.Tests/AncientStateMirrorGuardTests.cs`
- `docs/features/ancient-expansion-v2.2/risk-register.md`
- `docs/features/ancient-expansion-v2.2/api-research.md`
- `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `docs/issues.md`
- `docs/issues/ancient-expansion-v2.2.md`
- `PROJECT_STATE.md`

What changed:
- Added focused guards for `AncientPlayerState.Get/Set/SyncDeck`, owned-card and removed-card filters, Urda/Morvi/Lotha hook usage, recurrent `AfterCardChangedPiles` sync hooks, and direct encoded-field indexing bypasses.
- Documented that this strengthens source coverage only. Live save/load remains pending.
- No direct bypass of `UrdaStateKey`, `UrdaDeckStateKey`, `MorviStateKey`, `MorviDeckStateKey`, `LothaStateKey`, or `LothaDeckStateKey` was found.

Validation:
- `git status --short --branch`: ran; worktree remains heavily dirty from existing/unrelated work plus this new test file.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 142 passed, 18 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.

Package artifacts were not refreshed because no runtime source, resources, localization, export presets, or package inputs changed. No subagents were used. No live game, save-load, co-op, clicked UI, death/failure-path, publish/package, opt-in artifact, or Image API testing was run.