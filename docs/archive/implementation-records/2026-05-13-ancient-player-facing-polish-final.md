Completed the player-facing Ancient polish/source-hardening pass.

Subagents used:
- Beauvoir: dialogue/localization review.
- Volta: art/export/provenance review.
- Hooke: Vakuu fight/source-risk review.

What changed:
- Replaced active empty Ancient dialogue declarations in Urda/Morvi/Lotha with fallback SFX path usage and localization-backed visible dialogue.
- Rewrote active EN/ZHS Ancient option, relic, power, and custom card text for readability and removed source/testing jargon.
- Tightened rich-text/highlight handling and removed duplicated canonical Exhaust/Ethereal/Unplayable text from custom card bodies.
- Hardened Vakuu fight: awaited room transition, clearer fight/death/no-normal-reward wording, retained single-player gate, no `ExtraRewards`, and fallback continue option if three non-Vakuu Act 3 blessings cannot be produced.
- Added guard tests for empty dialogue, bilingual parity, raw token/mojibake leaks, banned dev terms, critical text/highlight coverage, Vakuu reward/fight shape, and temporary-art truthfulness.
- Refreshed package artifacts and hash docs.

Art status:
- Final generated images this pass: none.
- `OPENAI_API_KEY` was not set, so no Image API generation was claimed.
- Morvi/Lotha event backgrounds remain local generated art already integrated.
- Urda provenance hash mismatch remains documented.
- Urda/Morvi/Lotha/Vakuu option/icon art, generic custom card portraits, `lotha_verdict`, and Vakuu fight art remain documented temporary assets.
- Temptation remains unimplemented; no Temptation art/status/card exists.

Main files touched this pass:
- Code: `UrdaAncient.cs`, `MorviAncient.cs`, `LothaAncient.cs`, `VakuuFightPatch.cs`.
- Localization: `EZMicroBalance/localization/{eng,zhs}/ancients.json`, `cards.json`, `powers.json`, `relics.json`.
- Tests: `tests/EZMicroBalance.Tests/AncientPlayerFacingPolishGuardTests.cs`.
- Docs: `PROJECT_STATE.md`, `docs/issues.md`, `docs/issues/ancient-expansion-v2.2.md`, v2.2 README/source-design/implementation-plan/api-research/art-direction/work-log, `docs/mod-changelog.md`, release/hash docs, and `docs/archive/implementation-records/2026-05-13-ancient-player-facing-polish-final.md`.

Validation:
- `git status --short --branch`: `main...origin/main`, dirty worktree remains.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 104 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed; known nested `source code/project.godot` warning only.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 104 passed / 18 skipped.
- `scripts/package-spire-plus.ps1`: passed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 122 passed / 0 skipped.

Not performed: live game testing, live save/load, Vakuu failure/death-path testing, or co-op testing.

Remaining blockers: live Ancient UI/gameplay verification, Vakuu custom-combat save/load restore proof, final bespoke art generation, and Temptation implementation/art if that scope is later approved.