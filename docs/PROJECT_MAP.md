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
| `EZMicroBalanceCode/Ancients/Expansion/Urda/` | Current | Urda Ancient expansion feature implementation and gate control. |
| `tests/EZMicroBalance.Tests/` | Current | Source, localization, docs, package, and runtime-evidence guards. |
| `export_presets.cfg` | Current | Selected-resource PCK export for active mod resources only. |
| `Directory.Build.props.example` | Current | Template for local machine paths. |
| `Sts2PathDiscovery.props` | Current | Local path discovery helper used by build props. |
| `docs/` | Current | Documentation index, current release docs, feature records, and archive. |
| `scripts/` | Current | Repository helper scripts. |
| `docs/archive/` | Current | Historical planning, prompt material, release archaeology, archived audits, and implementation records. |
| `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/` | Archive | Historical v2.2 source-audit matrices; do not use as default next-development input. |
| `docs/archive/implementation-records/` | Archive | Compact implementation records moved out of the active reading path. |
| `docs/issues/` | Current support | Compact issue follow-up plus manual evidence queue retained for guard/reference support (`docs/issues/waiting-tests.md`). |
| `docs/features/ancient-expansion-v2.2/` | Current planning/prototype | Current Urda stabilization, default-on Morvi source slice, default-on Lotha source slice, and default-on single-player Vakuu fight source slice; live verification remains pending. |

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
| `legacy/` | Legacy | Preserved project artifacts from earlier migration work. |
| `source code/` | Ignored local scratch | Current decompiled game source reference. Do not commit or package. |
| `.tools/` | Ignored local tools | Downloaded GDRETools, Godot, ILSpy, and local helper binaries. |
| `.godot/` | Ignored generated output | Godot import/build cache. |
| `publish/` | Ignored release output | Package staging, versioned package, and private beta zip. |
| `art_pipeline/`, `asset/` | Ignored local art/work files | Not part of the active PCK unless explicitly promoted later. |

## Documentation Map

Start at `docs/README.md`.

| Area | Entry Point |
| --- | --- |
| Current environment and evidence | `docs/dev-environment.md` |
| Release gates | `docs/release-checklist.md` |
| Tester handoff | `docs/private-beta-verification-handoff.md` |
| Open issues | `docs/issues.md` |
| Test-ready development goal | `docs/test-ready-development-goal.md` |
| Manual evidence queue | `docs/issues/waiting-tests.md` (support only; next development starts from `docs/test-ready-development-goal.md`) |
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
