Implemented the Lotha corrective pass and published the updated resources. This is source/test-ready only; I did not run live game, save-load, or co-op testing.

**Files Changed**
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaPowers.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs`
- `EZMicroBalance/localization/eng/ancients.json`
- `EZMicroBalance/localization/eng/relics.json`
- `EZMicroBalance/localization/eng/powers.json`
- `EZMicroBalance/localization/zhs/ancients.json`
- `EZMicroBalance/localization/zhs/relics.json`
- `EZMicroBalance/localization/zhs/powers.json`
- `tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `PROJECT_STATE.md`
- `docs/issues.md`
- `docs/issues/ancient-expansion-v2.2.md`
- `docs/features/ancient-expansion-v2.2/api-research.md`
- `docs/features/ancient-expansion-v2.2/art-direction.md`
- `docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md`
- `docs/features/ancient-expansion-v2.2/implementation-plan.md`
- `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/milestone-roadmap.md`
- `docs/features/ancient-expansion-v2.2/source-design.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `docs/mod-changelog.md`

**Lotha Status**
1. Mirror Rebuttal: implemented. Real deck-card selection, combat-card pull to hand, Attack/Skill +2 plays, Power replacement is 2 Energy + draw 2.
2. Mirror Hall Echo: implemented. End-turn type recording, next-turn one-shot echo, Attack/Skill +1 play, Power 1 Energy + draw 1. No separate runtime power indicator; option/relic text and logs document it.
3. Presumption: implemented with conservative Core-backed detection for unblocked enemy attack damage: enemy dealer, `ValueProp.Move`, unblocked damage, and no card source.
4. Closed Court: implemented. Removes only combat `CardReward`; gold/potion/relic rewards remain. First-turn draw to 10, +4 Energy, first three player-played cards cost 1 less Energy.
5. Deferred Verdict: implemented. Turn 4 draw 4, +4 Energy, 3 player-owned Verdict, Power fallback 1/1, Verdict cleanup at turn/combat end.
6. Death Reprieve: implemented with a documented deviation. Player-turn lethal starts immediately; enemy-turn lethal starts at the next player turn because safe immediate enemy-turn interruption was not source-proven.
7. Single Sentence: implemented. First player-played Attack/Skill +2 plays, then four-card cap; Power fallback does not consume the sentence; autoplay/clones/extra executions excluded.
8. Public Evidence: implemented using `PowerType.Debuff`; Core-modeled non-Debuffs are not doubled.

**Validation**
- `dotnet build EZMicroBalance.sln`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 93 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed; CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: passed; known nested `source code/project.godot` ignore warning.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 93 passed / 18 skipped.
- Final doc-aware rerun `dotnet test EZMicroBalance.sln --no-build`: passed, 93 passed / 18 skipped.

No live game, save-load, or co-op testing was run. Bespoke Lotha icon generation was not run because `OPENAI_API_KEY` is not set; exact per-icon prompts are documented in `art-direction.md`.