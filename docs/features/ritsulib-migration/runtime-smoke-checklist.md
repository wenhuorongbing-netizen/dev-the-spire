# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**CURRENT V0.107.0 BETA.86 ADDITIVEBATCH1 LOADER SMOKE PASSED / GAMEPLAY STILL PENDING** - official STS2-RitsuLib `v0.4.16` is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.107.0` and satisfies the installed Spire Plus package dependency (`STS2-RitsuLib >= 0.3.2`). The current dirty source and beta.86 manifest line still use compile/manifest `0.3.2`; a future `0.4.16` metadata bump belongs in an owner-approved versioned package pass. Historical diagnostic Off, CanaryOnly, and AdditiveBatch1 smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied for the older `v0.106.1` lane. The beta.84 package-parity Off smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is red root-cause evidence: 17/25 Spire Plus patches, 8 optional failures, and an `EctoplasmGoldGatePatch` initializer exception. The beta.85 Off and CanaryOnly smokes remain previous-package `v0.107.0` loader context. The current beta.86 AdditiveBatch1 direct smoke at `.tools/runtime-evidence/v01070-beta86-additive-batch1-direct-20260618-031254/` reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus patches, found 30 SavedSpireFields, audited clean, and passed retained log/packet verifiers with 10 event types / 14 registration lines. The earlier beta.86 Steam-client attempt at `.tools/runtime-evidence/v01070-beta86-additive-batch1-20260618-031043/` is diagnostic only because StS1 stayed disabled when the already-running Steam client did not propagate the transient PowerShell environment. Gameplay, Mod Settings screenshots, save-load, co-op, independent QA rerun, clean-worktree decision, current-source package decision, and versioned tester-package handoff remain pending.

Coordination boundary: run this checklist's launch, gameplay, build, publish, package, or release-evidence steps only when a controlled validation lane is assigned. During a pause, use this checklist only for read-only/static planning, source-only `-PrintExpected` output, or verification of already-captured logs. The 2026-06-17 lane captured CanaryOnly successfully and a non-passing beta.85 AdditiveBatch1 diagnostic packet; the 2026-06-18 beta.86 direct packet supersedes that AdditiveBatch1 drift for loader/registration proof only.

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

The latest successful beta.86 AdditiveBatch1 direct log proves loader startup, RitsuLib compat selection, 25/25 Spire Plus ModPatcher application, and AdditiveBatch1 registration shape for `v0.107.0`. It does not prove event-encounter gameplay, screenshots, save-load, replacement, multiplayer, or QA runtime behavior.

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
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot.log.after-launch` | PASS / historical only: AdditiveBatch1 direct launch reached main menu, loaded exactly 3 mods, applied 25/25 patches, and registered exactly 10 event types through the old 11 registration calls. Current source expects 10 event types through 14 registration calls; current `v0.107.0` beta.86 loader/registration proof is the retained direct packet below. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot-log-audit.json` | PASS: clean audit, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\package-parity-restore-20260610-091943\package-parity-restore.json` | PASS: installed beta.84 DLL restored from package staging; stale installed DLL `69DEB870A226FD58EC9AF9D8895EEDC832B5D9A8903A2D79B1D6CEDC2E114EB1` was backed up and replaced with packaged DLL `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766`. |
| `scripts\check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'` | PASS after DLL restore: installed DLL, manifest, PCK, README, game-root ZIP, and Sere Talon/Tanx Claws PCK content match the beta.84 handoff. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot.log.after-launch` | FAIL / main menu reached: BaseLib loaded, RitsuLib `v0.4.16` selected compat branch `0.107.0`, and Spire Plus package beta.84 loaded far enough for main menu, but Spire Plus had 8 optional ModPatcher failures and a `TargetInvocationException` rooted in stale `EctoplasmGoldGatePatch` target API drift. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot-log-audit.json` | FAIL: 11 Godot ERROR hits and 1 Spire Plus error/exception hit. No `MissingMethodException` or `TypeLoadException` hits. |
| `.tools\runtime-evidence\v01070-current-source-getter-targets-20260610-1000\godot.log.after-launch` | PASS / source-fix probe: beta84/current-source smoke on `v0.107.0`, 25/25 patches applied, clean audit. Useful as drift-fix direction evidence, not beta.85 package proof. |
| `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510\godot.log.after-launch` | PASS / current Off loader proof: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.16` compat branch `0.107.0`, 25/25 Spire Plus patches applied, StS1Events default Off, main menu reached. |
| `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510\godot-log-audit.json` | PASS: clean audit with 0 Spire Plus error/exception, 0 Godot ERROR line, 0 MissingMethodException, and 0 TypeLoadException hits. |
| `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621\godot.log.after-launch` | PASS / current CanaryOnly proof: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.16` compat branch `0.107.0`, 6 registered-event lines, 4 event types, main menu reached. |
| `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | PASS: retained enabled-mode log verifier checks=20 / mismatches=0; packet verifier checks=45 / mismatches=0. A 2026-06-18 tuple-aware verifier dry-run returned 21 / 0 without overwriting the retained report. |
| `.tools\runtime-evidence\v01070-beta85-additive-batch1-20260617-233759\godot.log.after-launch` | FAIL as proof / diagnostic: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.16` compat branch `0.107.0`, main menu reached and audit clean, but runtime logged 13 registered-event lines instead of the current-source 14. Static log/source comparison shows `Sts1TheCleric` logged once as shared, while current source expects `Overgrowth` and `Underdocks` Act registrations. |
| `.tools\runtime-evidence\v01070-beta85-additive-batch1-20260617-233759\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | FAIL: retained enabled-mode log verifier mismatches=1 and packet verifier mismatches=1, both on the 13/14 registration-line count. A 2026-06-18 tuple-aware verifier dry-run returned 21 / 2 and reported missing `ActEvent:Overgrowth:Sts1TheCleric` plus `ActEvent:Underdocks:Sts1TheCleric`, with unexpected `SharedEvent:Shared:Sts1TheCleric`. |
| `.tools\runtime-evidence\v01070-beta86-additive-batch1-20260618-031043\godot.log.after-launch` | FAIL as enabled-mode proof / diagnostic: Spire Plus `v0.1.0-private-beta.86` reached main menu and audited clean, but StS1Events remained disabled because the already-running Steam client did not propagate the transient PowerShell `SPIREPLUS_STS1_EVENT_MODE` environment. |
| `.tools\runtime-evidence\v01070-beta86-additive-batch1-direct-20260618-031254\godot.log.after-launch` | PASS / current AdditiveBatch1 proof: Spire Plus `v0.1.0-private-beta.86`, RitsuLib `0.4.16` compat branch `0.107.0`, 25/25 Spire Plus patches applied, 30 SavedSpireFields, 14 registered-event lines, 10 event types, main menu reached. |
| `.tools\runtime-evidence\v01070-beta86-additive-batch1-direct-20260618-031254\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | PASS: retained enabled-mode log verifier checks=21 / mismatches=0 and packet verifier checks=45 / mismatches=0, including exact act/shared tuple parity. |

