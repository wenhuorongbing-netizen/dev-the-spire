# Spire Plus Private Beta Verification Handoff

Date: 2026-06-21 previous beta.93 handoff summary; older May notes below are retained only as historical context.

**Environment warning (2026-05-08):** The earlier `godot2026-05-08T05.06.30.log` came from v0.105.0 with 17 mods loaded and previous package `v3.1.0`, so it is not the required STS2-RitsuLib + Spire Plus-only setup.

Current docs keep current beta.96 RitsuLib-only source/package evidence separated from older previous-package runtime evidence: historical installed-PCK resource smoke, historical current-display Mod Settings list evidence, normal-Steam A0/A10/A20 combat smoke, A11 map/save-load spot checks, Act 2/3 A11 map-surface observations, and targeted A14 Rootblight hover/starter-notice evidence.
Previous beta.93 RitsuLib-only Off loader proof is clean under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-off-direct-20260621`, and previous beta.93 RitsuLib-only AdditiveBatch1 registration proof is clean under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`.
Beta.96 RitsuLib Mod Settings page-level proof is captured under `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`. Full Ancient reward gameplay, clicked Ancient UI, natural A11 traversal, full Rootblight combat behavior, and co-op verification are pending.

Current naming note: testers should see `Spire Plus`. The technical manifest id and package folder remain `EZMicroBalance` for compatibility. Historical normal Steam-client startup/log evidence confirms the display name and config registration for an earlier package. Current beta.96 Mod Settings evidence at `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701` shows the RitsuLib Mods list with `RitsuLib` and `Spire Plus` only, and the Spire Plus config page with Migration Status, `STS2-RitsuLib >= 0.4.31`, evidence-boundary, technical-id, and Preview Tools controls.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

Latest package note, 2026-06-21: the package hashes below identify the current beta.96 RitsuLib-only manual-test package copied to the local game root for testing. Detailed historical pass logs live in `docs/archive/**` and feature work logs; `docs/review.md` keeps only compact current source-review context. This handoff keeps only current tester-facing facts.

Current source/package highlights:

