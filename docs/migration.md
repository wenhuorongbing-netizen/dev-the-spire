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

**Status:** Done. Directory scaffolds created per `docs/refactor-map.md`. No files
moved — actual file moves will accompany behavior changes in later PRs.

**Directories created:**
- `Core/Integrations/RitsuLib/` — future RitsuLib bootstrap module
- `Ancients/Rebalance/` — shared rebalance helpers (extracted from Common)
- `Ascension/Ui/` — Ascension UI patches (if separated)
- `Ascension/Save/` — Ascension save/load code (if separated)

## PR 4: Test/Docs/Script Path Updates

**Status:** Done (no-op). No files were moved in PR 3, so no path updates needed.

## PR 5: RitsuLib Hard Dependency (Blocked)

**Blocked on:** Two version mismatches must be resolved first.

### Blocker 1: Game version mismatch

| Item | Value |
| --- | --- |
| Current repo StS2 target | v0.106.0 |
| Available RitsuLib runtime variants | 0.103.2, 0.105.1, 0.106.1 |
| Missing runtime variant | **0.106.0** |

### Blocker 2: NuGet compat package missing

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | 0.3.2 (latest) | Available |
| `STS2.RitsuLib.Compat.0.103.2` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.104.0` | 0.2.40 | Available |
| `STS2.RitsuLib.Compat.0.105.1` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.106.0` | -- | **Not published** |
| `STS2.RitsuLib.Compat.0.106.1` | -- | **Not published** |

No compat package exists for the current game target (0.106.0) or the
closest variant (0.106.1). The restructure plan referenced version 0.3.3
but the latest on NuGet is 0.3.2.

### Resolution options

1. **Update repo target to v0.106.1** -- build, test, and runtime smoke
   against 0.106.1, then adopt the 0.106.1 RitsuLib variant (if/when
   `STS2.RitsuLib.Compat.0.106.1` is published to NuGet).
2. **Obtain a 0.106.0-compatible RitsuLib build** -- confirm it exists
   and is tested before adding the hard dependency.
3. **Use `STS2.RitsuLib` 0.3.2 directly** -- try adding the base package
   without a compat package; may compile but risk runtime mismatches.
4. **Wait** -- keep RitsuLib as a staged runtime companion until a
   compatible variant and NuGet package are confirmed.

### When unblocked

- Add `<PackageReference Include="STS2.RitsuLib.Compat.0.106.0" Version="0.3.x" PrivateAssets="All" />`
  (or the appropriate compat package for the target game version)
- Add `{ "id": "STS2-RitsuLib", "min_version": "0.3.x" }` to manifest

## PR 6+: RitsuLib API Adoption (Blocked on PR 5)

**Batch order:**
1. Bootstrap, diagnostics, optional settings page
2. Future new content registration (not existing high-risk content)
3. Persistence sidecar experiments (not current 30 SavedSpireFields)
4. Low-risk patch wrappers
5. High-risk run/map/reward/save/multiplayer patches (only after manual
   evidence backlog is reduced)
