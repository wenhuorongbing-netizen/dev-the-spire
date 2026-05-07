# Project Map

## Active Release Target

`EZ Micro Balance` is the active private beta target. The architecture decision is to create an independent mod project with manifest id `EZMicroBalance`.

The original `EzDailyContent` project remains a legacy scaffold and its manifest id must not be changed in-place.

## Current Top-level Structure

```text
dev-the-spire/
  AGENTS.md
  README.md
  Directory.Build.props.example
  EzDailyContent.json
  EZMicroBalance.csproj
  EZMicroBalance.json
  EZMicroBalance.sln
  EZMicroBalance.sln.DotSettings
  Sts2PathDiscovery.props
  export_presets.cfg
  project.godot
  EzDailyContent/
    .gdignore
    images/
    localization/
      eng/
      zhs/
  EzDailyContentCode/
    .gdignore
    MainFile.cs
    AncientRewardNoopProbe.cs
    Ancients/
    Cards/
    Extensions/
    Powers/
    Relics/
  EZMicroBalance/
    images/
    localization/
      eng/
      zhs/
  EZMicroBalanceCode/
    .gdignore
    MainFile.cs
    Ancients/
      Common/
      Patches/
    Ascension/
      Cards/
  tests/
    EZMicroBalance.Tests/
  legacy/
    EzDailyContent/
      EzDailyContent.csproj.legacy.xml
  docs/
    architecture-ez-micro-balance.md
    mod-changelog.md
    issues.md
    features/ancients-rework-v4/
    archive/legacy-planning/
    BETA_COMPATIBILITY.md
    PROJECT_MAP.md
    dev-environment.md
    private-beta-verification-handoff.md
    test-plan.md
    release-checklist.md
    codex-workflow.md
    first-feature-backlog.md
    REMOTE_DEVELOPMENT_SETUP.md
    SETUP_SPEC.md
```

Generated build/cache/tool output such as `.godot/`, `.tools/`, `bin/`, and `obj/` is intentionally omitted.

Ignored local art/research folders currently present:

```text
art_pipeline/
asset/
source code/
```

They are ignored by `.gitignore` and are not part of the current packaged mod surface unless explicitly included in a future release decision. `source code/` is local decompile/reference scratch material only and must not be committed.

## Important Files

- `EZMicroBalance.sln`: active solution for bare `dotnet build`.
- `EZMicroBalance.csproj`: active private beta project.
- `tests/EZMicroBalance.Tests/`: release artifact and Harmony patch target tests for `EZMicroBalance`.
- `EZMicroBalance.json`: active private beta manifest. Current id is `EZMicroBalance`.
- `EzDailyContent.json`: legacy manifest. Current id is `EzDailyContent`; do not rename in-place.
- `export_presets.cfg`: selected-resource Godot export; packages only `EZMicroBalance` resources and the active manifest.
- `EZMicroBalanceCode/Ancients/Common/`: shared state fields, custom enchantment, and helper functions for Ancient patches.
- `EZMicroBalanceCode/Ancients/Patches/`: grouped Harmony patches by reward surface/relic family.
- `EZMicroBalanceCode/Ascension/`: default-off gated Ascension 11-20 implementation slices; currently Root-family cards and run/combat hooks only.
- `EzDailyContentCode/Ancients/`: legacy copy from before independent-project migration; not part of the active solution.
- `.gdignore` files: prevent legacy, source-only, docs, art, and archive folders from being imported as Godot resources.
- `.cs.uid` files: track when generated for C# script files; keep this policy consistent for new release-source files.
- `EzDailyContentCode/AncientRewardNoopProbe.cs`: legacy debug probe gated by `EZ_MICRO_BALANCE_DEBUG_PROBES=1`; not compiled into the active release project.
- `EZMicroBalance/localization/eng/`: English localization overrides for implemented behavior.
- `EZMicroBalance/localization/zhs/`: Simplified Chinese localization overrides for implemented behavior.
- `docs/architecture-ez-micro-balance.md`: architecture decision for independent mod identity.
- `docs/mod-changelog.md`: one-line mod-facing changelog; update this file for each future mod change.
- `docs/issues.md`: open player-reported/runtime issues that must stay visible until fixed and verified.
- `docs/features/ancients-rework-v4/source-design.md`: source design brief for Ancient reward changes.
- `docs/features/ancients-rework-v4/api-discovery.md`: API evidence and implementation notes.
- `docs/features/ancients-rework-v4/completion-audit.md`: prompt-to-artifact release gate checklist and current blockers.
- `docs/private-beta-verification-handoff.md`: concise tester handoff for the remaining manual/private-beta gates.
- `docs/features/ancients-rework-v4/work-log.md`: implementation work log.
- `docs/features/ascension-11-20/development-checklist-v2.md`: current forward-looking A11-A20 feature GDD and development checklist for Rootblight, Firemarked Elites, Fission, Banner Rooms, Deep Branches, Boss Royal Seals, and Dual King Brands.
- `docs/archive/legacy-planning/`: archived historical research and roadmap docs.

## Intended Private Beta Structure

The active private beta structure is:

```text
EZMicroBalance/
  localization/
    eng/
    zhs/
EZMicroBalanceCode/
  MainFile.cs
  Ancients/
  Ascension/
```

Published output should be:

```text
<GameRoot>/mods/EZMicroBalance/
  EZMicroBalance.json
  EZMicroBalance.dll
  EZMicroBalance.pck
```

## Milestones

- M0: Baseline setup complete.
- M1: Ancient reward rebalance implemented in legacy project.
- M2: Independent `EZMicroBalance` project created and root `dotnet build` succeeds.
- M3: Code organization, probe cleanup, localization validation, build, publish, and gated Ascension source guards complete; manual runtime verification pending.
- M4: Private beta release after clean commit and user-approved push.
