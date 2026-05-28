# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and
refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | Done |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Done |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Done (no-op: no files moved) |
| PR 5 | RitsuLib hard dependency (only after 0.106.1/0.106.1 decision) | Medium | Done |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Batch 1 done, Batch 4a+4b done |
| PR 7+ | High-risk patch migrations, one feature surface at a time | High | Blocked |

## PR 1: Baseline + Docs-Only Codex Harness Integration

**Goal:** Lock baseline, create thin Codex harness templates, stage RitsuLib
documentation, produce move-only refactor map. No behavior changes.

**Files:**
- `docs/codex-harness/` -- template directory (reference only)
- `harness/` -- live task-scoped state
- `docs/integrations/ritsulib.md` -- RitsuLib staging record
- `docs/refactor-map.md` -- move-only refactor planning
- `docs/migration.md` -- this file
- `docs/README.md` -- updated index
- `docs/PROJECT_MAP.md` -- updated path map
- `scripts/report-worktree-batches.ps1` -- classify new docs

**Constraints honored:**
- Root `AGENTS.md` not overwritten
- `EZMicroBalance` manifest id, project, resource folder, code folder, DLL,
  PCK, install folder not renamed
- No DLL/PCK/ZIP committed
- No behavior changes

## PR 3: Move-Only Source Folder Refactor

**Status:** Done. Directory scaffolds created per `docs/refactor-map.md`. No files
moved — actual file moves will accompany behavior changes in later PRs.

**Directories created:**
- `Core/Integrations/RitsuLib/` — future RitsuLib bootstrap module
- `Ancients/Rebalance/` — shared rebalance helpers (extracted from Common)
- `Ascension/Ui/` — Ascension UI patches (if separated)
- `Ascension/Save/` — Ascension save/load code (if separated)

## PR 4: Test/Docs/Script Path Updates

**Status:** Done (no-op). No files were moved in PR 3, so no path updates needed.

## PR 5: RitsuLib Hard Dependency

**Status:** Done. Used resolution option 2 — `STS2.RitsuLib` 0.3.2 base package
directly (no compat package for 0.106.1 exists on NuGet).

**What was added:**
- `<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />` in csproj
- `{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }` in manifest dependencies

**Verification:**
- Build: 0 errors, 0 warnings
- Tests: 302 passed, 21 skipped, 0 failed (1 pre-existing batch script failure unrelated to RitsuLib)
- Format: clean

**Risk accepted:** RitsuLib 0.3.2 base package compiled against a different game
version than 0.106.1. Compile-time compatibility confirmed; runtime API mismatches
possible but unlikely given the clean build. When `STS2.RitsuLib.Compat.0.106.1`
is published, upgrade to the compat package.

**NuGet status and upgrade path:** See `docs/integrations/ritsulib.md`.

## PR 6+: RitsuLib API Adoption

**Batch order:**
1. Bootstrap, diagnostics, optional settings page
2. Future new content registration (not existing high-risk content)
3. Persistence sidecar experiments (not current 30 SavedSpireFields)
4. Low-risk patch wrappers
5. High-risk run/map/reward/save/multiplayer patches (only after manual
   evidence backlog is reduced)

### Batch 1: Bootstrap + Diagnostics (Done)

**What was added:**
- `RitsuLibBootstrap.cs` — RitsuLib logger initialization, Harmony patch
  application with diagnostics, framework status reporting.
- `MainFile.cs` — now calls `RitsuLibBootstrap.ApplyPatches()` instead of
  raw `new Harmony(id).PatchAll()`.
- Guard manifest updated with new source file and coverage root.

**What was NOT changed (deferred):**
- Patch classes still use `[HarmonyPatch]` attributes, not RitsuLib's
  `IPatchMethod`/`IModPatchProvider` interfaces. Migrating 63 patch classes
  to RitsuLib's managed `ModPatcher` is a future batch.
- No content registration (`CreateContentPack`) — Spire Plus doesn't register
  new cards/relics/potions through RitsuLib.
- No settings page (`RegisterModSettings`) — existing BaseLib config stays.
- No persistence (`BeginModDataRegistration`) — existing SavedSpireFields stay.

### Batch 4a: Low-Risk Patch Migration (Done)

Migrated 10 low-risk patch classes to RitsuLib's `IPatchMethod` interface.
`RitsuLibBootstrap` now uses `ModPatcher` for migrated patches and raw
`Harmony.PatchAll()` for the remaining `[HarmonyPatch]`-attributed classes.

**Migrated patches:**

| File | Classes | PatchIds |
| --- | --- | --- |
| `FiddlePatches.cs` | 4 | `fiddle-vars`, `fiddle-hand-draw`, `fiddle-should-draw`, `fiddle-draw-cap` |
| `ChoicesParadoxPatches.cs` | 1 | `choices-paradox-turn-start` |
| `DistinguishedCapePatches.cs` | 3 | `distinguished-cape-vars`, `distinguished-cape-event-option`, `distinguished-cape-pickup` |
| `BlackStarCompensationPatches.cs` | 1 | `black-star-obtain` |

**Pattern:** Each class changed from `internal static class` with `[HarmonyPatch]`
to `internal sealed class : IPatchMethod` with `GetTargets()` returning
`ModPatchTarget[]`. `[HarmonyPrefix]`/`[HarmonyPostfix]` attributes kept.

**Verification:** Build 0 errors, 4 migration-related tests pass, format clean.

### Batch 4b: Medium-Risk Patch Migration (Done)

Migrated 16 medium-risk patch classes to `IPatchMethod`.

**Migrated patches:**

| File | Classes | PatchIds |
| --- | --- | --- |
| `CrossbowPatches.cs` | 2 | `crossbow-offer`, `crossbow-vanilla-after-turn` |
| `BrightestFlameExhaustDrawPatch.cs` | 3 | `brightest-flame-keywords`, `brightest-flame-vars`, `brightest-flame-exhaust-backstop` |
| `DebtAndCardPatches.cs` | 7 | `debt-after-created`, `debt-from-save`, `debt-keywords`, `debt-vars`, `debt-turn-end-effect`, `debt-turn-end-in-hand`, `card-model-on-play`, `debt-exhaust` |
| `SealOfGoldPatches.cs` | 2 | `seal-of-gold-max-energy`, `seal-of-gold-turn` |
| `PickupRewardPatches.cs` | 1 | `ancient-pickup-balance` |

**Total migrated:** 26 classes (10 from Batch 4a + 16 from Batch 4b).
**Remaining:** 141 `[HarmonyPatch]` declarations still on raw Harmony.

**Verification:** Build 0 errors, 4 migration tests pass, format clean.

### Batch 5: High-Risk Patches (Blocked on Evidence)

Run/map/reward/save/multiplayer patches require manual evidence backlog
reduction before migration.
