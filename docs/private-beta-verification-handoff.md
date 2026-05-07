# EZ Micro Balance Private Beta Verification Handoff

Date: 2026-05-07

This handoff is for manual verification that cannot be completed by the local automated build/test loop.

## Package Under Test

- Package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip`
- Zip SHA256: `6A5273519B2FD8F4D0256EA755D1E07525E7D185BEF9D0A607EEF261F4F81427`
- Manifest id: `EZMicroBalance`
- DLL SHA256: `B8303AC917540479B131FF6501E2643114220BFA05B6E63D63F1ECE41E0F54BA`
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`
- PCK SHA256: `1B89120EA299F4334CDC4D22D3ABBC704899894FF7AAF258AD04A6743BF98717`

## Known Automated Evidence

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: A11-A20 v2.0 source/package/localization guards, Ancient v4.3 regression guards, and package hash parity guards passed 75/75 after rebuilding the package and hash docs from the current installed artifacts.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed.
- `git diff --check`: exit 0 with the documented `EzDailyContent.json` and `docs/dev-environment.md` CRLF warnings.
- Prior controlled `--force-steam off` smoke temporarily enabled only BaseLib and EZ Micro Balance, loaded exactly 2 mods, initialized BaseLib and EZ Micro Balance, reported `Found 9 SavedSpireFields`, reached main menu in `4,076ms`, found 0 EZ Micro Balance error/exception lines, and restored `settings.save` plus `settings.save.backup` to their original contents. That smoke predates the Rootblight v2.2 card-state fields; the current source defines 12 SavedSpireFields and needs a refreshed runtime smoke before release claims.

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

The A11-A20 single-player and host-multiplayer selection patch is implemented but private-beta default-disabled. Full live Ascension verification is pending. Live co-op selection and desync verification are still pending. To test A11-A20 through the original UI, launch with `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`; use `EZMB_ASCENSION_DEBUG_LEVEL=12` through `20` for forced internal slice checks. Host-multiplayer selection can be disabled separately with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.

Execute `docs/features/ascension-11-20/manual-test-checklist.md` against A11 through A20 from the normal single-player and host-multiplayer character select flows after explicitly enabling the selector:

- Use the original Ascension arrows to select A11-A20.
- `EZMB_ASCENSION_DIAGNOSTICS=1` remains available for read-only diagnostics.

Co-op gameplay remains unverified. Either execute the multiplayer ownership/desync checks or keep release notes clear that the candidate has source-patched host selection but no live co-op verification.

## Author Decision

`EZMicroBalance.json` still contains `AUTHOR_NAME_REPLACE_ME`. Before final private-beta release, either replace it with the desired author name or explicitly accept that placeholder for this candidate.

## Commit And Push Handoff

No commit or push has been made.

Proposed commit scope after the remaining manual/user gates are resolved:

- Include the independent `EZMicroBalance` project, manifest, solution, resource folder, code folder, localization, tests, and current release docs.
- Include legacy preservation moves and historical doc archives needed to explain why `EzDailyContent` remains unchanged but inactive for this private beta.
- Include `.gitignore` and export preset hardening for ignored local art, calibration, package output, tooling, and `source code/` scratch material.

Do not include:

- `Directory.Build.props`, `.godot/`, `.tools/`, `bin/`, `obj/`, `packages/`, `publish/`, local binaries, downloaded archives, or Steam/game runtime files.
- `art_pipeline/`, `asset/`, or `source code/` local scratch/reference folders.
- Any copied official Slay the Spire 2 assets or large decompiled method bodies.

Push only after explicit user approval.

