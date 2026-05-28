# Harness - Live Task State

This directory holds current-task state only. It is **not** a place for
long-term project facts.

Templates for resetting these files live in `docs/codex-harness/templates/`.

## Purpose

Reduce irrelevant context and save tokens by keeping task-scoped state in
`harness/` at the project root, while long-term project facts remain in the
canonical locations below.

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

## Directory layout

```text
docs/codex-harness/          <- templates (reference only, not live state)
  README.md                  <- thin pointer to this file
  PROMPTS.md                 <- ready-to-copy task prompt templates
  templates/
    HCA_PROJECT_MAP.md
    TASK_FOCUS_PACK.md
    TASK_STATUS.md
    TASK_RESULT.md
    ERROR_LEDGER.md

harness/                     <- live task-scoped state (project root)
  README.md                  <- this file
  TASK_FOCUS_PACK.md
  TASK_STATUS.md
  TASK_RESULT.md
  ERROR_LEDGER.md
```

## Rules

- Before each task, check `ERROR_LEDGER.md` for related past mistakes.
- After each task, update `TASK_STATUS.md` and `TASK_RESULT.md`.
- Do not put long-term project facts here.
- Do not commit chat history, accounts, keys, or private paths.
- Do not copy the codex-harness AGENTS.md over the root AGENTS.md. The root
  AGENTS.md already contains StS2, BaseLib, manifest, source evidence, and
  release validation hard rules.