Latest prerequisite evidence: installed game `release_info.json` reports Slay the Spire 2 `v0.107.0`; installed manifest `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json` reports version `0.4.16`; `ritsulib-variants.json` includes `compatTarget` `0.107.0`. Older missing-path evidence remains in `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt` at HEAD `87820303`.

## Prerequisites

1. Clean Steam client install with a game version that has a matching RitsuLib variant. Historical proof used Slay the Spire 2 `v0.106.1`; current local install is `v0.107.0` with installed RitsuLib `v0.4.16` / `lib\0.107.0`.
2. BaseLib v3.1.4 installed at `<GameRoot>\mods\BaseLib`
3. STS2-RitsuLib v0.3.2+ installed at `<GameRoot>\mods\STS2-RitsuLib` (current local install: `v0.4.16` on E-drive)
4. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.86.zip` installed at `<GameRoot>\mods\EZMicroBalance`; package checker is recorded in `PROJECT_STATE.md` as passed on 2026-06-18.
5. No other mods enabled
6. If using `scripts\spire-plus-live-session.ps1`, prepare with explicit E-drive `-GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'`, `-SteamExe 'E:\Steam\steam.exe'`, the chosen `-SteamUserId`, and `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`; restore after evidence capture with `-Mode Restore -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`; ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.3.2` | PASS: E-drive install is `v0.4.16` with `lib\0.107.0` variant |
| 2 | Launch game via Steam | Main menu loads without crash | [PASS] beta85 v01070-beta85-current-package-runtime-fix-20260611-0510 reached main menu in 71,172ms |
| 3 | Check `godot.log` for EZMicroBalance init | Single Spire Plus initialization line, no errors | [PASS] beta85 log initializes Spire Plus and audit has 0 Spire Plus error/exception hits |
| 4 | Check `godot.log` for BaseLib init | BaseLib initializes before Spire Plus | [PASS] K1 smoke-k1-off-20260602-145938: BaseLib v3.1.4 initialized before RitsuLib and Spire Plus |
| 5 | Check `godot.log` for STS2-RitsuLib init | RitsuLib initializes, no errors | [PASS] beta85 log reports RitsuLib `0.4.16` compat branch `0.107.0`; audit clean |
| 6 | Check `godot.log` for RitsuLib bootstrap | Spire Plus RitsuLib bootstrap starts | [PASS] beta85 log has `RitsuLib 0.4.16 bootstrap starting` |
| 7 | Check `godot.log` for ModPatcher count | 25 ModPatcher patches applied; remaining raw Harmony patches load without dependency failures | [PASS] beta85 log has `25 applied, 0 ignored, 0 failed, 25 total` and `ModPatcher applied 25 patches (25 registered)` |
| 8 | Check `godot.log` for release-blocking log hits | 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failure, or release-blocking audit hits | [PASS] beta85 audit clean: 0 Spire Plus error/exception, 0 Godot ERROR line, 0 MissingMethodException, 0 TypeLoadException |
| 9 | Check SavedSpireFields count | 30 SavedSpireFields registered | [PASS] K1 smoke-k1-off-20260602-145938: 30 SavedSpireFields |

