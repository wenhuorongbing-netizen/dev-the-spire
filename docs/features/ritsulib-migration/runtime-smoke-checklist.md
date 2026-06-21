# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**CURRENT V0.107.1 BETA.93 RITSULIB-ONLY LOADER SMOKES PASSED / GAMEPLAY PENDING** - official STS2-RitsuLib `v0.4.31` is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.107.1` and satisfies the installed Spire Plus package dependency (`STS2-RitsuLib >= 0.4.31`). Spire Plus no longer depends on BaseLib in project, manifest, package, or current runtime proof. Historical diagnostic Off, CanaryOnly, and AdditiveBatch1 smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded for older lanes. The beta.84 package-parity Off smoke remains red root-cause evidence for stale Spire Plus API targets, beta.85/beta.86/beta.87 remain previous-package `v0.107.0` loader context, beta.88 remains previous BaseLib-backed `v0.107.1` loader context, and beta.90 remains previous RitsuLib-only package context. Current beta.93 Off proof at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance`, audited clean, applied 25/25 Spire Plus patches, and passed the Off packet verifier 43 / 0. Current beta.93 AdditiveBatch1 proof at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` reached main menu with the same two mods, registered 10 event types through 14 calls, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verifier 61 / 0. Gameplay, Mod Settings screenshots, save-load, co-op, independent QA rerun, and versioned tester-package handoff remain pending.

Coordination boundary: run this checklist's launch, gameplay, build, publish, package, or release-evidence steps only when a controlled validation lane is assigned. During a pause, use this checklist only for read-only/static planning, source-only `-PrintExpected` output, or verification of already-captured logs. The 2026-06-17 lane captured CanaryOnly successfully and a non-passing beta.85 AdditiveBatch1 diagnostic packet; the 2026-06-18 beta.87 direct packet superseded that AdditiveBatch1 drift for `v0.107.0` loader/registration proof only; the 2026-06-19 beta.88 direct packet superseded the BaseLib `v3.2.1` clean-audit failure for the previous BaseLib-backed package; the 2026-06-21 beta.93 direct packets supersede beta.88/beta.90 for current RitsuLib-only loader/registration proof only.

2026-05-31 Runtime Proof + Governance Closure check:

