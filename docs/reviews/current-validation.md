# Current Validation

Date: 2026-05-31

## Repository State

- HEAD: `85a38dd1 (HEAD -> main, origin/main, origin/HEAD) architecture canary: add DeathProtectionService + MultiplayerPolicy behavioral tests, update test counts to 444/465`
- Branch: `main...origin/main`
- Worktree: dirty before this pass; existing StS1 event, migration-goal, package-hash, and harness edits were preserved and reconciled only where they blocked validation.

## Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean EZMicroBalance.sln; dotnet build EZMicroBalance.sln` | PASS | Clean build: 0 errors, 89 warnings. Warnings are existing Sts1Events nullable warnings (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | 452 passed, 0 failed, 21 skipped, 473 total. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## Validation Fixes Applied

- Restored compact `docs/goal.md` and `docs/migration.md` guards expected by source tests.
- Refreshed current package hash metadata in `docs/issues.md` and `website/content-data.js` to match the existing beta.84 package on disk.
- Removed stale player-facing shorthand from `docs/goals/migration.md` and `docs/goals/debug.md`.
- Removed trailing whitespace from `docs/goals/event.md`.
- Active source manifest coverage now includes `ArchitectureCanaryBootstrap.cs` from the prior architecture canary pass.

## Warning Truth

- Current clean build warning count: 89.
- Warning codes: `CS8602`, `CS8604`, `CS8625`.
- Scope: all warnings are in `EZMicroBalanceCode/Sts1Events/Models/` staging code.
- Decision: warnings are issue-worthy and remain accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes.

## Runtime Smoke

- Status: BLOCKED.
- Local checks: `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`, `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`, `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` do not exist.
- Decision: Batch 4c remains blocked. No runtime safety or release-readiness claim is made.

## Architecture Status

- RewardPipeline diagnostics are now wired into `FeatureRegistry` bootstrap events as no-mutation diagnostics.
- `ArchitectureCanaryBootstrap` registers one RewardPipeline diagnostic handler and multiplayer policy records for preview tools, Ancients, Ascension, and combat hooks.
- Lotha extra-play paths touch `CardPlayContextCanary` through a single-depth adapter that returns `Allow`; play counts and gameplay branches are unchanged.
- Guard coverage was added for architecture wiring, multiplayer policy records, and source-manifest coverage.
