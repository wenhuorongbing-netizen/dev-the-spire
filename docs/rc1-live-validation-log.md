# RC1 Live Validation Log

Date: 2026-05-08 / 2026-05-09
Scope: RC1 live-validation gate for EZ Micro Balance on Slay the Spire 2 `v0.105.0` with BaseLib `v3.1.2`.

This log records what was actually run or observed. It does not close the live gates unless the corresponding result is marked executed with evidence.

## Repository State

- A1.05.01 review baseline: `ae910e8 (HEAD -> main, origin/main, origin/HEAD) a1.05.01`.
- `git status --short --branch` at the A1.05.02 cleanup start: `## main...origin/main`.
- A1.05.01 is a broad engineering/review commit, not only a handoff and `ReleaseCoverageGuardTests` update. It includes Ascension source directory reorganization, the no-op EZ Micro Balance Mod Settings config page, `settings_ui` localization, the manifest BaseLib `v3.1.2` dependency floor, `scripts/audit-godot-log.ps1`, export preset updates, documentation index/archive changes, test path rewrites, and handoff/RC1 evidence updates. Reviewers should review all of these surfaces.
- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no process was running before validation commands.

## Package Refresh

- `dotnet publish EZMicroBalance.sln` changed installed artifacts, so package staging, versioned package directory, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` were rebuilt from installed artifacts.
- Zip SHA256: `B1F8B0FBA1BBFA736233D27C83BF193CE661B22726FA37420CE2C4B2B1F8750E`.
- DLL SHA256: `599D4EF00CF207F8AB79AB90FCBE4B644E5C476B7F4DE2AB60CE8BBE9B460C50`.
- Manifest SHA256: `479C6AC4C5F9FD5B739C0A2E4442ADD7C0B12FC0514C7CF2153F12553F70FA84`.
- PCK SHA256: `94DA61B1C57316FF08AE9E39E1212E7B581E81AEB9D23633FF8DDF9B6BDE33CF`.

## Automated Results

- `dotnet build EZMicroBalance.sln`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh, 67 passed, 16 skipped, 0 failed; not rerun for the latest DLL/package.
- `dotnet publish EZMicroBalance.sln`: passed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh, 83 passed, 0 skipped, 0 failed; not rerun for the latest DLL/package.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: last passed before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh.
- `git diff --check`: last exit 0 before the Rootblight top-level notice hardening, optional portrait fallback, and generated-art/author refresh.

## Normal Steam-Client Launch Probe

- Command path: `D:\Steam\steam.exe -applaunch 2868840`.
- App manifest: `appmanifest_2868840.acf` names `Slay the Spire 2`.
- Result: SlayTheSpire2 started from Steam, loaded to main menu, then was closed after log collection.
- Log: `%APPDATA%\SlayTheSpire2\logs\godot.log`, last write `2026-05-08T07:32:55+02:00`, length `25818`.
- Positive log evidence: `Loaded 2 mods (19 total)`, BaseLib `Version=3.1.2.0`, `[BaseLib] Applied 177 patches successfully, 0 failed`, `Finished mod initialization for 'BaseLib' (BaseLib).`, `Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance).`, `[BaseLib] Found 13 SavedSpireFields.`, `Time to main menu: 14,444ms`.
- Strict scan: `Creature.get_ShowsInfiniteHp` 0, `BaseLib.Patches.UI.HealthBarForecastPatch` 0, BaseLib undefined-target patch failures 0, `TypeLoadException` 0, `MissingMethodException` 0, EZMB error/exception pattern 0.
- Clean-log gate status for this first probe: not closed. The captured log still contains unrelated manifest/dependency `ERROR` lines from discovered local mods, including `RouteSuggestConfig.json` missing `id` and `sts2-heybox-support` missing `id`. DamageMeter and RouteSuggest were discovered but skipped as disabled in settings. The isolated startup log below supersedes this first probe for clean-log evidence.
- Mod Settings UI status: superseded by the isolated Mod Settings recheck below.

## Normal Steam-Client Isolated Startup Log

- Command path: `D:\Steam\steam.exe -applaunch 2868840`.
- Isolation method: temporarily moved 23 non-BaseLib/EZMB entries out of `D:\Steam\steamapps\common\Slay the Spire 2\mods`, launched through Steam, copied the startup log at main menu, then restored the moved entries and `settings.save`.
- Snapshot: `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`.
- Positive log evidence: only `BaseLib\BaseLib.json` and `EZMicroBalance\EZMicroBalance.json` were discovered; `Loaded 2 mods (2 total)`; BaseLib `177 patches successfully, 0 failed`; BaseLib and EZ Micro Balance initialized; BaseLib reported `Found 13 SavedSpireFields`; main menu reached in `13,470ms`.
- Strict scan: startup snapshot has 0 `ERROR` lines, 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `DamageMeter`, 0 `RouteSuggest`, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits.
- Warning scan: startup snapshot has 8 warnings: D3D12 PSO caching, BaseLib missing `min_game_version`, EZMB prerelease version/min-game metadata warnings, and uncached startup assets.
- Clean-log gate status: startup log gate passed for the release-blocking signatures after isolation. Broader gameplay spot checks are tracked below and remain incomplete.

## Normal Steam-Client Mod Settings UI Probe

- Command path: `D:\Steam\steam.exe -applaunch 2868840`.
- First isolation probe: temporarily moved 23 non-BaseLib/EZMB entries out of `D:\Steam\steamapps\common\Slay the Spire 2\mods`, launched through Steam, opened `妯＄粍閰嶇疆`, captured BaseLib-only screenshots, copied `.tools\runtime-evidence\rc1-normal-steam-modsettings-godot-20260508-092717.log`, closed the game, then restored all moved entries.
- Source fix for UI visibility: added a no-op BaseLib `ModConfig` page for EZ Micro Balance. It registers `EZMicroBalanceModConfig`, exposes no gameplay options, and localizes the Chinese title as `寰钩琛�?with body text `鏃犲彲閰嶇疆閫夐」銆�?
- Recheck isolation probe: temporarily moved the same 23 non-BaseLib/EZMB entries out of the game `mods` directory, launched through Steam, opened `妯＄粍閰嶇疆`, captured screenshots, copied the log, closed the game, then restored all moved entries and temporarily minimized windows. Restore check: `RemainingIsolatedEntries: 0`, `SlayProcessRunning: 0`, `RestoredWindows: 24`.
- Main-menu loaded-mod evidence: `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-mainmenu-loadedmods.png` shows the game loaded 2 mods: `BaseLib, EZ Micro Balance`.
- BaseLib Mod Settings evidence: `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-modconfig.png` shows the `BaseLib` page and its main-menu display checkbox enabled.
- EZ Micro Balance Mod Settings evidence: `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` shows the EZ Micro Balance page entry as `寰钩琛�? `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` shows the page body `鏃犲彲閰嶇疆閫夐」銆�?
- Log snapshot: `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log`.
- Positive log evidence: `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialized, and `Found 13 SavedSpireFields`.
- Strict scan: 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `DamageMeter`, 0 `RouteSuggest`, 0 `TypeLoadException`, 0 `MissingMethodException`, 0 EZMB error/exception pattern hits, and 0 `ERROR` lines.
- Gate status: normal Steam-client Mod Settings visibility/enabled check passed for BaseLib and EZ Micro Balance. Broader gameplay spot checks are tracked below and remain incomplete.

