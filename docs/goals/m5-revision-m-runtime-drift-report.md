# M5 Revision M Runtime Drift Report

Date: 2026-06-11
Status: Off loader/runtime drift closed for beta.85; live gameplay/release proof still pending.

## Current Truth

- The old missing-`STS2-RitsuLib` blocker is closed locally. Official `STS2-RitsuLib` `v0.4.16` is installed under `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` and includes `lib\0.107.0`.
- The controlling red root-cause evidence is `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`: the beta.84 package reached main menu on game `v0.107.0`, but Spire Plus applied only 17/25 ModPatcher patches, logged 8 optional ModPatcher failures, and threw an `EctoplasmGoldGatePatch` initializer exception.
- Current dirty source contains the intended `v0.107.0` drift fixes:
  - `EctoplasmGoldGatePatch` targets `Ectoplasm.ModifyGoldGained(Player, decimal)` and prefixes with `decimal amount` / `ref decimal __result`.
  - ModPatcher property targets use property names with `MethodType.Getter` instead of compiler `get_*` method names.
- Local RitsuLib XML documentation for compile package `STS2.RitsuLib` `0.3.2` documents `ModPatchTarget(..., HarmonyLib.MethodType)` and says non-normal `MethodType` values resolve through Harmony `AccessTools` helpers.
- `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/` is useful source-fix smoke context: it reached main menu on game `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and has a clean audit. It still logs Spire Plus as beta.84, so it is source-direction evidence rather than current package proof.
- Current manifest is dirty at `v0.1.0-private-beta.85`. `PROJECT_STATE.md` records installed beta.85 package parity as passed via `scripts\check-installed-spire-plus-package.ps1`.
- Current beta.85 Off proof is `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`: the log reports Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.16` with compat branch `0.107.0`, 25/25 Spire Plus ModPatcher patches applied, StS1Events default Off, main menu reached, and a clean `godot-log-audit.json`.
- The checked-in local `source code/` snapshot is not authoritative for `v0.107.0` API drift; installed DLL/API inspection is the deciding source for `Ectoplasm.ModifyGoldGained`, `Vakuu.GenerateInitialOptions`, and canonical getter availability.

## What This Closes

- The beta.84 package API drift is fixed in the beta.85 loader lane.
- `EctoplasmGoldGatePatch` no longer throws during mod initialization in the beta.85 Off smoke.
- The getter ModPatcher targets apply under RitsuLib `v0.4.16` / compat `0.107.0`.
- Off-mode StS1Events remains disabled by default in the beta.85 Off smoke.

## What Remains Open

- Fresh CanaryOnly and AdditiveBatch1 `v0.107.0` smokes for beta.85 are not recorded.
- Gameplay, clicked UI, save-load, replacement, multiplayer, independent QA, and release handoff proof remain pending.
- Active same-repo `dotnet` / `testhost` processes were observed during the continuation, so do not start overlapping validation lanes. Treat any in-flight validation as external state until it reports back.

## Resume Gate

For the next validation lane:

1. Confirm no `dotnet`, `testhost`, `vstest.console`, `MSBuild`, `VBCSCompiler`, `SlayTheSpire2`, or Godot process is active for this repo/runtime lane.
2. Reconcile any in-flight build/test/package results with this document.
3. If no current static replay has been recorded for the final dirty worktree, replay static validation.
4. If StS1 staging proof is needed, run fresh beta.85 CanaryOnly and AdditiveBatch1 smokes.

## Stop Line

Current result for Revision M Off runtime drift is complete at loader scope. Do not extend that to runtime-ready, live-ready, or release-ready; those broader claims still require gameplay/manual evidence and any remaining validation lanes.
