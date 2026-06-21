# StS1 Event Port v18 Current Loader Hard Stop

Date: 2026-06-10
Scope: Mandatory Overnight Run v18, current `v0.107.0` loader reproof after RitsuLib `v0.4.16`.

Superseded note, 2026-06-21: this report remains root-cause history for the failed beta.84 package. Previous beta.93 `v0.107.1` RitsuLib-only Off and AdditiveBatch1 loader/registration proof is clean under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`; gameplay, save-load, replacement, multiplayer, and QA proof remain pending. Do not follow the historical previous package rerun instruction below as current setup guidance.

## Exact Gate Id

Historical blocker at capture time: `O24` loader proof was red for the package-parity beta.84 install.

Downstream blocked gates: `O25`, `O33`, `O34-O47`, and `O51-O52` remain blocked because the current Slay the Spire 2 `v0.107.0` Off loader smoke is non-clean before CanaryOnly/AdditiveBatch1/gameplay proof can be accepted.

## Blocker Reason

The current package-parity Off smoke was captured by a concurrent lane at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`, and it is not clean. RitsuLib selects the current `0.107.0` variant and previous package loads, but Spire Plus beta.84 fails during initialization because the installed package still contains stale patch targets from before the dirty-source installed-game API fixes.

Current prerequisites are better than the prior blocker:

- Slay the Spire 2 is installed at `v0.107.0`.
- Official `STS2-RitsuLib` `v0.4.16` is installed with `lib\0.107.0`.
- Installed Spire Plus DLL hash now matches packaged beta.84 DLL hash.

But the fresh `v0.107.0` loader smoke fails on the beta.84 package and cannot be used as current loader proof.

## Evidence

| Evidence | Current fact |
|---|---|
| Runtime prerequisite docs | `PROJECT_STATE.md`, `docs/reviews/current-validation.md`, `docs/features/ritsulib-migration/runtime-smoke-checklist.md` |
| Game version | `E:\Steam\steamapps\common\Slay the Spire 2\release_info.json` reports `v0.107.0` |
| RitsuLib manifest | `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json` reports `0.4.16` |
| RitsuLib variants | `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\lib` includes `0.107.0` |
| Installed/package DLL parity | Installed `EZMicroBalance.dll` and packaged beta.84 `EZMicroBalance.dll` both hash to `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766` |
| Current Off smoke folder | `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` |
| Current Off smoke audit | `godot-log-audit.json`: `Clean=false`, `Spire Plus error/exception=1`, `Godot ERROR line=11` |
| RitsuLib current variant | `godot.log.after-launch`: host `v0.107.0`, picked variant `0.107.0`, RitsuLib `0.4.16 [compat branch: 0.107.0]` |
| previous package status | `godot.log.after-launch`: previous package `v3.1.4`, 217 patches applied, 0 failed |
| Spire Plus package failure | `godot.log.after-launch`: `EctoplasmGoldGatePatch::Prefix(...)` undefined target method during mod initializer |
| Optional patch failures | `godot.log.after-launch`: 8 optional Spire Plus patches failed before the initializer exception |
| Active same-repo validation process during this thread | `dotnet exec ... vstest.console.dll ... solution-current-diag.log ... RunConfiguration.MaxCpuCount=1` with attached `testhost.exe` was still alive while this thread considered running another smoke, so this thread did not launch a second game session |

## Attempted Actions

1. Re-read `PROJECT_STATE.md` and the active event/runtime validation docs.
2. Verified current no-game validation state from `docs/reviews/current-validation.md`.
3. Inspected `scripts/spire-plus-live-session.ps1` and `scripts/audit-godot-log.ps1`.
4. Verified no Slay the Spire 2/Godot process was running.
5. Verified current installed game/RitsuLib/package prerequisites read-only.
6. Discovered and inspected the concurrent current Off smoke evidence under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`.
7. Waited for the active solution-level VSTest lane; it still had a live `vstest.console.dll` process and attached `testhost.exe` during this thread's smoke decision point.
8. Did not launch the game, move mods, rewrite settings, or run build/test/publish from this thread.

## Owner / External Action Required

1. Do not use beta.84 as current `v0.107.0` loader proof; the smoke is red.
2. Decide whether to cut a new versioned package from the dirty source API fixes, including the required version bump, publish/package refresh, and validation.
3. Historical action at capture time: after a fixed package is installed, rerun a controlled `v0.107.0` Off loader smoke with the then-required previous package + STS2-RitsuLib + Spire Plus stack. Previous beta.93 guidance is RitsuLib-only and lives in `PROJECT_STATE.md` plus `docs/features/sts1-events/README.md`.
4. If Off is clean, capture CanaryOnly and AdditiveBatch1 loader smokes under the same current runtime.
5. Preserve `godot.log`, audit JSON, session state, restore state, command/env notes, and hash/path prerequisites for each mode.

## Why Continuation Is Impossible In This Moment

The current package-parity smoke already proves the beta.84 package is not clean on `v0.107.0`; running CanaryOnly, AdditiveBatch1, replacement, or gameplay proof on top of that would create unsupported evidence. A second smoke from this thread was also unsafe while another solution-level VSTest lane was active. This is a hard-stop pause for the current loader proof only; it is not a StS1 event completion claim.
