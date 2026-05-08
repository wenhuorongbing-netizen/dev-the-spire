# EZ Micro Balance Private Beta Verification Handoff

Date: 2026-05-08

**Environment warning (2026-05-08):** The earlier live test log (`godot2026-05-08T05.06.30.log`) was collected in a v0.105.0 environment with 17 mods loaded and BaseLib `v3.1.0`, not the required BaseLib+EZMB-only setup. Current package evidence uses BaseLib `v3.1.2` and a controlled BaseLib+EZMB-only smoke; normal Steam-client Mod Settings and live gameplay verification are still pending.

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

## Package Under Test

- Package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip`
- Zip SHA256: `6C3A9CE64D7227BBC5204D1EC1215EA6877818E24E4400910DCE8BF9199BC090`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `215A4621019CA93ABB0157BBFEA094FE4C8DBDEA247ECA02222709298784CF5C`
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`
- PCK SHA256: `89D87BEB637EDE00A62A57491563A2254BBABBC471859C5B32F74C11F6D89A7F`

## Known Automated Evidence

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 65 passed, 16 skipped release artifact/runtime evidence tests, 0 failed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 0 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed.
- `git diff --check`: exit 0 with CRLF normalization warnings for touched files.
- Current controlled `--force-steam off` smoke temporarily enabled only BaseLib and EZ Micro Balance, loaded exactly 2 mods, initialized BaseLib and EZ Micro Balance, reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte. Normal Steam-client Mod Settings verification is still pending.

## Required Manual Results

Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md` and update `docs/release-checklist.md`.

Normal Steam-client Mod Settings verification is still pending.
Live Ancient reward gameplay, save/load, disable-gameplay, and multiplayer checks are still pending.

1. Launch through the normal Steam client.
2. Open Settings -> Mod Settings.
3. Confirm BaseLib appears and is enabled.
4. Confirm EZ Micro Balance appears with id `EZMicroBalance` and can be enabled.
5. Confirm legacy `EzDailyContent` is disabled or absent.
6. Start a run with BaseLib and EZ Micro Balance enabled.
7. Execute the Ancient reward matrix, including Velvet Choker soft-limit counting, Distinguished Cape v4.3 max-HP math/pay gate with same-pool replacement and locked fallback for unaffordable Vakuu Cape rolls, Prismatic Gem all-off-color reroll/exclusion checks plus reward-screen hint fallback log checks, zhs numeric formatting, and the save/load rows.
8. Execute disable-mod gameplay verification.
9. Inspect `%APPDATA%\SlayTheSpire2\logs\godot.log` after the Steam-client pass.

## Ascension Verification

A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Full live Ascension verification is pending. Live co-op selection and desync verification are still pending. No env var is needed for the default multiplayer test. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Use `EZMB_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks.

Execute `docs/features/ascension-11-20/manual-test-checklist.md` against A11 through A20 from the normal single-player and host-multiplayer character select flows after explicitly enabling the selector:

- Use the original Ascension arrows to select A11-A20.
- `EZMB_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.
- A20 host multiplayer selection/start should log: multiplayer A20 selection is enabled for development testing, Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification. Co-op gameplay remains unverified. Execute `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the two-PC matrix, ownership/desync checks, save/load rows, and result template, or keep release notes clear that the candidate has source-patched host selection but no live co-op verification.

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

- `git log -1 --oneline --decorate`: `7d74d68 (HEAD -> main, origin/main, origin/HEAD) try fix 1.05`
- `git status --short --branch`: `## main...origin/main` with modified source/docs/tests from the v0.105.0 Pumpkin Candle rollback, Quality Flame hardening, Door Wedge removal, Aeonglass +5 Strength, BaseLib v3.1.2 documentation, package/hash refresh, and guard-test pass. No release artifacts are tracked.

No commit or push was attempted in this pass. Re-run `git status --short --branch` before final release packaging or handoff, because this section is a point-in-time snapshot.

Proposed commit scope after the remaining manual/user gates are resolved:

- Include the independent `EZMicroBalance` project, manifest, solution, resource folder, code folder, localization, tests, and current release docs.
- Include legacy preservation moves and historical doc archives needed to explain why `EzDailyContent` remains unchanged but inactive for this private beta.
- Include `.gitignore` and export preset hardening for ignored local art, calibration, package output, tooling, and `source code/` scratch material.

Do not include:

- `Directory.Build.props`, `.godot/`, `.tools/`, `bin/`, `obj/`, `packages/`, `publish/`, local binaries, downloaded archives, or Steam/game runtime files.
- `art_pipeline/`, `asset/`, or `source code/` local scratch/reference folders.
- Any copied official Slay the Spire 2 assets or large decompiled method bodies.

Push only after explicit user approval.
