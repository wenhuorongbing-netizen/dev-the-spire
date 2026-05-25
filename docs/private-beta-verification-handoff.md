# Spire Plus Private Beta Verification Handoff

Date: 2026-05-15

**Environment warning (2026-05-08):** The earlier `godot2026-05-08T05.06.30.log` came from v0.105.0 with 17 mods loaded and BaseLib `v3.1.0`, so it is not the required BaseLib + Spire Plus-only setup.

Current docs keep useful BaseLib `v3.1.4` source/package evidence separated from older `v3.1.2` runtime evidence: current 30-field loader smoke, historical installed-PCK resource smoke, current Mod Settings list evidence, normal-Steam A0/A10/A20 combat smoke, A11 map/save-load spot checks, Act 2/3 A11 map-surface observations, and targeted A14 Rootblight hover/starter-notice evidence. Full Ancient reward gameplay, natural A11 traversal, full Rootblight combat behavior, and co-op verification are pending.

Current naming note: testers should see `Spire Plus`. The technical manifest id and package folder remain `EZMicroBalance` for compatibility. Historical normal Steam-client startup/log evidence confirms the display name and config registration for the earlier 22-field package. The refreshed Mod Settings UI capture `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` shows `Spire Plus` in the Mods list.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

Latest package note, 2026-05-23: the package hashes below identify the current manual-test package copied to the local game root for testing. Detailed historical pass logs live in `docs/archive/**`, `docs/review.md`, and feature work logs; this handoff keeps only current tester-facing facts.

Current source/package highlights:

- Root Eyes uses map-click selection for future reachable Monster, Unknown, or Elite nodes. Normal/elite previews read the generated Act room set, and selected encounters/events are committed only when the marked room is entered.
- Morvi, Lotha, Vakuu, RootBud, Banner, RootDeck, Ascension map helpers, and combat-only Ancient hook ownership have been split into focused service files without intended player-visible behavior changes.
- Latest verification after the Sere Talon website/manual-matrix closure and package README wording refresh: `dotnet build EZMicroBalance.sln`, normal tests with 272 passed / 20 skipped, focused website/documentation/release-evidence guards, `dotnet format`, `git diff --check`, installed-package checker, release verifier fail-closed check with 20 pending live/manual rows / 0 warnings, worktree batching with 320 dirty entries / 0 unclassified paths, and opt-in artifact tests with 292 passed / 0 skipped all passed in their expected modes.
- `TESTER_START_HERE.md` in the current handoff starts with the installed-package checker command. The current package loader row is pending again after the ZIP/README hash changed; the verifier is expected to fail closed with 19 manual rows until loader evidence, feature screenshots, gameplay logs, save-load, route traversal, preview-tools, and co-op evidence are added.
- The latest normal Steam smoke under `.tools\runtime-evidence\manual-test-handoff-20260524-161744\release\fresh-current-package-loader-smoke` covered the same DLL/PCK/manifest as the current package: only BaseLib plus Spire Plus loaded, `Found 30 SavedSpireFields`, and the log audit was clean. This is startup/log context only until the current ZIP is launched.
- Live gameplay, clicked Ancient UI, save-load, natural A11 route-click traversal, death/failure path, and co-op verification remain pending.

Browser GPTimage2 art rebuild recheck, 2026-05-15:

