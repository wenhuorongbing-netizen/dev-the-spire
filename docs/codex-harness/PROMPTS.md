# Codex Harness - Prompt Templates

## 1. Existing Project Adoption

```
This is an existing project. Adopt Codex Harness for it according to AGENTS.md.

Requirements:
1. First check whether the project is on the C drive.
2. Do not change business code.
3. If harness/ already exists, read and merge it without overwriting.
4. Read only README, dependency config, project structure, start/build/test config,
   and files I explicitly name for the current task.
5. Do not scan the whole project without a clear reason.
6. Fill TASK_FOCUS_PACK and TASK_STATUS for the current task.
7. Mark unconfirmed history, decisions, and completed work as "unconfirmed".
8. Record known mistakes, user prohibitions, and prior pitfalls in ERROR_LEDGER.
9. Report only updated files, confirmed modules, missing information, next
   suggestions, and current progress.
```

## 2. Every New Task

```
Follow AGENTS.md and harness/ rules for this task:

Task: [describe the task]
Requirements:
1. Check ERROR_LEDGER first to avoid repeated mistakes.
2. Update TASK_FOCUS_PACK with acceptance criteria, related files, out-of-scope
   areas, risks, and verification method.
3. Read only files relevant to this task.
4. Keep the change scope minimal and avoid unrelated refactors.
5. Verify the change with real evidence.
6. Update TASK_STATUS and TASK_RESULT.
7. Do not report completion without verification evidence.
```

## 3. Lightweight Task

```
Use a lightweight AGENTS.md flow for this task:

Task: [describe the task]
Requirements:
1. Define the goal.
2. Make the smallest necessary change.
3. Verify it.
4. Update TASK_STATUS.
5. Report the result and progress briefly.
```

## 4. Continue An In-Progress Task

```
Continue the current task according to AGENTS.md and harness/ rules.

Requirements:
1. Read TASK_FOCUS_PACK, TASK_STATUS, and ERROR_LEDGER first.
2. Do not depend on chat history. Prefer harness files for recovery.
3. If TASK_STATUS conflicts with actual files, stop and point out the mismatch.
4. Before continuing, define the goal, completed work, remaining work, next step,
   and verification method.
5. Continue with the smallest necessary change.
6. Update TASK_STATUS and TASK_RESULT when done.
```

## 5. Review A Task

```
Review this task according to AGENTS.md and harness/ rules.

Review scope: [describe task or change scope]
Requirements:
1. Read TASK_FOCUS_PACK, TASK_RESULT, and ERROR_LEDGER.
2. Inspect the actual diff and verification evidence.
3. Do not rely on verbal claims from the execution thread.
4. Check acceptance criteria, unrelated changes, verification evidence, new risks,
   and repeated ERROR_LEDGER mistakes.
5. Output pass/fail, evidence, required rework, and current progress.
```
