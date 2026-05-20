# Project Map

`Spire Plus` is the active private beta target. Its stable manifest id is `EZMicroBalance`. The original `EzDailyContent` scaffold remains for traceability and must not be renamed in-place.

## Top-Level Layout

| Path | Status | Purpose |
| --- | --- | --- |
| `README.md` | Current | Short project overview, build/publish commands, and release policy. |
| `AGENTS.md` | Current | Agent rules and project hard constraints. |
| `EZMicroBalance.sln` | Current | Active solution for build/test/publish. |
| `EZMicroBalance.csproj` | Current | Active private beta C# project. |
| `EZMicroBalance.json` | Current | Active mod manifest, display name `Spire Plus`, id `EZMicroBalance`. |
| `EZMicroBalance/` | Current | Active Godot resources, images, and localization. |
| `EZMicroBalanceCode/` | Current | Active C# source. See `EZMicroBalanceCode/README.md`. |
| `EZMicroBalanceCode/Ancients/Common/` | Current | Shared Ancient saved state, card helpers, reward relic service, and feature-gate helper. |
| `EZMicroBalanceCode/Ancients/Expansion/Urda/` | Current | Urda Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Morvi/` | Current | Morvi Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Lotha/` | Current | Lotha Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Vakuu/` | Current | Hidden-by-default Vakuu fight slice, encounter, monster, Temptation card, and fight gate. |
| `tests/EZMicroBalance.Tests/` | Current | Source, localization, docs, package, and runtime-evidence guards. |
| `export_presets.cfg` | Current | Selected-resource PCK export for active mod resources only. |
| `Directory.Build.props.example` | Current | Template for local machine paths. |
| `Sts2PathDiscovery.props` | Current | Local path discovery helper used by build props. |
| `docs/` | Current | Documentation index, current release docs, feature records, and archive. |
| `scripts/` | Current | Repository helper scripts. |
| `.github/workflows/repository-hygiene.yml` | Current | Hosted CI-safe manifest, JSON, docs, patch inventory, and whitespace checks. |
| `.github/workflows/full-local-validation.yml` | Current | Self-hosted Windows full no-game validation lane; requires explicit StS2 and Godot paths. |
| `docs/archive/` | Current | Historical planning, prompt material, release archaeology, archived audits, and implementation records. |
| `docs/archive/feature-inputs/` | Archive | Historical feature prompts, superseded implementation specs, and old source-design inputs moved out of active feature folders. |
| `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/` | Archive | Historical v2.2 source-audit matrices; do not use as default next-development input. |
| `docs/archive/feature-audits/review-pre-slim-20260518.md` | Archive | Full historical source-review log before the compact current `docs/review.md`. |
| `docs/archive/feature-audits/toreview-pre-slim-20260518.md` | Archive | Full historical retest queue before the compact current `docs/toreview.md`. |
| `docs/archive/implementation-records/` | Archive | Compact implementation records moved out of the active reading path. |
| `docs/archive/superseded/setup-spec-original-scaffold.md` | Archive | Historical original `EzDailyContent` setup specification; current setup starts from `README.md`, `docs/README.md`, and `docs/dev-environment.md`. |
| `docs/archive/project-state-history-20260516.md` | Archive | Pre-cleanup `PROJECT_STATE.md` snapshot preserving superseded per-pass validation/package history. |
| `docs/issues/` | Current support | Compact issue follow-up plus compact manual evidence queue retained for guard/reference support (`docs/issues/waiting-tests.md`). |
| `docs/worktree-cleanup-audit.md` | Current support | Current cleanup/refactor inventory, archive decisions, and owner-decision areas for this dirty worktree. |
| `docs/patch-inventory.md` | Current support | Generated Harmony patch owner/risk inventory. Regenerate with `scripts/generate-patch-inventory.ps1`. |
| `docs/release-evidence-status.md` | Current support | Compact current package and live/manual evidence dashboard. |
| `docs/adr/` | Current support | Architecture decision records. Start with `docs/adr/0000-template.md`. |
| `docs/specs/` | Current support | Release scope, website claim audit, and release traceability matrix. |
| `docs/source-research/` | Current support | Compact source evidence for high-risk run, room, reward, save, RNG, and multiplayer surfaces. |
| `docs/architecture/` | Current support | Bounded context, patch boundary, and save-state contract maps. |
| `docs/month-plan/` | Current support | 30-day plan outputs such as baseline freeze and commit boundaries. |
| `docs/features/ancient-expansion-v2.2/` | Current planning/prototype | Current Urda stabilization, default-on Morvi and Lotha source slices, and the hidden-by-default dedicated Vakuu fight slice; live verification remains pending. |

## Active Mod Surface

```text
EZMicroBalance/
  images/
  localization/
    eng/
    zhs/

EZMicroBalanceCode/
  MainFile.cs
  Ancients/
    Common/
    Patches/
    Expansion/
      Urda/
      Morvi/
      Lotha/
      Vakuu/
  Ascension/
    Cards/
    Combat/
    Core/
    Enchantments/
    Events/
    Map/
    Patches/
    Powers/
    Relics/
    Rewards/