- Root Eyes uses map-click selection for future reachable Monster, Unknown, or Elite nodes. Normal/elite previews read the generated Act room set, and selected encounters/events are committed only when the marked room is entered.
- Morvi, Lotha, Vakuu, RootBud, Banner, RootDeck, Ascension map helpers, and combat-only Ancient hook ownership have been split into focused service files without intended player-visible behavior changes.
- Latest source/package validation on 2026-06-21: `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false`, `dotnet publish EZMicroBalance.sln -m:1`, and package refresh passed for beta.96 after the RitsuLib `0.4.31` settings UI migration. Previous beta.93 RitsuLib-only Off and AdditiveBatch1 loader proof passed under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-off-direct-20260621` and `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`; `.tools\publish-game-root` is isolated tooling context only and is not the current package-parity source.
- `TESTER_START_HERE.md` in the current handoff starts with the installed-package checker command. The retained beta.87 AdditiveBatch1 loader smoke is clean previous-game evidence, beta.88 has previous package current-game loader proof, beta.90 has previous RitsuLib-only loader proof, beta.93 now has current RitsuLib-only Off and AdditiveBatch1 loader proof, and beta.96 has current RitsuLib Mod Settings clicked UI proof. Feature screenshots, gameplay logs, save-load, route traversal, preview-tools live behavior, event gameplay, and co-op evidence are still required before the release-evidence verifier can pass.
- The previous direct enabled-mode smoke under `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` covered the beta.88 package on Slay the Spire 2 `v0.107.1`: previous package `v3.3.0`, RitsuLib `0.4.24`, and Spire Plus loaded, RitsuLib selected compat branch `0.107.0`, `v0.1.0-private-beta.88` was logged, Spire Plus applied 25/25 ModPatcher patches, startup reached main menu, the log audit was clean, enabled-mode verifier passed 31 / 0, and packet verification had 0 mismatches. It is previous-package loader context only. Feature gameplay rows remain pending.
- StS1 event prototype note: beta.93 AdditiveBatch1 loader/registration proof is current for registration only and is not event gameplay proof.
  Use `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-gate-ledger.csv` for the current O0-O76 gate split, plus `docs/features/sts1-events/v20-final-gate-overlay.csv` for the O76-O84 final documentation/handoff overlay and `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` for the current v20 hard-stop/next-run start point; the ledgers are guarded by `scripts/check-sts1-v19-gate-ledger.ps1` and `scripts/check-sts1-v20-final-gate-overlay.ps1`.
Static source/doc checks passed for the beta.96 migration pass after latest RitsuLib-only source alignment.
Build, focused guards, publish/package refresh, installed package parity, hash-doc updates, runtime preflight, and source-workspace checks passed for the current package.
Previous beta.93 RitsuLib-only Off loader proof and AdditiveBatch1 registration proof remain loader/registration context only.
Beta.85 default-Off/CanaryOnly, beta.86 AdditiveBatch1, beta.87 AdditiveBatch1, beta.88 AdditiveBatch1, and beta.90 RitsuLib-only loader proof remain previous-package/game-version or previous-package contexts.
Gameplay, save-load, replacement, multiplayer, image/render, and QA gates remain pending or blocked.
Current AdditiveBatch1 enabled-mode proof remains loader/registration evidence only and does not prove gameplay.
  `docs/features/sts1-events/localization-source-gap-scan-20260611.md` records 33 source-referenced StS1 result-page keys missing from both EN and ZHS, and `docs/features/sts1-events/localization-gap-closure-plan.md` records the static closure order.
  Do not claim source-complete StS1 localization until that resource gap is closed, validated, and repackaged.
Closing only the direct Golden Idol missing key remains a localization unblocker, not gameplay proof or a replacement for verifier reports.
- Live gameplay, clicked Ancient UI, save-load, natural A11 route-click traversal, death/failure path, and co-op verification remain pending.

Browser GPTimage2 art rebuild recheck, 2026-05-15:

- Promoted Urda/Morvi/Lotha/Vakuu option relics, Ancient identity icons, Lotha Verdict, Ascension indicators, neutral fallback power/relic assets, and six custom Ancient card portraits into active resources.
- Review sheets are under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`, especially `active-small-art-contact.png` and `processed/batch5-card-portraits-contact.png`.
- Event-background repair promoted the Lotha mirror ensemble, reframed Urda and Morvi to 1831x859, and changed all three scenes to keep-aspect centered fitting. Preview sheet: `.tools/art-generation/event-background-reframe-20260515/active-event-backgrounds-1831x859-contact.png`.
- `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch -FailOnMissingFinal` passed with 95 manifest assets, 90 `final_generated`, and 0 missing/temporary/export/hash issues.
- Generic fallback, source-local small-art, and event-background provenance/ratio blockers are resolved. Live clicked-UI preview remains pending.

Older 2026-05-14 text/art-fit and source-guard rechecks are superseded by the current package snapshot above. Their detailed command history is preserved in `docs/archive/**` and feature work logs; `docs/review.md` keeps only the compact current review summary. Do not use those older hashes as the package under test.

Clicked Ancient UI handoff, 2026-05-15:

- Use the force-evidence protocol in `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`; it creates `.tools\runtime-evidence\ancient-ui-click-smoke-YYYYMMDD-HHMMSS`, records expected option counts, and restores the test session afterward.
- Preferred UI-smoke commands: `spireplus_test_ancient URDA confirm`, `spireplus_test_ancient MORVI confirm`, `spireplus_test_ancient LOTHA confirm`, `spireplus_test_ancient VAKUU confirm`, and `spireplus_test_ancient VAKUU confirm fight`.
- These commands start an unsaved single-player test run and refuse to run over an existing run. Expected visible option counts are Urda 4, Morvi 3, Lotha 3, and Vakuu 3 by default; the focused `fight` case has one fight option.
- The gated Vakuu fight can also be enabled with `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` or `EZMB_ENABLE_VAKUU_FIGHT=1`. It uses a dedicated enemy and encounter scene, but still needs post-victory no-black-screen proof.
- Legacy active-run DevConsole commands `ancient EZMB_URDA`, `ancient EZMB_MORVI`, `ancient EZMB_LOTHA`, and `ancient VAKUU` remain valid only after a run is already in progress. Prefer `spireplus_test_ancient ...` from the main menu, and mark all DevConsole routes as UI render smoke, not natural gameplay proof.

## Package Under Test