| Path | Result |
| --- | --- |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Present (`v0.4.31`, includes `lib\0.107.1`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present locally for other-mod/history context; not a current Spire Plus prerequisite |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Present in v15 run; archived as `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` |

The retained successful beta.93 Off and AdditiveBatch1 direct logs prove loader startup, RitsuLib compat selection, 25/25 Spire Plus ModPatcher application, BaseLib independence for the controlled lane, and AdditiveBatch1 registration shape for current `v0.107.1`. They do not prove event-encounter gameplay, screenshots, save-load, replacement, multiplayer, or QA runtime behavior. The retained beta.87 direct packet remains clean `v0.107.0` previous-game-version context only, and the beta.88 packet is previous BaseLib-backed context.

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
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot.log.after-launch` | PASS: K1 fresh Off-mode Steam smoke at HEAD 8f2d79b4 reached main menu in 40s, loaded exactly 3 mods (BaseLib v3.2.1, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84), applied 25/25 Spire Plus patches, found 30 SavedSpireFields, and logged Sts1Events disabled/default Off. |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot-log-audit.json` | PASS: K1 clean audit: 0 Godot ERROR, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot.log.after-launch` | PASS: K1 fresh CanaryOnly direct launch (steam_appid.txt + env var) at HEAD 8f2d79b4 reached main menu in 22s, loaded exactly 3 mods, applied 25/25 patches, found 30 SavedSpireFields, and registered exactly 4 canary events: Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot-log-audit.json` | PASS: K1 clean audit: 0 Godot ERROR, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot.log.after-launch` | PASS / historical only: AdditiveBatch1 direct launch reached main menu, loaded exactly 3 mods, applied 25/25 patches, and registered exactly 10 event types through the old 11 registration calls. Current source expects 10 event types through 14 registration calls; retained `v0.107.0` beta.87 loader/registration proof is the direct packet below. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot-log-audit.json` | PASS: clean audit, 0 release-blocking signature hits. |
| `.tools\runtime-evidence\package-parity-restore-20260610-091943\package-parity-restore.json` | PASS: installed beta.84 DLL restored from package staging; stale installed DLL `69DEB870A226FD58EC9AF9D8895EEDC832B5D9A8903A2D79B1D6CEDC2E114EB1` was backed up and replaced with packaged DLL `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766`. |
| `scripts\check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'` | PASS after DLL restore: installed DLL, manifest, PCK, README, game-root ZIP, and Sere Talon/Tanx Claws PCK content match the beta.84 handoff. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot.log.after-launch` | FAIL / main menu reached: BaseLib loaded, RitsuLib `v0.4.24` selected compat branch `0.107.0`, and Spire Plus package beta.84 loaded far enough for main menu, but Spire Plus had 8 optional ModPatcher failures and a `TargetInvocationException` rooted in stale `EctoplasmGoldGatePatch` target API drift. |
| `.tools\runtime-evidence\v01070-off-package-parity-20260610-092045\godot-log-audit.json` | FAIL: 11 Godot ERROR hits and 1 Spire Plus error/exception hit. No `MissingMethodException` or `TypeLoadException` hits. |
| `.tools\runtime-evidence\v01070-current-source-getter-targets-20260610-1000\godot.log.after-launch` | PASS / source-fix probe: beta84/current-source smoke on `v0.107.0`, 25/25 patches applied, clean audit. Useful as drift-fix direction evidence, not beta.85 package proof. |
| `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510\godot.log.after-launch` | PASS / retained Off loader proof: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.24` compat branch `0.107.0`, 25/25 Spire Plus patches applied, StS1Events default Off, main menu reached. |
| `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510\godot-log-audit.json` | PASS: clean audit with 0 Spire Plus error/exception, 0 Godot ERROR line, 0 MissingMethodException, and 0 TypeLoadException hits. |
| `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621\godot.log.after-launch` | PASS / retained CanaryOnly proof: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.24` compat branch `0.107.0`, 6 registered-event lines, 4 event types, main menu reached. |
| `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | PASS: retained enabled-mode log verifier checks=20 / mismatches=0; packet verifier checks=45 / mismatches=0. A 2026-06-18 tuple-aware verifier dry-run returned 21 / 0 without overwriting the retained report. |
| `.tools\runtime-evidence\v01070-beta85-additive-batch1-20260617-233759\godot.log.after-launch` | FAIL as proof / diagnostic: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.24` compat branch `0.107.0`, main menu reached and audit clean, but runtime logged 13 registered-event lines instead of the current-source 14. Static log/source comparison shows `Sts1TheCleric` logged once as shared, while current source expects `Overgrowth` and `Underdocks` Act registrations. |
| `.tools\runtime-evidence\v01070-beta85-additive-batch1-20260617-233759\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | FAIL: retained enabled-mode log verifier mismatches=1 and packet verifier mismatches=1, both on the 13/14 registration-line count. A 2026-06-18 tuple-aware verifier dry-run returned 21 / 2 and reported missing `ActEvent:Overgrowth:Sts1TheCleric` plus `ActEvent:Underdocks:Sts1TheCleric`, with unexpected `SharedEvent:Shared:Sts1TheCleric`. |
| `.tools\runtime-evidence\v01070-beta86-additive-batch1-20260618-031043\godot.log.after-launch` | FAIL as enabled-mode proof / diagnostic: Spire Plus `v0.1.0-private-beta.87` reached main menu and audited clean, but StS1Events remained disabled because the already-running Steam client did not propagate the transient PowerShell `SPIREPLUS_STS1_EVENT_MODE` environment. |
| `.tools\runtime-evidence\v01070-beta87-additive-batch1-direct-20260618-152531\godot.log.after-launch` | PASS / retained AdditiveBatch1 proof: Spire Plus `v0.1.0-private-beta.87`, RitsuLib `0.4.24` compat branch `0.107.0`, 25/25 Spire Plus patches applied, 30 SavedSpireFields, 14 registered-event lines, 10 event types, main menu reached. |
| `.tools\runtime-evidence\v01070-beta87-additive-batch1-direct-20260618-152531\enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` | PASS: retained enabled-mode log verifier checks=21 / mismatches=0 and packet verifier checks=45 / mismatches=0, including exact act/shared tuple parity. |
| `.tools\runtime-evidence\v01071-beta87-additive-batch1-direct-20260619-102309\godot.log.current-iteration` | FAIL as clean loader proof / current `v0.107.1` blocker: main menu reached in 33.81s, RitsuLib `0.4.24` selected compat branch `0.107.0`, Spire Plus `v0.1.0-private-beta.87` applied 25/25 patches, and AdditiveBatch1 registered 10 event types / 14 calls with exact tuple parity. BaseLib `v3.2.1` logged 2 patch failures (`Applied 241 patches successfully, 2 failed`) before Spire Plus registration. |
| `.tools\runtime-evidence\v01071-beta87-additive-batch1-direct-20260619-102309\godot-log-audit.json`, `enabled-mode-log-check.json`, and `runtime-evidence-packet-check.json` | FAIL: audit dirty with 3 BaseLib patch-failure signature hits and 2 Godot ERROR lines; enabled-mode verifier mismatches=2; packet verifier mismatches=1. This is blocker evidence, not current loader proof. |
| `.tools\runtime-evidence\v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937\godot.log.current-iteration` | PASS / previous BaseLib-backed `v0.107.1` loader proof: main menu reached in 13.25s, BaseLib `v3.3.0`, RitsuLib `0.4.24` compat branch `0.107.0`, Spire Plus `v0.1.0-private-beta.88`, 25/25 Spire Plus patches, and AdditiveBatch1 registered 10 event types / 14 calls. |
| `.tools\runtime-evidence\v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937\godot-log-audit.json`, `sts1-enabled-mode-report.json`, and `sts1-runtime-evidence-packet.json` | PASS: audit clean, retained enabled-mode verifier 31 / 0, and packet verifier 0 mismatches. This is previous BaseLib-backed loader/registration proof only, not current beta.93 or gameplay proof. |

Latest prerequisite evidence: installed game `release_info.json` reports Slay the Spire 2 `v0.107.1`; installed manifest `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json` reports version `0.4.31`; `ritsulib-variants.manifest` includes `compatTarget` `0.107.1`. Older missing-path evidence remains in `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt` at HEAD `87820303`.

## Prerequisites

1. Clean Steam client install with a game version that has a matching RitsuLib variant. Historical proof used Slay the Spire 2 `v0.106.1`; current local install is `v0.107.1` with installed RitsuLib `v0.4.31` / `lib\0.107.1`.
2. STS2-RitsuLib v0.4.31+ installed at `<GameRoot>\mods\STS2-RitsuLib` (current local install: `v0.4.31` on E-drive).
3. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.93.zip` installed at `<GameRoot>\mods\EZMicroBalance`; package checker is recorded in `PROJECT_STATE.md` as passed on 2026-06-20.
4. BaseLib is not a Spire Plus prerequisite for the current beta.93 lane.
5. No other mods enabled
6. If using `scripts\spire-plus-live-session.ps1`, prepare with explicit E-drive `-GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'`, `-SteamExe 'E:\Steam\steam.exe'`, the chosen `-SteamUserId`, and `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`; restore after evidence capture with `-Mode Restore -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`; ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.4.31` | PASS: E-drive install is `v0.4.31` with `lib\0.107.1` variant |
| 2 | Launch game via controlled direct smoke | Main menu loads without crash | [PASS] beta93 Off and AdditiveBatch1 direct smokes reached main menu |
| 3 | Check `godot.log` for EZMicroBalance init | Single Spire Plus initialization line, no errors | [PASS] beta93 logs initialize Spire Plus and audits have 0 release-blocking hits |
| 4 | Check `godot.log` for BaseLib absence from Spire Plus lane | Spire Plus current lane loads without BaseLib requirement | [PASS] beta93 isolated smokes loaded exactly `STS2-RitsuLib` and `EZMicroBalance` |
| 5 | Check `godot.log` for STS2-RitsuLib init | RitsuLib initializes, no errors | [PASS] beta93 logs report RitsuLib `0.4.31` compat branch `0.107.1`; audits clean |
| 6 | Check `godot.log` for RitsuLib bootstrap | Spire Plus RitsuLib bootstrap starts | [PASS] beta93 logs have Spire Plus RitsuLib bootstrap lines |
| 7 | Check `godot.log` for ModPatcher count | 25 ModPatcher patches applied; remaining raw Harmony patches load without dependency failures | [PASS] beta93 logs have 25/25 Spire Plus ModPatcher patches applied |
| 8 | Check `godot.log` for release-blocking log hits | 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failure, or release-blocking audit hits | [PASS] beta93 audits clean |
| 9 | Check saved attached-state registration | RitsuLib saved attached-state registration succeeds | [PASS] beta93 loader proof completes Spire Plus bootstrap and reaches main menu |

### Sts1Events Runtime Gates

| Mode | Required env | Expected | Evidence |
| --- | --- | --- | --- |
| Off | unset / empty / invalid `SPIREPLUS_STS1_EVENT_MODE` | 0 Sts1Events registrations, no `[StS1 Events]` registration lines | [CURRENT PASS] beta93 RitsuLib-only Off proof: clean audit, Off packet verifier 43 / 0 |
| CanaryOnly | `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` | 4 canary event types / 6 registration calls: Big Fish and Golden Idol in both Act 1 buckets, plus The Lab and Divine Fountain as shared events | [PASS] beta85 v01070-beta85-canary-20260617-233621: 4 event types / 6 registered-event lines; retained log verifier 20 / 0; tuple-aware dry-run 21 / 0; packet verifier 45 / 0 |
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | Controlled prototype only: 14 registration calls / 10 event types, no TODO/BLOCKED events | [CURRENT PASS] beta93 v01071 RitsuLib-only proof: clean audit, enabled-mode verifier 31 / 0, packet verifier 61 / 0 |
| AdditiveAllDraft | `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; dev-only all-draft mode includes TODO/BLOCKED content | [DO NOT USE for tester/release paths] |
| ReplaceUnknownEventsPrototype | `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` plus `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; debug-only replacement prototype; normal builds fail closed | [DO NOT USE for tester/release paths] |

After any future enabled-mode smoke copies `godot.log` and writes `godot-log-audit.json`, verify the copied files without launching anything:

```powershell
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
```

For helper-created evidence folders, prefer the packet verifier to verify the packet has the expected copied files, session state, restore state, isolated-mod list, and clean nested log/audit result. For enabled modes it uses `godot.log.current-iteration` as canonical proof; retained current slices must byte-match `godot.log.after-launch` after the `godot.log.before` prefix, and when that retained slice is absent, it derives the slice only if `godot.log.before` is a byte prefix of `godot.log.after-launch`, then generates `godot-log-current-iteration-audit.json` and runs the nested log verifier against that current slice rather than the full copied log:

```powershell
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
```

For enabled-mode copied logs, the log verifier requires explicit expected package-version, Ritsu compat-branch, RitsuLib package-version, and game-version checks, and the observed registered event-line count matches the source-derived registration-call count. It verifies registration-call count, event class set, and observed registration tuples parsed from `Registered act event` / `Registered shared event` log lines when those tuple details are present. The retained current-slice audit must bind its `Path`, `Length`, and `Sha256` to the copied `-LogPath`, and the verifier recomputes `audit-godot-log.ps1` against that copied log so a hand-edited clean audit cannot hide dirty log content. If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly. For enabled-mode packets, the helper-created `session-state.json` must record `Sts1EventModeEnvironment` equal to the requested mode, `AllowedModIds` exactly equal to STS2-RitsuLib and EZMicroBalance for the current beta.93 RitsuLib-only lane, moved-mod source/destination paths stay under the recorded mods root and evidence `isolated-mods` folder, restore counts match the session moved-mod and moved-current-run lists, and the helper-copied `game-release-info.json` must match the expected game version. The packet verifier rejects missing or mismatched enabled-mode setup metadata, rejects retained current slices that do not match `godot.log.after-launch` after the `godot.log.before` byte prefix unless the full copied log was rewritten and byte-matches the retained current iteration, rejects unsafe-mode environment leakage for CanaryOnly/AdditiveBatch1 evidence, rejects full-log-only canonical verifier input, rejects `-AllowMissingSessionState` / `-AllowMissingRestoreState` for enabled-mode packets, and requires explicit expected package-version, Ritsu compat-branch, RitsuLib package-version, and game-version checks. Keep `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` in the same evidence folder as the copied logs and current-slice audit so the verifier decisions remain reviewable.

Source-only expected shapes can be printed during the coordination pause with `-PrintExpected`. That output is not enabled-mode proof; it only preserves the current expected class set and source-derived registration-call count.

### Mod Settings UI

Use the focused helper for this gate so the current-display screenshots, route note, package hashes, and checklist stay in one reviewable evidence folder:

```powershell
.\scripts\collect-mod-settings-evidence.ps1 -NoLaunch
# Launch through the normal Steam-client live-session path, open Settings -> Mod Settings, then capture the list.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "<evidence-dir>" -Capture List -RequireSpireForeground
# Open the Spire Plus config page, then capture the page.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "<evidence-dir>" -Capture Page -RequireSpireForeground
```

The helper does not launch the game, navigate UI, audit logs, or mark this row passed by itself. The row still needs same-session `godot.log`, `godot-log-audit.json`, screenshots, route note, and filled checklist.

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

Current exit status: beta.93 Off and AdditiveBatch1 loader rows pass for `v0.107.1` with RitsuLib `v0.4.31` / `lib\0.107.1` and no Spire Plus BaseLib dependency; beta.85/beta.87/beta.88/beta.90 loader rows remain previous-package or previous-dependency context. Current Mod Settings UI, gameplay, save-load, and multiplayer rows remain pending.

## Notes

- This checklist supplements `docs/test-plan.md` and `docs/release-checklist.md`.
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/` or a documented runtime-evidence folder.
- If any loader smoke item fails, do not proceed to gameplay items; diagnose first.
- Loader-gate smoke is sufficient only for Batch 4c candidate review. No Batch 4c patch migration is allowed without explicit owner acceptance and fresh validation.
