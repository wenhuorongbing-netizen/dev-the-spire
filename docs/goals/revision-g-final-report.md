# Revision G Final Report — M4 Owner-Review Run

Date: 2026-05-31T16:05:00+02:00
HEAD: `24d4fe9a` (`ci: regenerate patch inventory consistently`)

## Verdict

```text
Not complete: hard blocker encountered.
```

## Hard Blocker

The current worktree cannot produce a ready-to-owner-review packet because required validation does not pass on current HEAD and dirty state.

| Command | Exit code | Result |
|---|---:|---|
| `dotnet clean .\EZMicroBalance.csproj` | 0 | Clean succeeded |
| `dotnet build .\EZMicroBalance.csproj` | 0 | Build succeeded: 0 errors, 89 warnings |
| `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` | non-zero | 1 failed, 451 passed, 21 skipped, 473 total |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | 0 | Format verification succeeded |
| `git diff --check` | non-zero | Trailing whitespace in `docs/goals/debug.md` and `docs/goals/event.md` |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | 0 | 12 dirty entries, 0 unclassified |

## Exact Failures

`dotnet test` fails on:

- `DocumentationCompactnessGuardTests.PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable`
- Offender: `docs/goals/migration.md`
- Reason: current player/tester-facing markdown still contains old display-name wording instead of `Spire Plus`.

`git diff --check` fails on:

- `docs/goals/debug.md:3`
- `docs/goals/debug.md:4`
- `docs/goals/event.md:3`
- `docs/goals/event.md:4`
- `docs/goals/event.md:723`

## Current State

- Branch: `main`
- HEAD: `24d4fe9a`
- Remote: `main...origin/main`, no ahead/behind shown in subagent audit
- Current dirty state after validation: 12 entries, all classified, 0 unclassified
- Untracked files: none reported by the dirty-state subagent before validation-generated status expansion

## Governance Recommendations

- Sts1Events: staging-only. Do not formalize until runtime gameplay, save/load, event art/images, warning/null-safety, and manual proof are resolved.
- Debug: accept-scaffold. It is not feature-complete; keep `Warn` for real risk/fallback messages only and treat `LogPreview` as dead/superseded by `PreviewLog` until removed or documented.
- RitsuLib: compile/manifest dependency attempted; runtime unverified. Do not call it runtime-validated or release-ready until loader evidence exists.
- Patch inventory: 142 raw Harmony declarations are documented alongside 25 migrated `IPatchMethod` classes; class-level double-patching is guarded, but runtime patcher behavior and target-level stacking remain unproven.

## Owner Decisions Needed

- Fix or reject the `docs/goals/migration.md` display-name wording change that currently fails tests.
- Fix or reject the whitespace-bearing edits in `docs/goals/debug.md` and `docs/goals/event.md`.
- Decide whether the four dirty M4 audit docs are retained, revised, or rolled back.
- Decide whether unauthorized commits `f4247553` and `aed2a498` are accepted with governance notation; later commits build on them.

No commit, push, stash, checkout, reset, restore, broad clean, feature expansion, high-risk patch migration, or runtime-readiness claim was made.