- Package: `publish\SpirePlus-v0.1.0-private-beta.96.zip`
- Player-facing name: `Spire Plus`
- Zip SHA256: `6E313D383E49B750E3C5809E92D7795CC5E196B5A7511707D2AB4357E24D4265`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `28D9AA1632B783CE34BC4D4174C5A84DEB26FD74947529656A71757BF660309F`
- Manifest SHA256: `A752A38EFF068FDB75B629D4A0DC92153D115EFD76D369B406E3D7077E4E4593`
- PCK SHA256: `549FD8B2A90B2AF74F8D6C591107F423588EFD868A61D1C901585E6FE188D20C`
- README_INSTALL SHA256: `C8171889B7B03E177CAC6428E4CCA3873BEEDB054180A10A7E6703DEBC72BDFE`

## Known Automated Evidence

- Current automated snapshot: beta.96 passed `dotnet build`, `dotnet publish`, package refresh, installed package parity, runtime preflight, and source-workspace parity after the settings UI migration.
- Current beta.96 RitsuLib Mod Settings page proof is `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`.
- Previous beta.93 Off evidence reached main menu with exactly `RitsuLib [STS2-RitsuLib] (0.4.31)` and `Spire Plus [EZMicroBalance] (v0.1.0-private-beta.93)` loaded, audited clean, and passed packet verification with 43 / 0 checks.
- Previous beta.93 AdditiveBatch1 evidence registered 10 event types through 14 calls, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verification with 61 / 0 checks. Beta.96 loader proof and gameplay proof are still pending.
- Previous split no-build test coverage after beta.85 packaging: the isolated `ReleaseEvidenceGateTests` class passed 9 passed / 0 skipped / 0 failed / 9 total, and the complementary test-project lane excluding `ReleaseEvidenceGateTests` passed 466 passed / 21 skipped / 0 failed / 487 total, for combined coverage of 475 passed / 21 skipped / 0 failed / 496 total. Fresh beta.86 AdditiveBatch1 loader smoke is previous-package context; the stale local `godot.log` from beta.0 was archived so it is not treated as current evidence.
- Current package parity is represented by the hashes in **Package Under Test** and by `scripts/check-installed-spire-plus-package.ps1 -ModDirectory "E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance"`, which also checks the packaged PCK's Sere Talon imported textures and Sere Talon / Tanx Claws content split.
- Historical loader/resource evidence remains useful context only.
- Retained beta.86 AdditiveBatch1 loader proof covers previous-package loader behavior only.
- Beta.87 has previous-game AdditiveBatch1 loader proof at `.tools\runtime-evidence\v01070-beta87-additive-batch1-direct-20260618-152531`.
- Beta.88 current-game loader proof is `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` and is previous-package context only.
- Beta.93 is previous-package RitsuLib-only loader/registration context after the beta.96 package bump.
- Beta.96 Mod Settings clicked UI proof is current for settings-page visibility only.
- Gameplay, clicked Ancient UI, save-load, death/failure, route traversal, preview-tools live behavior, event gameplay, and co-op rows remain open.
- Historical detailed command logs are preserved in `docs/archive/project-state-history-20260516.md`, `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`, and feature work logs; `docs/review.md` keeps compact current source-review context. This handoff intentionally keeps only the current tester-facing summary.
- Automated evidence does not close clicked UI, live gameplay, save-load, death/failure, route traversal, preview-tools, or co-op rows.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
Historical RC1 notes now live at `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`. Current live/manual evidence should be summarized in `docs/release-evidence-status.md` and the evidence manifest generated by `scripts/collect-release-evidence.ps1 -NoLaunch` or `scripts/verify-spire-plus-release-evidence.ps1 -WriteTemplate`.

Current manual-proof summary:

