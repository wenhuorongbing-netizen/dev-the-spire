# Current Validation

Date: 2026-05-31

## Revision I Current Snapshot

- HEAD: `87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1`
- Branch: `main...origin/main`
- Worktree: dirty before this pass and still dirty. Current-state reconciliation preserves existing source/test/goal-doc edits and adds owner-review artifacts; no commit, push, stash, checkout, reset, restore, or broad clean was performed.
- Runtime smoke: hard blocked because `STS2-RitsuLib` is missing at checked D/E game-root mod paths and no active `godot.log` exists.

## Revision I Required Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean .\EZMicroBalance.csproj` | PASS | 0 warnings, 0 errors. |
| `dotnet build .\EZMicroBalance.csproj` | PASS | 0 errors, 89 warnings. Warnings remain Sts1Events nullable staging debt (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | Current rerun passed with 464 passed, 0 failed, 21 skipped, 485 total. Earlier plain no-build attempts intermittently aborted with a test-host crash and no assertion failures; the single-worker VSTest command remains a fallback if the crash recurs. |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | PASS | Final rerun reported 55 dirty entries, 0 unclassified. |
| `.\scripts\generate-patch-inventory.ps1 -Check` | PASS | Final check reported `docs/patch-inventory.md` is fresh. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## StS1 Unsafe-Gate Continuation Validation

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet test --filter Sts1EventFeatureGuardTests` | PASS | 31 passed, 0 failed, 0 skipped after adding unsafe-mode and replacement fail-closed guards. |
| `dotnet test --filter PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable` | PASS | 1 passed; active player-facing markdown naming guard is green. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS after clearing stale testhost locks | Final rerun passed with 0 errors and 89 Sts1Events nullable warnings. Earlier attempts failed only because stale `testhost` processes locked `EZMicroBalance.Tests.dll`. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS after retry | Latest default no-build rerun passed with 464 passed, 0 failed, 21 skipped, 485 total after stale testhost locks were absent. Earlier normal reruns intermittently aborted with the known testhost crash and no assertion failures; `RunConfiguration.MaxCpuCount=1` remains the documented fallback if needed. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors; PowerShell emitted a CRLF normalization warning for existing `docs/patch-inventory.md`. |

## Revision I Runtime Path Check

| Path | Exists |
| --- | --- |
| `E:\Steam\steamapps\common\Slay the Spire 2` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | False |
| `D:\Steam\steamapps\common\Slay the Spire 2` | False |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | False |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | False |

## Revision I Stop Decision

- Status: NOT COMPLETE / HARD BLOCKED.
- Owner-review packet: prepared for review; no owner approval recorded.
- Commit readiness: not complete because the worktree is dirty and runtime smoke is blocked.
- Batch 4c: remains blocked until STS2-RitsuLib install plus loader smoke passes.
- Release-ready: no.

## Governance Closure Validation Snapshot (Supersedes 24d4fe9a)

- HEAD: `87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1`
- Branch: `main...origin/main`
- Worktree: dirty before and after this pass; existing goal-doc edits were preserved. This continuation fixed the Sts1Events mode bootstrap override, refreshed runtime prerequisite evidence, and did not attempt to normalize unrelated dirty files.

## Prior M4 Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean EZMicroBalance.sln -m:1` | PASS | Exited `0`; clean completed with 0 warnings and 0 errors. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 warnings. Warnings are existing Sts1Events nullable warnings (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | Exited `0`; no formatting changes required. |
| `git diff --check` | PASS | Exited `0`; no whitespace errors. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## Validation Fixes Applied

- Replayed clean/build/test/format/diff-check on the current `87820303` HEAD after doc and test-guard reconciliation.
- Removed the trailing whitespace that previously blocked `git diff --check` in goal/status docs.
- Cleared stale `testhost` locks before final validation so clean/build/test could rebuild the current source and test assembly.
- Fixed the current `docs/issues.md` compactness guard regression and updated its guard to assert the active dirty-worktree truth instead of the obsolete beta.84 clean-worktree phrase.
- Kept the test assembly serialized by default to reduce prior default test-host instability; the latest default `dotnet test EZMicroBalance.sln --no-build` rerun passes, while earlier crash logs remain historical/intermittent evidence rather than assertion failures.
- Earlier full and `--no-build` test runs both passed with 462/0/21/483; the final current project no-build rerun passed with 464/0/21/485.
- Ran independent QA/Red-Team review. Verdict: FAIL / HARD BLOCKED because runtime proof is absent; Green Stop is not allowed.
- Fixed QA-flagged stale wording in `docs/goals/refactor.md` and `docs/goals/event.md` without changing the runtime hard-block decision.
- Reconciled active RitsuLib/Sts1Events validation docs and doc guard tests to the 464/0/21/485 project no-build test count and current runtime dependency blocker.
- Fixed `Sts1EventsFeatureModule` so `SPIREPLUS_STS1_EVENT_MODE` is no longer treated as a generic FeatureRegistry disable override; CanaryOnly/AdditiveBatch1 can now reach `Sts1EventFeatureGate` in the source-level bootstrap path.
- Tightened StS1 unsafe modes so AdditiveAllDraft requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, and ReplaceUnknownEventsPrototype reports disabled/fail-closed in normal builds unless `REPLACEMENT_PROTOTYPE_ENABLED` and the unsafe override are present.
- Added independent refactor QA report at `docs/reviews/refactor-overnight-qa-20260531.md`; verdict is FAIL / HARD BLOCKED until runtime evidence exists.
- Added StS1 v14 hard-stop report at `docs/features/sts1-events/hard-stop-blocker-report-v14.md`; blocked runtime gates remain open.
- Preserved existing goal-doc edits that were outside this continuation.

## Warning Truth

- Current clean build warning count: 89.
- Warning codes: `CS8602`, `CS8604`, `CS8625`.
- Scope: all warnings are in `EZMicroBalanceCode/Sts1Events/Models/` staging code.
- Decision: warnings are issue-worthy and remain accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes.

## Runtime Smoke

- Status: BLOCKED.
- Local checks: `E:\Steam\steamapps\common\Slay the Spire 2` and `E:\Steam\steamapps\common\Slay the Spire 2\mods` returned `True`; E-drive `BaseLib` and `EZMicroBalance` returned `True`; E-drive `STS2-RitsuLib` returned `False`. The D-drive game root, `mods` folder, `BaseLib`, `STS2-RitsuLib`, and `EZMicroBalance` checks returned `False`. Latest evidence: `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt`.
- Decision: Hard Block Stop. Batch 4c remains blocked. Off mode `0` registrations, CanaryOnly `4` registrations, runtime safety, and release-readiness are not claimed.

## Independent QA

- Subagent QA/Red-Team verdict: FAIL / HARD BLOCKED.
- QA-confirmed blocker: no STS2-RitsuLib install, no active `godot.log`, no Off=0 runtime proof, and no CanaryOnly=4 runtime proof.
- QA fixes applied: current docs no longer say all runtime path checks failed, the StS1 guard-test score row uses current source guards instead of stale 24/24, and Default Off is labeled source-level/runtime-blocked instead of green/amber.
- Stop decision: Hard Block Stop remains required; Green Stop is not allowed.

## Architecture Status

- RewardPipeline diagnostics are wired into `FeatureRegistry` bootstrap events and the low-risk `AscensionRewardService` reward/card-reward surfaces as no-mutation diagnostics.
- `ArchitectureCanaryBootstrap` registers FeatureRegistry and Ascension reward diagnostic handlers, a no-op DeathProtection provider, and multiplayer policy records for preview tools, Ancients, Ascension, reward surfaces, and combat hooks.
- Lotha extra-play paths touch `CardPlayContextCanary` through a single-depth adapter that returns `Allow`; play counts and gameplay branches are unchanged.
- Existing co-op gates still make the same allow/disable decisions; their evidence payloads now include policy registration/category/env/verification metadata.
- Guard coverage was added for reward-surface diagnostics, multiplayer policy metadata, no-op DeathProtection registration, architecture wiring, multiplayer policy records, and source-manifest coverage.
