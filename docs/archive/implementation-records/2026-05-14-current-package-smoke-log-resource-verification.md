# 2026-05-14 Current Package Smoke/Log/Resource Verification

Evidence directory: `.tools/runtime-evidence/current-package-smoke-20260514-015901`

No caller-provided final-output path was found in the visible environment variables, so this run record is archived here and summarized in active docs.

## Scope

- Verified the existing `publish/SpirePlus-v0.1.0-private-beta.0.zip` package and installed `EZMicroBalance` artifacts.
- Checked Ancient UI/art resource coverage and installed-PCK resource loading for Urda, Morvi, Lotha, and Vakuu option art.
- Ran a bounded normal Steam-client startup/log smoke through repository helpers with only previous framework plus Spire Plus / `EZMicroBalance` enabled.
- Did not implement gameplay features, did not run gameplay/manual matrix checks, and did not collect save/load, co-op, clicked Ancient UI, or Mod Settings screenshot evidence in this pass.

## Artifact And Resource Evidence

- `git status --short --branch`: `## main...origin/main` with the existing broad dirty worktree.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
- Final artifact parity passed after syncing the installed `README_INSTALL.txt` to the staged/package copy.
- Final hashes:
  - Zip: `83EC2AA5AE6B9EED032A787B625A43B0369ACEA6BB04FAEB3CE6FCE7D99CE7A5`
  - DLL: `EB69E895652E610E9C709C1DE9E7929B56AC451C7453F8249A22A85DEFCD719A`
  - PCK: `D7FD71CE7AF29DA31DAE464ABE7F94719E104A91D2B67B32B82C191494D61722`
  - Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
  - README: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`
- `ancient-resource-localization-coverage.json`: 0 missing assets and 0 missing EN/zhs localization keys.
- Headless installed-PCK resource load: `godot-ancient-resource-load-summary.json` reports exit code 0, `HasOkMarker: true`, 0 error lines, and 0 warning lines while loading 3 Ancient scenes and 43 Ancient textures.
- Static inspection confirmed Urda/Morvi/Lotha background scenes are Control-root scenes, event/map/run-history/option art paths are separate, and Lotha event art uses the large event illustration rather than the small map thumbnail.

## Startup/Log Smoke

- Preflight: `scripts/check-spire-window-preflight.ps1` recorded no Slay the Spire 2 process before launch.
- Launch: `scripts/spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir .tools/runtime-evidence/current-package-smoke-20260514-015901 -MoveOtherMods -MoveCurrentRuns -Launch`.
- Positive log evidence:
  - previous framework `177 patches successfully, 0 failed`
  - `Registered config for mod EZMicroBalance`
  - `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`
  - `Loaded 2 mods (2 total)`
  - `Found 22 previous saved-state registrations`
  - `Time to main menu: 14,045ms`
- `scripts/audit-godot-log.ps1 -FailOnHit` reported `Clean: true`.
- Manual log scan found 0 hits for Spire Plus / `EZMicroBalance` errors, missing resource paths, Ancient scene failures, previous framework patch failures, `NullReferenceException`, `InvalidOperationException`, `TargetInvocationException`, `ERROR`, `ERR_`, failed resource loading, resource-file-not-found, and cannot-open-file signatures.
- Restore: `scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir .tools/runtime-evidence/current-package-smoke-20260514-015901 -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`.
- Restore stopped one `SlayTheSpire2` process, restored 24 moved mod entries, restored 2 current-run files, preserved Steam-rehydrated test current-run files under evidence, and restored settings to the original hash.
- Final process check found 0 `SlayTheSpire2` processes.

## Validation Commands

- `dotnet test EZMicroBalance.sln`: passed, 109 passed / 18 skipped.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped.
- `$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build; Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS`: passed, 127 passed / 0 skipped.
- `git diff --check`: passed with CRLF normalization warnings only.

## Changes Made

- Synced installed `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\README_INSTALL.txt` to the staged/package README so installed artifacts match the package.
- Updated active docs to record the 2026-05-14 current-package smoke/log/resource evidence and keep gameplay/manual gates pending.
- Updated release-safety guard expectations from the old 16-field smoke evidence to the current 22-field startup/log evidence.
- No production code, gameplay behavior, exported resources, or package archive contents were changed in this pass.

Files touched by this verification pass:

- `PROJECT_STATE.md`
- `README.md`
- `docs/BETA_COMPATIBILITY.md`
- `docs/dev-environment.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `docs/features/ancients-rework-v4/completion-audit.md`
- `docs/features/ancients-rework-v4/manual-verification-matrix.md`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/issues.md`
- `docs/mod-changelog.md`
- `docs/private-beta-release-completion-audit.md`
- `docs/private-beta-verification-handoff.md`
- `docs/release-checklist.md`
- `docs/test-plan.md`
- `docs/test-ready-completion-audit.md`
- `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`
- `docs/archive/implementation-records/2026-05-14-current-package-smoke-log-resource-verification.md`
- `.tools/runtime-evidence/current-package-smoke-20260514-015901/artifact-hash-mismatches-final.txt`

## Remaining Blockers

- Live Ancient gameplay/manual matrix.
- Clicked Ancient UI verification for Urda, Morvi, Lotha, and Vakuu option flows.
- Save/load-sensitive Ancient and Ascension rows.
- Vakuu fight failure/death and unfinished parent-linked child-combat save/load behavior.
- Disable-mod gameplay in an actual run.
- Full Rootblight visual/combat-end/co-op behavior.
- Natural A11 click-by-click traversal.
- Two-client multiplayer/co-op matrix.
- Clean intentional commit and user-approved push.