- Historical normal Steam-client startup/log verification passed for the beta.17 package hash and remains historical startup context only.
- Direct AdditiveBatch1 startup/log verification passed for the beta.93 package hash under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`; beta.93, beta.90, beta.88, beta.87, beta.85 Off/CanaryOnly, and beta.19 startup proof remain historical or previous-package context only for beta.96.
- Historical helper-driven startup/log verification confirms loader health only. This is loader/helper evidence, not live gameplay evidence.
- Current beta.96 Mod Settings proof is captured under `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`; the older current-display screenshot remains historical list context only.
- Normal Steam-client A0/A10/A20 DevConsole combat smoke, A11 map/save-load spot checks, saved-map boss-reachability graph proof, Act 2/3 A11 map-surface checks, and targeted A14 Rootblight hover/starter-notice spot checks passed.
- Live Ancient reward gameplay, Urda gameplay, Rootblight combat-end behavior/notices, natural A11 click-by-click traversal, disable-gameplay, broader save/load, and multiplayer checks remain pending.
Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Mod Settings row is already captured for beta.96 at `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`; recapture only if package, RitsuLib version, game version, or settings UI code changes.
4. Confirm legacy `EzDailyContent` is disabled or absent if you recapture the Mod Settings row.
5. Start a run with STS2-RitsuLib and Spire Plus enabled.
6. Execute the Ancient reward matrix, including Velvet Choker soft-limit counting, Distinguished Cape v4.3 max-HP math/pay gate with same-pool replacement and locked fallback for unaffordable Vakuu Cape rolls, Prismatic Gem all-off-color reroll/exclusion checks plus reward-screen hint fallback log checks, zhs numeric formatting, and the save/load rows.
7. Execute disable-mod gameplay verification.
8. Inspect `%APPDATA%\SlayTheSpire2\logs\godot.log` after the Steam-client pass.
9. Fill `.tools\runtime-evidence\manual-test-handoff-20260523-current\release\release-evidence-manifest.json` and run `scripts\verify-spire-plus-release-evidence.ps1 -WritePassMarker` before treating the package as release-proven. Use `scripts\collect-release-evidence.ps1 -NoLaunch` to create the evidence folder plus manifest, or `-WriteTemplate` to create only the manifest.
    Keep the manifest, each row `EvidenceDir`, required file, screenshot path, and `command.txt` inside the evidence root. Unknown or blank rows appear in `Warnings` and do not satisfy any release row. The verifier derives the default package path from `EZMicroBalance.json` and hashes that current ZIP by default; pass `-PackagePath` only for a deliberate alternate package. Use `-AllowDeferred` only after an explicit owner-approved release-note deferral.

## Ascension Verification

A11-A20 selection is default-on only for single-player standard lobbies.
After the 2026-05-25 co-op crash logs, host-multiplayer A11-A20 selection and gameplay fail closed by default until two-client proof exists.
Leave `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY` unset for normal testing; set it to `1` only for deliberate co-op debugging.
Full live Ascension verification is pending.
Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
`SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` remains a narrow multiplayer selector rollback switch.
`EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
Use `SPIREPLUS_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks.

Run `docs/features/ascension-11-20/manual-test-checklist.md` with default-on selection first, then repeat comparison rows with disable env vars:

- Use the original Ascension arrows to select A11-A20.
- `SPIREPLUS_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.
- A20 host multiplayer selection/start should log: multiplayer A20 selection is enabled for development testing, A20 Branded Form / second-boss enhanced dedicated ability gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

A20 multiplayer selection is not full A20 co-op support. A20 Branded Form / second-boss enhanced dedicated ability gameplay remains disabled or downgraded in co-op pending live verification. Co-op gameplay remains unverified. Live co-op selection and desync verification are still pending. Execute `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the two-PC matrix, ownership/desync checks, save/load rows, and result template, or keep release notes clear that the candidate has source-patched host selection without live co-op verification.

## Log Audit Helper

For each copied live `godot.log`, run:

```powershell
scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit
```

For known-failing diagnostic attempts, omit `-FailOnHit` so the JSON audit still records the signature counts without stopping the collection script.

## Live Session Helper

Use `scripts/spire-plus-live-session.ps1` for restore-safe normal Steam live-test setup when repeating manual rows locally.

- `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch` creates a `.tools\runtime-evidence\live-spire-plus-session-*` evidence folder, backs up Steam settings, enables only STS2-RitsuLib and Spire Plus, optionally moves current-run save files aside, and launches through Steam.
- Add `-DisableSpirePlus` with `-MoveOtherMods` for RitsuLib-only plug-off startup/log evidence; the helper temporarily isolates `EZMicroBalance` because settings-only disable was proven insufficient.
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

Push after validation and an intentional commit. Stop instead if validation fails, packaging fails, authentication is missing, or the push would include unrelated local changes.
