# Codex Workflow

## Starting a session
1. Read `AGENTS.md`.
2. Run `git status --short --branch`.
3. Run `dotnet build`.
4. Read `docs/dev-environment.md` for unresolved TODOs.
5. Read `docs/first-feature-backlog.md` for the corrected mission.
6. For future balance or new-character work, read `docs/design-operating-brief.md` before proposing or implementing gameplay.

## Project direction
The project is a Slay the Spire 2 system expansion mod.

Priority order:
1. Ancient reward optimization.
2. Ascension 11-20-30 design and implementation.
3. New custom character design and implementation.

Future feature implementation must start from a design spec. The next spec should be `docs/ANCIENT_REWARD_SPEC_v0.104.md`.

## Local path configuration
`Directory.Build.props` is local and gitignored because it contains machine-specific absolute paths.

On a new machine:
1. Copy `Directory.Build.props.example` to `Directory.Build.props`.
2. Fill in `GodotPath`.
3. Fill in `Sts2Path`.
4. Run `dotnet build`.
5. Run `dotnet publish`.

## Useful prompts
- `review`
- `continue`
- `diagnose this build error`
- `update docs after this setup change`
- `draft docs/ANCIENT_REWARD_SPEC_v0.104.md`
- `create a design artifact for this balance or character idea`

## Error handling
Paste errors from:
- `dotnet build`
- `dotnet publish`
- `godot.log`

Current known note:
- `EzDailyContent.sln` exists and should be kept in sync with the generated project.
- BaseLib runtime should be installed under the game `mods\BaseLib` folder and should match the project package version.

## Git workflow
- Create checkpoints before and after each phase/feature.
- Keep commits small and reviewable.
- Avoid bundling unrelated repo-root changes.

## Build mode control words
- `build`: begin implementation phases.
- `continue`: resume next unfinished phase.
- `review`: setup/design review only, no new feature implementation.
