# Revision H Commit Slice Plan

Date: 2026-05-31T03:50:00+02:00
Status: Planning only. No commit authorized or made.

## Current Gate

Do not commit current dirty state as-is. Validation is failing and several owner decisions are unresolved.

## Proposed Slices After Blockers Are Resolved

1. Guarded-doc restoration or retirement decision.
   - Files: `docs/goal.md`, `docs/migration.md`, related tests if retirement is intentional.
   - Current risk: tests fail because both active docs are deleted.

2. Architecture canary source/test alignment.
   - Files: `EZMicroBalanceCode/Core/Architecture/ArchitectureCanaryBootstrap.cs`, `CardPlayContext.cs`, `FeatureRegistry.cs`, `SpirePlusFeatureRegistry.cs`, architecture/engineering guard tests.
   - Current risk: untracked source file is not reflected in active source manifest expectations.

3. Sts1Events registration and event-option fixes.
   - Files: `EZMicroBalanceCode/Sts1Events/**`, Sts1Events guard tests, Sts1Events docs.
   - Current risk: enum guard expectations fail; runtime remains unverified.

4. RitsuLib/runtime-truth docs.
   - Files: `docs/features/ritsulib-migration/**`, `harness/TASK_STATUS.md`, `harness/TASK_FOCUS_PACK.md`.
   - Current risk: must not claim runtime verification.

5. Goal/audit ledger cleanup.
   - Files: `docs/goals/debug.md`, `event.md`, `migration.md`, `refactor.md`, overnight ledgers, Revision G/H audit docs.
   - Current risk: large doc churn, trailing whitespace in `event.md`, stale counts mixed across Revision G/H.

6. Package hash/site metadata refresh or explicit deferral.
   - Files: release hash docs, website content, package evidence docs/tests as needed.
   - Current risk: package/site hash guards fail against current expectations.

## Commit Preconditions

- `dotnet build .\EZMicroBalance.csproj` exits 0.
- `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` exits 0 or owner explicitly accepts a documented failing test gate.
- `dotnet format .\EZMicroBalance.csproj --verify-no-changes` exits 0.
- `git diff --check` exits 0.
- `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` exits 0.
- Owner explicitly authorizes commits.
