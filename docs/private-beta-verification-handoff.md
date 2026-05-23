# Spire Plus Private Beta Verification Handoff

Date: 2026-05-15

**Environment warning (2026-05-08):** The earlier `godot2026-05-08T05.06.30.log` came from v0.105.0 with 17 mods loaded and BaseLib `v3.1.0`, so it is not the required BaseLib+EZMB-only setup.

Current docs keep useful BaseLib `v3.1.4` source/package evidence separated from older `v3.1.2` runtime evidence: historical 22-field loader smoke, historical installed-PCK resource smoke, current Mod Settings list evidence, normal-Steam A0/A10/A20 combat smoke, A11 map/save-load spot checks, Act 2/3 A11 map-surface observations, and targeted A14 Rootblight hover/starter-notice evidence. Current source defines 26 SavedSpireFields and still needs a fresh loader smoke. Full Ancient reward gameplay, natural A11 traversal, full Rootblight combat behavior, and co-op verification are pending.

Current display-name note: the player-facing name is now `Spire Plus`, while manifest id and package folder remain `EZMicroBalance`. Historical normal Steam-client startup/log evidence confirms that display name and config registration for the earlier 22-field package. The refreshed Mod Settings UI capture `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` shows `Spire Plus` in the Mods list under the current display-name package.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

Latest package note, 2026-05-23: the package hashes below identify the current manual-test package copied to the local game root for testing. Detailed historical pass logs live in `docs/archive/**`, `docs/review.md`, and feature work logs; this handoff keeps only current tester-facing facts.

Current source/package highlights:

- Root Eyes uses map-click selection for future reachable Monster, Unknown, or Elite nodes. Normal/elite previews read the generated Act room set, and selected encounters/events are committed only when the marked room is entered.
- Morvi, Lotha, Vakuu, RootBud, Banner, RootDeck, Ascension map helpers, and combat-only Ancient hook ownership have been split into focused service files without intended player-visible behavior changes.
- Latest no-launch verification: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` with 258 passed / 18 skipped, `dotnet format`, `git diff --check`, website syntax checks, and Vakuu/co-op evidence-helper no-launch template smokes passed after the package no-refresh guard update. Latest package refresh evidence also includes `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, `scripts/package-spire-plus.ps1 -NoRefreshFromInstalled`, opt-in artifact tests with 276 passed / 0 skipped, and installed game-root artifact hash parity.
- The latest normal Steam smoke under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` is historical for the earlier 22-field package. Current source defines 26 SavedSpireFields and needs a fresh loader smoke before any current-package runtime-parity claim.
- Live gameplay, clicked Ancient UI, save-load, natural A11 route-click traversal, death/failure path, and co-op verification remain pending.

Browser GPTimage2 art rebuild recheck, 2026-05-15:

- Promoted Urda/Morvi/Lotha/Vakuu option relics, Ancient identity icons, Lotha Verdict, Ascension indicators, neutral fallback power/relic assets, and six custom Ancient card portraits into active resources.
- Review sheets are under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`, especially `active-small-art-contact.png` and `processed/batch5-card-portraits-contact.png`.
- Event-background repair promoted the Lotha mirror ensemble, reframed Urda and Morvi to 1831x859, and changed all three scenes to keep-aspect centered fitting. Preview sheet: `.tools/art-generation/event-background-reframe-20260515/active-event-backgrounds-1831x859-contact.png`.
- `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch -FailOnMissingFinal` passed with 82 manifest assets, 69 `final_generated`, and 0 missing/temporary/export/hash issues.
- Generic fallback, source-local small-art, and event-background provenance/ratio blockers are resolved. Live clicked-UI preview remains pending.

V22 text/art-fit recheck, 2026-05-14: reran art export audit, build, normal tests, format, diff-check, publish, package refresh, post-package build, post-hash normal tests, and opt-in release artifact tests. Results were clean: `dotnet build` passed with 0 warnings/errors, normal tests passed with 150 passed / 18 skipped / 0 failed, opt-in artifact tests passed with 168 passed / 0 skipped / 0 failed, and the package hashes below match installed/staging/versioned/zip artifacts. This historical recheck is superseded by the 2026-05-15 browser GPTimage2 rebuild and event-background repair above; live gameplay, clicked UI, save-load, failure/death, route traversal, co-op, and live event-background preview remain pending.

