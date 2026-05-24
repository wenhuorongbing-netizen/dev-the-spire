# Git Commit And Push Policy

This repository should stay easy to resume. After a change is implemented and the relevant tests pass, commit it and push it to GitHub instead of leaving a long-lived dirty worktree.

## Required Rule

Every completed, tested work slice must end with:

1. Run the relevant validation commands.
2. Review `git status --short` and `git diff --stat`.
3. Create one focused Git commit for the tested slice.
4. Push the commit to GitHub.
5. Confirm `git status --short --branch` is clean and not ahead of the remote.

Do not commit or push if validation failed. Fix the failure first, rerun the validation, then commit.

## Default Validation

Use the smallest validation set that covers the changed surface, but keep the standard lanes explicit:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

After resource, localization, package, or release-hash changes, also run:

```powershell
dotnet publish EZMicroBalance.sln
./scripts/package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
$testExit = $LASTEXITCODE
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
exit $testExit
```

If the package has been copied to the local game install, verify it:

```powershell
./scripts/check-installed-spire-plus-package.ps1 -ModDirectory "D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance"
```

## Commit Discipline

- Keep each commit about one tested work slice.
- Use a clear message, for example `Add Urda elite victory heal relic`.
- Include source, localization, tests, scripts, and current docs that are needed to explain the change.
- Do not commit local machine files, ignored build output, `.tools/`, `.godot/`, `bin/`, `obj/`, or copied game/runtime files.
- Do not use `git reset --hard`, `git checkout --`, or force push to hide dirty state.

## Push Rule

After the commit succeeds, push the current branch:

```powershell
git push origin HEAD
```

If the push is rejected because the remote changed, fetch and inspect before doing anything else:

```powershell
git fetch origin
git status --short --branch
git log --oneline --decorate --left-right --graph HEAD...origin/main
```

Resolve the divergence deliberately. Do not force push unless the project owner explicitly asks for it.

## Final Cleanliness Check

Before ending a development pass, run:

```powershell
git status --short --branch
```

Expected result after push:

```text
## main...origin/main
```

Any remaining modified or untracked files must be explained, committed, archived, ignored, or intentionally left for a documented reason.
