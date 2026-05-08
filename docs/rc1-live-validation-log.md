# RC1 Live Validation Log

Date: 2026-05-08  
Scope: RC1 live-validation gate for EZ Micro Balance on Slay the Spire 2 `v0.105.0` with BaseLib `v3.1.2`.

This log records what was actually run or observed. It does not close the live gates unless the corresponding result is marked executed with evidence.

## Repository State

- `git log -1 --oneline --decorate`: `38927ce (HEAD -> main, origin/main, origin/HEAD) tryfix 1.05`.
- `git status --short --branch`: clean at `## main...origin/main` before this RC1 documentation/live-validation hygiene pass.
- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no process was running before validation commands.

## Package Refresh

- `dotnet publish EZMicroBalance.sln` changed the installed Release DLL hash, so package staging, versioned package directory, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` were rebuilt from installed artifacts.
- Zip SHA256: `C928B50616109FF198405F3990A1F4DA40FA9460E8CC6DFE69CC95784DBEEAE2`.
- DLL SHA256: `70E7D2FF06C067A139027E2B64DFAA76E9638C990E40B0A504CCD34EE6FE9174`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.
- PCK SHA256: `89D87BEB637EDE00A62A57491563A2254BBABBC471859C5B32F74C11F6D89A7F`.

## Automated Results

- `dotnet build EZMicroBalance.sln`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln`: passed, 65 passed, 16 skipped, 0 failed.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 65 passed, 16 skipped, 0 failed.
- `dotnet publish EZMicroBalance.sln`: passed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 0 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: exit 0 with CRLF normalization warning for `docs/features/ancients-rework-v4/completion-audit.md`.

## Normal Steam-Client Launch Probe

- Command path: `D:\Steam\steam.exe -applaunch 2868840`.
- App manifest: `appmanifest_2868840.acf` names `Slay the Spire 2`.
- Result: SlayTheSpire2 started from Steam, loaded to main menu, then was closed after log collection.
- Log: `%APPDATA%\SlayTheSpire2\logs\godot.log`, last write `2026-05-08T07:32:55+02:00`, length `25818`.
- Positive log evidence: `Loaded 2 mods (19 total)`, BaseLib `Version=3.1.2.0`, `[BaseLib] Applied 177 patches successfully, 0 failed`, `Finished mod initialization for 'BaseLib' (BaseLib).`, `Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance).`, `[BaseLib] Found 12 SavedSpireFields.`, `Time to main menu: 14,444ms`.
- Strict scan: `Creature.get_ShowsInfiniteHp` 0, `BaseLib.Patches.UI.HealthBarForecastPatch` 0, BaseLib undefined-target patch failures 0, `TypeLoadException` 0, `MissingMethodException` 0, EZMB error/exception pattern 0.
- Clean-log gate status: not closed. The log still contains unrelated manifest/dependency `ERROR` lines from discovered local mods, including `RouteSuggestConfig.json` missing `id`, `sts2-heybox-support` missing `id`, and old-style dependency warnings. DamageMeter and RouteSuggest were discovered but skipped as disabled in settings. This is a useful normal Steam launch probe, not a clean release log.
- Mod Settings UI status: not executed by Codex in this pass.

## User-Reported Live Baseline

- User reports single-player A0/A10/A20 and boss/basic combats pass after the BaseLib update.
- Treat this as useful player evidence, not a Codex-collected clean Steam-client log.
- Clean normal Steam-client `godot.log` is still required.

## Source-Verified Spot Checks

- Pumpkin Candle: active EZMB source has no `internal static class PumpkinCandlePatch`, no `ExtinguishedSentinel`, and no EZMB Act 3 extinguish-upgrade behavior. The intended spot check is vanilla/no override.
- Quality Flame / BrightestFlame: current v0.105 source class is `MegaCrit.Sts2.Core.Models.Cards.BrightestFlame`. Vanilla `CanonicalVars` are Max HP 1, Energy 2, Cards 2; `OnUpgrade()` raises Energy and Cards by 1. EZMB adds `CardKeyword.Exhaust` through `CardModel.CanonicalKeywords`, raises the `CardsVar` by 1, and keeps an `OnPlayWrapper` exhaust backstop. Expected live behavior is unupgraded draw 3, upgraded draw 4, visible Exhaust keyword, and exhaust-pile result after play.
- Doormaker / Door Wedge: active source, localization, and guard tests are expected to require absence of Door Wedge and no `DOORMAKER_BOSS` mapping. Historical docs may mention the removed behavior only as historical context.
- Aeonglass: v0.105 source has `AEONGLASS_BOSS` and `MONSTER.AEONGLASS`; `AeonglassBoss.GenerateMonsters()` creates `ModelDb.Monster<Aeonglass>().ToMutable()`. EZMB maps `AEONGLASS_BOSS` to `BossSealId.AeonglassStrength` and applies `+5 Strength` to the enemy with model id `MONSTER.AEONGLASS`, with no complex Brand/seal mechanic added.

## Live Gates

| Gate | Result |
| --- | --- |
| Normal Steam-client Mod Settings | Steam launch/log probe executed; Mod Settings UI was not opened, so this remains pending. |
| Clean normal Steam-client `godot.log` | Collected from Steam launch, but strict clean-log gate remains pending because unrelated local invalid-manifest/dependency `ERROR` lines are present. |
| A0/A10/A20 single-player spot checks | User-reported pass; not Codex re-run in this log unless updated below. |
| Pumpkin Candle vanilla/no override | Source-verified pending live spot check. |
| Quality Flame unupgraded/upgraded | Source-verified pending live spot check. |
| Door Wedge absence | Active source/localization `rg` returned no Door Wedge / `DOORMAKER_BOSS` matches; release-facing docs mention it only as removed/historical. |
| Aeonglass +5 Strength | Source-verified pending live boss route/seed check. |
| A11 map geometry | Pending live screenshot/row-count observation. |
| Save/load | Pending. |
| Multiplayer matrix | Pending two-PC Steam-client runbook execution. |

## Pending RC1 Items

- Rootblight visual feedback.
- Rootblight card art.
- A11 geometry diagnostics.
- Clean normal Steam-client `godot.log`.
- Multiplayer matrix.
- Steam-client Mod Settings.
- Save/load verification.
