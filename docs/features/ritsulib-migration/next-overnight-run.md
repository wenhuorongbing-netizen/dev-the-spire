# Next Runtime QA Run - RitsuLib Migration

Run date: TBD.
Status: compact execution boundary. This replaces the long historical overnight plan; use archive docs and Git history for old detail.

## Objective

Manual QA after clean RitsuLib-only Off loader proof, plus post-Batch 4c runtime recapture. This is not approval to migrate high-risk patches and not a release-ready claim.

## Start State

- Use `git log -1 --oneline --decorate` and `git status --short --branch` as the source of truth; older run-start hashes from prior follow-ups are historical notes and must not be reused for handoff.
- Any dirty files after the latest pushed HEAD are post-baseline follow-up scope. Classify them before any validation claim, package handoff, commit, or push.
- Latest beta.123 package validation is summarized in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`: build/publish/package refresh, installed-package parity, runtime preflight, and source-workspace checks passed for the current dependency target. Previous beta.99 settings/Off proof, beta.96 Off proof, and beta.93 AdditiveBatch1 packet verification remain previous-package or historical loader/registration evidence only.
- Current beta.123 smoke applied all 127 migrated RitsuLib patch classes from the packaged beta.123 state. Current source has since moved to 151 migrated patch classes and 19 raw Harmony declarations; recapture publish/package/runtime proof before claiming that source state in-game.
- Coordination boundary: do not run overlapping validation, package/release, runtime/game smoke, staging, commit, or push steps.

## Retained Evidence Boundaries

- CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.
- AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.
- Retained CanaryOnly enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`: 4 event types / 6 registration lines with retained verifier reports.
- Before any StS1 canary gameplay claim, recapture current-version CanaryOnly loader proof; `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` is retained previous-package/game-version context only.
- Before any AdditiveBatch1 gameplay claim, confirm the beta.123 package and STS2-RitsuLib `v0.4.34` direct NuGet runtime are installed and recapture current-package enabled-mode proof.

## Required Run Order

1. Recapture HEAD/worktree and installed game/RitsuLib versions.
2. Run no-game validation in one controlled lane if source changed.
3. Capture current beta.123 enabled-mode loader/registration proof separately before any current enabled-mode claim.
4. Capture current StS1 CanaryOnly/AdditiveBatch1 enabled-mode proof before StS1 gameplay claims.
5. Capture gameplay, gated Vakuu fight-option UI, save-load, replacement, co-op, QA, and handoff evidence, or record exact blockers.

## Batch 4c Runtime Boundary

- the 2026-06-22 continuation migrated the remaining 6 low-risk localization fallback candidates through RitsuLib after owner approval.
- This is not current enabled-mode, gameplay, save-load, replacement, co-op, QA, release, or handoff proof.
- Forbidden candidate surfaces remain: run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, reward-state.
- Do not migrate high-risk candidates without owner approval and fresh validation.

## Success Checklist

- [x] Retained `v0.107.0` beta.85 CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.
- [x] Retained `v0.107.0` beta.87 AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports; beta.85 13/14 attempt remains root-cause history only.
- [x] Batch 4c localization owner decision recorded and implemented for the six fallback localization patches.
- [x] Current beta.123 clicked Ancient UI smoke captured under `.tools/runtime-evidence/monkey-stability-20260622-235746/`; previous beta.99 Off loader proof remains previous-package context under `.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/`.
- [ ] Current CanaryOnly/AdditiveBatch1 enabled-mode proof captured if StS1 gameplay claims are needed.
- [ ] Gameplay, gated Vakuu fight-option UI, save-load, replacement, co-op, QA, and handoff rows completed or blocked with evidence.
