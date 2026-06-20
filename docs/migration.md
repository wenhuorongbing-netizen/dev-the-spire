# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | Done |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Done |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Done (no-op: no files moved) |
| PR 5 | RitsuLib hard dependency | Medium | Compile/manifest dependency added; historical `v0.106.1` loader-gate validated; current `v0.107.1` dependency setup installed with STS2-RitsuLib `v0.4.28` / `lib\0.107.1`; beta.91 RitsuLib-only Off and AdditiveBatch1 smokes are clean |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Batch 4a+4b source migrated; historical `v0.106.1` loader-gate validated; retained `v0.107.0` beta.87 package proof and BaseLib-backed beta.88 proof remain previous-context evidence; current `v0.107.1` beta.91 proof is clean for loader/patch application and AdditiveBatch1 registration shape |
| PR 7+ | High-risk patch migrations, one feature surface at a time | High | Blocked on live/manual proof |

## PR 5: RitsuLib Hard Dependency

Compile and manifest dependencies are present:

- `<PackageReference Include="STS2.RitsuLib" Version="0.4.28" PrivateAssets="All" />`
- `{ "id": "STS2-RitsuLib", "min_version": "0.4.28" }`

Historical runtime loader-gate verification is no longer blocked for the Slay the Spire 2 `v0.106.1` setup. Historical clean diagnostic smokes exist for Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed.

Current runtime dependency drift is resolved for loader and AdditiveBatch1 registration proof: the local installed game reports Slay the Spire 2 `v0.107.1`, official STS2-RitsuLib `v0.4.28` is installed with `lib\0.107.1`, and Spire Plus no longer depends on BaseLib in project, manifest, package, or current runtime proof. Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only. The retained `v0.107.0` beta.87 package proof, BaseLib-backed beta.88 proof, and previous RitsuLib-only beta.90 proof remain previous-context evidence; current `v0.107.1` beta.91 proof is clean at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` for loader/patch application and AdditiveBatch1 registration shape. The beta.85 Off and CanaryOnly smokes remain previous-package loader context, the beta.85 AdditiveBatch1 mismatch remains root-cause history for stale package/source shape, and the `v0.107.1` beta.87 / BaseLib `v3.2.1` clean-audit failure remains root-cause history for dependency drift. The prior beta.84 failure was Spire Plus API-target drift, not a BaseLib/RitsuLib version blocker. Gameplay, Mod Settings UI page refresh, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and tester-package handoff remain pending; handoff must recapture HEAD and worktree status after any later edits. Post-baseline no-game/doc-governance recaptures do not close runtime/manual gates.

Package metadata decision for the current source and beta.91 package line: the owner-requested RitsuLib blocker fix intentionally bumped the repo compile package and manifest minimum to `0.4.28` in the same versioned package pass and removed BaseLib from current Spire Plus requirements. Future dependency-floor changes must again bump the package version and rerun build, tests, publish/package, opt-in artifact checks, and fresh loader smoke as appropriate.

## Batch 4a: Low-Risk Patch Migration

Migrated 9 low-risk patch classes to RitsuLib's `IPatchMethod` interface.

| File | Classes | PatchIds |
| --- | --- | --- |
| `FiddlePatches.cs` | 4 | `fiddle-vars`, `fiddle-hand-draw`, `fiddle-should-draw`, `fiddle-draw-cap` |
| `ChoicesParadoxPatches.cs` | 1 | `choices-paradox-turn-start` |
| `DistinguishedCapePatches.cs` | 3 | `distinguished-cape-vars`, `distinguished-cape-event-option`, `distinguished-cape-pickup` |
| `BlackStarCompensationPatches.cs` | 1 | `black-star-obtain` |

## Batch 4b: Medium-Risk Patch Migration

Migrated 16 medium-risk patch classes to RitsuLib's `IPatchMethod` interface.

| File | Classes | PatchIds |
| --- | --- | --- |
| `CrossbowPatches.cs` | 2 | `crossbow-offer`, `crossbow-vanilla-after-turn` |
| `BrightestFlameExhaustDrawPatch.cs` | 3 | `brightest-flame-keywords`, `brightest-flame-vars`, `brightest-flame-exhaust-backstop` |
| `DebtAndCardPatches.cs` | 8 | `debt-after-created`, `debt-from-save`, `debt-keywords`, `debt-vars`, `debt-turn-end-effect`, `debt-turn-end-in-hand`, `card-model-on-play`, `debt-exhaust` |
| `SealOfGoldPatches.cs` | 2 | `seal-of-gold-max-energy`, `seal-of-gold-turn` |
| `PickupRewardPatches.cs` | 1 | `ancient-pickup-balance` |

**Total migrated:** 25 classes (9 from Batch 4a + 16 from Batch 4b).
**Remaining:** 146 `[HarmonyPatch]` declarations still on raw Harmony.

Inventory rechecked on 2026-06-20 against the current source tree: 25 migrated `IPatchMethod` classes, 146 raw `[HarmonyPatch]` declarations, and 171 tracked patch units. This is source-inventory evidence only, not approval to migrate Batch 4c or any high-risk patch.

## Batch 5: High-Risk Patches

High-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence. Batch 4c may be reviewed as a low-risk candidate proposal only; do not migrate Batch 4c or high-risk patches without explicit owner approval and fresh validation. The current candidate proposal is `docs/features/ritsulib-migration/batch-4c-candidates.md`; it records per-candidate unchanged behavior, source evidence, targeted tests, and rollback paths.
