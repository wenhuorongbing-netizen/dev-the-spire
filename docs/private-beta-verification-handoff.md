# Spire Plus Private Beta Verification Handoff

Date: 2026-06-23 beta.129 handoff summary; older May/June package notes below are retained only as historical context.

**Environment warning (2026-05-08):** The earlier `godot2026-05-08T05.06.30.log` came from v0.105.0 with 17 mods loaded and previous package `v3.1.0`, so it is not the required STS2-RitsuLib + Spire Plus-only setup.

Current docs keep current beta.129 RitsuLib-only package/source validation separated from older previous-package runtime evidence: beta.129 package parity, runtime preflight, and source-workspace validation are current; previous beta.128 clicked Ancient UI smoke, beta.99 RitsuLib Mod Settings page proof, beta.96 Off loader proof, historical installed-PCK resource smoke, historical current-display Mod Settings list evidence, normal-Steam A0/A10/A20 combat smoke, A11 map/save-load spot checks, Act 2/3 A11 map-surface observations, and targeted A14 Rootblight hover/starter-notice evidence are previous-package or feature-specific historical context.
Previous beta.96 RitsuLib-only Off loader proof is clean under `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056`, and previous beta.93 RitsuLib-only AdditiveBatch1 registration proof is clean previous-package context under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`.
Previous beta.128 clicked Ancient UI smoke proof is captured under `.tools\runtime-evidence\monkey-stability-20260623-062913`; it passed 4 / 4 forced UI iterations for Urda, Morvi, Lotha, and normal Vakuu, applied 152/152 default runtime Spire Plus patches, and passed packet verification 1621 / 0. Full Ancient reward gameplay, gated Vakuu fight-option/victory return, natural A11 traversal, full Rootblight combat behavior, and co-op verification are pending.

Current naming note: testers should see `Spire Plus`. The technical manifest id and package folder remain `EZMicroBalance` for compatibility.
Historical normal Steam-client startup/log evidence confirms the display name and config registration for an earlier package.
Previous beta.99 Mod Settings evidence at `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210` shows the RitsuLib Mods list with `RitsuLib` and `Spire Plus` only.
It also shows the Spire Plus config page with Migration Status, `STS2-RitsuLib >= 0.4.34`, evidence-boundary, technical-id, and Preview Tools controls, plus a clean same-session log audit and StS1 Off runtime shape verification 21 / 0.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

Latest package note, 2026-06-23: the package hashes below identify the current beta.129 RitsuLib-only manual-test package copied to the local game root for testing. Detailed historical pass logs live in `docs/archive/**` and feature work logs; `docs/review.md` keeps only compact current source-review context. This handoff keeps only current tester-facing facts.

Current source/package highlights:

- Root Eyes uses map-click selection for future reachable Monster, Unknown, or Elite nodes. Normal/elite previews read the generated Act room set, and selected encounters/events are committed only when the marked room is entered.
- Morvi, Lotha, Vakuu, RootBud, Banner, RootDeck, Ascension map helpers, and combat-only Ancient hook ownership have been split into focused service files without intended player-visible behavior changes.
- Latest source/package validation on 2026-06-23: `dotnet build`, `dotnet publish`,
  package refresh, installed package parity, runtime preflight, and source-workspace
  validation passed for beta.129 after the RitsuLib `0.4.34` dependency refresh,
  SavedAttachedState initialization hardening, RitsuLib default localization aliases,
  low-risk Ancient reward hook migration, and Urda Root Sight room-routing
  migration. beta.129 clicked UI proof is pending. Previous beta.96 RitsuLib-only Off loader
  proof passed under `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056`.
  Previous beta.93 AdditiveBatch1 loader/registration proof remains previous-package
  context under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`;
  `.tools\publish-game-root` is isolated tooling context only and is not the current
  package-parity source.
- `TESTER_START_HERE.md` in the current handoff starts with the installed-package checker command. The retained beta.87 AdditiveBatch1 loader smoke is clean previous-game evidence, beta.88 has previous package current-game loader proof, beta.90 has previous RitsuLib-only loader proof, beta.93 has previous-package AdditiveBatch1 loader/registration proof, beta.96 has previous-package RitsuLib-only Off loader proof, beta.99 has previous-package RitsuLib Mod Settings clicked UI proof, beta.128 has previous-package clicked Ancient UI smoke proof, and beta.129 has current no-launch package/source validation.
  Gameplay logs, save-load, route traversal, preview-tools live behavior, gated Vakuu fight-option/victory return, current enabled-mode event gameplay, and co-op evidence are still required before the release-evidence verifier can pass.
- The previous direct enabled-mode smoke under `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` covered the beta.88 package on Slay the Spire 2 `v0.107.1`: previous package `v3.3.0`, RitsuLib `0.4.24`, and Spire Plus loaded, RitsuLib selected compat branch `0.107.0`, `v0.1.0-private-beta.88` was logged, Spire Plus applied 25/25 ModPatcher patches, startup reached main menu, the log audit was clean, enabled-mode verifier passed 31 / 0, and packet verification had 0 mismatches. It is previous-package loader context only. Feature gameplay rows remain pending.
- StS1 event prototype note: beta.93 AdditiveBatch1 loader/registration proof is retained previous-package registration evidence only and is not event gameplay proof.
  Use `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-gate-ledger.csv` for the current O0-O76 gate split, plus `docs/features/sts1-events/v20-final-gate-overlay.csv` for the O76-O84 final documentation/handoff overlay and `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` for the current v20 hard-stop/next-run start point; the ledgers are guarded by `scripts/check-sts1-v19-gate-ledger.ps1` and `scripts/check-sts1-v20-final-gate-overlay.ps1`.
Static source/doc checks passed for the beta.96 migration pass after latest RitsuLib-only source alignment.
The beta.129 pass updates the package and must use the validation section below as the current package truth.
Build, focused guards, publish/package refresh, installed package parity, hash-doc updates, runtime preflight, and source-workspace checks passed for the current package.
Previous beta.99 settings/off proof, beta.96 RitsuLib-only Off loader proof, and beta.93 AdditiveBatch1 registration proof remain previous-package context only after the beta.129 package refresh.
Beta.85 default-Off/CanaryOnly, beta.86 AdditiveBatch1, beta.87 AdditiveBatch1, beta.88 AdditiveBatch1, and beta.90 RitsuLib-only loader proof remain previous-package/game-version or previous-package contexts.
Gameplay, save-load, replacement, multiplayer, image/render, and QA gates remain pending or blocked.
Retained AdditiveBatch1 enabled-mode proof remains previous-package loader/registration evidence only and does not prove beta.129 gameplay.
  `docs/features/sts1-events/localization-source-gap-scan-20260611.md` records 33 source-referenced StS1 result-page keys missing from both EN and ZHS, and `docs/features/sts1-events/localization-gap-closure-plan.md` records the static closure order.
  Do not claim source-complete StS1 localization until that resource gap is closed, validated, and repackaged.
Closing only the direct Golden Idol missing key remains a localization unblocker, not gameplay proof or a replacement for verifier reports.
- Live gameplay, gated Vakuu fight-option UI, save-load, natural A11 route-click traversal, death/failure path, and co-op verification remain pending.

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

- Package: `publish\SpirePlus-v0.1.0-private-beta.129.zip`
- Player-facing name: `Spire Plus`
- Zip SHA256: `20077741751E2252254987466E213A3CF119AE04F1BFFE7115FC96099C895FF4`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `626871157BA5C898ABDD52081E6B107E9E3AB57B5E27B88F5E9D93296BB04CC1`
- Manifest SHA256: `2AF698BFB78B87B2AA471083F3255A5B2C704A9A12C7E582ED2A42B7B4CFEE79`
- PCK SHA256: `8BCFCD730EE68416827B7E7A200DE8CDE27ACCD9FEE4A1E97B3346C0AEF9F3CE`
- README_INSTALL SHA256: `234C405E77CCFF940EB24CA5778E480DCE47B0512D48427FF3CC4D621F801E38`

## Known Automated Evidence

- Current automated snapshot: beta.129 passed `dotnet build` with 0 warnings / 0 errors, `dotnet publish` with only the known Godot ignored `source code` project warning, package refresh, installed-package parity, runtime preflight 28 / 0, and current source-workspace validation 57 checks / 0 mismatches with retained GDRE warnings only and local RitsuLib XML/API marker coverage.
- Latest clicked Ancient UI smoke proof remains previous beta.128 evidence: `.tools\runtime-evidence\monkey-stability-20260623-062913`; 4 / 4 `AncientUiSmoke` iterations passed for `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU`, with command ACKs, foreground screenshots, clean log audits, StS1 Off verifier pass, exact game/Ritsu/package markers, 152/152 default runtime Spire Plus patches applied, and packet verification 1621 / 0.
- Previous beta.99 RitsuLib Mod Settings page proof is `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`; it rendered the RitsuLib I18N settings page, retained same-session `godot.log`, audited clean, and passed StS1 Off runtime shape verification 21 / 0. Treat it as previous-package settings-page context after beta.129.
- Previous beta.96 Off evidence reached main menu with exactly `RitsuLib [STS2-RitsuLib] (0.4.31)` and `Spire Plus [EZMicroBalance] (v0.1.0-private-beta.96)` loaded, audited clean, logged StS1Events disabled with 0 registration lines, and passed packet verification with 43 / 0 checks.
- Previous beta.99 Off loader proof is `.tools\runtime-evidence\v01071-beta99-ritsulib0432-off-direct-20260621-234221`; it reached main menu with exactly STS2-RitsuLib and Spire Plus loaded, audited clean, logged StS1Events disabled with 0 registration lines, passed Off verifier 21 / 0, and passed packet verification with 43 / 0 checks.
- Previous beta.93 AdditiveBatch1 evidence registered 10 event types through 14 calls, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verification with 61 / 0 checks for the previous package only. Current beta.129 enabled-mode proof and gameplay proof are still pending.
- Previous split no-build test coverage after beta.85 packaging: the isolated `ReleaseEvidenceGateTests` class passed 9 passed / 0 skipped / 0 failed / 9 total, and the complementary test-project lane excluding `ReleaseEvidenceGateTests` passed 466 passed / 21 skipped / 0 failed / 487 total, for combined coverage of 475 passed / 21 skipped / 0 failed / 496 total. Fresh beta.86 AdditiveBatch1 loader smoke is previous-package context; the stale local `godot.log` from beta.0 was archived so it is not treated as current evidence.
- Current package parity is represented by the hashes in **Package Under Test** and by `scripts/check-installed-spire-plus-package.ps1 -ModDirectory "E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance" -ExpectedPackageVersion "v0.1.0-private-beta.129"`, which also checks the packaged PCK's Sere Talon imported textures and Sere Talon / Tanx Claws content split.
- Historical loader/resource evidence remains useful context only.
- Retained beta.86 AdditiveBatch1 loader proof covers previous-package loader behavior only.
- Beta.87 has previous-game AdditiveBatch1 loader proof at `.tools\runtime-evidence\v01070-beta87-additive-batch1-direct-20260618-152531`.
- Beta.88 current-game loader proof is `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` and is previous-package context only.
- Beta.93 is previous-package RitsuLib-only enabled-mode loader/registration context after the beta.96 package bump.
- Beta.99 Mod Settings clicked UI proof is previous-package settings-page visibility context after beta.129, while beta.96 Off loader proof is previous-package startup/loading context after the beta.99 package bump.
- Smoke-level clicked Ancient UI is covered by previous beta.128 package evidence for Urda, Morvi, Lotha, and normal Vakuu. Gameplay, gated Vakuu fight-option/victory return, save-load, death/failure, route traversal, preview-tools live behavior, event gameplay, and co-op rows remain open.
- Historical detailed command logs are preserved in `docs/archive/project-state-history-20260516.md`, `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`, and feature work logs; `docs/review.md` keeps compact current source-review context. This handoff intentionally keeps only the current tester-facing summary.
- Automated evidence closes only the smoke-level clicked Ancient UI rows above; it does not close live gameplay, gated Vakuu fight-option/victory return, save-load, death/failure, route traversal, preview-tools, or co-op rows.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
Historical RC1 notes now live at `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`. Current live/manual evidence should be summarized in `docs/release-evidence-status.md` and the evidence manifest generated by `scripts/collect-release-evidence.ps1 -NoLaunch` or `scripts/verify-spire-plus-release-evidence.ps1 -WriteTemplate`.

Current manual-proof summary:

- Historical normal Steam-client startup/log verification passed for the beta.17 package hash and remains historical startup context only.
- Direct Off startup/log verification passed for the beta.96 package hash under `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056` and remains previous-package context only for beta.129; direct AdditiveBatch1 startup/log verification passed for the beta.93 package hash under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621` and remains previous-package context only for beta.129.
- Historical helper-driven startup/log verification confirms loader health only. This is loader/helper evidence, not live gameplay evidence.
- Previous beta.99 Mod Settings proof is captured under `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`; older current-display screenshots remain historical context only.
- Normal Steam-client A0/A10/A20 DevConsole combat smoke, A11 map/save-load spot checks, saved-map boss-reachability graph proof, Act 2/3 A11 map-surface checks, and targeted A14 Rootblight hover/starter-notice spot checks passed.
- Live Ancient reward gameplay, Urda gameplay, Rootblight combat-end behavior/notices, natural A11 click-by-click traversal, disable-gameplay, broader save/load, and multiplayer checks remain pending.
Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Mod Settings row was last recaptured for beta.99 at `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`; rerun it only if settings UI code/resources or RitsuLib version change again.
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
