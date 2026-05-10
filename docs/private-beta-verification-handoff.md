# EZ Micro Balance Private Beta Verification Handoff

Date: 2026-05-09

**Environment warning (2026-05-08):** The earlier live test log (`godot2026-05-08T05.06.30.log`) was collected in a v0.105.0 environment with 17 mods loaded and BaseLib `v3.1.0`, not the required BaseLib+EZMB-only setup. Current package evidence uses BaseLib `v3.1.2`, a controlled BaseLib+EZMB-only smoke, normal Steam-client Mod Settings evidence, normal-Steam A0/A10/A20 combat smoke, an Act 1 A11 map/save-load spot check, Act 2/3 A11 map-surface observations, and targeted A14 Rootblight English/ZHS hover/starter-notice evidence. Full Ancient reward gameplay, natural A11 traversal, full Rootblight combat behavior, and co-op verification are still pending.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

## Package Under Test

- Package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip`
- Zip SHA256: `4E22172A2078DBCD67F9DEFFDC447BC35E7619107D19961154B21A6B9A72FDDF`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `B1E3C4A7F419B54F339947403FB48FAFF768F8259DB039CA44F79FEDC2E6BF78`
- Manifest SHA256: `479C6AC4C5F9FD5B739C0A2E4442ADD7C0B12FC0514C7CF2153F12553F70FA84`
- PCK SHA256: `6493913D0B1F57A92CF0D1BD24841FCF41E9753C871ED04B4A218531C35BCCDD`

## Known Automated Evidence

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh, 67 passed, 16 skipped release artifact/runtime evidence tests, 0 failed. Not rerun for the latest DLL/package.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh, 83 passed, 0 skipped, 0 failed. Not rerun for the latest DLL/package.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh. Not rerun for the latest source/resource refresh.
- `dotnet publish EZMicroBalance.sln`: passed.
- `git diff --check`: last exit 0 before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh. Not rerun for the latest source/resource refresh.
- Current controlled `--force-steam off` smoke physically isolated unrelated mods and temporarily enabled only BaseLib and EZ Micro Balance. Evidence under `.tools\runtime-evidence\rootblight-notice-package-smoke-clean-20260509-035904` loaded exactly 2 mods, initialized BaseLib and EZ Micro Balance, reported `Found 13 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `settings.save`, `settings.save.backup`, and 22 moved mod entries.
- RC1 normal Steam-client isolated startup log started Slay the Spire 2 through `D:\Steam\steam.exe -applaunch 2868840`, temporarily isolated non-BaseLib/EZMB local mod entries, loaded to main menu with `Loaded 2 mods (2 total)`, BaseLib `v3.1.2`, BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialization, `Found 13 SavedSpireFields`, 0 startup `ERROR` lines, and 0 release-blocking signatures. Snapshot: `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`. The moved mod entries and `settings.save` were restored afterward.
- RC1 normal Steam-client Mod Settings verification passed after adding the no-op EZ Micro Balance BaseLib config page. Evidence screenshots: `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-modconfig.png` for BaseLib, `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` for the EZ Micro Balance `寰钩琛 page entry, and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` for the `鏃犲彲閰嶇疆閫夐」銆俙 page. Log snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- RC1 A11 Act 1 map/save-load spot check launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11 through the original single-player Ascension arrows, confirmed the Act 1 map log `columns=8; rows=17` with `inserted 1 late route row(s)`, saved after the first node, continued the run, and reopened the map after load with the same geometry. Evidence: `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\08-character-select-a11.png`, `11-a11-act1-map-after-neow-continue.png`, `15-after-continue-load.png`, `16-map-open-after-load-attempt.png`, `a11-map-save-load-godot-live.log`, and `a11-save-map-dimensions.json`. The live log used for the gate has 0 `ERROR` lines and 0 release-blocking signatures.
- RC1 A11 Act 2/3 map-surface observation launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11 through the original single-player Ascension arrows, reached the Act 1 map normally, then used DevConsole `act 2` and `act 3` only to inspect later-act map surfaces. Evidence: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355\19-character-select-a11.png`, `25-a11-act2-map-clean.png`, `27-a11-act3-map-clean.png`, and `a11-act23-godot-live.log`. The log records Act 2 `columns=8; rows=16` with 1 late row and Act 3 `columns=8; rows=16` with 2 late rows, with 0 `ERROR` lines and 0 release-blocking signatures. Natural route traversal and boss reachability remain pending.
- Rootblight targeted normal Steam-client evidence: `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` captured the English A14 Neow starter Rootblight-added notice plus English Rootblight I/II/III and Blight Sprout hovers with one visible Exhaust keyword, no raw `[gold]` tags, and expected previews. `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516` captured the same hover/text checks in Simplified Chinese. `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455\07-run-start-06.png` captured the A14 ZHS Neow starter Rootblight-added notice after the event-room fallback. Combat-end notices are source-hardened with a top-level high-z, input-passthrough, 5-second overlay path, and generated Rootblight-family card art is packaged, but clean non-paused timing, Blight Sprout, full Rootblight/Blight Sprout behavior, generated-art visual verification, and co-op ownership/desync checks remain pending.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
This pass also starts `docs/rc1-live-validation-log.md` for source-verified RC1 notes and any live evidence collected during the normal Steam-client gate.

