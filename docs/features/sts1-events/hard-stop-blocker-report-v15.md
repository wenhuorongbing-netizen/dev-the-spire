# StS1 Event Port v15 Hard Stop Blocker Report

Date: 2026-05-31
Scope: Mandatory Overnight Run v15 continuation for `docs/goals/event.md`

Superseded note, 2026-06-11: this report is historical root-cause evidence only. Current beta.85 `v0.107.0` proof covers default-Off loader startup and patch application only; it supersedes the red v15 loader blocker but does not prove CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, image/render, QA, handoff, or release readiness. Keep the v15 `v0.3.10` dependency and red-loader details below as history, not current runtime truth.

## Exact Gate Id

Primary hard blocker: `O24` loader proof is not clean enough to continue to runtime gameplay proof.

Downstream blocked gates: `O25-O56` remain blocked because canary runtime, simple batch runtime, save-load, image/render, replacement functional, multiplayer/fail-closed, and independent QA proof cannot be accepted while the current loader audit is red and no screenshots/result logs exist.

## Evidence Paths

| Evidence | Path | Result |
| --- | --- | --- |
| v15 loader log | `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` | BaseLib, RitsuLib, and Spire Plus loaded; main menu reached; StS1Events default Off logged. |
| v15 loader audit | `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/audit-godot-log.after-launch.json` | FAIL: 11 `Godot ERROR` hits. |
| v15 live-session state | `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/session-state.json` | Controlled E-drive run with BaseLib, STS2-RitsuLib, and EZMicroBalance allowed. |
| v15 restore state | `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/restore-state.json` | Restored 25 isolated mods, stopped `SlayTheSpire2`, restored settings hashes. |
| failed empty helper dir | `.tools/runtime-evidence/sts1-events-v15-loader-20260531-230153/` | Empty; no `session-state.json`; cannot be restored or used as proof. |
| failed direct Steam-init log | `.tools/runtime-evidence/ritsulib-runtime-proof-20260531-2304/godot.log.failed-steam-init` | Direct executable path failed before mod loading; preserved as blocker evidence. |
| failed direct Steam-init audit | `.tools/runtime-evidence/ritsulib-runtime-proof-20260531-2304/audit-godot-log.failed-steam-init.json` | FAIL: 5 `Godot ERROR` hits from Steam initialization failure. |

## Blocker Reason

The current best loader log proves the game can reach main menu with the intended three mods loaded, but it is not clean runtime proof:

| Log evidence | Reason |
| --- | --- |
| `godot.log.after-launch:19` | `STS2-RitsuLib\ritsulib-variants.json` is read as a mod manifest and logged as missing `id`. |
| `godot.log.after-launch:96-330` | 8 optional Spire Plus ModPatcher failures are logged. |
| `godot.log.after-launch:328-331` | Spire Plus patch summary is `17 applied, 0 ignored, 8 failed, 25 total`; summary `ERROR` lines remain. |
| `audit-godot-log.after-launch.json` | `Clean=false`; `Godot ERROR line` count is `11`. |

This is not a missing-dependency blocker anymore. `STS2-RitsuLib` `v0.3.10`, BaseLib, and Spire Plus are present on the checked E-drive game root. The blocker is clean loader/error disposition plus absent runtime gameplay proof.

## Attempted Actions

1. Read `PROJECT_STATE.md`, `docs/goals/event.md`, `docs/features/sts1-events/status-board.md`, and `docs/reviews/current-validation.md` before editing current docs.
2. Used subagents for runtime bootstrap and documentation gap review.
3. Preserved and audited the direct-launch Steam initialization failure log under `.tools/runtime-evidence/ritsulib-runtime-proof-20260531-2304/`.
4. Restored the unintended live-session isolation recorded in `.tools/runtime-evidence/ritsulib-runtime-proof-20260531-2304/session-state.json`; 25 moved mods and settings hashes were restored.
5. Verified no `SlayTheSpire2`/Godot process remained after restore.
6. Attempted a bounded v15 loader smoke with explicit E-drive `-GameRoot`, `-SteamExe`, and `-SteamUserId`. The first attempt stopped before prepare because the old `godot.log` was locked; it left only `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231050/godot.log.pre-attempt`.
7. Reran without deleting the old log. The controlled loader run produced `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch`, reached main menu, archived audit JSON, and restored the live game session.
8. Updated current docs to record the new truth: dependency and log existence are no longer the blocker, but clean runtime proof remains blocked.

## Owner / External Action Required

1. Resolve or explicitly accept the `ritsulib-variants.json` manifest parse error with source/package evidence from the installed official STS2-RitsuLib distribution.
2. Resolve or explicitly classify the 8 optional Spire Plus ModPatcher failures before using the loader log as runtime-safety proof.
3. Rerun controlled loader smoke and preserve a clean or explicitly accepted `godot.log` plus audit JSON.
4. After clean/dispositioned loader proof, run `CanaryOnly` and `AdditiveBatch1` gameplay sessions and capture screenshots, result logs, save-load, EN/ZHS render, image/license/render, replacement, multiplayer, and QA evidence.

## Why Continuation Is Impossible In Current Environment

The current environment can launch the game and capture loader logs, but it cannot honestly make `O24-O56` green now. The best log is red by the repository audit helper, and no current evidence exists for canary gameplay screenshots, event result logs, save-load, EN/ZHS runtime rendering, event images/license disposition, replacement pool behavior, multiplayer/fail-closed behavior, or independent QA pass.

Stopping here is a hard blocker pause only. It does not mark StS1 event runtime parity, gameplay readiness, or release readiness complete.