### Sts1Events Runtime Gates

| Mode | Required env | Expected | Evidence |
| --- | --- | --- | --- |
| Off | unset / empty / invalid `SPIREPLUS_STS1_EVENT_MODE` | 0 Sts1Events registrations, no `[StS1 Events]` registration lines | [PASS] beta85 v01070-beta85-current-package-runtime-fix-20260611-0510: StS1Events `bootstrap=disabled, live=Disabled`; audit clean |
| CanaryOnly | `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` | 4 canary event types / 6 registration calls: Big Fish and Golden Idol in both Act 1 buckets, plus The Lab and Divine Fountain as shared events | [PASS] beta85 v01070-beta85-canary-20260617-233621: 4 event types / 6 registered-event lines; retained log verifier 20 / 0; tuple-aware dry-run 21 / 0; packet verifier 45 / 0 |
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | Controlled prototype only: 14 registration calls / 10 event types, no TODO/BLOCKED events | [PASS] beta86 v01070-beta86-additive-batch1-direct-20260618-031254: 10 event types / 14 registered-event lines; retained log verifier 21 / 0; packet verifier 45 / 0 |
| AdditiveAllDraft | `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; dev-only all-draft mode includes TODO/BLOCKED content | [DO NOT USE for tester/release paths] |
| ReplaceUnknownEventsPrototype | `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` plus `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; debug-only replacement prototype; normal builds fail closed | [DO NOT USE for tester/release paths] |

After any future enabled-mode smoke copies `godot.log` and writes `godot-log-audit.json`, verify the copied files without launching anything:

```powershell
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath "<evidence>\godot.log.after-launch" -AuditPath "<evidence>\godot-log-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.86 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.16 -ExpectedGameVersion 0.107.0 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath "<evidence>\godot.log.after-launch" -AuditPath "<evidence>\godot-log-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.86 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.16 -ExpectedGameVersion 0.107.0 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
```

For helper-created evidence folders, also verify the packet has the expected copied files, session state, restore state, isolated-mod list, and clean nested log/audit result:

```powershell
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.86 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.16 -ExpectedGameVersion 0.107.0 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.86 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.16 -ExpectedGameVersion 0.107.0 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
```

For enabled-mode copied logs, the log verifier requires explicit expected package-version, Ritsu compat-branch, RitsuLib package-version, and game-version checks, and the observed registered event-line count matches the source-derived registration-call count. It verifies registration-call count, event class set, and observed registration tuples parsed from `Registered act event` / `Registered shared event` log lines when those tuple details are present. If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly. For enabled-mode packets, the helper-created `session-state.json` must record `Sts1EventModeEnvironment` equal to the requested mode, `AllowedModIds` exactly equal to BaseLib, STS2-RitsuLib, and EZMicroBalance, moved-mod source/destination paths stay under the recorded mods root and evidence `isolated-mods` folder, restore counts match the session moved-mod and moved-current-run lists, and the helper-copied `game-release-info.json` must match the expected game version. The packet verifier rejects missing or mismatched enabled-mode setup metadata, rejects unsafe-mode environment leakage for CanaryOnly/AdditiveBatch1 evidence, rejects `-AllowMissingSessionState` / `-AllowMissingRestoreState` for enabled-mode packets, and requires explicit expected package-version, Ritsu compat-branch, RitsuLib package-version, and game-version checks. Keep `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` in the same evidence folder as the copied log and audit so the verifier decisions remain reviewable.

Source-only expected shapes can be printed during the coordination pause with `-PrintExpected`. That output is not enabled-mode proof; it only preserves the current expected class set and source-derived registration-call count.

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
- CanaryOnly proves 4 canary event types through 6 registration calls in `godot.log`.
- AdditiveBatch1 proves 10 event types through 14 registration calls.
- Mod Settings UI verified.
- At least 3 of 5 basic gameplay items pass, with shop and save/load mandatory.
- Multiplayer disposition confirmed fail-closed.
- `godot.log` contains 0 release-blocking hits.

Current exit status: current beta.86 AdditiveBatch1 loader row passes for `v0.107.0`; beta.85 Off and CanaryOnly loader rows remain previous-package context. Mod Settings UI, gameplay, save-load, and multiplayer rows remain pending.

## Notes

- This checklist supplements `docs/test-plan.md` and `docs/release-checklist.md`.
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/` or a documented runtime-evidence folder.
- If any loader smoke item fails, do not proceed to gameplay items; diagnose first.
- Loader-gate smoke is sufficient only for Batch 4c candidate review. No Batch 4c patch migration is allowed without explicit owner acceptance and fresh validation.