Normal Steam-client Mod Settings verification passed for BaseLib and EZ Micro Balance. Normal Steam-client A0/A10/A20 single-player DevConsole combat smoke passed for draw/energy/combat initialization. A11 Act 1 map/save-load and Act 2/3 map-surface spot checks passed. Targeted A14 Rootblight English/ZHS hover/starter-notice spot checks passed. Live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the A11 spot check, natural A11 traversal/boss reachability, disable-gameplay, broader save/load, and multiplayer checks are still pending.
Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Confirm BaseLib appears and is enabled.
4. Confirm EZ Micro Balance appears with id `EZMicroBalance` and can be enabled. RC1 evidence already shows the localized BaseLib config page as `寰钩琛; rerun only if package or local mod state changes.
5. Confirm legacy `EzDailyContent` is disabled or absent.
6. Start a run with BaseLib and EZ Micro Balance enabled.
7. Execute the Ancient reward matrix, including Velvet Choker soft-limit counting, Distinguished Cape v4.3 max-HP math/pay gate with same-pool replacement and locked fallback for unaffordable Vakuu Cape rolls, Prismatic Gem all-off-color reroll/exclusion checks plus reward-screen hint fallback log checks, zhs numeric formatting, and the save/load rows.
8. Execute disable-mod gameplay verification.
9. Inspect `%APPDATA%\SlayTheSpire2\logs\godot.log` after the Steam-client pass.

## Ascension Verification

A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Full live Ascension verification is pending. Live co-op selection and desync verification are still pending. No env var is needed for the default multiplayer test. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Use `EZMB_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks.

Run `docs/features/ascension-11-20/manual-test-checklist.md` with default-on selection first, then repeat comparison rows with disable env vars:

- Use the original Ascension arrows to select A11-A20.
- `EZMB_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.
- A20 host multiplayer selection/start should log: multiplayer A20 selection is enabled for development testing, Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification. Co-op gameplay remains unverified. Execute `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the two-PC matrix, ownership/desync checks, save/load rows, and result template, or keep release notes clear that the candidate has source-patched host selection but no live co-op verification.

## Log Audit Helper

For each copied live `godot.log`, run:

```powershell
scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit
```

For known-failing diagnostic attempts, omit `-FailOnHit` so the JSON audit still records the signature counts without stopping the collection script.

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

## A1.05.01 Review Scope

A1.05.01 (`ae910e8`) is a broad engineering/review commit, not only a handoff and `ReleaseCoverageGuardTests` update. It includes Ascension source directory reorganization, the no-op EZ Micro Balance Mod Settings config page, `settings_ui` localization, the manifest BaseLib `v3.1.2` dependency floor, `scripts/audit-godot-log.ps1`, export preset updates, documentation index/archive changes, test path rewrites, and handoff/RC1 evidence updates. Reviewers should review all of these surfaces.

## Commit And Push Handoff

Current git status before the Rootblight resolved-status release-hygiene commit:

- `git log -1 --oneline --decorate`: `b82023c (HEAD -> main, origin/main, origin/HEAD) 1.05.02`
- `git status --short --branch`: `## main...origin/main` with modified and untracked source, resource, documentation, and test-guard files for the Rootblight notice/text/art pass and release-handoff hygiene.

The existing `main` branch is aligned with `origin/main` at the recorded commit before this release-hygiene commit. The current local changes include Rootblight text/preview/notice hardening, generated Rootblight-family art, manifest author resolution, export/package metadata, documentation updates, and guard updates. Re-run `git status --short --branch` before final release packaging or handoff, because this section is a point-in-time snapshot.

Pre-commit local cleanup status summary:

```text
## main...origin/main
 M EZMicroBalance.json
 M EZMicroBalanceCode/Ascension/Cards/RootCards.cs
 M EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs
 M export_presets.cfg
 M tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs
?? EZMicroBalance/images/card_portraits/rootblight_i.png
?? EZMicroBalance/images/card_portraits/big/rootblight_i.png
?? docs/style/card-localization-style-guide.md
```

Proposed commit scope after the remaining manual/user gates are resolved:

- Include the independent `EZMicroBalance` project, manifest, solution, resource folder, code folder, localization, tests, and current release docs.
- Include legacy preservation moves and historical doc archives needed to explain why `EzDailyContent` remains unchanged but inactive for this private beta.
- Include `.gitignore` and export preset hardening for ignored local art, calibration, package output, tooling, and `source code/` scratch material.

Do not include:

- `Directory.Build.props`, `.godot/`, `.tools/`, `bin/`, `obj/`, `packages/`, `publish/`, local binaries, downloaded archives, or Steam/game runtime files.
- `art_pipeline/`, `asset/`, or `source code/` local scratch/reference folders.
- Any copied official Slay the Spire 2 assets or large decompiled method bodies.

Push only after explicit user approval.

