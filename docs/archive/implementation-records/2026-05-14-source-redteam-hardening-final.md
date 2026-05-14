Implemented the source red-team hardening pass and kept it to source-supported cleanup.

**Fixes Made**
- Hardened Morvi runtime paths: generated combat-card cleanup, Forbidden Loan add-result handling, Open-Book sealed-card saved marker, Red Ink visible-power debt fallback, nonlethal Debt Settlement HP fallback, and Blueprint Proof Status/Curse exclusion.
- Added `MorviOpenBookSealedCard` saved field; current source now defines 22 SavedSpireFields.
- Added Urda `SPIREPLUS_*` gate aliases and removed stray Seed Bank Trial Plant marker assignment.
- Polished EN/zhs player text for Urda/Morvi/Lotha/Vakuu, including Draw/Discard/Exhaust pile terms, nonlethal debt wording, Vakuu fight/death/fallback wording, and Lotha Mirror Rebuttal truth.
- Added/tightened guards for Morvi source shape, Lotha text/source truth, Vakuu Temptation text, saved-field coverage, Urda gate aliases, and stale docs.
- Updated active docs to reflect source-complete/live-pending state and no release-ready claim.

**Changed Files**
- Source: `EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs`, `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviRunHook.cs`, `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaFeatureGate.cs`, `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRunHook.cs`
- Localization: `EZMicroBalance/localization/{eng,zhs}/ancients.json`, `cards.json`, `powers.json`, `relics.json`
- Tests: `AncientHighRiskSourceGuardTests.cs`, `MorviV22GuardTests.cs`, `ReleaseCoverageGuardTests.cs`, `ReleaseSafetyExpandedGuardTests.cs`, `LothaPolishGuardTests.cs`, `VakuuTemptationGuardTests.cs`
- Docs: `PROJECT_STATE.md`, `README.md`, `docs/issues.md`, `docs/test-ready-development-goal.md`, `docs/mod-changelog.md`, `docs/test-ready-completion-audit.md`, and active Ancient v2.2/Urda feature docs.

**Validation**
- `git status --short --branch`: ran; worktree remains dirty with existing broad project changes.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: exit 0; CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed; known nested `source code/project.godot` ignore warning.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped.

**Skipped**
- No live game/Steam run.
- No save/load, death/failure-path, co-op, or manual gameplay verification.
- No package script or opt-in release-artifact hash tests, because package artifacts were not refreshed by script in this pass.

**Remaining Blockers**
- Live Urda/Morvi/Lotha/Vakuu gameplay matrix.
- Save/load proof, especially Vakuu parent-linked child combat and Morvi/Open-Book/Red Ink state recovery.
- Lotha Death Reprieve lethal timing proof.
- Co-op/multiplayer verification.
- Bespoke final art, A11 traversal proof, Rootblight visual/save-load proof, and final package smoke.