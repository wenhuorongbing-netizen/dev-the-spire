# M5 Revision M Runtime Drift Report

Date: 2026-06-18
Status: Loader/runtime drift remains closed and dependency-floor proof was refreshed through beta.87 AdditiveBatch1 registration proof; live gameplay/release proof still pending.

2026-06-20 supersession: beta.91 is now the current RitsuLib-only loader/registration truth for `v0.107.1` with `STS2-RitsuLib` `v0.4.28`. This Revision M report is retained as the beta.87 `v0.107.0` package/source-shape and previous framework-drift history only.

## Current Truth

- The old missing-`STS2-RitsuLib` blocker is closed locally. Official `STS2-RitsuLib` `v0.4.24` is installed under `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` and includes `lib\0.107.0`; previous framework `v3.2.1` is installed under `mods\previous framework`.
- The controlling red root-cause evidence is `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`: the beta.84 package reached main menu on game `v0.107.0`, but Spire Plus applied only 17/25 ModPatcher patches, logged 8 optional ModPatcher failures, and threw an `EctoplasmGoldGatePatch` initializer exception.
- Current dirty source contains the intended `v0.107.0` drift fixes:
  - `EctoplasmGoldGatePatch` targets `Ectoplasm.ModifyGoldGained(Player, decimal)` and prefixes with `decimal amount` / `ref decimal __result`.
  - ModPatcher property targets use property names with `MethodType.Getter` instead of compiler `get_*` method names.
- Local RitsuLib XML documentation for compile package `STS2.RitsuLib` `0.3.2` documents `ModPatchTarget(..., HarmonyLib.MethodType)` and says non-normal `MethodType` values resolve through Harmony `AccessTools` helpers.
- `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/` is useful source-fix smoke context: it reached main menu on game `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and has a clean audit. It still logs Spire Plus as beta.84, so it is source-direction evidence rather than current package proof.
- Current manifest is dirty at `v0.1.0-private-beta.87`. `PROJECT_STATE.md` records the beta.87 package/hash target and the current dependency-floor pass.
- Retained beta.87 AdditiveBatch1 proof is `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`: the log reports Spire Plus `v0.1.0-private-beta.87`, RitsuLib `0.4.24` with compat branch `0.107.0`, 25/25 Spire Plus ModPatcher patches applied, 10 event types / 14 registration calls, main menu reached, clean `godot-log-audit.json`, retained enabled-mode log verifier 31 / 0, and retained runtime packet verifier 52 / 0. Beta.85/beta.86/beta.87 loader proof is now previous-package or previous-game-version context.
- The checked-in local `source code/` snapshot is not authoritative for `v0.107.0` API drift; installed DLL/API inspection is the deciding source for `Ectoplasm.ModifyGoldGained`, `Vakuu.GenerateInitialOptions`, and canonical getter availability.

## What This Closes

- The beta.84 package API drift is fixed in the beta.85 Off lane and remains clean in the beta.87 AdditiveBatch1 loader/registration lane.
- `EctoplasmGoldGatePatch` no longer throws during mod initialization in the retained beta.85 Off smoke or the beta.87 AdditiveBatch1 smoke.
- The getter ModPatcher targets apply under RitsuLib `v0.4.24` / compat `0.107.0`.
- AdditiveBatch1 source/runtime registration shape now matches 10 event types / 14 registration calls in the beta.87 direct packet.

## What Remains Open

- Fresh beta.87 default-Off and CanaryOnly smokes are not recorded; retained beta.85/beta.86 evidence is previous-package loader context.
- Gameplay, clicked UI, save-load, replacement, multiplayer, independent QA, and release handoff proof remain pending.
- Active same-repo `dotnet` / `testhost` processes were observed during the continuation, so do not start overlapping validation lanes. Treat any in-flight validation as external state until it reports back.

## Resume Gate

For the next validation lane:

1. Confirm no `dotnet`, `testhost`, `vstest.console`, `MSBuild`, `VBCSCompiler`, `SlayTheSpire2`, or Godot process is active for this repo/runtime lane.
2. Reconcile any in-flight build/test/package results with this document.
3. If no current static replay has been recorded for the final dirty worktree, replay static validation.
4. If StS1 staging proof needs to be refreshed, run fresh beta.87 Off/CanaryOnly loader smokes only after coordination clears; preserve the existing beta.87 AdditiveBatch1 loader/registration packet unless package/source changes require recapture.

## Stop Line

Current result for Revision M runtime drift is complete at loader scope. Do not extend that to runtime-ready, live-ready, or release-ready; those broader claims still require gameplay/manual evidence and any remaining validation lanes.
