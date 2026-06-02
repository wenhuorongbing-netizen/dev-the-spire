# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**OFF + CANARY + ADDITIVEBATCH1 LOADER SMOKE PASSED / QA AND GAMEPLAY PENDING** - official STS2-RitsuLib `v0.3.10` is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` and satisfies the installed Spire Plus package dependency (`STS2-RitsuLib >= 0.3.2`). After the RitsuLib target-name fix, diagnostic Off and CanaryOnly smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. This is loader/gate proof only; gameplay, Mod Settings screenshots, save-load, co-op, independent QA rerun, clean-worktree decision, and versioned tester-package handoff remain pending.

2026-05-31 Runtime Proof + Governance Closure check:

| Path | Result |
| --- | --- |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Present (`v0.3.10`, includes `lib\0.106.1`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Present in v15 run; archived as `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` |

The latest diagnostic logs prove the loader and Sts1Events gates, but they do not prove live gameplay behavior. No AdditiveBatch1, gameplay, screenshot, save-load, replacement, multiplayer, or QA runtime smoke was captured.

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

Latest prerequisite evidence: installed manifest `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json` reports version `0.3.10`; `ritsulib-variants.json` includes `compatTarget` `0.106.1`. Older missing-path evidence remains in `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt` at HEAD `87820303`.

## Prerequisites

1. Clean Steam client install with Slay the Spire 2 v0.106.1
2. BaseLib v3.1.4 installed at `<GameRoot>\mods\BaseLib`
3. STS2-RitsuLib v0.3.2+ installed at `<GameRoot>\mods\STS2-RitsuLib` (current local install: `v0.3.10` on E-drive)
4. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.84.zip` installed at `<GameRoot>\mods\EZMicroBalance`
5. No other mods enabled
6. If using `scripts\spire-plus-live-session.ps1`, invoke it with the E-drive `-GameRoot` and `-SteamExe`, pass the chosen `-SteamUserId`, and ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.3.2` | PASS: E-drive install is `v0.3.10` with `lib\0.106.1` variant |
| 2 | Launch game via Steam | Main menu loads without crash | [PASS] K1 smoke-k1-off-20260602-145938: main menu reached in 40s |
| 3 | Check `godot.log` for EZMicroBalance init | Single Spire Plus initialization line, no errors | [PASS] K1 smoke-k1-off-20260602-145938: Spire Plus initialized, clean audit |
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
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | Controlled prototype only: 11 registration calls / 10 event types, no TODO/BLOCKED events | [PENDING] Only after Off + CanaryOnly smoke passes |
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

- All loader smoke items pass
- Off mode proves 0 Sts1Events registrations in `godot.log`
- CanaryOnly proves exactly 4 canary registrations in `godot.log`
- Mod Settings UI verified
- At least 3 of 5 basic gameplay items pass, with shop and save/load mandatory
- Multiplayer disposition confirmed fail-closed
- `godot.log` contains 0 release-blocking hits

## Notes

- This checklist supplements `docs/test-plan.md` and `docs/release-checklist.md`.
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/` or a documented runtime-evidence folder.
- If any loader smoke item fails, do not proceed to gameplay items; diagnose first.
- Runtime smoke is the Batch 4c decision gate. No Batch 4c patch migration is allowed while this checklist is blocked.
