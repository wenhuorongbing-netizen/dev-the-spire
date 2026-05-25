# Codex Workflow

## Starting A Session

1. Read `AGENTS.md`.
2. Run `git status --short --branch`.
3. Identify the active mode from the latest user message.
4. Read `docs/README.md`.
5. Read `docs/dev-environment.md`.
6. For feature work, start from the relevant feature README.

## Active Project Direction

The active deliverable is private beta readiness for `Spire Plus` (`EZMicroBalance` manifest id).

Do not implement Ascension 21-30 or a custom character during this cycle.

Ascension 11-20 is an active development track and is default-on for single-player testing, but host-multiplayer A11-A20 selection/gameplay fails closed by default after the 2026-05-25 crash logs. Live readiness is still gated by manual verification. Do not expand Ascension scope without explicit request, and keep public/multiplayer disable switches documented and working.

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
- After validation succeeds, commit the intended changes and push the current branch to `origin`. Stop instead if validation fails, packaging fails, authentication is missing, or the push would include unrelated local changes.

## Worktree Hygiene

Use this cleanup order whenever a pass leaves many files changed:

1. Inspect with `git status --porcelain=v1 -uall`, `git diff --stat`, and `git clean -ndX`.
2. Classify every path as source/resource, tests, docs, package evidence, local environment, ignored build output, or obsolete prompt/audit material.
3. Never run broad `git reset --hard`, `git restore .`, or `git clean -fdx` while source work is still uncommitted.
4. Keep ignored local/runtime paths ignored: `.godot/`, `.tools/`, `publish/`, `bin/`, `obj/`, `Directory.Build.props`, local game source, and local art scratch.
5. Commit useful source, resource, test, and current documentation changes in a few named groups.
6. Remove redundant active prompt dumps after their final implementation records exist in `docs/archive/implementation-records/`.
7. Keep active docs small: `PROJECT_STATE.md`, `docs/test-ready-development-goal.md`, and `docs/issues.md` should carry current state; archives should not be required reading.
8. After packaging, update only current status/hash docs and release guards; do not rewrite old archive history.
9. Finish with build/test/format/diff-check, then `git status --short --branch`.
10. A clean pass means no unstaged or untracked project files remain, not that ignored local build/runtime caches were deleted.

## Local Paths

`Directory.Build.props` is local and gitignored. Copy from `Directory.Build.props.example` on a new machine and fill in local `GodotPath` and `Sts2Path`.