Source-audited text correction recheck, 2026-05-14: reran JSON parse checks, build, normal tests, format, diff-check, publish, package refresh, post-hash rebuild/test, and opt-in artifact tests after correcting Rootblight, Marginal Note, Red Ink Overdraft, Seedbed, After the Rain, Forbidden Loan, and Debt Settlement player-facing text. `dotnet build` passed with 0 warnings/errors; normal tests passed with 150 passed / 18 skipped / 0 failed; opt-in artifact tests passed with 168 passed / 0 skipped / 0 failed; `dotnet format --verify-no-changes` and `git diff --check` passed. The package hashes below match the refreshed installed/staging/versioned/zip artifacts.

Source-guard follow-up, 2026-05-14: Mirror Rebuttal's full-hand fallback remains guarded, and the Single Sentence branch-specific residual risk is closed by method-scoped source tests for pre-ruling Power fallback, the four-card post-ruling cap, autoplay/clone/non-first/ruling-card exclusions, and EN/zhs stale-text prevention. This follow-up changed tests/docs only, so the package hashes below were not refreshed.

Clicked Ancient UI handoff, 2026-05-15:

- Use the force-evidence protocol in `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`; it creates `.tools\runtime-evidence\ancient-ui-click-smoke-YYYYMMDD-HHMMSS`, records expected option counts, and restores the test session afterward.
- Preferred UI-smoke commands: `spireplus_test_ancient URDA confirm`, `spireplus_test_ancient MORVI confirm`, `spireplus_test_ancient LOTHA confirm`, `spireplus_test_ancient VAKUU confirm`, and `spireplus_test_ancient VAKUU confirm fight`.
- These commands start an unsaved single-player test run and refuse to run over an existing run. Expected visible option counts are Urda 4, Morvi 3, Lotha 3, and Vakuu 3 by default; the focused `fight` case has one fight option.
- The gated Vakuu fight can also be enabled with `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` or `EZMB_ENABLE_VAKUU_FIGHT=1`. It uses a dedicated enemy and encounter scene, but still needs post-victory no-black-screen proof.
- Vanilla active-run DevConsole commands `ancient EZMB_URDA`, `ancient EZMB_MORVI`, `ancient EZMB_LOTHA`, and `ancient VAKUU` remain valid only after a run is already in progress. Mark all DevConsole routes as UI render smoke, not natural gameplay proof.

## Package Under Test

- Package: `publish\SpirePlus-v0.1.0-private-beta.0.zip`
- Player-facing name: `Spire Plus`
- Zip SHA256: `11CCD08698F72F4A27547E0FB0D4E7793323ED729DA7CFE3F548CC39F4C51120`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `C9030FCDC459E35256B00CE71925854AF44CFE97667DEB07F751CEA540C86522`
- Manifest SHA256: `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6`
- PCK SHA256: `68F6DCCC5564AE402B3FFB1DD9A65B92555B58CF72282E8EC64FF273EEC4E0F8`
- README_INSTALL SHA256: `2441D012F12D0FB81BCAF7E1C99B1E60F18187937B9911D7D4FD54ACC47BCC6A`

## Known Automated Evidence

