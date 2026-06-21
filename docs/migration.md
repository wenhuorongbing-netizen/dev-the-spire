# RitsuLib Migration Status Index

This file is the compact active index for migration status. It intentionally
does not carry the old PR-sequencing history; current implementation work should
start from:

- `docs/goals/migration.md` for the active goal, success criteria, and next
  actions.
- `docs/integrations/ritsulib.md` for the dependency, installed runtime, API,
  and evidence record.
- `docs/features/ritsulib-migration/batch-4c-candidates.md` for the proposal
  boundary before any further patch migration.
- `PROJECT_STATE.md` and `docs/reviews/current-validation.md` for current
  command output and evidence scope.

## Current Boundary

Spire Plus is on the beta.99 RitsuLib-only target:

- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.32`.
- `EZMicroBalance.json` declares `STS2-RitsuLib >= 0.4.32` as the shared runtime
  framework dependency.
- Current package parity exists for beta.99 after the RitsuLib settings-page
  I18N resource migration.
- Beta.99 clicked RitsuLib Mod Settings UI proof is captured at
  `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
- Beta.99 direct Off loader proof is captured under `.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/` for startup/loading and default-Off StS1Events behavior only.
- Previous beta.96 RitsuLib-only Off proof is startup/loading evidence only.
  Prior beta.93 RitsuLib-only AdditiveBatch1 proof is previous-package
  registration evidence only. Recapture beta.99 loader proof before making a
  current runtime claim.

Gameplay, event screenshots, save-load, image/render, replacement functional
proof, co-op/fail-closed proof, independent QA, current enabled-mode proof, and
tester-package handoff remain pending; handoff must recapture HEAD and worktree
status after any later edits.

## Migrated Patch Inventory

Batch 4a source status: Migrated 9 low-risk patch classes to RitsuLib
`IPatchMethod`.

| File | Classes | PatchIds |
| --- | ---: | --- |
| `FiddlePatches.cs` | 4 | `fiddle-vars`, `fiddle-hand-draw`, `fiddle-should-draw`, `fiddle-draw-cap` |
| `ChoicesParadoxPatches.cs` | 1 | `choices-paradox-turn-start` |
| `DistinguishedCapePatches.cs` | 3 | `distinguished-cape-vars`, `distinguished-cape-event-option`, `distinguished-cape-pickup` |
| `BlackStarCompensationPatches.cs` | 1 | `black-star-obtain` |

Batch 4b source status: Migrated 16 medium-risk patch classes to RitsuLib
`IPatchMethod`.

| File | Classes | PatchIds |
| --- | ---: | --- |
| `CrossbowPatches.cs` | 2 | `crossbow-offer`, `crossbow-vanilla-after-turn` |
| `BrightestFlameExhaustDrawPatch.cs` | 3 | `brightest-flame-keywords`, `brightest-flame-vars`, `brightest-flame-exhaust-backstop` |
| `DebtAndCardPatches.cs` | 8 | `debt-after-created`, `debt-from-save`, `debt-keywords`, `debt-vars`, `debt-turn-end-effect`, `debt-turn-end-in-hand`, `card-model-on-play`, `debt-exhaust` |
| `SealOfGoldPatches.cs` | 2 | `seal-of-gold-max-energy`, `seal-of-gold-turn` |
| `PickupRewardPatches.cs` | 1 | `ancient-pickup-balance` |

**Total migrated:** 25 classes (9 from Batch 4a + 16 from Batch 4b).
**Remaining:** 146 `[HarmonyPatch]` declarations still on raw Harmony.

Inventory rechecked on 2026-06-20 against the current source tree: 25 migrated `IPatchMethod` classes, 146 raw `[HarmonyPatch]` declarations, and 171 tracked patch units.

## Batch 4c Boundary

Batch 4c may be reviewed as a low-risk candidate proposal only; do not migrate Batch 4c or high-risk run, map, reward, save, and multiplayer patches without explicit owner approval, current source evidence, focused tests, package-version planning when artifacts change, and fresh validation.

The candidate proposal is
`docs/features/ritsulib-migration/batch-4c-candidates.md`.
