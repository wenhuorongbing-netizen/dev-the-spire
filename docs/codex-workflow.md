# Codex Workflow

## Starting A Session

1. Read `AGENTS.md`.
2. Run `git status --short --branch`.
3. Identify the active mode from the latest user message.
4. Read `docs/dev-environment.md`.
5. For EZ Micro Balance work, read `docs/architecture-ez-micro-balance.md` and `docs/features/ancients-rework-v4/api-discovery.md`.

## Active Project Direction

The active deliverable is private beta readiness for `EZ Micro Balance`.

Do not implement Ascension 21-30 or a custom character during this cycle. Ascension 11-20 work is allowed only when explicitly requested and must stay gated or independently disableable until public selection/progress and runtime behavior are proven safe.

## Build Mode Flow

When the user says `build`:

1. Start from the next unfinished phase.
2. Run `git status -sb` before each phase.
3. Keep changes small and reviewable.
4. Run `dotnet build` after code or project changes.
5. Run `dotnet publish` after resource, localization, manifest, or packaging changes, only if build succeeds.
6. Summarize changes and blockers after each phase.

When the user says `continue`, resume the next unfinished phase.

When the user says `review`, review only and do not implement new behavior.

## Error Handling

Use the Failure Report format from the active user instruction when a phase cannot proceed.

Capture exact command, error summary, likely cause, what was checked, minimal fix attempted, and remaining blocker.

## Git Workflow

- Preserve the existing dirty worktree.
- Do not revert unrelated user changes.
- Use checkpoints before and after major phases when practical.
- Do not push to `origin/main` without explicit user approval.

## Local Paths

`Directory.Build.props` is local and gitignored. Copy from `Directory.Build.props.example` on a new machine and fill in local `GodotPath` and `Sts2Path`.