- `dotnet build EZMicroBalance.sln --no-restore`: initial exact run failed because `Godot.NET.Sdk/4.5.1` was not visible without the local NuGet cache; rerun with `NUGET_PACKAGES=C:\Users\Jack\.nuget\packages` passed on 2026-05-14 with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: first run after the Lotha guard update exposed one over-broad `PowerFallbackEnergy` assertion; after tightening the assertion and rebuilding, rerun passed with 125 passed, 18 skipped, 0 failed.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed and generated the package PCK with the known nested `source code/project.godot` warning.
- `scripts/package-spire-plus.ps1`: rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip` from the configured installed artifacts.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed with installed/staging/versioned/zip artifact parity.
- `dotnet build EZMicroBalance.sln --no-restore`: passed on 2026-05-14 after the clicked-Ancient UI hardening pass with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed on 2026-05-14 after the clicked-Ancient UI hardening pass, 114 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed on 2026-05-14 after the clicked-Ancient UI hardening pass; refreshed installed DLL/manifest/PCK with the known nested `source code/project.godot` warning.
- `scripts/package-spire-plus.ps1`: rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip` after the clicked-Ancient UI hardening pass.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: first run after the clicked-Ancient UI package refresh exposed stale documented hashes, then passed after hash doc updates with 132 passed, 0 skipped, 0 failed.
- `dotnet build EZMicroBalance.sln`: passed on 2026-05-13 after the BaseLib-only plug-off startup/log refresh with 0 warnings and 0 errors.
- `dotnet build EZMicroBalance.sln --no-restore`: passed on 2026-05-14 after the Vakuu Temptation implementation with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed on 2026-05-14 after the Vakuu Temptation implementation, 109 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- `dotnet test EZMicroBalance.sln --no-build`: passed again on 2026-05-14 after the current-package smoke/log/resource documentation update, 109 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- `dotnet test EZMicroBalance.sln -c Release`: passed on 2026-05-13 after the BaseLib-only plug-off startup/log refresh, 81 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed on 2026-05-14 after the source red-team hardening package/hash documentation refresh, 127 passed, 0 skipped, 0 failed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed again on 2026-05-14 after the current-package smoke/log/resource documentation update, 127 passed, 0 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed after the Vakuu Temptation implementation.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed on 2026-05-14 after the source red-team hardening pass; refreshed the installed/package DLL, manifest, PCK, package README, and `SpirePlus` private-beta zip.
- `git diff --check`: exit 0 after the current-package smoke/log/resource documentation update, with CRLF normalization warnings only.
- Previous package hash parity: `.tools\runtime-evidence\current-package-smoke-20260514-015901\artifact-hash-parity.json` verifies installed, staging, versioned, and zip-entry DLL/PCK/manifest/README hashes for the earlier package. The refreshed package above has automated artifact parity; the latest live loader smoke is historical for the earlier 22-field package, and gameplay proof remains pending.
- Historical installed-PCK Ancient resource verification: `.tools\runtime-evidence\current-package-smoke-20260514-015901\godot-ancient-resource-load-summary.json` reports exit code 0, `HasOkMarker: true`, 0 error lines, and 0 warning lines while loading Urda/Morvi/Lotha scenes and Ancient textures from the installed PCK. Static coverage in `ancient-resource-localization-coverage.json` found 0 missing assets and 0 missing EN/zhs localization keys. This is resource-load evidence, not fresh 30-field loader parity.
- Historical normal Steam-client helper startup/log evidence under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` used `scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`, audited `godot.log`, then restored with `-StopGameOnRestore -PreserveNewCurrentRunsOnRestore`.
  Positive evidence: BaseLib `177 patches successfully, 0 failed`, config registered for `EZMicroBalance`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, and `Time to main menu: 13,539ms`. Scans found 0 `ERROR`, BaseLib patch-failure, or Spire Plus / `EZMicroBalance` exception hits. Current source defines 26 SavedSpireFields, so this remains historical until a fresh live loader smoke is run.
- Previous controlled `--force-steam off` smoke evidence under `.tools\runtime-evidence\current-package-smoke-20260513-044306` temporarily enabled BaseLib and Spire Plus / EZMicroBalance in the default profile, loaded exactly 2 mods (`Loaded 2 mods (22 total)`), initialized BaseLib and Spire Plus, reported `Found 16 SavedSpireFields`, reached main menu in `13,884ms`, found 0 Spire Plus / EZMicroBalance error signatures, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte. The audit is not fully clean because unrelated disabled local-mod manifest/name noise from RouteSuggest, sts2-heybox-support, and DamageMeter remains in this developer mods folder.
- Previous normal Steam-client isolated startup/log evidence under `.tools\runtime-evidence\current-spire-plus-normal-steam-20260513-054241` launched through Steam with only BaseLib and `EZMicroBalance` enabled.
  Positive evidence: manifest list `0: BaseLib (BaseLib)` and `1: Spire Plus (EZMicroBalance)`, config registered, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, `Time to main menu: 12,790ms`, and `scripts/audit-godot-log.ps1` reported `Clean: true` with 0 `ERROR` lines.
- Previous helper-driven normal Steam startup/log evidence under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` prepared a restore-safe session, enabled only BaseLib and `EZMicroBalance`, launched through Steam, and restored the session afterward.
  Positive evidence: config registered, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, `Time to main menu: 13,849ms`, and clean `godot-log-audit.json`. This is loader/helper evidence, not live gameplay evidence.