## User-Reported Live Baseline

- User reports single-player A0/A10/A20 and boss/basic combats pass after the BaseLib update.
- Treat this as useful player evidence.
- Clean normal Steam-client startup and Mod Settings logs are collected.

## Codex-Observed Single-Player Combat Smoke

- Method: temporarily isolated all non-BaseLib/EZMB local mods, launched through the normal Steam client, used the standard single-player Ironclad character-select flow, and used the built-in DevConsole command `fight CULTISTS_NORMAL` after run start to enter a live combat quickly.
- Scope note: this verifies combat initialization, draw, energy, HP, enemy visuals/intents, and basic animation surfaces in a live normal-Steam session. It is not a natural route-click first-node run, and it does not replace the full Ancient/Ascension manual feature matrix.
- Save/mod hygiene: copied the pre-test `modded/profile1/saves` directory before abandoning test runs, restored it afterward, restored the 23 temporarily moved local mod entries, and confirmed `SlayTheSpire2` was no longer running.
- A0 evidence: `.tools\runtime-evidence\rc1-live-attempt-20260508-102213\a0-character-select-after-abandon.png` shows A0 selected; `.tools\runtime-evidence\rc1-live-attempt-20260508-102213\a0-debug-fight-clean.png` shows 80/80 HP, 3/3 energy, five cards in hand, enemies with HP/intents, and normal combat visuals.
- A10 evidence: `.tools\runtime-evidence\rc1-live-attempt-20260508-102213\a10-first-combat-clean.png` shows A10 combat with 64/80 HP, 3/3 energy, five cards in hand, enemies with HP/intents, and normal combat visuals.
- A20 evidence: `.tools\runtime-evidence\rc1-live-attempt-20260508-102213\a20-character-select-after-abandon.png` shows A20 selected; `.tools\runtime-evidence\rc1-live-attempt-20260508-102213\a20-debug-fight-clean.png` shows A20 combat with 64/80 HP, 3/3 energy, five cards in hand, Rootblight in deck, enemies with HP/intents, and normal combat visuals.
- Logs: `a10-debug-fight-godot.log`, `a20-debug-fight-godot.log`, and `a0-a10-a20-debug-fight-godot.log` each show `Loaded 2 mods (2 total)`, BaseLib `177 patches successfully, 0 failed`, the expected `Embarking ... Ascension` line for the tested level, and `DevConsole: fight CULTISTS_NORMAL`.
- Blocking-signature scan across the combat-smoke logs: 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits.
- Clean-log caveat: the combat-smoke logs are not the clean-log gate snapshots. They include Godot exit resource-leak `ERROR` lines after automated window closing, and A20/A0 include a save-backup delete `ERROR` caused by the temporary test-run abandonment/save restoration flow. The clean-log gate remains the earlier isolated startup and Mod Settings snapshots with 0 `ERROR` lines.

