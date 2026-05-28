# Codex Harness — Spire Plus

This directory stores thin Codex workflow templates adapted from the
[codex-app-better-token](https://github.com/example/codex-app-better-token) harness.

## Purpose

Reduce irrelevant context and save tokens by keeping task-scoped state in
harness/ at the project root, while long-term project facts remain in the
canonical locations below.

## Canonical long-term facts

| File | Purpose |
| --- | --- |
| ../AGENTS.md | Hard rules, manifest constraints, release validation |
| ../PROJECT_STATE.md | Current status, blockers, next action |
| docs/README.md | Documentation index |
| docs/PROJECT_MAP.md | Active/support/archive path map |
| docs/issues.md | Active blocker and next-development issue index |
| docs/codex-workflow.md | Repeatable Codex session workflow |
| docs/worktree-cleanup-audit.md | Cleanup/refactor scope and archive decisions |
| docs/features/*/README.md | Per-feature status and doc map |

## Directory layout

```text
docs/codex-harness/          <- templates (reference only, not live state)
  README.md
  PROMPTS.md
  templates/
    TASK_FOCUS_PACK.md
    TASK_STATUS.md
    TASK_RESULT.md
    ERROR_LEDGER.md

harness/                     <- live task-scoped state (project root)
  README.md
  TASK_FOCUS_PACK.md
  TASK_STATUS.md
  TASK_RESULT.md
  ERROR_LEDGER.md
```

## Rules

- harness/ contains only current-task state. Do not put long-term project
  facts here; those live in the canonical locations above.
- Do not copy this zip's AGENTS.md over the root AGENTS.md. The root
  AGENTS.md already contains StS2, BaseLib, manifest, source evidence, and
  release validation hard rules.
- Before each task, check harness/ERROR_LEDGER.md for related past mistakes.
- After each task, update harness/TASK_STATUS.md and harness/TASK_RESULT.md.
- Do not commit chat history, accounts, keys, private paths, or secrets into
  any harness file.
