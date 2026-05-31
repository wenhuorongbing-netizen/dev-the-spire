# Revision G Owner-Review Packet — M4 Run

Date: 2026-05-31T16:05:00+02:00
HEAD: `24d4fe9a`

## Status

Not ready for owner-review completion. This packet documents the hard blocker instead.

## Current Dirty Files

Before validation-generated status expansion, the dirty-state subagent found exactly four modified tracked docs and no untracked files:

| File | Classification | Decision Needed |
|---|---|---|
| `docs/goals/debug.md` | M4 owner-review governance spec rewrite | Accept, revise, or rollback; remove trailing whitespace first |
| `docs/goals/event.md` | StS1 Event Port audit rewrite | Accept, revise, or rollback; remove trailing whitespace first |
| `docs/goals/migration.md` | RitsuLib migration/architecture audit rewrite | Accept, revise, or rollback; fix old display-name wording first |
| `docs/goals/refactor.md` | Refactor/architecture hardening audit rewrite | Accept, revise, or rollback |

After validation commands, the batch classifier reported 12 dirty entries and 0 unclassified entries, including issue/status docs and Sts1Events docs already present in the broader dirty state.

## Validation Gate

Validation is blocked:

- Build passes with 89 warnings.
- Tests fail due to `docs/goals/migration.md` display-name wording.
- Diff whitespace check fails in `docs/goals/debug.md` and `docs/goals/event.md`.
- Batch classifier passes with 0 unclassified entries.

## Commit Guidance

Do not commit current state. The minimum preconditions for any owner-authorized commit are:

- `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` exits 0.
- `git diff --check` exits 0.
- Owner explicitly accepts the audit-doc scope and the process notation for prior unauthorized commits.

## Runtime Truth

- No runtime smoke was performed in this run.
- RitsuLib remains runtime-unverified.
- Sts1Events remains staging-only and runtime-unverified.
- No release-ready claim is supported.
