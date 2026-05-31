# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | Done |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Done |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Done (no-op: no files moved) |
| PR 5 | RitsuLib hard dependency | Medium | Compile/manifest dependency added; runtime unverified |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Batch 4a+4b source migrated; runtime unverified |
| PR 7+ | High-risk patch migrations, one feature surface at a time | High | Blocked |

## PR 5: RitsuLib Hard Dependency

Compile and manifest dependencies are present:

- `<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />`
- `{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }`

Runtime verification is still blocked until STS2-RitsuLib is installed in the local game mod folder and a clean loader log is captured. The 2026-05-31 blocker check found the E-drive game root, `mods`, `BaseLib`, and `EZMicroBalance` folders present, but `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` missing; the checked D-drive game root/mod paths were absent.

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

High-risk run, map, reward, save, and multiplayer patches remain blocked on runtime smoke and live/manual evidence. Do not start Batch 4c or high-risk patch migration from source-level guards alone; the 2026-05-31 decision remains blocked/no-advance because STS2-RitsuLib is not installed and no `godot.log` runtime smoke was captured.
