# Fixed Goal: DevSpire Longhaul One-File Audit

## Goal

Systematically audit every tracked file in the dev-the-spire repository. Process one file per round. Fix real bugs when found. Skip clean files. Never batch.

## Hard Constraints

1. Do NOT change `EZMicroBalance` manifest id / project name / install folder / DLL / PCK / saved-field namespace.
2. Do NOT implement features beyond Ascension 20 or custom characters.
3. Do NOT commit zip / dll / pck / .tools/ / publish/ / source code/ files.
4. Do NOT claim completion without source evidence, test, command, or diff proof.
5. Each round: exactly one current file. Touch related files only when fixing a real bug requires it.
6. No broad refactor. No opportunistic cleanup of unrelated files.
7. All validation must pass before marking a fix as done.

## Validation Commands

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

If resource/localization/manifest/export/package changed:

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

## Source Evidence

For Harmony patches targeting StS2 v0.106.1, compare against `source code/src/Core/**`. If local source is missing, mark blocked.

## Completion

A file is done when: audited, decision recorded (fixed/skipped/blocked), evidence written, queue updated.