```

Published runtime output should be:

```text
<GameRoot>/mods/EZMicroBalance/
  EZMicroBalance.json
  EZMicroBalance.dll
  EZMicroBalance.pck
```

The private beta zip should contain only those three installable files plus `README_INSTALL.txt` under an `EZMicroBalance/` folder.

## Legacy And Local-Only Material

| Path | Status | Notes |
| --- | --- | --- |
| `EzDailyContent.json` | Legacy | Original scaffold manifest id `EzDailyContent`; do not rename in-place. |
| `EzDailyContent/` | Legacy | Original resource folder; not the active private beta surface. |
| `EzDailyContentCode/` | Legacy | Original code folder; not part of the active release solution. |
| `docs/archive/legacy-planning/legacy-project-files/` | Archive | Preserved legacy project metadata from earlier migration work; top-level `legacy/` was removed during cleanup. |
| `source code/` | Ignored local scratch | Current decompiled game source reference. Do not commit or package. |
| `.tools/` | Ignored local tools | Downloaded GDRETools, Godot, ILSpy, and local helper binaries. |
| `.godot/` | Ignored generated output | Godot import/build cache. |
| `publish/` | Ignored release output | Package staging, versioned package, and private beta zip. |
| `website/` | Removed ignored local draft | Root draft was deleted after snapshotting; `.gitignore` keeps future regenerated copies out of release-candidate diffs unless deliberately promoted. |
| `.github/workflows/spire-plus-site.yml` | Removed ignored local draft | Website-only Pages workflow was deleted after snapshotting; promote only with repaired ownership/build docs. |
| `.tools/archive/local-art-and-calibration-20260515/` | Ignored local archive | Former root `art_pipeline/` and `asset/` generated art/calibration material; not part of the active PCK unless explicitly revalidated and promoted later. |
| `.tools/archive/local-root-clutter-20260515/` | Ignored local archive | Former root local-only package/source-analysis/website zip clutter preserved before any deletion decision. |
| `.tools/archive/local-website-preview-20260516/` | Ignored local archive | Snapshot of the removed `website/` static preview and `.github` Pages workflow taken during cleanup; use only if the draft is deliberately promoted later. |

## Documentation Map

Start at `docs/README.md`.

| Area | Entry Point |
| --- | --- |
| Current environment and evidence | `docs/dev-environment.md` |
| Release gates | `docs/release-checklist.md` |
| Tester handoff | `docs/private-beta-verification-handoff.md` |
| Open issues | `docs/issues.md` |
| Patch inventory | `docs/patch-inventory.md` |
| Release evidence dashboard | `docs/release-evidence-status.md` |
| Release scope | `docs/specs/release-scope-v1.md` |
| Traceability matrix | `docs/specs/release-traceability-matrix.md` |
| Baseline freeze | `docs/month-plan/baseline-2026-05-20.md` |
| Commit boundaries | `docs/month-plan/commit-boundaries.md` |
| Full local CI lane | `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` |
| Test-ready development goal | `docs/test-ready-development-goal.md` |
| Manual evidence queue | `docs/issues/waiting-tests.md` (compact support queue only; full historical rows are archived, and next development starts from `docs/test-ready-development-goal.md`) |
| Ancient reward rebalance | `docs/features/ancients-rework-v4/README.md` |
| Ancient expansion v2.2 | `docs/features/ancient-expansion-v2.2/README.md` |
| Ancient expansion v2.2 source audit archive | `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/README.md` |
| Urda current test slice | `docs/features/ancient-expansion-urda/README.md` |
| Ascension 11-20 | `docs/features/ascension-11-20/README.md` |
| Architecture decision | `docs/architecture-ez-micro-balance.md` |
| Historical planning | `docs/archive/README.md` |

## Extension Landmarks

- Add Ancient behavior under `EZMicroBalanceCode/Ancients/`, with shared state/helpers in `Ancients/Common/` and patch families in `Ancients/Patches/`.
- Add Ascension behavior under `EZMicroBalanceCode/Ascension/`; use the existing `Core/`, `Map/`, `Combat/`, `Rewards/`, `Enchantments/`, `Patches/`, `Cards/`, `Powers/`, `Relics/`, and `Events/` boundaries, and keep public/multiplayer selection disableable with live-readiness claims gated.
- Add user-facing text in both `EZMicroBalance/localization/eng/` and `EZMicroBalance/localization/zhs/`.
- Add or update tests in `tests/EZMicroBalance.Tests/` whenever source shape, localization, docs, package contents, or release evidence changes.
- Update feature README files and `docs/dev-environment.md` when implementation status, validation evidence, versions, or package hashes change.

## Milestones

| Milestone | Meaning |
| --- | --- |
| M0 | Local setup and baseline build/publish established. |
| M1 | Ancient reward rebalance implemented from the original scaffold. |
| M2 | Independent `EZMicroBalance` project created and root build succeeds. |
| M3 | Current v0.105.0/BaseLib v3.1.2 source, build, publish, package, and controlled smoke evidence refreshed. |
| M4 | Private beta release after normal Steam-client Mod Settings, live gameplay/manual matrix, clean handoff, and user-approved push. |
