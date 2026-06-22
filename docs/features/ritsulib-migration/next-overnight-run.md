# Next Runtime QA Run - RitsuLib Migration

Run date: TBD.
Status: compact execution boundary. This replaces the long historical overnight plan; use archive docs and Git history for old detail.

## Objective

Manual QA after clean RitsuLib-only Off loader proof, plus remaining Batch 4c owner review. This is not approval to migrate more patches and not a release-ready claim.

## Start State

- Use `git log -1 --oneline --decorate` and `git status --short --branch` as the source of truth; older run-start hashes from prior follow-ups are historical notes and must not be reused for handoff.
- Any dirty files after the latest pushed HEAD are post-baseline follow-up scope. Classify them before any validation claim, package handoff, commit, or push.
- Latest beta.105 no-game/package validation is summarized in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`: build/publish/package refresh, installed-package parity, runtime preflight, source-workspace checks, and clicked Ancient UI smoke passed for the current dependency target. Previous beta.99 settings/Off proof, beta.96 Off proof, and beta.93 AdditiveBatch1 packet verification remain previous-package or historical loader/registration evidence only.
- Current HEAD/worktree validation refreshed after the beta.105 RitsuLib default-key localization and clicked UI pass; recheck again before handoff if any later edits appear.
- Coordination boundary: do not run overlapping validation, package/release, runtime/game smoke, staging, commit, or push steps.

## Retained Evidence Boundaries

- CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.
- AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.
- Retained CanaryOnly enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`: 4 event types / 6 registration lines with retained verifier reports.
- Before any StS1 canary gameplay claim, recapture current-version CanaryOnly loader proof; `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` is retained previous-package/game-version context only.
- Before any AdditiveBatch1 gameplay claim, confirm the beta.105 package and STS2-RitsuLib `v0.4.33` direct NuGet runtime are installed and recapture current-package enabled-mode proof.

## Required Run Order

1. Recapture HEAD/worktree and installed game/RitsuLib versions.
2. Run no-game validation in one controlled lane if source changed.
3. Capture beta.105 enabled-mode loader/registration proof separately before any current enabled-mode claim.
4. Capture current StS1 CanaryOnly/AdditiveBatch1 enabled-mode proof before StS1 gameplay claims.
5. Capture gameplay, clicked UI, save-load, replacement, co-op, QA, and handoff evidence, or record exact blockers.

## Batch 4c Owner Review

- the 2026-06-18 static recapture confirmed 10 low-risk candidates; 2 Sere Talon UI candidates were migrated through RitsuLib on 2026-06-22, leaving 8 proposal-only candidates and no forbidden high-risk categories.
- The current static recapture is not that decision.
- Forbidden candidate surfaces remain: run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, reward-state.
- Do not migrate the remaining Batch 4c candidates without owner approval and fresh validation.

## Success Checklist

- [x] Retained `v0.107.0` beta.85 CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.
- [x] Retained `v0.107.0` beta.87 AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports; beta.85 13/14 attempt remains root-cause history only.
- [x] Batch 4c candidate list static review recaptured: 8 proposal-only candidates remain after the 2 Sere Talon UI candidates were migrated through RitsuLib.
- [x] Current beta.105 clicked Ancient UI smoke captured under `.tools/runtime-evidence/monkey-stability-20260622-025733/`; previous beta.99 Off loader proof remains previous-package context under `.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/`.
- [ ] Current CanaryOnly/AdditiveBatch1 enabled-mode proof captured if StS1 gameplay claims are needed.
- [ ] Gameplay, clicked UI, save-load, replacement, co-op, QA, and handoff rows completed or blocked with evidence.
- [ ] Remaining Batch 4c owner decision recorded.
