# Harness - Live Task State

This directory holds current-task state only. It is **not** a place for
long-term project facts.

## Canonical long-term facts

| File | Purpose |
| --- | --- |
| `PROJECT_STATE.md` | Current status, blockers, next action |
| `docs/README.md` | Documentation index |
| `docs/PROJECT_MAP.md` | Active/support/archive path map |
| `docs/issues.md` | Active blocker and next-development issue index |
| `docs/codex-workflow.md` | Repeatable Codex session workflow |
| `docs/features/*/README.md` | Per-feature status and doc map |

## Files

| File | Purpose |
| --- | --- |
| `TASK_FOCUS_PACK.md` | Current task context (only task-relevant info) |
| `TASK_STATUS.md` | Short status for progress recovery |
| `TASK_RESULT.md` | Task result evidence pack |
| `ERROR_LEDGER.md` | Error recurrence prevention ledger |

## Rules

- Before each task, check `ERROR_LEDGER.md` for related past mistakes.
- After each task, update `TASK_STATUS.md` and `TASK_RESULT.md`.
- Do not put long-term project facts here.
- Do not commit chat history, accounts, keys, or private paths.