- Promoted Urda/Morvi/Lotha/Vakuu option relics, Ancient identity icons, Lotha Verdict, Ascension indicators, neutral fallback power/relic assets, and six custom Ancient card portraits into active resources.
- Review sheets are under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`, especially `active-small-art-contact.png` and `processed/batch5-card-portraits-contact.png`.
- Event-background repair promoted the Lotha mirror ensemble, reframed Urda and Morvi to 1831x859, and changed all three scenes to keep-aspect centered fitting. Preview sheet: `.tools/art-generation/event-background-reframe-20260515/active-event-backgrounds-1831x859-contact.png`.
- `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch -FailOnMissingFinal` passed with 82 manifest assets, 69 `final_generated`, and 0 missing/temporary/export/hash issues.
- Generic fallback, source-local small-art, and event-background provenance/ratio blockers are resolved. Live clicked-UI preview remains pending.

Older 2026-05-14 text/art-fit and source-guard rechecks are superseded by the current package snapshot above. Their detailed command history is preserved in `docs/review.md` and `docs/archive/**`; do not use those older hashes as the package under test.

Clicked Ancient UI handoff, 2026-05-15:

- Use the force-evidence protocol in `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`; it creates `.tools\runtime-evidence\ancient-ui-click-smoke-YYYYMMDD-HHMMSS`, records expected option counts, and restores the test session afterward.
- Preferred UI-smoke commands: `spireplus_test_ancient URDA confirm`, `spireplus_test_ancient MORVI confirm`, `spireplus_test_ancient LOTHA confirm`, `spireplus_test_ancient VAKUU confirm`, and `spireplus_test_ancient VAKUU confirm fight`.
- These commands start an unsaved single-player test run and refuse to run over an existing run. Expected visible option counts are Urda 4, Morvi 3, Lotha 3, and Vakuu 3 by default; the focused `fight` case has one fight option.
- The gated Vakuu fight can also be enabled with `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` or `EZMB_ENABLE_VAKUU_FIGHT=1`. It uses a dedicated enemy and encounter scene, but still needs post-victory no-black-screen proof.
- Legacy active-run DevConsole commands `ancient EZMB_URDA`, `ancient EZMB_MORVI`, `ancient EZMB_LOTHA`, and `ancient VAKUU` remain valid only after a run is already in progress. Prefer `spireplus_test_ancient ...` from the main menu, and mark all DevConsole routes as UI render smoke, not natural gameplay proof.

## Package Under Test

- Package: `publish\SpirePlus-v0.1.0-private-beta.8.zip`
- Player-facing name: `Spire Plus`
- Zip SHA256: `BD412625FF6BB72B7B493EDAD6D20F793512B6600C1B1B46CCD961AA65B30971`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `29B7A4D505AD1ED68998C5AF9E2FB3E1AEBDEC15FDFE79C9B385DF74DDB8F26C`
- Manifest SHA256: `5E76B8441D999119110ED6AEC2074F812E80E0F68AF5F675019098EA7174AE99`
- PCK SHA256: `456378240576DB6EF8FF1669A4E2E8B747BB64AC269470635CED1BC0A90ADEFF`
- README_INSTALL SHA256: `CCBD00A154028A897BDD71A15B7057FC9DF3E17930DB48312E552D0253121336`

## Known Automated Evidence

- Current automated snapshot: `dotnet build`, normal `dotnet test`, opt-in release-artifact tests, `dotnet publish`, package refresh, `dotnet format`, `git diff --check`, website syntax checks, and installed-package checking passed after the 2026-05-25 co-op combat safety fix. Current-package live loader and gameplay proof are still pending.
- Current normal test count: 272 passed / 20 skipped. Current opt-in artifact test count with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`: 292 passed / 0 skipped.
- Current package parity is represented by the hashes in **Package Under Test** and by `scripts/check-installed-spire-plus-package.ps1`, which also checks the installed PCK's Sere Talon imported textures and Sere Talon / Tanx Claws content split.
- Historical loader/resource evidence remains useful context only. The 30-field loader proof covers the same DLL/PCK/manifest as the current package, but the current ZIP/README hash still needs a fresh loader row; gameplay, clicked UI, save-load, death/failure, route traversal, preview-tools, and co-op rows remain open.
- Historical detailed command logs are preserved in `docs/review.md`, `docs/archive/project-state-history-20260516.md`, and `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`. This handoff intentionally keeps only the current tester-facing summary.
- Automated evidence does not close clicked UI, live gameplay, save-load, death/failure, route traversal, preview-tools, or co-op rows.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
Historical RC1 notes now live at `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`. Current live/manual evidence should be summarized in `docs/release-evidence-status.md` and the evidence manifest generated by `scripts/collect-release-evidence.ps1 -NoLaunch` or `scripts/verify-spire-plus-release-evidence.ps1 -WriteTemplate`.

Current manual-proof summary:

- Current normal Steam-client startup/log verification passed for this 30-field package's DLL/PCK/manifest only; current ZIP/README startup/log verification is pending after the README wording refresh. The latest 30-field smoke remains useful same-DLL/PCK startup context only.
- Historical helper-driven startup/log verification confirms loader health only. This is loader/helper evidence, not live gameplay evidence.
- Current Mod Settings list screenshot shows `Spire Plus`; page-level BaseLib/old-display-name evidence remains historical.
- Normal Steam-client A0/A10/A20 DevConsole combat smoke, A11 map/save-load spot checks, saved-map boss-reachability graph proof, Act 2/3 A11 map-surface checks, and targeted A14 Rootblight hover/starter-notice spot checks passed.
- Live Ancient reward gameplay, Urda gameplay, Rootblight combat-end behavior/notices, natural A11 click-by-click traversal, disable-gameplay, broader save/load, and multiplayer checks remain pending.
Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Confirm BaseLib appears and is enabled.
4. Confirm Spire Plus appears with id `EZMicroBalance` and can be enabled. Historical normal Steam logs confirm the refreshed display name and config registration for earlier package states, and `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` captures `Spire Plus` in the Mods list.
5. Confirm legacy `EzDailyContent` is disabled or absent.
6. Start a run with BaseLib and Spire Plus enabled.
7. Execute the Ancient reward matrix, including Velvet Choker soft-limit counting, Distinguished Cape v4.3 max-HP math/pay gate with same-pool replacement and locked fallback for unaffordable Vakuu Cape rolls, Prismatic Gem all-off-color reroll/exclusion checks plus reward-screen hint fallback log checks, zhs numeric formatting, and the save/load rows.
8. Execute disable-mod gameplay verification.
9. Inspect `%APPDATA%\SlayTheSpire2\logs\godot.log` after the Steam-client pass.
10. Fill `.tools\runtime-evidence\release-ready-manual\release-evidence-manifest.json` and run `scripts\verify-spire-plus-release-evidence.ps1 -WritePassMarker` before treating the package as release-proven. Use `scripts\collect-release-evidence.ps1 -NoLaunch` to create the evidence folder plus manifest, or `-WriteTemplate` to create only the manifest.
    Keep the manifest, each row `EvidenceDir`, required file, screenshot path, and `command.txt` inside the evidence root. Unknown or blank rows appear in `Warnings` and do not satisfy any release row. The verifier hashes `publish\SpirePlus-v0.1.0-private-beta.8.zip` by default; pass `-PackagePath` only for a deliberate alternate package. Use `-AllowDeferred` only after an explicit owner-approved release-note deferral.

## Ascension Verification

A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Full live Ascension verification is pending. Live co-op selection and desync verification are still pending. No env var is needed for the default multiplayer test. Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Use `SPIREPLUS_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks.

Run `docs/features/ascension-11-20/manual-test-checklist.md` with default-on selection first, then repeat comparison rows with disable env vars:

- Use the original Ascension arrows to select A11-A20.
- `SPIREPLUS_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.
- A20 host multiplayer selection/start should log: multiplayer A20 selection is enabled for development testing, A20 Branded Form / second-boss enhanced dedicated ability gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

A20 multiplayer selection is not full A20 co-op support. A20 Branded Form / second-boss enhanced dedicated ability gameplay remains disabled or downgraded in co-op pending live verification. Co-op gameplay remains unverified. Execute `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the two-PC matrix, ownership/desync checks, save/load rows, and result template, or keep release notes clear that the candidate has source-patched host selection but no live co-op verification.

## Log Audit Helper

For each copied live `godot.log`, run:

```powershell
scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit
```

For known-failing diagnostic attempts, omit `-FailOnHit` so the JSON audit still records the signature counts without stopping the collection script.

## Live Session Helper

Use `scripts/spire-plus-live-session.ps1` for restore-safe normal Steam live-test setup when repeating manual rows locally.

- `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch` creates a `.tools\runtime-evidence\live-spire-plus-session-*` evidence folder, backs up Steam settings, enables only BaseLib plus Spire Plus, optionally moves current-run save files aside, and launches through Steam.
- Add `-DisableSpirePlus` with `-MoveOtherMods` for BaseLib-only plug-off startup/log evidence; the helper temporarily isolates `EZMicroBalance` because settings-only disable was proven insufficient.
- After screenshots/log notes, run `-Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore` for any session that starts or continues a run. The preserve switch moves test-created `current_run*` files into the evidence folder before restoring the user's original current-run files.
- The script was no-launch smoke-tested on 2026-05-13 for settings restore, mod isolation restore, current-run isolation, and preserve-new-current-run restore with byte-for-byte settings restoration.

Before capturing gameplay screenshots, run `scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\window-preflight.json -RequireSpireForeground`. If it exits nonzero, another app is covering the game or Slay the Spire 2 is not running; fix that before collecting screenshots. This avoids counting desktop captures that do not actually show Slay the Spire 2 gameplay.

## Release Artifact Test Mode

Normal developer tests do not require ignored `publish/`, staging, versioned, zip, installed DLL/PCK, or local smoke-log artifacts. After `dotnet publish EZMicroBalance.sln` and package staging/zip refresh, run:

```powershell
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

If `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` is set and artifacts are missing or stale, the release artifact tests should fail with missing-file or hash-mismatch details. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.

## Author Decision

Resolved for this candidate: `EZMicroBalance.json` author is `wenhuorongbing-netizen`, taken from the local Git user name.

## Rootblight Card Art Decision

Resolved for this candidate: Rootblight I/II/III and Blight Sprout use original generated portrait art at the documented per-card filenames. The art is procedural/source-local and does not use official Slay the Spire 2 assets. Live in-game visual verification is still pending.

## Review Scope

This handoff is not a commit manifest. Review the current diff directly before any release or push decision. Known broad surfaces include the single `EZMicroBalance` project/resources/code, Ascension and Ancient feature code, localization, export/package docs, guard tests, and validation scripts.

## Commit And Push Handoff

Do not trust a point-in-time dirty-file list in this document. Before any final release packaging, commit, or handoff, rerun:

```powershell
git status --short --branch
git log -1 --oneline --decorate
git diff --stat
```

Treat the output of those commands as authoritative. Keep only intentional source, resource, docs, localization, tests, and script changes in the commit scope.

Proposed commit scope after the remaining manual/user gates are resolved:

- Include the single `EZMicroBalance` project, manifest, solution, resource folder, code folder, localization, tests, and current release docs.
- Include historical doc archives needed to explain why older scaffold work is not part of the active single-mod package.
- Include `.gitignore` and export preset hardening for ignored local art, calibration, package output, tooling, and `source code/` scratch material.

Do not include:

- `Directory.Build.props`, `.godot/`, `.tools/`, `bin/`, `obj/`, `packages/`, `publish/`, local binaries, downloaded archives, or Steam/game runtime files.
- archived local `art_pipeline` / `asset` material under `.tools/archive/local-art-and-calibration-20260515/`, or `source code/` local scratch/reference folders.
- Any copied official Slay the Spire 2 assets or large decompiled method bodies.

Push only after explicit user approval.
