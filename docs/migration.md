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
| PR 6 | Low-risk RitsuLib API adoption | Medium | Ready |
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
