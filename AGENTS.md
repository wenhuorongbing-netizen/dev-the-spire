# AGENTS.md

## Project
This repository workspace (`D:\Game\FOTN\dev-the-spire`) is for a Slay the Spire 2 content mod named `EzDailyContent`, authored by `AUTHOR_NAME_REPLACE_ME`, using C#/.NET, the Alchyr Slay the Spire 2 template, and BaseLib.

## Project Mission
This repository is for building a Slay the Spire 2 system expansion mod with Codex-assisted development. The first feature target is Ancient reward optimization. The second major target is Ascension 11-20-30 design and implementation. The final major target is a new custom character.

## Current setup status
The setup baseline is complete: build succeeds, publish succeeds, and manual game verification succeeded on public beta `v0.104.0` (`2026.04.23`) with BaseLib `v3.1.0`.

## Hard rules
- Do not change the manifest id after project creation. Current manifest id: `EzDailyContent`.
- Do not copy original Slay the Spire 2 game assets into this repository.
- Do not copy large chunks of decompiled game code into this repository.
- Do not implement gameplay features during setup or design-only tasks.
- Prefer BaseLib and template-supported APIs.
- Prefer game command APIs over direct state mutation when gameplay work begins.
- For content features, do not set `affects_gameplay` to `false` without a documented reason.
- If a command fails, diagnose before editing.
- Keep changes small and reviewable.
- Use git checkpoints.
- Update docs when setup or design decisions change.

## Build commands
- `dotnet build`

## Publish commands
- `dotnet publish`
- Do not continue to publish when build fails.

## Documentation
- `docs/SETUP_SPEC.md` contains the setup spec.
- `docs/PROJECT_MAP.md` contains the project map.
- `docs/dev-environment.md` records local environment values and TODOs.
- `docs/test-plan.md` contains validation steps.
- `docs/release-checklist.md` contains release checks.
- `docs/codex-workflow.md` explains Codex operating flow.
- `docs/first-feature-backlog.md` contains the corrected feature roadmap.
- The next feature spec should be `docs/ANCIENT_REWARD_SPEC_v0.104.md`.

## Directory conventions
Preserve template-generated structure if it differs. Current template structure uses:
- `EzDailyContent/` for Godot resources, localization placeholders, and placeholder images.
- `EzDailyContentCode/` for generated C# scaffolding.
- `docs/` for documentation.

The generated `Cards/`, `Powers/`, and `Relics/` folders currently contain abstract base scaffolding only. Do not add concrete cards, powers, relics, patches, or gameplay behavior during setup or design-only tasks.

## Testing expectations
- After code/config changes: run `dotnet build` when SDK is available.
- After resource/localization/packaging changes: run `dotnet publish` when SDK and Godot/MegaDot are available.
- Record build/publish status and blockers in `docs/dev-environment.md`.

## Safety rules
- Do not run destructive commands unless explicitly justified.
- Do not delete files unless exact paths and reasons are listed.
- Do not rewrite the whole project unless explicitly requested.

## BaseLib dependency rule
Document BaseLib dependency status and expected on-disk location. Do not fabricate BaseLib files.

## Early Access warning
Slay the Spire 2 APIs, BaseLib, templates, and tooling may change during Early Access. Revalidate versions and paths each session.

## Future gameplay implementation rules
When setup is complete and feature work starts, begin from a design spec, prefer template-supported extensibility paths and command APIs, then re-run build/publish verification after each feature increment.
