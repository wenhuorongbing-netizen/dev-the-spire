# M5 Revision M Final Report

Date: 2026-06-11
Status: Complete for Off loader runtime-drift closure; not live-ready or release-ready.

2026-06-20 supersession: Revision M is historical. Current package/runtime truth is beta.91 on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `0.4.28` / `lib/0.107.1` and no Spire Plus previous framework dependency.

## Result

M5 Revision M reached a clean beta.85 `v0.107.0` Off loader packet in recorded evidence. The beta.84 runtime blocker is closed for loader/patch application.

## Completed In This Pass

- Created Revision M runtime drift docs:
  - `docs/goals/m5-revision-m-runtime-drift-report.md`
  - `docs/goals/m5-revision-m-patch-failure-ledger.md`
  - `docs/goals/m5-revision-m-owner-review-packet.md`
  - `docs/goals/m5-revision-m-commit-slices.md`
  - `docs/goals/m5-revision-m-version-decision.md`
- Updated active state docs so older Revision L/K1 evidence was not mistaken for the then-current beta.85 runtime proof.
- Recorded the beta.84 red smoke failure list and the current source-fix direction.
- Recorded that beta.85 package parity is reported in `PROJECT_STATE.md`.
- Recorded then-current beta.85 Off loader smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`: RitsuLib compat branch `0.107.0`, Spire Plus `v0.1.0-private-beta.85`, 25/25 patches, StS1Events default Off, clean audit.
- Removed the unused `SpirePlusDebug.LogPreview` helper and added static guard coverage for debug logging gates: general info diagnostics are internal-only, live in `SpirePlusDebug` instead of the previous framework config class, require `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`, and trim false-like values (`0`, `false`, `off`, `no`); preview diagnostics require the localized `ShowPreviewDebugLogs` setting; warnings remain available for degraded runtime paths.
- Used read-only subagent audits for runtime drift, package parity, StS1 governance, validation planning, and docs truth.

## Remaining Blockers

- Fresh beta.85 CanaryOnly and AdditiveBatch1 runtime smokes are not recorded.
- Gameplay, clicked UI, save-load, replacement, multiplayer, independent QA, and release handoff proof remain pending.
- Active repo-local `dotnet` / `testhost` processes were observed during the continuation; do not start overlapping validation lanes.

## Required Next Lane

When coordination allows:

1. Check for active validation/runtime processes and stop if any are present.
2. Reconcile any in-flight static/package validation reports.
3. Run fresh beta.85 CanaryOnly/AdditiveBatch1 only if needed and only after coordination is clear.
4. Keep gameplay/manual/release proof as separate lanes.

No commit, push, or release-ready claim should happen while validation is in flight or before owner approval.
