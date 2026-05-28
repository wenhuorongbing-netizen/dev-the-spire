# Restructure Migration Plan

This document tracks the PR sequencing for the `dev-the-spire` integration and
refactor work described in `docs/restructure.md`.

## PR Sequence

| PR | Scope | Risk | Status |
| --- | --- | --- | --- |
| PR 1 | Baseline lock + docs-only Codex harness integration | None | **Current** |
| PR 2 | RitsuLib staging docs + install instructions + version mismatch record | None | Done (in PR 1) |
| PR 3 | Move-only source folder refactor, no behavior changes | Low | Planned |
| PR 4 | Test/docs/script path updates after move-only refactor | Low | Planned |
| PR 5 | RitsuLib hard dependency (only after 0.106.0/0.106.1 decision) | Medium | Blocked |
| PR 6 | Low-risk RitsuLib API adoption | Medium | Blocked |
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

**Goal:** Restructure `EZMicroBalanceCode/` internal directories without
changing behavior. See `docs/refactor-map.md` for the target layout.

**New directories:**
- `Core/Integrations/RitsuLib/` -- future RitsuLib bootstrap module
- `Ancients/Rebalance/` -- shared rebalance helpers
- `Ascension/Ui/` -- Ascension UI patches (if separated)
- `Ascension/Save/` -- Ascension save/load code (if separated)

**Constraints:**
- No behavior changes in this PR
- High-risk patches (run, room, save, lobby, multiplayer, lifecycle) not moved
  with behavior changes
- RitsuLib patcher migration not mixed with folder moves

## PR 5: RitsuLib Hard Dependency (Blocked)

**Blocked on:** Version mismatch resolution (current target v0.106.0, available
RitsuLib variants: 0.103.2, 0.105.1, 0.106.1 -- no 0.106.0).

**Resolution options:**
1. Update repo target to v0.106.1
2. Obtain a 0.106.0-compatible RitsuLib build
3. Wait for variant confirmation

**When unblocked:**
- Add `<PackageReference Include="STS2.RitsuLib" Version="0.3.3" PrivateAssets="All" />`
- Add `{ "id": "STS2-RitsuLib", "min_version": "0.3.3" }` to manifest

## PR 6+: RitsuLib API Adoption (Blocked on PR 5)

**Batch order:**
1. Bootstrap, diagnostics, optional settings page
2. Future new content registration (not existing high-risk content)
3. Persistence sidecar experiments (not current 30 SavedSpireFields)
4. Low-risk patch wrappers
5. High-risk run/map/reward/save/multiplayer patches (only after manual
   evidence backlog is reduced)
