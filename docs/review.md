# Source-Only Review: 2026-05-17 Final Test-Ready Loop

Scope: current final worktree state in `D:\Game\FOTN\dev-the-spire`.

Mode: source/package review only. The game was not launched.

## Conclusion

No new source/package blocker was found.

Nothing from this review needs to be moved into `docs/issues.md`. The remaining open work is correctly framed as manual verification: clicked Ancient UI, live gameplay, save/load, Vakuu victory/failure/death paths, co-op, and broader feature matrix proof.

## Reviewed User-Feedback Slice

### Urda Root Eyes

Source review supports the requested redesign:

- Clicking the Root Eyes relic starts map selection instead of auto-marking rooms.
- Map clicks are intercepted only while Root Eyes selection is active.
- Valid targets are current reachable Monster, Unknown, and Elite nodes.
- Campfires, shops, treasure, and bosses are excluded.
- The chosen node stores a concrete preview, and room-entry patches consume that stored result.

Manual retest still needs to confirm hover text, click targeting, and the actual room entered.

### Urda Seed Bank

Source review supports the requested interactive relic behavior:

- Boss-entry settlement is no longer the active path.
- The relic shows stored-card count and card hovers.
- Clicking the relic opens extraction only when stored cards exist and the relic is not used up.
- Extraction allows up to 2 cards, upgrades the first selected card, clears storage, and disables the relic.

Manual retest still needs to confirm relic-click extraction and Boss transition behavior.

### Morvi Text And State

Source and localization now match the feedback:

- Overdue Library uses card/power variables that match its text.
- Temporary archive powers use valid Morvi icons.
- Blueprint Proof initializes at combat start and has late guards before cost/play consumption.
- Misprint Press text matches the first eligible manual natural-deck Attack/Skill replay behavior.
- Forbidden Loan and Debt Settlement text now explains the concrete card/gold/debt values.

Runtime restore behavior for Morvi state remains a manual save/load gate.

### Vakuu Documentation Evidence

Vakuu source and docs now line up with the save-risk stance:

- Fight Vakuu remains hidden unless explicit enable/force gates are set.
- Combat entry uses direct `EnterRoomWithoutExitingCurrentRoom(...)`.
- It does not call Core's rejected non-shared `EnterCombatWithoutExitingEvent(...)` path.
- The active fight no longer assigns `ParentEventId`.
- The active combat room does not store `ParentEventId` while active.
- Parent id is written only for prefinished serialization.

This removes the previous source/doc mismatch. It does not prove live victory return or save/load safety.

## Package Script Review

`scripts/package-spire-plus.ps1` now creates deterministic zip files:

- It writes entries with `System.IO.Compression.ZipArchive`.
- Entries are sorted by file name.
- Entry timestamps are fixed.
- Entry names remain `EZMicroBalance/<file>`.
- The zip contains only `EZMicroBalance.dll`, `EZMicroBalance.json`, `EZMicroBalance.pck`, and `README_INSTALL.txt`.

The produced zip hash stayed stable across two consecutive package runs:

`EA0EC3611DC21FD33C9B87E592326A9000ECE593512554D720843D7490CC589C`

Artifact tests also verify installed/staging/versioned/zip parity and docs hash claims.

## Docs Review

`docs/issues.md` is compact and keeps remaining work as manual proof gates. It no longer reads like a source implementation backlog.

`docs/toreview.md` lists implemented or researched items that need user retest: Root Eyes, Seed Bank, Morvi fixes, Vakuu gate/evidence, and the no-new-blockers source review note.

## Validation

These checks support source/package test-readiness only:

- `dotnet build EZMicroBalance.sln`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 171 passed, 18 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with line-ending warnings only.
- `dotnet publish EZMicroBalance.sln`: passed.
- `.\scripts\package-spire-plus.ps1`: passed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 189 passed, 0 skipped, 0 failed.
- Deterministic zip replay: passed, same SHA256 on consecutive runs.

These checks do not prove live gameplay, clicked UI, save/load, failure/death, or co-op behavior. The docs correctly leave those as pending manual gates.