- BaseLib-only plug-off normal Steam startup/log evidence under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` used the helper with `-DisableSpirePlus` to temporarily isolate `EZMicroBalance` out of the mods folder, launched through Steam, and restored settings, the current-run save, and 25 moved entries afterward. Positive evidence: `Loaded 1 mods (1 total)`, `Finished mod initialization for 'BaseLib' (BaseLib)`, no Spire Plus / `EZMicroBalance` initialization markers in `disabled-startup-summary.json`, and a clean `godot-log-audit.json`. This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.
- RC1 normal Steam-client isolated startup log started Slay the Spire 2 through `D:\Steam\steam.exe -applaunch 2868840`, temporarily isolated non-BaseLib/EZMB local mod entries, loaded to main menu with `Loaded 2 mods (2 total)`, BaseLib `v3.1.2`, BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialization, `Found 13 SavedSpireFields`, 0 startup `ERROR` lines, and 0 release-blocking signatures. Snapshot: `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`. The moved mod entries and `settings.save` were restored afterward.
- Current Mod Settings UI list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` launched through Steam with non-BaseLib/EZMB mods temporarily moved aside, enabled only BaseLib and `EZMicroBalance`, reached main menu, captured `02-mod-config-list.png` with `Spire Plus` visible in the Mods list, copied `godot.log`, restored settings byte-for-byte, and restored moved mods. That UI-capture log has `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, 0 `ERROR` lines, and 0 release-blocking signatures; its field count is historical and superseded by the current 30-field source state.
- Current A14 Rootblight generated-art hover probe under `.tools\runtime-evidence\current-rootblight-art-hover-20260513-114103` is negative evidence: it reached the default-on Urda Ancient event before combat and exposed missing vanilla-derived Urda asset paths.
  Source/package now use BaseLib `CustomAncientModel` custom Urda icon/background-scene paths and include the Urda background scene resource. Headless installed-PCK resource-load evidence under `.tools\runtime-evidence\urda-pck-resource-load-20260513-123345` resolved the Urda scene/image, emitted `URDA_RESOURCE_LOAD_OK`, and had 0 `ERROR` / `WARNING` lines. Post-fix live Urda and Rootblight visual/gameplay verification remains pending.
- RC1 normal Steam-client Mod Settings verification passed after adding the no-op BaseLib config page under the historical EZ Micro Balance display name. Evidence screenshots: `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-modconfig.png` for BaseLib, `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` for the historical localized page entry, and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` for that settings page. Log snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- RC1 A11 Act 1 map/save-load spot check launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11, confirmed `columns=8; rows=17` with `inserted 1 late route row(s)`, saved after the first node, continued the run, and reopened the map after load with the same geometry.
  Evidence lives under `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008`, including screenshots, `a11-map-save-load-godot-live.log`, `a11-save-map-dimensions.json`, and `a11-boss-reachability-from-save.json`. The graph proof shows a 17-node path from post-load coord `(3,1)` to boss `(3,17)`. Natural click-by-click traversal through the whole path remains pending.
