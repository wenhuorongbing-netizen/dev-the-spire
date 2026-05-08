# EZ Micro Balance Private Beta Verification Handoff

Date: 2026-05-08

**Environment warning (2026-05-08):** The earlier live test log (`godot2026-05-08T05.06.30.log`) was collected in a v0.105.0 environment with 17 mods loaded and BaseLib `v3.1.0`, not the required BaseLib+EZMB-only setup. Current package evidence uses BaseLib `v3.1.2`, a controlled BaseLib+EZMB-only smoke, normal Steam-client Mod Settings evidence, normal-Steam A0/A10/A20 combat smoke, an Act 1 A11 map/save-load spot check, and Act 2/3 A11 map-surface observations. Full Ancient reward gameplay, natural A11 traversal, and co-op verification are still pending.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

## Package Under Test

- Package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip`
- Zip SHA256: `BE05559B4EA1180FB88129235A980978B1E2498187F1CB665882EC7DCC1CD314`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `1AEE7CD1C6EB945F022CB85997ADC709D930C3E6FC318E7E0EFE1A13436C589F`
- Manifest SHA256: `68466CF2BDE07AE7F911AE75EBF6FCAAFE80F70570E3F0D6ECA796B496DB8DB0`
- PCK SHA256: `435D55B14FAD38F611C550F4ACAF604EE1A2C3E63E75C52FC3FA9FCE52D064CA`

## Known Automated Evidence

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 66 passed, 16 skipped release artifact/runtime evidence tests, 0 failed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 82 passed, 0 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed.
- `git diff --check`: exit 0 with CRLF normalization warnings for touched files.
- Current controlled `--force-steam off` smoke temporarily enabled only BaseLib and EZ Micro Balance, loaded exactly 2 mods, initialized BaseLib and EZ Micro Balance, reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte.
- RC1 normal Steam-client isolated startup log started Slay the Spire 2 through `D:\Steam\steam.exe -applaunch 2868840`, temporarily isolated non-BaseLib/EZMB local mod entries, loaded to main menu with `Loaded 2 mods (2 total)`, BaseLib `v3.1.2`, BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialization, `Found 12 SavedSpireFields`, 0 startup `ERROR` lines, and 0 release-blocking signatures. Snapshot: `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`. The moved mod entries and `settings.save` were restored afterward.
- RC1 normal Steam-client Mod Settings verification passed after adding the no-op EZ Micro Balance BaseLib config page. Evidence screenshots: `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-modconfig.png` for BaseLib, `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` for the EZ Micro Balance `微平衡` page entry, and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` for the `无可配置选项。` page. Log snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- RC1 A11 Act 1 map/save-load spot check launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11 through the original single-player Ascension arrows, confirmed the Act 1 map log `columns=8; rows=17` with `inserted 1 late route row(s)`, saved after the first node, continued the run, and reopened the map after load with the same geometry. Evidence: `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\08-character-select-a11.png`, `11-a11-act1-map-after-neow-continue.png`, `15-after-continue-load.png`, `16-map-open-after-load-attempt.png`, `a11-map-save-load-godot-live.log`, and `a11-save-map-dimensions.json`. The live log used for the gate has 0 `ERROR` lines and 0 release-blocking signatures.
- RC1 A11 Act 2/3 map-surface observation launched through normal Steam with only BaseLib + EZ Micro Balance, selected A11 through the original single-player Ascension arrows, reached the Act 1 map normally, then used DevConsole `act 2` and `act 3` only to inspect later-act map surfaces. Evidence: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355\19-character-select-a11.png`, `25-a11-act2-map-clean.png`, `27-a11-act3-map-clean.png`, and `a11-act23-godot-live.log`. The log records Act 2 `columns=8; rows=16` with 1 late row and Act 3 `columns=8; rows=16` with 2 late rows, with 0 `ERROR` lines and 0 release-blocking signatures. Natural route traversal and boss reachability remain pending.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.
This pass also starts `docs/rc1-live-validation-log.md` for source-verified RC1 notes and any live evidence collected during the normal Steam-client gate.

Normal Steam-client Mod Settings verification passed for BaseLib and EZ Micro Balance. Normal Steam-client A0/A10/A20 single-player DevConsole combat smoke passed for draw/energy/combat initialization. A11 Act 1 map/save-load and Act 2/3 map-surface spot checks passed. Live Ancient reward gameplay, natural route-click first-node checks beyond the A11 spot check, natural A11 traversal/boss reachability, disable-gameplay, broader save/load, and multiplayer checks are still pending.
Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Confirm BaseLib appears and is enabled.
4. Confirm EZ Micro Balance appears with id `EZMicroBalance` and can be enabled. RC1 evidence already shows the localized BaseLib config page as `微平衡`; rerun only if package or local mod state changes.
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

`EZMicroBalance.json` still contains `AUTHOR_NAME_REPLACE_ME`. Before final private-beta release, either replace it with the desired author name or explicitly accept that placeholder for this candidate.

## Commit And Push Handoff

Current git status at this handoff refresh:

- `git log -1 --oneline --decorate`: `96bfa50 (HEAD -> main, origin/main, origin/HEAD) fix try 10`
- `git status --short --branch`: `## main...origin/main` with current uncommitted documentation/test/source-organization edits, including modified files, deleted moved originals, and untracked new patch/doc/archive files. The dirty tree includes Ancient patch file moves, documentation index/archive updates, and this issues/handoff hygiene refresh.

The existing `main` branch is already aligned with `origin/main` at the recorded commit, but the working tree is not clean. Do not describe the local checkout as fully pushed until the pending edits are reviewed, committed, and pushed. Re-run `git status --short --branch` before final release packaging or handoff, because this section is a point-in-time snapshot.

Proposed commit scope after the remaining manual/user gates are resolved:

- Include the independent `EZMicroBalance` project, manifest, solution, resource folder, code folder, localization, tests, and current release docs.
- Include legacy preservation moves and historical doc archives needed to explain why `EzDailyContent` remains unchanged but inactive for this private beta.
- Include `.gitignore` and export preset hardening for ignored local art, calibration, package output, tooling, and `source code/` scratch material.

Do not include:

- `Directory.Build.props`, `.godot/`, `.tools/`, `bin/`, `obj/`, `packages/`, `publish/`, local binaries, downloaded archives, or Steam/game runtime files.
- `art_pipeline/`, `asset/`, or `source code/` local scratch/reference folders.
- Any copied official Slay the Spire 2 assets or large decompiled method bodies.

Push only after explicit user approval.
