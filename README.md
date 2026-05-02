# EzDailyContent

A Slay the Spire 2 system expansion mod workspace.

## Project direction
EzDailyContent is no longer scoped as a temporary tiny card pack. The corrected project direction is:

1. Ancient reward tuning first.
2. Expanded Ascension 11-20-30 system second.
3. Custom character design and implementation last.

The next design spec should be `docs/ANCIENT_REWARD_SPEC_v0.104.md`.

## Status
Setup automated checks and manual in-game verification have succeeded. The template project has been generated, `EzDailyContent.sln` exists, `dotnet build` succeeds, `dotnet publish` verifies the DLL, manifest, and PCK artifacts, and Slay the Spire 2 Mod Settings shows BaseLib and EzDailyContent as enabled.

## Requirements
- Slay the Spire 2 at `D:\Steam\steamapps\common\Slay the Spire 2`
- Verified public beta version `v0.104.0`, date `2026.04.23`
- .NET SDK 9.0.313 or compatible
- Git
- Godot .NET / Mono 4.5.1 installed under `.tools\godot-4.5.1-mono`
- BaseLib runtime `v3.1.0` installed under `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`

## Local path configuration
`Directory.Build.props` is local and gitignored because it contains machine-specific absolute paths.

On a new machine:
1. Copy `Directory.Build.props.example` to `Directory.Build.props`.
2. Fill in `GodotPath`.
3. Fill in `Sts2Path`.

Do not commit local `Directory.Build.props` changes.

## BaseLib status
BaseLib runtime is installed at the expected path:

```text
D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib\
  BaseLib.json
  BaseLib.dll
  BaseLib.pck
```

Installed runtime version: `v3.1.0`

Project package version: `Alchyr.Sts2.BaseLib` `3.1.0`

The older root-level folder `D:\Steam\steamapps\common\Slay the Spire 2\BaseLib` still exists and was intentionally left untouched.

## Build
```powershell
dotnet build
```

## Publish
```powershell
dotnet publish
```

Current publish note: publish succeeds without the previous missing-solution warnings.

## Install
Current published output:

```text
D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\
  EzDailyContent.dll
  EzDailyContent.json
  EzDailyContent.pck
```

## Current content
No concrete gameplay content implemented yet. Template-generated card, power, and relic files are abstract base scaffolding only.

## Remaining setup note
`EzDailyContent.json` still uses `AUTHOR_NAME_REPLACE_ME`. Replace it only after the user supplies the desired author name. Do not change manifest id `EzDailyContent`.

## Documentation
- `docs/SETUP_SPEC.md`
- `docs/PROJECT_MAP.md`
- `docs/dev-environment.md`
- `docs/test-plan.md`
- `docs/release-checklist.md`
- `docs/codex-workflow.md`
- `docs/design-operating-brief.md`
- `docs/downfall-character-reference.md`
- `docs/boss-character-design-knowledgebase.md`
- `docs/boss-character-concepts-v2.md`
- `docs/ceremonial-beast-character-draft.md`
- `docs/ceremonial-beast-v3-bell-crowned-design.md`
- `docs/first-feature-backlog.md`