- RC1 A11 Act 2/3 map-surface observation launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11 through the original single-player Ascension arrows, reached the Act 1 map normally, then used DevConsole `act 2` and `act 3` only to inspect later-act map surfaces. Evidence: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355\19-character-select-a11.png`, `25-a11-act2-map-clean.png`, `27-a11-act3-map-clean.png`, and `a11-act23-godot-live.log`. The log records Act 2 `columns=8; rows=16` with 1 late row and Act 3 `columns=8; rows=16` with 2 late rows, with 0 `ERROR` lines and 0 release-blocking signatures. Natural click-by-click route traversal remains pending.
- Rootblight targeted normal Steam-client evidence captured A14 Neow starter notices and Rootblight I/II/III plus Blight Sprout hovers in English and Simplified Chinese.
  Evidence folders: `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010`, `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516`, and `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455`. Combat-end notices and generated art are source/package hardened, but clean non-paused timing, full behavior, visual verification, and co-op ownership/desync checks remain pending.
- Urda is source-patched and packaged for default-on private-beta testing: Seedbed, Humus Pact, Molting, and Moss Map have first-pass gameplay hooks plus EN/ZHS custom card and Seedbed alternative localization. Live Urda selection, reward-screen behavior, save/load, UI, and co-op verification remain pending.
- Latest source/package refresh includes Urda act-entry/Moss Map active-player filtering, Morvi combat-start active-player filtering, owned/non-removed deck-card recovery, Urda UI/art repair, default-on Lotha, and the hidden-by-default single-player Vakuu fight. Vakuu now has a dedicated enemy and encounter scene, but still needs live victory, save-load, failure-path, and clicked-UI proof before being exposed normally.
- A12/A16/A19 modifier-variety and preview pass is source-patched and packaged: Firemark/Banner assignment uses stable seed/act/coord hashing with act-level kind shuffle, Firemarked Elite/Banner/Boss hover previews expose exact modifier summaries, and Fission diagnostics log source label/chance/eligible count/roll/applied/card id under `EZMB_ASCENSION_DIAGNOSTICS=1`. Live multi-seed, save/load, hover rendering, and effect-match verification remain pending.
- Multiplayer join mismatch diagnostics are source-patched and packaged: when vanilla would show the generic "game version differs" popup for a release-version, gameplay-mod, or ModelDb hash mismatch, Spire Plus logs host/local version, host/local ModelDb hash, gameplay-relevant mod lists, and missing mod deltas before the disconnect. This does not bypass vanilla's ModelDb hash check.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
Historical RC1 notes now live at `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`. Current live/manual evidence should be summarized in `docs/release-evidence-status.md` and the evidence manifest generated by `scripts/collect-release-evidence.ps1 -NoLaunch` or `scripts/verify-spire-plus-release-evidence.ps1 -WriteTemplate`.

Current manual-proof summary:

- Historical normal Steam-client startup/log verification passed for an earlier Spire Plus display-name package and remains loader evidence only until rerun for the current 30-field package.
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
    Keep the manifest, each row `EvidenceDir`, required file, screenshot path, and `command.txt` inside the evidence root. Unknown or blank rows appear in `Warnings` and do not satisfy any release row. The verifier hashes `publish\SpirePlus-v0.1.0-private-beta.0.zip` by default; pass `-PackagePath` only for a deliberate alternate package. Use `-AllowDeferred` only after an explicit owner-approved release-note deferral.

## Ascension Verification

A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Full live Ascension verification is pending. Live co-op selection and desync verification are still pending. No env var is needed for the default multiplayer test. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Use `EZMB_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks.

Run `docs/features/ascension-11-20/manual-test-checklist.md` with default-on selection first, then repeat comparison rows with disable env vars:

- Use the original Ascension arrows to select A11-A20.
- `EZMB_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.
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

- `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch` creates a `.tools\runtime-evidence\live-spire-plus-session-*` evidence folder, backs up Steam settings, enables only BaseLib plus Spire Plus / `EZMicroBalance`, optionally moves current-run save files aside, and launches through Steam.
- Add `-DisableSpirePlus` with `-MoveOtherMods` for BaseLib-only plug-off startup/log evidence; the helper temporarily isolates `EZMicroBalance` because settings-only disable was proven insufficient.
- After screenshots/log notes, run `-Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore` for any session that starts or continues a run. The preserve switch moves test-created `current_run*` files into the evidence folder before restoring the user's original current-run files.
- The script was no-launch smoke-tested on 2026-05-13 for settings restore, mod isolation restore, current-run isolation, and preserve-new-current-run restore with byte-for-byte settings restoration.

Before capturing gameplay screenshots, run `scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\window-preflight.json -RequireSpireForeground`. If it exits nonzero, another app is covering the game or Slay the Spire 2 is not running; fix that before collecting screenshots. This avoids counting desktop captures that do not actually show Slay the Spire 2 gameplay.

## Release Artifact Test Mode

Normal developer tests do not require ignored `publish/`, staging, versioned, zip, installed DLL/PCK, or local smoke-log artifacts. After `dotnet publish EZMicroBalance.sln` and package staging/zip refresh, run:

```powershell
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

If `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set and artifacts are missing or stale, the release artifact tests should fail with missing-file or hash-mismatch details.

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
