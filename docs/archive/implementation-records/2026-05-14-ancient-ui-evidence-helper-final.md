Implemented the Ancient UI live-evidence helper without gameplay/package changes.

**Files changed**
- [scripts/collect-ancient-ui-evidence.ps1](D:/Game/FOTN/dev-the-spire/scripts/collect-ancient-ui-evidence.ps1)
- [scripts/README.md](D:/Game/FOTN/dev-the-spire/scripts/README.md)
- [docs/features/ancient-expansion-v2.2/manual-test-checklist.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/manual-test-checklist.md)
- [PROJECT_STATE.md](D:/Game/FOTN/dev-the-spire/PROJECT_STATE.md)
- [docs/issues.md](D:/Game/FOTN/dev-the-spire/docs/issues.md)
- [tests/EZMicroBalance.Tests/AncientUiReadinessGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/AncientUiReadinessGuardTests.cs)

**Helper behavior**
`collect-ancient-ui-evidence.ps1` supports `Prepare` and `Restore`. Prepare creates a timestamped `.tools/runtime-evidence/ancient-ui-click-<ancient>-<timestamp>` folder by default, writes `ancient-ui-evidence-plan.json` and `manual-instructions.md`, runs window preflight unless `-NoPreflight` is used, and does not launch unless `-Launch` is explicit. With `-Launch`, it sets the force env vars for the child process and calls `spire-plus-live-session.ps1`.

Restore calls `spire-plus-live-session.ps1 -Mode Restore` with the evidence dir, stops/restores via the live-session helper, preserves new current runs, and appends a restore note.

Vakuu counts are documented from current source: normal single-player fight-enabled screen expects 4 options, disabled/ineligible expects 3, and `-ForceVakuuFight` expects 1 focused fight option.

**Validation**
- `git status --short --branch` run before and after.
- No-launch helper syntax smoke passed with `-NoPreflight`; no game launch.
- `dotnet build EZMicroBalance.sln --no-restore` passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build` passed: 115 passed, 18 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed.
- `git diff --check` passed with existing CRLF normalization warnings only.
- No `dotnet publish` run.

No live game, clicked UI, co-op, save-load, gameplay, failure/death-path, or release-readiness validation was performed. The clicked Ancient UI issue remains pending until screenshots/logs are captured.