# RitsuLib Migration Current Boundary

Status: compact active boundary for RitsuLib-only migration work. Historical planning detail remains in Git history and archived planning docs; future work should start from `PROJECT_STATE.md`, `docs/integrations/ritsulib.md`, `docs/reviews/current-validation.md`, and `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.

## Current State

- Current target: Spire Plus `v0.1.0-private-beta.98`, Slay the Spire 2 `v0.107.1`, STS2-RitsuLib `v0.4.32`, direct NuGet runtime layout.
- Spire Plus uses NuGet `STS2.RitsuLib` `0.4.32` and manifest dependency `STS2-RitsuLib >= 0.4.32`.
- 25 patch classes use RitsuLib `IPatchMethod`; 146 raw `[HarmonyPatch]` declarations remain.
- Beta.97 Off loader smoke remains pending after the settings-page I18N resource migration; previous beta.96 Off loader smoke is clean previous-package startup/loading context only.
- Previous beta.93 AdditiveBatch1 enabled-mode smoke is clean with 10 event types / 14 registration calls and retained verifier reports for previous-package registration context only.
- Handoff must recapture HEAD and worktree status after any later edits.
- Live-ready and release-ready remain blocked by gameplay, clicked Ancient UI, save-load, replacement behavior, current beta.98 enabled-mode registration/gameplay proof, co-op, QA, and tester handoff.

## Evidence Rules

- Use previous beta.96 Off proof only for historical startup/loading and default-Off StS1Events context; do not advance to CanaryOnly, AdditiveBatch1, replacement, or gameplay proof from the Off smoke alone.
- Use the retained CanaryOnly enabled-mode smoke at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` only as beta.85 / `v0.107.0` previous-package proof; recapture before making a current-version CanaryOnly claim.
- Use the retained AdditiveBatch1 enabled-mode smoke at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` only as beta.93 / `v0.107.1` 10 event types / 14 registration-call proof; recapture it after any package/source change before using it for a new claim.
- Only after those enabled-mode smokes match current source shape, capture CanaryOnly gameplay evidence for Big Fish, Golden Idol, The Lab, and Divine Fountain.
- Settings screenshots prove RitsuLib Mod Settings visibility only.

## Batch 4c

Batch 4c is proposal-only. Review `docs/features/ritsulib-migration/batch-4c-candidates.md`; do not migrate any candidate without explicit owner approval and fresh validation. Forbidden migration surfaces remain run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, and reward-state mutation.

## Next Actions

1. Keep docs and tests pointed at RitsuLib-only APIs.
2. Recapture beta.98 loader proof and clicked settings UI proof before making current runtime claims.
3. Capture current enabled-mode, gameplay/save-load/co-op evidence after loader proof.
4. Record owner decision for Batch 4c before any source migration.
