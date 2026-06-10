# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**HISTORICAL OFF + CANARY + ADDITIVEBATCH1 LOADER SMOKE PASSED / CURRENT V0.107.0 PACKAGE SMOKE FAILED CLEAN AUDIT** - official STS2-RitsuLib `v0.4.16` is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.107.0` and satisfies the installed Spire Plus package dependency (`STS2-RitsuLib >= 0.3.2`). The current dirty source and beta.84 package line stay on compile/manifest `0.3.2`; a future `0.4.16` metadata bump belongs in an owner-approved versioned package pass. After the earlier RitsuLib target-name fix, diagnostic Off, CanaryOnly, and AdditiveBatch1 smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. This is historical `v0.106.1` loader/gate proof only. The current local game install is `v0.107.0`, RitsuLib now has a matching installed variant, and installed beta.84 package parity was restored on 2026-06-10. The fresh package-parity Off smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` reached main menu but is not clean current-runtime proof: 11 Godot ERROR hits, 1 Spire Plus error/exception hit, 8 optional ModPatcher failures, and an `EctoplasmGoldGatePatch` initializer exception from stale package API targets. Gameplay, Mod Settings screenshots, save-load, co-op, independent QA rerun, clean-worktree decision, current-source package decision, and versioned tester-package handoff remain pending.

2026-05-31 Runtime Proof + Governance Closure check:

