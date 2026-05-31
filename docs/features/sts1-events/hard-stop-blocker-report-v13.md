# StS1 Event Port v13 Hard Stop Blocker Report

Date: 2026-05-31
Scope: Mandatory Overnight Run v13 for `docs/goals/event.md`

## Evidence Paths

- O0 snapshot: `.tools/runtime-evidence/sts1-events-v13/o0-*`
- O1 clean/build logs: `.tools/runtime-evidence/sts1-events-v13/o1-clean-full.log`, `.tools/runtime-evidence/sts1-events-v13/o1-build-full.log`
- O2 full test log: `.tools/runtime-evidence/sts1-events-v13/o2-test-full.log`
- O3/O4 count and skip explanation: `.tools/runtime-evidence/sts1-events-v13/o3-o4-test-count-and-skips.md`
- O8-O16 focused StS1 guard log: `.tools/runtime-evidence/sts1-events-v13/o8-o16-sts1-guard-tests.log`
- Final validation rerun logs after doc/test guard stabilization: `.tools/runtime-evidence/sts1-events-v13/final-clean-after-parallel-fix.log`, `.tools/runtime-evidence/sts1-events-v13/final-build-after-parallel-fix.log`, `.tools/runtime-evidence/sts1-events-v13/final-full-test-after-parallel-fix.log`, `.tools/runtime-evidence/sts1-events-v13/final-test-after-parallel-fix-rerun.log`, `.tools/runtime-evidence/sts1-events-v13/final-format-after-parallel-fix.log`, `.tools/runtime-evidence/sts1-events-v13/final-diff-check-after-parallel-fix.log`
- Runtime prerequisite check: `.tools/runtime-evidence/sts1-events-v13/runtime-prereq-paths.txt`

## Current Source-Side Evidence

- O1 build: `dotnet clean EZMicroBalance.sln -m:1` and `dotnet build EZMicroBalance.sln -m:1` completed with 0 errors and 89 tracked Sts1Events nullable warnings in the clean build.
- O2/O3 tests: `dotnet test EZMicroBalance.sln --no-build` passed with `461 passed + 0 failed + 21 skipped = 482 total` after v13 source/doc guard reconciliation.
- Final command-layer rerun: clean, build, full tests, normal no-build tests, format, and `git diff --check` all passed after compactness guard and test-harness stabilization.
- O4 skipped tests: 21 skipped tests are release-artifact, installed-package, or runtime-evidence guards; they do not prove live gameplay.
- O8-O16 source guards: focused `Sts1EventFeatureGuardTests` passed `28/28`.
- O10-O14 source-mode evidence: Off defaults to 0 registrations; CanaryOnly is exactly Big Fish, Golden Idol, The Lab, and Divine Fountain; AdditiveBatch1 is 10 event types through 11 registration calls; AdditiveAllDraft is dev-only; `ReplaceUnknownEventsPrototype` is debug-symbol gated.
- O7 matrix red-team: independent audit found the matrix mostly consistent but required v13 fixes for the 52 baseline explanation and stale Joust / The Ssssserpent combat classification. Those source/docs corrections were applied.

## Runtime Prerequisite Check

```text
D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib: False
D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib: False
D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance: False
E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib: False
E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib: True
E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance: True
```

## Non-Green Gates / Blockers

| Gate | Status | Blocker | Required owner/external action |
| --- | --- | --- | --- |
| O18-O22 | BLOCKED | Canary runtime screenshots, result logs, save/load, EN/ZHS render, and image/render proof require live game execution. | Install STS2-RitsuLib, launch with `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`, capture screenshots/logs/save-load/render evidence. |
| O25-O30 | BLOCKED | AdditiveBatch1 simple-batch runtime/result/save-load/render/image proof requires live game execution and art/license decisions. | Launch with `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1`; capture Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, and Shining Light evidence; provide art/license strategy. |
| O32-O35 | BLOCKED | `ReplaceUnknownEventsPrototype` functional proof requires debug symbol, game launch, seeded unknown-room proof, event-bag/no-repeat proof, and save/load proof. | Compile with `REPLACEMENT_PROTOTYPE_ENABLED`, launch with `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype`, and capture seeded runtime/save-load evidence. |
| O36 | BLOCKED | Multiplayer fail-closed or verified behavior requires a runtime multiplayer session. | Run multiplayer smoke or two-client proof and capture logs. |
| O40 | FAIL / BLOCKED | Independent QA/Red-Team ran and returned fail because runtime gates are missing. | Rerun QA after runtime evidence exists. |
| O42-O46 | BLOCKED | Handoff/next-run closure cannot be green while runtime, art, replacement, multiplayer, and QA-pass gates remain open. | Start the next run from the unresolved gates above; do not expand to broad Phase 2 content. |

## Hard Stop Reason

Continuation cannot make O0-O46 all green in the current environment because STS2-RitsuLib is not installed at either checked game-root mod path and live gameplay, save-load, image/render, replacement-pool, multiplayer, and QA-pass evidence require game execution or owner/external action.

## Attempted Actions

- Captured O0 git snapshot.
- Captured O1 clean/build evidence and O2 full-test evidence.
- Reconciled test count truth and skipped-test meaning.
- Ran independent matrix, feature-gate, and QA/Red-Team subagents.
- Fixed stale Joust / The Ssssserpent combat classification in source/docs and guarded the classification.
- Updated current Sts1Events status, registry reconciliation, blocker, and handoff docs without marking runtime gates complete.

## Next Command After Owner Action

After STS2-RitsuLib is installed, rerun loader smoke with only BaseLib, STS2-RitsuLib, and Spire Plus enabled, then capture Off and CanaryOnly `godot.log` evidence before attempting AdditiveBatch1 or replacement-pool proof.