## A11 Act 1 Map And Save/Load Spot Check

- Method: temporarily isolated all non-BaseLib/EZMB local mods, launched through the normal Steam client, selected A11 through the original single-player Ascension arrows, took the first Neow option, captured the Act 1 map, clicked a first-route monster node to force a run save, used in-game Save & Quit, then continued the saved run and opened the map again from combat.
- Save/mod hygiene: copied the pre-test `modded/profile1/saves` directory, restored it afterward, restored the 23 temporarily moved local mod entries, and confirmed `SlayTheSpire2` was no longer running.
- Evidence directory: `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008`.
- Selection evidence: `08-character-select-a11.png` shows A11 selected through the live UI with the `瀹藉闀胯矾` description.
- Initial map evidence: `11-a11-act1-map-after-neow-continue.png` shows the Act 1 A11 map rendered with normal route nodes and no A11-specific marker or hover tooltip.
- Save/load evidence: `15-after-continue-load.png` shows the saved A11 run continuing into the selected first combat; `16-map-open-after-load-attempt.png` shows the map reopened after load with the same widened/longer Act 1 geometry.
- Log evidence: `a11-map-save-load-godot-live.log` has `Loaded 2 mods (2 total)`, `Embarking on a singleplayer IRONCLAD run. Ascension: 11`, `Ascension A11 applied ... inserted 1 late route row(s); actIndex=0; columns=8; rows=17`, multiple `current_run.save` writes, `Continuing run with character: CHARACTER.IRONCLAD`, and a post-load `Ascension A11 gate active ... columns=8; rows=17` line.
- Save evidence: `a11-save-map-dimensions.json` records `Ascension: 11`, `CurrentActIndex: 0`, `MapHeight: 17`, `BossRow: 17`, `RouteRowCount: 16`, `ColumnCount: 8`, and `Columns: 0,1,2,3,4,5,6,7`.
- Strict scan for `a11-map-save-load-godot-live.log`: 0 `ERROR` lines, 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `DamageMeter`, 0 `RouteSuggest`, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits. The after-close log has forced-window-close Godot resource errors and is not used as the clean-log gate snapshot.
- Scope note: this closes the normal-Steam Act 1 A11 map/save-load spot check only. Act 2/3 route-length observation is tracked below; broader natural traversal, A12/A13/A14/A16/A17/A19/A20 slice checks, Ancient save/load rows, and co-op save/load remain pending.

