# M5 Revision M Owner-Review Packet

Date: 2026-06-11
Status: owner review prepared; Off loader drift closed; not live-ready or release-ready.

2026-06-20 supersession: Revision M is historical. Current package/runtime truth is beta.91 on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `0.4.28` / `lib/0.107.1` and no Spire Plus BaseLib dependency.

## Scope Reviewed

- Runtime drift closure for Slay the Spire 2 `v0.107.0` with installed official `STS2-RitsuLib` `v0.4.16`.
- Current dirty source changes that replace ModPatcher compiler getter names with property-name + `MethodType.Getter` targets.
- Current dirty source `EctoplasmGoldGatePatch` targeting `ModifyGoldGained`.
- Then-current beta.85 manifest/package/hash documentation and StS1 event source/localization updates already present in the worktree.

## Decisions For Owner

| Area | Recommendation | Reason |
|---|---|---|
| Runtime drift source fix | Accept | The red beta.84 failures map directly to stale target names/signatures; beta.85 Off smoke on `v0.107.0` applies 25/25 patches and audits clean. |
| `v0.1.0-private-beta.85` package | Accept as loader-smoke package, not gameplay/release proof | `PROJECT_STATE.md` records package checker pass, and `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` records clean Off loader smoke. |
| RitsuLib compile/manifest minimum | Defer bump to `0.4.16` unless owner makes a package-version decision | Source still compiles against `STS2.RitsuLib` `0.3.2` and manifest min version remains `0.3.2`; installed runtime is `0.4.16`. Bumping dependency metadata should be a deliberate versioned tester-package slice. |
| StS1 event changes | Keep staging-only | Big Fish text/source and Divine Fountain eligibility changed while StS1 events remain default-Off/staged. Runtime event encounter proof is still pending. |
| Debug scaffold | Accept with narrow governance | The unused `SpirePlusDebug.LogPreview` helper was removed. General info diagnostics are internal-only and require `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`; preview diagnostics stay in `PreviewLog` behind the localized `ShowPreviewDebugLogs` setting; warnings remain available for degraded runtime paths. This is not a feature-complete or release-readiness claim. |
| Commit/push | Do not commit or push while validation processes are active | Active repo-local `dotnet` / `testhost` processes were observed; wait for the validation lane to report. |
| CanaryOnly/AdditiveBatch1 | May run only after process coordination | Off is clean, but fresh beta.85 CanaryOnly/AdditiveBatch1 smokes should not overlap active validation/runtime processes. |

## Required Validation Before Handoff

The minimum Off-loader closure packet is now present:

- package checker pass is recorded in `PROJECT_STATE.md`,
- beta.85 `v0.107.0` Off smoke reaches main menu,
- BaseLib, RitsuLib, and Spire Plus load,
- RitsuLib selects compat branch `0.107.0`,
- Spire Plus applies 25/25 ModPatcher patches,
- `EctoplasmGoldGatePatch` exception is absent,
- audit has 0 blocking hits,
- evidence folder is recorded in docs.

Remaining before handoff: reconcile any in-flight validation results, then run fresh CanaryOnly/AdditiveBatch1 only if needed and safe to schedule.

## Current Result

Off loader drift closure is complete for beta.85. Broader runtime/live/release readiness remains incomplete.
