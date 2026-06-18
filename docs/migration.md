# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | Done |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Done |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Done (no-op: no files moved) |
| PR 5 | RitsuLib hard dependency | Medium | Compile/manifest dependency added; historical `v0.106.1` loader-gate validated; current `v0.107.0` dependency installed, beta.86 AdditiveBatch1 smoke clean |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Batch 4a+4b source migrated; historical `v0.106.1` loader-gate validated; current `v0.107.0` beta.86 package proof clean for loader/patch application and AdditiveBatch1 registration shape |
| PR 7+ | High-risk patch migrations, one feature surface at a time | High | Blocked on live/manual proof |

## PR 5: RitsuLib Hard Dependency

Compile and manifest dependencies are present:

- `<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />`
- `{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }`

Historical runtime loader-gate verification is no longer blocked for the Slay the Spire 2 `v0.106.1` setup. Official STS2-RitsuLib `v0.3.10` is installed in the E-drive local game mod folder with a `0.106.1` runtime variant; the E-drive game root, `mods`, `BaseLib`, `EZMicroBalance`, and `STS2-RitsuLib` folders are present. Historical clean diagnostic smokes exist for Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed.

Current runtime dependency drift is resolved for loader and AdditiveBatch1 registration proof: the local installed game reports Slay the Spire 2 `v0.107.0`, and official STS2-RitsuLib `v0.4.16` is installed with `lib\0.107.0`. Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only. Installed beta.86 package parity passed on 2026-06-18; the fresh `v0.107.0` beta.86 AdditiveBatch1 direct smoke at `.tools/runtime-evidence/v01070-beta86-additive-batch1-direct-20260618-031254/` is clean with retained verifier reports proving 10 event types / 14 registered-event lines and exact act/shared tuple parity. The beta.85 Off and CanaryOnly smokes remain previous-package loader context, and the beta.85 AdditiveBatch1 mismatch remains root-cause history for stale package/source shape. The prior beta.84 failure was Spire Plus API-target drift, not a BaseLib/RitsuLib version blocker. Gameplay, Mod Settings UI page refresh, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and tester-package handoff remain pending; the beta.86 migration validation was pushed at `eaaeb5a1`, but handoff must recapture HEAD and worktree status after any later edits. Post-baseline no-game/doc-governance recaptures include `bdc002d6`; they do not refresh the beta.86 package artifact or close runtime/manual gates.

Package metadata decision for the current source and beta.86 package line: keep the repo compile package and manifest minimum at `0.3.2` for compatibility until the owner explicitly chooses a dependency-floor bump. The current runtime fix did not require bumping to `0.4.16`; it required fixing Spire Plus patch targets and then rerunning build, publish/package, package parity, and loader smoke. If a future tester package intentionally targets `STS2-RitsuLib` `0.4.16` as the minimum, bump `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum in the same versioned package pass, then rerun build, tests, publish/package, opt-in artifact checks, and fresh loader smoke.

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
**Remaining:** 142 `[HarmonyPatch]` declarations still on raw Harmony.

Inventory rechecked on 2026-06-17 against the current source tree: 25 migrated `IPatchMethod` classes, 142 raw `[HarmonyPatch]` declarations, and 167 tracked patch units. This is source-inventory evidence only, not approval to migrate Batch 4c or any high-risk patch.

## Batch 5: High-Risk Patches

High-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence. Batch 4c may be reviewed as a low-risk candidate proposal only; do not migrate Batch 4c or high-risk patches without explicit owner approval and fresh validation. The current candidate proposal is `docs/features/ritsulib-migration/batch-4c-candidates.md`; it records per-candidate unchanged behavior, source evidence, targeted tests, and rollback paths.
