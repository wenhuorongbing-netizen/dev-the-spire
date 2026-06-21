# External Codex Progress

## Current Slice

- Slice: `V22-LOTHA-POLISH + V22-RICH-TEXT`
- Updated: 2026-05-13 21:27:12 +02:00
- Scope: Lotha behavior/text polish for Single Sentence, Deferred Verdict, Mirror Rebuttal, and Public Evidence; English/zhs rich text and hover coverage; guard tests; focused docs.
- Constraints honored: no archive prompt/audit-matrix reading, no A21-A30, no custom character work, no audit-only pass, no live/save-load/co-op claims.

## Files Changed In This Slice

- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaPowers.cs`
- `EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs`
- `EZMicroBalance/localization/eng/ancients.json`
- `EZMicroBalance/localization/zhs/ancients.json`
- `EZMicroBalance/localization/eng/powers.json`
- `EZMicroBalance/localization/zhs/powers.json`
- `EZMicroBalance/localization/eng/relics.json`
- `EZMicroBalance/localization/zhs/relics.json`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`
- `tests/EZMicroBalance.Tests/AncientHighRiskSourceGuardTests.cs`
- `tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs`
- `docs/issues.md`
- `docs/issues/ancient-expansion-v2.2.md`
- `docs/features/ancient-expansion-v2.2/source-design.md`
- `docs/features/ancient-expansion-v2.2/implementation-plan.md`
- `docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md`
- `docs/features/ancient-expansion-v2.2/api-research.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `docs/mod-changelog.md`
- `PROJECT_STATE.md`
- `README.md`
- `docs/test-ready-completion-audit.md`
- `docs/external-codex-progress.md`

## Corrective Note

The previous Lotha polish pass passed automated tests but still deviated from v2.2 in four player-visible ways:

- Mirror Rebuttal was implemented as an "after first unblocked damage" rule instead of choosing a real Rebuttal card.
- Deferred Verdict used enemy Verdict/evidence as the main mechanic and only prepared one extra-play ruling, instead of player-owned turn-4 Verdict stacks consumed by the next non-Status cards that turn.
- Public Evidence was presented as a target-already-debuffed draw rule rather than a Debuff-application doubling and Enlightenment rule.
- Simplified Chinese Lotha relic/power localization was mojibake and, in the relic/power files, invalid JSON under UTF-8-aware parsing.

This corrective pass removes those paths from source, text, and guard expectations. Remaining evidence gaps are runtime-only: live gameplay, save/load, lethal-path, and co-op verification were not run in this source pass.

## Completed Work

- Reworked Mirror Rebuttal to open a source-safe deck selector at blessing pickup, mark one real non-Curse/non-Status deck card with `previous saved-state API<CardModel,bool>`, move the matching combat card to hand at combat start when needed, and resolve only when that marked card is played.
- Reworked Deferred Verdict to trigger on turn 4 by drawing 4, gaining 4 Energy, and applying 3 player-owned Verdict stacks. This turn, each next non-Status card consumes 1 Verdict; Attacks/Skills play once more and Powers use the Energy/draw replacement. If combat ends before turn 4, the source hook heals 4 HP.
- Reworked Single Sentence so the first player-driven Attack/Skill each turn plays two additional times, then the turn is capped at four more cards via `ShouldPlay`. The first Power fallback grants Energy/draw and does not consume the sentence.
- Reworked Public Evidence to use source power-amount hooks: player-applied Debuffs to enemies double and grant Enlightenment; enemy-applied Debuffs to the player double and remove Enlightenment; turn start consumes up to 3 Enlightenment for draw and Block.
- Updated Lotha hover tips to include replay/Energy/Verdict/Enlightenment/Block terms where relevant.
- Updated English and Simplified Chinese Lotha option, relic, Verdict power, and Enlightenment power localization with rich text and no stale old-mechanic wording. The zhs relic and power JSON files now parse cleanly as UTF-8 and no longer contain Lotha mojibake.
- Added Lotha guard tests for chosen-card Mirror Rebuttal, turn-4 player-owned Deferred Verdict stacks, Power-card safety, nonrecursive generated copies, corrected Public Evidence text, rich text, no mojibake fragments, and no stale placeholder/old behavior strings.
- Updated the requested issue, work-log, changelog, and external progress docs with this run's evidence.

## Commands Run

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: first run failed on two intentional localization/source-guard mismatches: Beautiful Bracelet zhs needed to preserve `迅速2`, and one Lotha guard expected the stale negated `!cardPlay.IsAutoPlay` string. Both root causes were patched.
- `dotnet build EZMicroBalance.sln`: rerun passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: rerun passed with 89 passed and 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed; only line-ending normalization warnings were reported on already dirty tracked files.
- `dotnet publish EZMicroBalance.sln`: passed because localization/resources changed. Godot emitted the known nested `source code/project.godot` ignore warning.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 89 passed and 18 skipped.

## Blockers / Pending

- Live gameplay verification was not run.
- Save/load verification was not run.
- Co-op verification was not run.
- Lotha lethal-path verification for Death Reprieve remains pending.
- Broader non-Lotha v2.2 rich-text parity remains open.

## Next Step

- Run focused live Lotha gameplay verification for the polished blessings, then save/load and co-op checks before claiming private beta readiness.
