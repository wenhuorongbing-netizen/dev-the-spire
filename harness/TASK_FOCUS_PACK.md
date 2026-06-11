# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- M5 Revision M: `v0.107.0` runtime drift closure and owner-review truth.
- The old missing-runtime-folder blocker is closed for local review. Historical diagnostic loader proof exists for the `v0.106.1` setup; the current local game is `v0.107.0`, and official RitsuLib `v0.4.16` with `lib\0.107.0` is installed.
- Current Off package/runtime proof is complete at loader scope: beta.85 Off smoke is clean on `v0.107.0`. Broader gameplay/live/release proof is still pending.

## Acceptance Criteria

- RitsuLib historical loader-gate verified only: clean Off/CanaryOnly/AdditiveBatch1 logs, 25/25 ModPatcher patches, historical v0.3.10 / `lib\0.106.1` evidence.
- Revision M Off-loader closure is backed by `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`.
- Manifest dependency for `STS2-RitsuLib` remains present in `EZMicroBalance.json`.
- Build: 0 errors, 0 warnings.
- Warning ledger: prior 70-warning ledger is superseded by the current zero-warning build; keep it only as historical triage context.
- Tests: 0 failed in latest recorded no-game validation; `PROJECT_STATE.md` records split no-build coverage at 474/0/21/495 after the beta.85 runtime-fix pass.
- Patch inventory: 142 raw + 25 migrated = 167 tracked units.
- Sts1Events formal/staging/remove recommendation recorded.
- Debug accept-scaffold/feature-complete/rollback recommendation recorded; Revision M removed the unused `SpirePlusDebug.LogPreview` helper, makes broad info diagnostics internal-only through `SPIREPLUS_ENABLE_DEBUG_LOGS=1` / `EZMB_ENABLE_DEBUG_LOGS=1`, and guards the remaining preview diagnostics setting.
- RitsuLib status recorded as current beta.85 Off-loader validated on `v0.107.0`, not gameplay/live/release validated.
- Dirty entries reconciled; commit slices prepared for owner review.
- Runtime smoke evidence collected historically for Off, CanaryOnly, and AdditiveBatch1 diagnostic modes.
- Current Off-loader proof is present: package parity is recorded, RitsuLib `v0.4.16` / `lib\0.107.0` is selected, 25/25 expected patches apply, and the Ectoplasm initializer exception is absent.

## Blocked Scope

- CanaryOnly/AdditiveBatch1 runtime reruns while repo-local validation/runtime processes are active.
- Batch 4c patch migration (pending gameplay proof and owner decision).
- Batch 5 / PR7 / high-risk patch migration.
- New gameplay behavior.
- Debug expansion.
- Sts1Events formalization (blocked by gameplay/render/save-load/image/replacement/multiplayer proof, not current build warnings).
- Longhaul audit (blocked pending owner-review acceptance).
- Release-ready claims (pending gameplay/UI/save-load/co-op proof).
- Live-ready/release-ready claims from loader proof alone.

## Related Files Or Modules

- `EZMicroBalance.csproj` (RitsuLib PackageReference)
- `EZMicroBalance.json` (manifest dependency)
- `scripts/spire-plus-live-session.ps1` (runtime smoke helper)
- `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` (red beta.84 package evidence)
- `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/` (clean beta84/current-source drift probe)
- `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` (clean beta85 Off loader proof)
- `docs/goals/m5-revision-m-runtime-drift-report.md`
- `docs/goals/m5-revision-m-patch-failure-ledger.md`
- `docs/goals/m5-revision-m-owner-review-packet.md`
- `docs/goals/m5-revision-m-commit-slices.md`
- `docs/goals/warning-ledger.md`

## Risks

- Sts1Events June 11 source/resource changes remain unvalidated while the feature is default-Off/prototype-gated.
- Gameplay verification has not been performed; runtime proof covers loader/startup only.
- Current beta.85 `v0.107.0` Off-loader compatibility is proven, but CanaryOnly/AdditiveBatch1 and gameplay are not.
- Co-op verification has not been performed.
- Versioned tester-package handoff has not been performed.
- Dirty source/docs/test entries need owner-approved commit slices.