| Path | Result |
| --- | --- |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Present (`v0.4.16`, includes `lib\0.107.0`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Present in v15 run; archived as `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` |

The latest successful diagnostic logs prove the loader and Sts1Events gates for the historical `v0.106.1` setup, but they do not prove current `v0.107.0` runtime behavior or live gameplay behavior. The local RitsuLib install now has a matching `0.107.0` variant, and the current-package beta.84 smoke failed clean audit under `v0.107.0`. No event-encounter gameplay, screenshot, save-load, replacement, multiplayer, or QA runtime smoke was captured.

Revision J/v15 failed-smoke evidence and target-fix follow-up:

| Evidence | Result |
| --- | --- |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-direct-exe-steam-init-fail.log` | Direct executable launch failed Steam initialization before mod loading. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch.log` | RitsuLib/BaseLib loaded; `EZMicroBalance` skipped as disabled; stale/duplicate manifest errors present. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch-audit.json` | Not clean; 3 Godot ERROR lines. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\attempt-notes.md` | Records cleanup and settings restore. |
| `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` | BaseLib, RitsuLib, and Spire Plus loaded; main menu reached; StS1Events default Off logged. |
| `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/audit-godot-log.after-launch.json` | Not clean; 11 Godot ERROR hits from `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. |
| `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/restore-state.json` | Restored 25 isolated mods, stopped `SlayTheSpire2`, restored settings hashes. |
| `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/godot.log.after-launch` | PASS: Steam Off-mode smoke reached main menu, loaded exactly 3 mods, applied 25/25 Spire Plus patches, and logged Sts1Events disabled/default Off. |
| `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/godot-log-audit.json` | PASS: clean audit, 0 release-blocking signature hits. |
| `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/godot.log.after-direct-launch` | PASS: direct CanaryOnly smoke reached main menu, loaded exactly 3 mods, applied 25/25 patches, and registered 4 canary events. |
| `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/godot-log-audit.json` | PASS: clean audit, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot.log.after-launch` | PASS: K1 fresh Off-mode Steam smoke at HEAD 8f2d79b4 reached main menu in 40s, loaded exactly 3 mods (BaseLib v3.1.4, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84), applied 25/25 Spire Plus patches, found 30 SavedSpireFields, and logged Sts1Events disabled/default Off. |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot-log-audit.json` | PASS: K1 clean audit: 0 Godot ERROR, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot.log.after-launch` | PASS: K1 fresh CanaryOnly direct launch (steam_appid.txt + env var) at HEAD 8f2d79b4 reached main menu in 22s, loaded exactly 3 mods, applied 25/25 patches, found 30 SavedSpireFields, and registered exactly 4 canary events: Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot-log-audit.json` | PASS: K1 clean audit: 0 Godot ERROR, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot.log.after-launch` | PASS: AdditiveBatch1 direct launch reached main menu, loaded exactly 3 mods, applied 25/25 patches, and registered exactly 10 event types through 11 registration calls. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot-log-audit.json` | PASS: clean audit, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\package-parity-restore-20260610-091943\package-parity-restore.json` | PASS: installed beta.84 DLL restored from package staging; stale installed DLL `69DEB870A226FD58EC9AF9D8895EEDC832B5D9A8903A2D79B1D6CEDC2E114EB1` was backed up and replaced with packaged DLL `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766`. |
| `scripts\check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'` | PASS after DLL restore: installed DLL, manifest, PCK, README, game-root ZIP, and Sere Talon/Tanx Claws PCK content match the beta.84 handoff. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot.log.after-launch` | FAIL / main menu reached: BaseLib loaded, RitsuLib `v0.4.16` selected compat branch `0.107.0`, and Spire Plus package beta.84 loaded far enough for main menu, but Spire Plus had 8 optional ModPatcher failures and a `TargetInvocationException` rooted in stale `EctoplasmGoldGatePatch` target API drift. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot-log-audit.json` | FAIL: 11 Godot ERROR hits and 1 Spire Plus error/exception hit. No `MissingMethodException` or `TypeLoadException` hits. |

Latest prerequisite evidence: installed game `release_info.json` reports Slay the Spire 2 `v0.107.0`; installed manifest `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json` reports version `0.4.16`; `ritsulib-variants.json` includes `compatTarget` `0.107.0`. Older missing-path evidence remains in `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt` at HEAD `87820303`.

## Prerequisites

1. Clean Steam client install with a game version that has a matching RitsuLib variant. Historical proof used Slay the Spire 2 `v0.106.1`; current local install is `v0.107.0` with installed RitsuLib `v0.4.16` / `lib\0.107.0`.
2. BaseLib v3.1.4 installed at `<GameRoot>\mods\BaseLib`
3. STS2-RitsuLib v0.3.2+ installed at `<GameRoot>\mods\STS2-RitsuLib` (current local install: `v0.4.16` on E-drive)
4. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.84.zip` installed at `<GameRoot>\mods\EZMicroBalance`
5. No other mods enabled
6. If using `scripts\spire-plus-live-session.ps1`, invoke it with the E-drive `-GameRoot` and `-SteamExe`, pass the chosen `-SteamUserId`, and ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.3.2` | PASS: E-drive install is `v0.4.16` with `lib\0.107.0` variant |
| 2 | Launch game via Steam | Main menu loads without crash | [HISTORICAL PASS] K1 smoke-k1-off-20260602-145938: main menu reached in 40s; [CURRENT PACKAGE FAIL CLEAN AUDIT] v01070-off-package-parity-20260610-092045 reached main menu but had Spire Plus initializer errors |
| 3 | Check `godot.log` for EZMicroBalance init | Single Spire Plus initialization line, no errors | [HISTORICAL PASS] K1 smoke-k1-off-20260602-145938: Spire Plus initialized, clean audit; [CURRENT PACKAGE FAIL] beta.84 on `v0.107.0` throws during Spire Plus initialization |
| 4 | Check `godot.log` for BaseLib init | BaseLib initializes before Spire Plus | [PASS] K1 smoke-k1-off-20260602-145938: BaseLib v3.1.4 initialized before RitsuLib and Spire Plus |
| 5 | Check `godot.log` for STS2-RitsuLib init | RitsuLib initializes, no errors | [PASS] K1 smoke-k1-off-20260602-145938: RitsuLib v0.3.10 initializes, clean audit |
| 6 | Check `godot.log` for RitsuLib bootstrap | Spire Plus RitsuLib bootstrap starts | [PASS] K1 smoke-k1-off-20260602-145938: RitsuLib bootstrap starting |
| 7 | Check `godot.log` for ModPatcher count | 25 ModPatcher patches applied; remaining raw Harmony patches load without dependency failures | [PASS] K1 smoke-k1-off-20260602-145938: 25/25 patches applied |
| 8 | Check `godot.log` for release-blocking log hits | 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failure, or release-blocking audit hits | [PASS] K1 smoke-k1-off-20260602-145938 and smoke-k1-canary3-20260602-151104: clean audits, 0 release-blocking hits |
| 9 | Check SavedSpireFields count | 30 SavedSpireFields registered | [PASS] K1 smoke-k1-off-20260602-145938: 30 SavedSpireFields |

### Sts1Events Runtime Gates

| Mode | Required env | Expected | Evidence |
| --- | --- | --- | --- |
| Off | unset / empty / invalid `SPIREPLUS_STS1_EVENT_MODE` | 0 Sts1Events registrations, no `[StS1 Events]` registration lines | [PASS] K1 smoke-k1-off-20260602-145938: 0 StS1 registrations, clean audit |
| CanaryOnly | `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` | Exactly 4 canary registrations: Big Fish, Golden Idol, The Lab, Divine Fountain | [PASS] K1 smoke-k1-canary3-20260602-151104: exactly 4 canary registrations (BigFish, GoldenIdol, TheLab, DivineFountain), clean audit |
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | Controlled prototype only: 11 registration calls / 10 event types, no TODO/BLOCKED events | [PASS] `.tools\runtime-evidence\additive-batch1-20260602-150445`: exactly 10 event types through 11 registration calls, clean audit |
| AdditiveAllDraft | `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; dev-only all-draft mode includes TODO/BLOCKED content | [DO NOT USE for tester/release paths] |
| ReplaceUnknownEventsPrototype | `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` plus `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; debug-only replacement prototype; normal builds fail closed | [DO NOT USE for tester/release paths] |

### Mod Settings UI

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Navigate to Mod Settings | Spire Plus appears in mod list | [PENDING] |
| 2 | Open Spire Plus settings | Settings UI renders without errors | [PENDING] |
| 3 | Verify feature toggles | All default-on features listed, toggles functional | [PENDING] |

### Basic Gameplay

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Start new run | Run starts without errors | [PENDING] |
| 2 | Play first combat | Combat resolves normally | [PENDING] |
| 3 | Visit first shop | Shop renders, no errors | [PENDING] |
| 4 | Check Ancient reward visibility | Default-on Ancients show rebalanced rewards | [PENDING] |
| 5 | Save and reload | Save/load succeeds, no data loss | [PENDING] |

### Multiplayer Disposition

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Attempt co-op with Spire Plus enabled | Co-op fails closed for unverified shared-state gameplay | [PENDING] |
| 2 | Check multiplayer diagnostics log | No crash, clean fail-closed message | [PENDING] |

## Exit Criteria

- All loader smoke items pass.
- Off mode proves 0 Sts1Events registrations in `godot.log`.
- CanaryOnly proves exactly 4 canary registrations in `godot.log`.
- AdditiveBatch1 proves 10 event types through 11 registration calls.
- Mod Settings UI verified.
- At least 3 of 5 basic gameplay items pass, with shop and save/load mandatory.
- Multiplayer disposition confirmed fail-closed.
- `godot.log` contains 0 release-blocking hits.

Current exit status: historical loader-only rows pass for `v0.106.1`; installed beta.84 package parity passes after the 2026-06-10 DLL restore, but fresh `v0.107.0` current-package loader proof failed clean audit because beta.84 still targets stale game APIs. Mod Settings UI, gameplay, save-load, and multiplayer rows remain pending.

## Notes

- This checklist supplements `docs/test-plan.md` and `docs/release-checklist.md`.
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/` or a documented runtime-evidence folder.
- If any loader smoke item fails, do not proceed to gameplay items; diagnose first.
- Loader-gate smoke is sufficient only for Batch 4c candidate review. No Batch 4c patch migration is allowed without explicit owner acceptance and fresh validation.
