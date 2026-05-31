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
- Automated release/source-guard tests currently pass after Ancient hardening, Ascension 11-20 selector/slice guards, diagnostics guards, release-art guards, package-drift guards, documentation freshness guards, and the current RitsuLib/Sts1Events governance guards.
- Current local source is refreshed from Slay the Spire 2 `v0.106.1`; BaseLib runtime/project package are aligned on `v3.1.4`.
- Current no-game validation target at HEAD `6b149ba0` is 0 build errors, 89 Sts1Events nullable warnings, and 464 passed / 0 failed / 21 skipped / 485 total tests after the required Revision J replay.
- Controlled `--force-steam off` smoke loading and normal Steam-client startup/log verification exist for earlier package states only.
- Current runtime dependency setup now has official `STS2-RitsuLib` `v0.3.10` installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.106.1\STS2-RitsuLib.dll`. A fresh controlled loader log exists at `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` and reaches main menu with BaseLib, RitsuLib, and Spire Plus loaded, but runtime smoke remains blocked because the audit reports 11 `Godot ERROR` hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. Do not claim current-package runtime parity, Off=0 closure, CanaryOnly=4, runtime safety, live-ready, or release-ready until clean loader/error disposition plus runtime proof exist. Mod Settings UI screenshot evidence for `EZMicroBalance` is still partly historical from before the display-name refresh. Live gameplay feature verification remains pending.

Revalidate build, publish, and game load before claiming private beta readiness.

## Hard Rules

- Do not change an existing manifest id in-place.
- If creating a new independent mod project, choose and document a stable manifest id before the first build.
- Do not copy original Slay the Spire 2 non-art game assets into this repository, including code, data tables, text dumps, scenes, serialized gameplay resources, and other non-visual source materials.
- Original Slay the Spire 2 art may be used in tracked/public files only when redistribution permission is confirmed and documented. Without that permission, keep original art outside the repository and use Spire Plus-owned, generated, or otherwise redistributable replacement art.
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

### Versioning, package, and push rules

- Every successful implementation pass that changes player-visible behavior, resources, localization, package contents, tests that guard shipped behavior, or tester handoff docs must increment the Spire Plus package version. Do not ship two different builds with the same visible version.
- During private beta, use the manifest/package version form `v0.1.0-private-beta.N` and increment `N` for each delivered tester build unless the owner explicitly chooses a new semantic version line.
- Keep `EZMicroBalance.json`, package names, release hashes, tester handoff docs, and website/package metadata aligned with the new version before handoff.
- After code or resource changes, run the required validation commands for the touched surface. At minimum, run `dotnet build`; after resource/localization/packaging changes, also run `dotnet publish` and refresh the package.
- After a successful implementation pass, commit the intended changes and push the current branch to `origin` so other testers and agents can get the latest version immediately.
- Do not push if validation fails, packaging fails, the remote is unavailable, authentication is missing, or the only possible push would include unrelated local changes that were not part of the task. In that case, stop and report the exact blocker.
- Preserve `EZMicroBalance` only as the stable technical manifest id, install folder, saved-field namespace, and compatibility surface. Player-facing version notes and tester instructions should call the mod `Spire Plus`.

## Build Commands

- `dotnet build`

## Publish Commands

- `dotnet publish`
- Do not continue to publish when build fails.

## Local Machine Setup

- Clone the repository on the target machine.
- Copy `Directory.Build.props.example` to `Directory.Build.props`.
- Fill local `GodotPath` and `Sts2Path` values.
- Install BaseLib `v3.1.4` under `<GameRoot>\mods\BaseLib`.
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
