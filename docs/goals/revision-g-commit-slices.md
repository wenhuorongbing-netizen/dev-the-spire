# Revision G — Commit Slice Plan

Date: 2026-05-29T20:05:00+02:00
HEAD: `aed2a498` ("debug")
Dirty: 18 files (all modified tracked)
Total changes: ~3500 insertions, ~3000 deletions across 18 doc/config/test files

## Current Dirty Files (18)

| Batch | File | Category |
|---|---|---|
| 1 | `docs/issues.md` | Status doc |
| 2 | `harness/TASK_FOCUS_PACK.md` | Harness |
| 2 | `harness/TASK_STATUS.md` | Harness |
| 3 | `docs/features/sts1-events/o24-handoff.md` | Sts1Events |
| 3 | `docs/features/sts1-events/status-board.md` | Sts1Events |
| 5 | `docs/features/ritsulib-migration/monthly-dev-spec.md` | RitsuLib |
| 5 | `docs/features/ritsulib-migration/next-overnight-run.md` | RitsuLib |
| 5 | `tests/.../ArchitectureSkeletonGuardTests.cs` | Test |
| 5 | `tests/.../EZMicroBalance.Tests.csproj` | Test config |
| 8 | `docs/goals/debug.md` | Audit |
| 8 | `docs/goals/event.md` | Audit |
| 8 | `docs/goals/migration.md` | Audit |
| 8 | `docs/goals/overnight-diff-ledger.md` | Ledger |
| 8 | `docs/goals/overnight-run-ledger.md` | Ledger |
| 8 | `docs/goals/overnight-run-status.md` | Status |
| 8 | `docs/goals/refactor.md` | Refactor |
| 8 | `docs/goals/revision-f-final-report.md` | Report |
| 8 | `docs/goals/warning-ledger.md` | Ledger |

## Slice Recommendation: 4 Separate Commits

### Slice 1: Governance and Stale Count Fixes (10 files)

| File | Purpose |
|---|---|
| `docs/goals/overnight-run-status.md` | Updated HEAD, test count, dirty count, warning breakdown |
| `docs/goals/overnight-run-ledger.md` | Updated HEAD, test count, dirty count |
| `docs/goals/overnight-diff-ledger.md` | Rewritten with Revision G column, correct dirty count |
| `docs/goals/warning-ledger.md` | Resolved TBD: CS8604=57, CS8602=34, CS8625=1 |
| `docs/goals/revision-f-final-report.md` | Added superseded notice, resolved ZHS/TBD items |
| `harness/TASK_STATUS.md` | Updated test count 387→444 |
| `harness/TASK_FOCUS_PACK.md` | Updated test count 387→444 |
| `docs/features/sts1-events/status-board.md` | Updated warning count 0→92 |
| `docs/features/sts1-events/o24-handoff.md` | Updated warning count 0→92 |
| `docs/features/ritsulib-migration/next-overnight-run.md` | Updated HEAD d290598c→aed2a498 |

**Risk:** LOW. Doc-only count/status updates.
**Rollback:** `git revert HEAD`

### Slice 2: Event.md Encoding Fix (1 file)

| File | Purpose |
|---|---|
| `docs/goals/event.md` | Converted from GBK to UTF-8 encoding (mojibake fix) |

**Risk:** LOW. Encoding conversion, no content change.
**Rollback:** `git revert HEAD`
**Dependency:** Should land before Slice 3 (event.md content is referenced by debug.md).

### Slice 3: Audit Document Rewrites (3 files)

| File | Purpose |
|---|---|
| `docs/goals/debug.md` | M3 Revision G spec rewrite |
| `docs/goals/migration.md` | Migration review rewrite |
| `docs/goals/refactor.md` | Minor update |

**Risk:** LOW. Doc rewrites with stale references to parallel commit.
**Rollback:** `git revert HEAD`
**Dependency:** Should land after Slice 2 (event.md encoding must be fixed first).

### Slice 4: Remaining Docs (4 files)

| File | Purpose |
|---|---|
| `docs/issues.md` | Minor update |
| `docs/features/ritsulib-migration/monthly-dev-spec.md` | Count update |
| `tests/.../ArchitectureSkeletonGuardTests.cs` | Test assertion changes |
| `tests/.../EZMicroBalance.Tests.csproj` | Compile link additions |

**Risk:** LOW. Test/config changes, build-verified.
**Rollback:** `git revert HEAD`

## Commit Order

1. Slice 1 (governance fixes) — largest, most critical for accuracy
2. Slice 2 (encoding fix) — prerequisite for Slice 3
3. Slice 3 (audit rewrites) — depends on Slice 2
4. Slice 4 (remaining) — independent

## Authorization

**No commits will be made without explicit owner authorization.** This plan is prepared for owner review only.

## M4 Update — 2026-05-31

Current HEAD is `24d4fe9a`; the previous 18-file Revision G plan is historical. The current pre-validation dirty state was four Batch 8 audit docs:

| Slice | Files | Purpose | Precondition |
|---|---|---|---|
| 1 | `docs/goals/debug.md` | M4 owner-review governance spec | Remove trailing whitespace; owner accepts scope |
| 2 | `docs/goals/event.md` | StS1 Event Port audit rewrite | Remove trailing whitespace; owner accepts scope |
| 3 | `docs/goals/migration.md` | RitsuLib migration audit rewrite | Fix old display-name wording; owner accepts scope |
| 4 | `docs/goals/refactor.md` | Refactor/architecture hardening audit rewrite | Owner accepts scope |

Do not commit these slices as-is. Current validation fails tests and `git diff --check`.
