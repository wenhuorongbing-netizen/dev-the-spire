Implemented the goal file pass.

**Files touched**
- Localization: `EZMicroBalance/localization/{eng,zhs}/ancients.json`, `relics.json`, `powers.json`
- Tests: `tests/EZMicroBalance.Tests/AncientPlayerFacingPolishGuardTests.cs`, `ReleaseCoverageGuardTests.cs`
- Docs: `PROJECT_STATE.md`, `docs/issues.md`, release/dev handoff docs, v2.2 work log/manual checklist, changelog, package hash docs
- Package refreshed: `publish/SpirePlus-v0.1.0-private-beta.0.zip`

**Player-facing fixes**
- Removed legacy Urda option hover text saying “option art marker / cannot be obtained” and the zhs equivalent.
- Replaced those legacy Urda hovers with real Seedbed, Humus Pact, Molting, and Moss Map behavior text.
- Added richer `[gold]`/`[blue]` markup for:
  `Compost Reward`, `Store Seed`, `Borrowed`, `Archive Pages`, `Waste Paper`, `red-ink debt`, `Rebuttal Card`, and zhs equivalents.
- Fixed affected zhs numeric markup in Red Ink and selection prompts.

**Art**
- No real/final art was generated or integrated.
- Art audit confirmed routing remains clean: event backgrounds separate from map/run-history icons and option/relic art.
- `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport`: passed with 0 missing targets, 0 hash mismatches, 0 missing exports.

**Validation**
- `git status --short --branch`: dirty worktree remained broad and pre-existing; unrelated changes were left alone.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors
- `dotnet test EZMicroBalance.sln --no-build`: passed, 149 passed / 18 skipped
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed
- `git diff --check`: passed, only existing CRLF normalization warnings
- `dotnet publish EZMicroBalance.sln --no-restore`: passed, known nested `source code/project.godot` warning
- `scripts\package-spire-plus.ps1`: rebuilt package
- Opt-in artifact tests: first run exposed stale hash docs; after updating hashes and rebuilding, passed 167/167

**Current package hashes**
- Zip: `B97FF7B84AFAD394705004F35B21FF7A0A5271DF76C277BC3780FC793A422E8F`
- DLL: `DF83EA7A7D0DAACAF2DC33416FEC63A04436D9A36070561DEB00F0C40DA1AF21`
- PCK: `2937930EBA8CCED6577D44F87698D49CB1DB744249666096EE194A152FD1B60C`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live gameplay, clicked UI, save/load, death/failure-path, natural route-click traversal, co-op testing, or Image API art generation was run.