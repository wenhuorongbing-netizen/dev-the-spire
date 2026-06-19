# M5 Revision N Validation Replay

Date: 2026-06-19
Status: Planned. Not run from this thread because same-repo validation and runtime process launches are paused.

## Coordination Rule

Do not start a validation lane while another same-repo migration validation lane is active. The current coordination note blocks new `dotnet build`, `dotnet test`, `dotnet format`, `dotnet publish`, package/release-evidence validation, game/runtime smoke, process cleanup, staging, commit, and push actions from this thread.

This document is the replay plan for the next single coordinated lane.

## Current Recorded Validation Truth

`PROJECT_STATE.md` and `docs/reviews/current-validation.md` record the latest beta.88 state:

- `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false`: passed, 0 warnings / 0 errors.
- `dotnet publish EZMicroBalance.sln -m:1`: passed with the known Godot ignored-folder warning.
- Package refresh produced `publish/SpirePlus-v0.1.0-private-beta.88.zip`.
- Installed beta.88 package parity passed.
- Runtime preflight passed 27 / 0.
- Retained beta.88 AdditiveBatch1 packet verification passed 62 / 0.
- Current-doc claims passed 1314 / 0 after the AutoSlay/runtime-monkey proof-mode current-target parameter, AutoSlay exact standard artifact path, runtime-monkey live-session child EvidenceDir, and summary prepare-output path/hash guard follow-up.
- Static suite passed 15 / 0 and static-file hygiene passed 12 / 0.
- Split no-build runtime-harness coverage passed 81 / 0 / 0 / 81.

Those results were not rerun in this pause-safe documentation pass.

## Replay Commands

Run only after coordination is clear:

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

If package/resource/localization files changed after beta.88, add:

```powershell
dotnet publish EZMicroBalance.sln -m:1
.\scripts\package-spire-plus.ps1 -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

## Pass Criteria

- No validation command fails.
- No solution-level VSTest/testhost instability is treated as green without a split-lane rerun that covers the same surface.
- `git diff --check`, patch inventory, and worktree batch classification pass.
- Version/package/hash docs remain aligned if packaging is refreshed.
- The final report records exact counts and commands.

## Failure Rule

If any command fails, stop and record:

- exact command;
- exit code;
- relevant log path;
- dirty files involved;
- whether the failure is source, package, harness, environment, or runner contamination;
- owner decision required.

Do not stage, commit, push, or claim readiness after a failed replay.