## A11 Act 2/3 Map Observation

- Method: temporarily isolated all non-BaseLib/EZMB local mods, launched through the normal Steam client, selected A11 through the original single-player Ascension arrows, reached the Act 1 map normally, then used DevConsole `act 2` and `act 3` to observe the later-act map surfaces without adding gameplay code.
- Save/mod hygiene: copied the pre-test `modded/profile1/saves` directory, restored it afterward, restored the 23 temporarily moved local mod entries, and confirmed `SlayTheSpire2` was no longer running.
- Evidence directory: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355`.
- Selection evidence: `19-character-select-a11.png` shows A11 selected through the live UI with the `瀹藉闀胯矾` description.
- Act 2 evidence: `25-a11-act2-map-clean.png` shows an A11 Act 2 map surface with normal route nodes and no A11-specific marker or hover tooltip.
- Act 3 evidence: `27-a11-act3-map-clean.png` shows an A11 Act 3 map surface with normal route nodes and no A11-specific marker or hover tooltip.
- Log evidence: `a11-act23-godot-live.log` has `Loaded 2 mods (2 total)`, `Embarking on a singleplayer IRONCLAD run. Ascension: 11`, `Ascension A11 applied ... inserted 1 late route row(s); actIndex=0; columns=8; rows=17`, `Ascension A11 applied ... inserted 1 late route row(s); actIndex=1; columns=8; rows=16`, and `Ascension A11 applied ... inserted 2 late route row(s); actIndex=2; columns=8; rows=16`.
- Strict scan for `a11-act23-godot-live.log`: 0 `ERROR` lines, 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `DamageMeter`, 0 `RouteSuggest`, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits.
- Scope note: this closes the normal-Steam Act 2/3 A11 width/row/no-marker observation only. It does not prove natural route traversal, every-start boss reachability, A17 Deep Branch metadata, or multiplayer map behavior.

## A14 Rootblight UI And Notice Spot Checks

- Method: temporarily isolated all non-BaseLib/EZMB local mods, launched through the normal Steam client, selected A14 through the original single-player Ascension arrows, and captured Rootblight-family hover/text and starter-notice screenshots. This was a targeted UI/notice pass, not a full A14 combat-behavior pass.
- English hover/text evidence directory: `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010`. Screenshot `07-after-confirm-a14-neow.png` shows A14 selected and the English Rootblight-added thought bubble at Neow with the starter deck at 11 cards. Screenshots `12-hover-rootblight-i.png`, `13-hover-rootblight-ii.png`, `14-hover-rootblight-iii.png`, and `15-hover-blight-sprout.png` show one visible Exhaust keyword, no raw `[gold]` tags, and the expected Rootblight previews.
- Hover/text evidence directory: `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516`. The UI remained Simplified Chinese despite the attempted English switch. Screenshots `14-hover-rootblight-i.png`, `15-hover-rootblight-ii.png`, `16-hover-rootblight-iii.png`, and `17-hover-blight-sprout.png` show one visible Exhaust keyword, no raw `[gold]` tags, and the expected Rootblight previews.
- Notice evidence directory: `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455`. Screenshot `06-character-select-a14.png` shows A14 selected through the live UI; `07-run-start-06.png` shows the localized Rootblight-added thought bubble at Neow with the starter deck at 11 cards.
- Save/mod hygiene: restore checks for the English, ZHS UI, and ZHS notice sessions confirm settings/saves were restored, all 22 moved mod entries were restored, and no Slay the Spire 2 process was left running.
- Log caveat: the English hover/notice log and the ZHS notice-run log each include one setup-noise `ERROR` from deliberately abandoning a pre-existing temporary current run before the A14 start. They are not used as clean-log gates. A separate normal Steam-client BaseLib+EZMB-only main-menu log from `.tools\runtime-evidence\rootblight-a14-notice-zhs-no-current-20260509-041615\godot-mainmenu.log` audited clean with 0 `ERROR` lines and 0 release-blocking signatures.
- Scope note: this closes only the English/ZHS hover/text spot checks and the A14 Neow starter add-notice spot checks. Combat-end Rootblight-add notices, full Rootblight/Blight Sprout behavior, generated-art visual verification, and co-op ownership/desync checks remain pending.

## Source-Verified Spot Checks

- Pumpkin Candle: active EZMB source has no `internal static class PumpkinCandlePatch`, no `ExtinguishedSentinel`, and no EZMB Act 3 extinguish-upgrade behavior. The intended spot check is vanilla/no override.
- Quality Flame / BrightestFlame: current v0.105 source class is `MegaCrit.Sts2.Core.Models.Cards.BrightestFlame`. Vanilla `CanonicalVars` are Max HP 1, Energy 2, Cards 2; `OnUpgrade()` raises Energy and Cards by 1. EZMB adds `CardKeyword.Exhaust` through `CardModel.CanonicalKeywords`, raises the `CardsVar` by 1, and keeps an `OnPlayWrapper` exhaust backstop. Expected live behavior is unupgraded draw 3, upgraded draw 4, visible Exhaust keyword, and exhaust-pile result after play.
- Doormaker / Door Wedge: active source, localization, and guard tests are expected to require absence of Door Wedge and no `DOORMAKER_BOSS` mapping. Historical docs may mention the removed behavior only as historical context.
- Aeonglass: v0.105 source has `AEONGLASS_BOSS` and `MONSTER.AEONGLASS`; `AeonglassBoss.GenerateMonsters()` creates `ModelDb.Monster<Aeonglass>().ToMutable()`. EZMB maps `AEONGLASS_BOSS` to `BossSealId.AeonglassStrength` and applies `+5 Strength` to the enemy with model id `MONSTER.AEONGLASS`, with no complex Brand/seal mechanic added.

## Live Gates

| Gate | Result |
| --- | --- |
| Normal Steam-client Mod Settings | Passed. BaseLib page visible/enabled; EZ Micro Balance page appears as `寰钩琛�?with `鏃犲彲閰嶇疆閫夐」銆�? main menu/log show only BaseLib + EZ Micro Balance loaded; log has 0 `ERROR` lines and 0 release-blocking signatures. |
| Clean normal Steam-client `godot.log` | Isolated Steam startup snapshot and Mod Settings log collected; both have 0 `ERROR` lines and 0 release-blocking signatures. |
| A0/A10/A20 single-player spot checks | User-reported pass, plus Codex-observed normal-Steam DevConsole combat smoke for A0/A10/A20. Natural route-click first-node checks remain unrun. |
| Pumpkin Candle vanilla/no override | Source-verified pending live spot check. |
| Quality Flame unupgraded/upgraded | Source-verified pending live spot check. |
| Door Wedge absence | Active source/localization `rg` returned no Door Wedge / `DOORMAKER_BOSS` matches; release-facing docs mention it only as removed/historical. |
| Aeonglass +5 Strength | Source-verified pending live boss route/seed check. |
| A11 map geometry | Act 1 normal-Steam spot check passed: A11 selected through the original UI, log reports columns=8/rows=17 with 1 late route row, screenshot shows normal route nodes/no A11 marker, and saved-map JSON records 8 columns. Act 2/3 normal-Steam DevConsole observation passed for map surface and log geometry: Act 2 columns=8/rows=16 with 1 late route row; Act 3 columns=8/rows=16 with 2 late route rows. Natural route traversal and boss reachability remain pending. |
| Rootblight English/ZHS hover and A14 starter notice | Targeted normal-Steam spot checks passed for English and Simplified Chinese Rootblight I/II/III and Blight Sprout hovers, plus the A14 Neow starter Rootblight-added notice in both languages. Combat-end notices, full behavior, generated-art visual verification, and co-op checks remain pending. |
| Save/load | Minimal A11 map save/load spot check passed after first node: `current_run.save` was written, Continue loaded the run, and the map reopened with columns=8/rows=17. Ancient save/load rows and co-op save/load remain pending. |
| Multiplayer matrix | Pending two-PC Steam-client runbook execution. |

Earlier Mod Settings UI attempts are superseded by the `20260508-095137` recheck, which captured the EZ Micro Balance config page after the no-op page was added.

## Pending RC1 Items

- Remaining Rootblight visual feedback: full combat-end behavior checks, clean non-paused combat-end notice timing, Blight Sprout combat-end notices, co-op ownership/desync checks, and generated-art visual verification. The final source now uses a top-level high-z, input-passthrough, 5-second overlay notice for combat-end additions, but that final hardening still needs manual verification.
- Rootblight generated-art visual check.
- Broader A11 map traversal and boss-reachability diagnostics beyond the Act 1/2/3 width/row spot checks.
- Natural route-click first-node checks, if required beyond the DevConsole combat smoke.
- Multiplayer matrix.
- Ancient reward and co-op save/load verification.



## 2026-05-10 Upload Review: `godot2026-05-10T06.07.51.log` INVALID for release evidence

Uploaded player report `godot2026-05-10T06.07.51.log` is not valid RC1/A1.05.08 release evidence.

Reasons:
- Runtime is `v0.105.1`, not the `v0.105.0`/expected-clean package context in earlier baseline notes.
- Log reports `Loaded 18 mods (19 total)` and is not BaseLib + EZMicroBalance isolated.
- Non-EZMB mods emit runtime errors before gameplay:
  - `RouteSuggestConfig.json` missing `id`.
  - `sts2-heybox-support\mod_mainfest.json` missing `id`.
  - `Heybox`: `ModManager.GetModNameList Method NotFound`.
  - `SpeedX`: undefined target patch on `NRewardsScreen`.
  - `Act4Heart`: `ConfigMessage.get_ShouldBuffer` `TypeLoadException`.
- BaseLib and EZMB initialize, but BaseLib logs `Found 12 SavedSpireFields` (expected `13` for this package state).
- Therefore gameplay conclusions from this file are blocked by polluted environment + hash-mismatch risk.

Clean retest rules:
1. Before collecting release logs, move all entries in `<GameRoot>\mods` except:
   - `BaseLib`
   - `EZMicroBalance`
2. Run a fresh normal-Steam launch and confirm `Loaded 2 mods (2 total)` and `Found 13 SavedSpireFields`.
3. Run `scripts/check-installed-ezmb-package.ps1` against the live install and require PASS before gameplay evidence.
4. If clean hash-matching logs still report `Found 12 SavedSpireFields`, do not judge gameplay from that run; open a source/doc mismatch investigation around `AncientSavedStateFields.UrdaStateKey` registration and BaseLib SavedSpireField discovery.
5. Tag this file as invalid and do not attach it to release artifact acceptance.
