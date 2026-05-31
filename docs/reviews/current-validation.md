# Current Validation

Date: 2026-05-31

## Repository State

- HEAD: `24d4fe9a (HEAD -> main, origin/main, origin/HEAD) ci: regenerate patch inventory consistently`
- Branch: `main...origin/main`
- Worktree: dirty before and after this pass; existing source, Sts1Events, migration-goal, package-hash, harness, and test edits were preserved. This continuation refreshed validation/runtime evidence, cleared stale `testhost` processes, stabilized the current guard run, and did not attempt to normalize unrelated dirty files.

## Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean EZMicroBalance.sln -m:1` | PASS after retry | First clean completed with exit code 0 but emitted 17 `MSB3061` locked-file warnings from stale `testhost` PID `10932`; after stopping the process, the rerun completed with 0 warnings and 0 errors. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 warnings. Warnings are existing Sts1Events nullable warnings (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln` | PASS after retry | Earlier runs aborted with `Test host process crashed`; the final rerun exited `0` with 461 passed, 0 failed, 21 skipped, 482 total after guard stabilization. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | Final rerun exited `0` with 461 passed, 0 failed, 21 skipped, 482 total after guard stabilization. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | Exited `0`; no formatting changes required. |
| `git diff --check` | PASS | Exited `0`; no whitespace errors. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## Validation Fixes Applied

- Replayed clean/build/test/format/diff-check on the current `24d4fe9a` HEAD after doc and test-guard reconciliation.
- Removed the trailing whitespace that blocked `git diff --check` in `docs/goals/debug.md`, `docs/goals/event.md`, and `docs/features/sts1-events/multiplayer-fail-closed-guard.md`.
- Diagnosed stale `testhost` interference: initial clean had locked-file warnings and the first full test run aborted. Those partial results are rejected as validation truth.
- Stopped stale `testhost` PIDs `10932`, `17460`, `36700`, and final lingering PID `12588` during validation cleanup.
- Fixed the current `docs/issues.md` compactness guard regression and updated its guard to assert the active dirty-worktree truth instead of the obsolete beta.84 clean-worktree phrase.
- Diagnosed default test-host instability after the guard update. Focused guard classes and a full no-parallel run passed, so the test assembly now disables parallelization by default; normal `dotnet test EZMicroBalance.sln` and `dotnet test EZMicroBalance.sln --no-build` both pass.
- Final full and `--no-build` test runs both passed with 461/0/21/482.
- Ran independent QA/Red-Team review. Verdict: FAIL / HARD BLOCKED because runtime proof is absent; Green Stop is not allowed.
- Fixed QA-flagged stale wording in `docs/goals/refactor.md` and `docs/goals/event.md` without changing the runtime hard-block decision.
- Reconciled active RitsuLib/Sts1Events validation docs and doc guard tests to the 461/0/21/482 test count and current runtime dependency blocker.
- Preserved existing dirty source, harness, package-hash, and goal-doc edits that were outside this continuation.

## Warning Truth

- Current clean build warning count: 89.
- Warning codes: `CS8602`, `CS8604`, `CS8625`.
- Scope: all warnings are in `EZMicroBalanceCode/Sts1Events/Models/` staging code.
- Decision: warnings are issue-worthy and remain accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes.

## Runtime Smoke

- Status: BLOCKED.
- Local checks: `E:\Steam\steamapps\common\Slay the Spire 2` and `E:\Steam\steamapps\common\Slay the Spire 2\mods` returned `True`; E-drive `BaseLib` and `EZMicroBalance` returned `True`; E-drive `STS2-RitsuLib` returned `False`. The D-drive game root, `mods` folder, `BaseLib`, `STS2-RitsuLib`, and `EZMicroBalance` checks returned `False`.
- Decision: Hard Block Stop. Batch 4c remains blocked. Off mode `0` registrations, CanaryOnly `4` registrations, runtime safety, and release-readiness are not claimed.

## Independent QA

- Subagent QA/Red-Team verdict: FAIL / HARD BLOCKED.
- QA-confirmed blocker: no STS2-RitsuLib install, no active `godot.log`, no Off=0 runtime proof, and no CanaryOnly=4 runtime proof.
- QA fixes applied: current docs no longer say all runtime path checks failed, the StS1 guard-test score row uses 28/28 instead of stale 24/24, and Default Off is labeled source-level/runtime-blocked instead of green/amber.
- Stop decision: Hard Block Stop remains required; Green Stop is not allowed.

## Architecture Status

- RewardPipeline diagnostics are wired into `FeatureRegistry` bootstrap events and the low-risk `AscensionRewardService` reward/card-reward surfaces as no-mutation diagnostics.
- `ArchitectureCanaryBootstrap` registers FeatureRegistry and Ascension reward diagnostic handlers, a no-op DeathProtection provider, and multiplayer policy records for preview tools, Ancients, Ascension, reward surfaces, and combat hooks.
- Lotha extra-play paths touch `CardPlayContextCanary` through a single-depth adapter that returns `Allow`; play counts and gameplay branches are unchanged.
- Existing co-op gates still make the same allow/disable decisions; their evidence payloads now include policy registration/category/env/verification metadata.
- Guard coverage was added for reward-surface diagnostics, multiplayer policy metadata, no-op DeathProtection registration, architecture wiring, multiplayer policy records, and source-manifest coverage.
