# AGENTS.md

## Project

This repository is a Slay the Spire 2 mod workspace using C#/.NET, the Alchyr Slay the Spire 2 template, and BaseLib.

The original scaffold project was `EzDailyContent`; its manifest id must not be renamed in-place. Legacy top-level scaffold files are not part of the active deliverable.

The active private beta deliverable is one mod: `Spire Plus`, a Slay the Spire 2 balance, progression, and preview-tools expansion. Its stable manifest id remains `EZMicroBalance`.

## Current Mission

Complete `Spire Plus` for private beta testing and eventual private beta release.

In scope:

- Ancient reward rebalance from `docs/features/ancients-rework-v4/source-design.md`.
- Single plug-in / plug-off mod structure under `EZMicroBalance`.
- English and Simplified Chinese localization.
- Release documentation, build, publish, and manual verification checklist.
- Ascension 11-20 expansion work when explicitly requested, kept safely gated unless public selection/progress support is proven.

Out of scope this cycle:

- Ascension 21-30 implementation.
- Custom character implementation.
- Unrelated cards, relics, powers, assets, or balance systems.

## Current Setup Status

Baseline setup is complete on the local machine:

- Build has succeeded.
- Publish has succeeded.
- Historical `EzDailyContent` Mod Settings verification succeeded on public beta `v0.104.0` (`2026.04.23`) with BaseLib `v3.1.0`.
- `EZMicroBalance` build and publish have succeeded.
- Automated release/source-guard tests currently pass after Ancient hardening, Ascension 11-20 selector/slice guards, diagnostics guards, release-art guards, package-drift guards, and documentation freshness guards.
- Current local source is refreshed from Slay the Spire 2 `v0.105.0`; BaseLib runtime/project package are aligned on `v3.1.2`.
- Controlled `--force-steam off` smoke loading has verified only BaseLib and `EZMicroBalance` initialization to main menu from the final installed artifacts.
- Normal Steam-client startup/log verification exists for earlier `Spire Plus` display-name packages with exactly BaseLib and `EZMicroBalance` loaded, config registration, and 0 release-blocking log hits. Current source defines 24 SavedSpireFields after the latest static fixes, while the latest live loader log still reports the earlier 22-field package; rerun live loader smoke before claiming current-package runtime parity. Mod Settings UI screenshot evidence for `EZMicroBalance` is still partly historical under the old `EZ Micro Balance` display name. Live gameplay feature verification remains pending.

Revalidate build, publish, and game load before claiming private beta readiness.

## Hard Rules

- Do not change an existing manifest id in-place.
- If creating a new independent mod project, choose and document a stable manifest id before the first build.
- Do not copy original Slay the Spire 2 game assets into this repository.
- Do not copy large chunks of decompiled game code into this repository.
- Do not claim Ascension release readiness until direct API/runtime evidence supports it. If A11-A20 selection is explicitly requested for development testing, keep the patch narrow, documented, independently disableable, and out of A21-A30/custom-character scope.
- Keep experimental Ascension systems independently disableable or behind an explicit internal/debug gate unless release docs intentionally say otherwise.
- Do not implement Ascension 21-30 this cycle.
- Do not implement a custom character this cycle.
- Prefer BaseLib and template-supported APIs.
- Use Harmony only where no safer API exists.
- Prefer game command APIs over direct state mutation.
- Before changing Ascension map, UI, reward, combat, save/load, or hook behavior, inspect the relevant local game source under `source code/src/Core/`, inspect BaseLib/template APIs, and record the evidence in `docs/features/ascension-11-20/`. Keep the tutorial index `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html` and its BaseLib/RitsuLib sections as secondary references; local game source remains the primary implementation authority.
- Any type inheriting `AbstractModel` must be obtained from `ModelDb` when used as a canonical marker/hook/model. Do not call constructors directly except where creating mutable/runtime card/relic/etc. instances is explicitly supported by game APIs.
- Keep changes small and reviewable.
- If a command fails, diagnose before editing.
- Update docs when setup, architecture, behavior, build, publish, or validation status changes.
- Preserve useful historical research by archiving or moving docs rather than silently discarding them.

### Documentation and workflow rules

- Read `PROJECT_STATE.md` first; then read current docs before archive docs.
- For the next test-ready implementation push, read `docs/test-ready-development-goal.md` before coding.
- Fix root causes; do not add normalizers that hide bad upstream state.
- Large tasks must declare success criteria in docs before implementation.
- Do not force future agents to read archived prompt dumps; route current work through current docs + active feature docs first.

## Build Commands

- `dotnet build`

## Publish Commands

- `dotnet publish`
- Do not continue to publish when build fails.

## Local Machine Setup

- Clone the repository on the target machine.
- Copy `Directory.Build.props.example` to `Directory.Build.props`.
- Fill local `GodotPath` and `Sts2Path` values.
- Install BaseLib `v3.1.2` under `<GameRoot>\mods\BaseLib`.
- Do not commit `Directory.Build.props`, `.tools/`, `.godot/`, `bin/`, `obj/`, downloaded archives, or local binaries.

## Documentation

- `README.md` contains the human-facing project state.
- `docs/README.md` is the documentation index and should point readers to the current source of truth.
- `docs/architecture-ez-micro-balance.md` records the independent-mod architecture decision.
- `docs/test-ready-development-goal.md` is the current long-scope development goal for taking `Spire Plus` to a test-ready candidate.
- `docs/features/ancients-rework-v4/` contains the Spire Plus Ancient reward source design, implementation plan, API discovery, and work log.
- `docs/style/card-localization-style-guide.md` records source-backed card text, visible keyword, rich-text, dynamic variable, preview, and bilingual terminology rules.
- `docs/skills/sts2-godot-mod-development.md` is the repository-local agent reference for future StS2/Godot/.NET mod development passes.
- `docs/PROJECT_MAP.md` contains the project map.
- `docs/dev-environment.md` records local environment values and validation status.
- `docs/test-plan.md` contains automated and manual validation steps.
- `docs/release-checklist.md` contains private beta release checks.
- `docs/archive/legacy-planning/` preserves historical planning docs.

## Directory Conventions

Private beta target:

- `EZMicroBalance/` for Spire Plus resources and localization.
- `EZMicroBalanceCode/` for Spire Plus C# source.
- `EZMicroBalanceCode/Preview/` for integrated preview tools such as Crystal Sphere peek and transform preview.

## Testing Expectations

- After code/config changes: run `dotnet build`.
- After resource/localization/packaging changes: run `dotnet publish` after build succeeds.
- Before release: verify BaseLib and Spire Plus load in-game, inspect `godot.log`, and complete the feature manual verification matrix.

## BaseLib Dependency Rule

Document BaseLib dependency status and expected on-disk location. The expected runtime location is `<GameRoot>\mods\BaseLib`. Do not fabricate BaseLib files.

## Early Access Warning

Slay the Spire 2 APIs, BaseLib, templates, and tooling may change during Early Access. Revalidate versions and paths each session.
