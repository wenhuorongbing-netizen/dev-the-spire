Implemented Vakuu Temptation as real gameplay.

**Subagents / Review**
No subagents used. I did three self-review passes: source/API safety, docs/tests coverage, and release/package validation.

**Behavior**
Temptation is `EZMB_VAKUU_TEMPTATION`: hidden Status card, cost `-1`, Ethereal + Unplayable, not shown in the card library, not normally generated in combat, and not modifier-generatable. On exhaust, it grants 1 Energy and deals 3 HP loss via source-backed commands.

Vakuu fight injection is deterministic: only in `EzmbVakuuTrialEncounter`, single-player gated, after normal player-turn draw on turns 1, 3, 5, and onward. Each injection adds one Temptation to the top of the draw pile and logs the action.

Art is temporary: `OPENAI_API_KEY` was absent, so Temptation uses the existing generic custom card portrait. No official assets were copied. Final bespoke Temptation art remains pending.

**Files Changed**
Core: `MainFile.cs`, `AncientCardHelpers.cs`, new `VakuuTemptationCard.cs`, new `VakuuFightRunHook.cs`.

Localization/docs/tests: updated EN/ZHS cards, Vakuu option/relic text, v2.2 docs/issues/manual checklist/work log, changelog, release/hash docs, completion audits, and added `docs/archive/implementation-records/2026-05-13-vakuu-temptation-implementation-final.md`. Added `VakuuTemptationGuardTests.cs` and adjusted stale coverage guards.

**Validation**
- `git status --short --branch`: `main...origin/main`, broad pre-existing dirty worktree remains.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped / 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed; known nested `source code/project.godot` warning only.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped / 0 failed.
- `scripts/package-spire-plus.ps1`: refreshed `publish/SpirePlus-v0.1.0-private-beta.0.zip`.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 127 passed / 0 skipped / 0 failed.

**Not Performed**
No live game launch, in-combat Vakuu verification, save-load verification, failure/death-path testing, or co-op verification was performed.

**Remaining Blockers**
Live Vakuu fight/manual verification, Temptation exhaust verification in-game, save-load restore after child combat, death/failure behavior, co-op disposition, and final bespoke art.