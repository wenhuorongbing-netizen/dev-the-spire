# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | Done |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Done |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Done (no-op: no files moved) |
| PR 5 | RitsuLib hard dependency | Medium | Compile/manifest dependency added; historical `v0.106.1` loader-gate validated; current `v0.107.0` dependency installed, beta.84 Off smoke captured but non-clean |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Batch 4a+4b source migrated; historical `v0.106.1` loader-gate validated; current `v0.107.0` package proof blocked by non-clean beta.84 Off smoke |
| PR 7+ | High-risk patch migrations, one feature surface at a time | High | Blocked on live/manual proof |

## PR 5: RitsuLib Hard Dependency

Compile and manifest dependencies are present:

- `<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />`
- `{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }`

Historical runtime loader-gate verification is no longer blocked for the Slay the Spire 2 `v0.106.1` setup. Official STS2-RitsuLib `v0.3.10` is installed in the E-drive local game mod folder with a `0.106.1` runtime variant; the E-drive game root, `mods`, `BaseLib`, `EZMicroBalance`, and `STS2-RitsuLib` folders are present. Historical clean diagnostic smokes exist for Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed.

Current runtime dependency drift is partially resolved: the local installed game reports Slay the Spire 2 `v0.107.0`, and official STS2-RitsuLib `v0.4.16` is installed with `lib\0.107.0`. Treat the previous Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only. Installed beta.84 package parity was restored on 2026-06-10 and `scripts\check-installed-spire-plus-package.ps1` passed, but the fresh `v0.107.0` beta.84 Off smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is non-clean: main menu was reached with RitsuLib compat `0.107.0`, but Spire Plus had 8 optional ModPatcher failures and an `EctoplasmGoldGatePatch` initializer exception from API drift. Clean current-runtime loader proof, gameplay, Mod Settings UI, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, clean-worktree decision, current-source package decision, and versioned tester-package handoff remain pending.

Package metadata decision for the current dirty source state: keep the repo compile package and manifest minimum at `0.3.2` for the existing beta.84 package line. Do not silently bump to `0.4.16` in this mixed worktree. If the next owner-approved tester package targets the current `v0.107.0` runtime, bump `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.16` in the same versioned package pass, then rerun build, tests, publish/package, opt-in artifact checks, and fresh loader smoke.

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

## Batch 5: High-Risk Patches

High-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence. Batch 4c may be reviewed as a low-risk candidate proposal only; do not migrate Batch 4c or high-risk patches without explicit owner approval and fresh validation. The current candidate proposal is `docs/features/ritsulib-migration/batch-4c-candidates.md`; it records per-candidate unchanged behavior, source evidence, targeted tests, and rollback paths.
