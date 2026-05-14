# Next Implementation Goals

Reviewed source baseline: `a2183ee`.

Do not implement Morvi/Lotha/Vakuu, A21-A30, or custom-character work before the current RC blockers are acknowledged and either closed or explicitly release-noted.

## 1. Prove Or Replace Critical State Carriers

- Why: Urda, Morvi, and Rootblight rely on Player-scoped saved fields; local game source did not prove general Player saved-properties serialization.
- Likely files: `AncientSavedStateFields.cs`, `UrdaRunHook.cs`, `MorviRunHook.cs`, `AscensionSavedStateFields.cs`, `RootDeckService.cs`, tests/docs.
- Source evidence needed: direct serialization proof for `SavedSpireField<Player,T>`, or a safer state carrier.
- Implementation sketch: create a tiny controlled save/load proof first. If Player fields fail, migrate persistent state to a source-proven model/relic/card/run carrier instead of adding a normalizer.
- Acceptance criteria: save/load preserves Urda selected blessing/progress, Morvi debt state when gated, and Rootblight level across relevant transitions.
- Tests/commands: source guard plus `dotnet build`, `dotnet test`, release-artifact tests when packaging changes.
- Manual evidence: save before/after Ancient selection, reward screen, act transition, and combat end.

## 2. Close Reward Reentry And Softlock Risks

- Why: Seedbed, Humus, Debt, Prismatic Gem, and Fission all touch reward screens where custom alternatives or runtime context can be lost.
- Likely files: `UrdaRunHook.cs`, `MorviRunHook.cs`, `PrismaticGemPatches.cs`, `AscensionRewardService.cs`, tests/manual docs.
- Source evidence needed: `CardReward` serialization behavior, `RewardsSetSynchronizer` behavior, and live reward-screen save/load.
- Implementation sketch: add targeted diagnostics or source guards only after reproducing failure mode; avoid downstream cleanup that hides invalid state.
- Acceptance criteria: no duplicate card/gold/max-HP changes, no stuck reward screen, payoff pending clears exactly once.
- Tests/commands: `dotnet test`, new source guards for alternative count and pending-clears-on-success behavior if code changes.
- Manual evidence: reward reroll/reopen/save/load rows for each affected feature.

## 3. Finish Current RC Blockers Before More Content

- Why: Current private-beta readiness is blocked by manual Ancient reward rows, Urda live rows, Rootblight visuals, A11 traversal, and multiplayer evidence.
- Likely files: docs and possibly source fixes found by manual tests.
- Source evidence needed: none unless a manual test fails.
- Implementation sketch: run the master matrix, capture logs/screenshots, update handoff/checklist only with actual evidence.
- Acceptance criteria: release checklist blocker rows are pass or explicitly release-noted.
- Tests/commands: build/test/format/diff-check after any source/doc changes; publish only when source/resources/package changed.
- Manual evidence: Tier 0-5 matrix closure.

## 4. Active Urda Verification And Fixes

- Why: Urda is default-on but still prototype-grade without live/save/load/co-op proof.
- Likely files: `UrdaRunHook.cs`, `UrdaAncient.cs`, `UrdaCards.cs`, localization, tests/docs.
- Source evidence needed: reward alternative synchronization, room-enter ownership, Player state persistence.
- Implementation sketch: verify Seedbed/Humus/Molting/Moss Map in single-player first. If co-op duplicate application appears, fix at the source authority boundary using `LocalContext.IsMe`, `Player.IsActiveForHooks`, or game command ownership as supported by source.
- Acceptance criteria: each blessing applies once, to the correct player, survives intended reloads, and disables cleanly.
- Tests/commands: build/test plus new guard tests for any ownership fix.
- Manual evidence: Urda Tier 2 rows.

## 5. Ascension Preview, Balance, And Final Polish

- Why: Source now addresses several player-feedback issues, but map variety, previews, Fission visibility, Rootblight art, Boss Seal clarity, and A20 flow need live proof.
- Likely files: `AscensionMapService.cs`, `AscensionMapUiPatches.cs`, `AscensionRewardService.cs`, `AscensionCombatModifierService.cs`, `RootDeckService.cs`, A20 patches/events/localization.
- Source evidence needed: current v0.105.1 source refresh if game API drift appears.
- Implementation sketch: use diagnostics to verify seed variety and marked-node distribution; tune only from evidence.
- Acceptance criteria: preview text matches effects; no deterministic first-kind monotony; no clean-log regressions.
- Tests/commands: build/test/format plus diagnostics-run notes.
- Manual evidence: Tier 4 rows.

## 6. Multiplayer Blocking Investigation

- Why: Multiplayer is the largest release risk and the likely cause of misleading "version differs" reports.
- Likely files: `MultiplayerDiagnostics.cs`, selector patches, Urda/Rootblight ownership paths, release docs.
- Source evidence needed: host/client `JoinFlow`, `StartRunLobby`, reward synchronization, save/quit behavior.
- Implementation sketch: capture A10 baseline first, then A11/A12/A14/A16/A20. Fix only source-proven ownership/desync failures.
- Acceptance criteria: host/client logs explain mismatch causes, supported co-op rows behave consistently, unsupported rows are explicitly gated or release-noted.
- Tests/commands: build/test after fixes; no package claims without artifact refresh.
- Manual evidence: Tier 5 rows with both logs.

## 7. Only Then Start Ancient Expansion v2.2 Milestone 1

- Why: Additional content compounds existing state, reward, and multiplayer risks.
- Likely files: future Urda or default-off Morvi files only; no Lotha/Vakuu until blockers clear.
- Source evidence needed: all critical state/reward/multiplayer proofs above.
- Implementation sketch: choose one narrow, independently disableable feature. Prefer a low-risk Urda addition or Morvi hardening behind `EZMB_ENABLE_MORVI_V22=1`; do not add Lotha/Vakuu.
- Acceptance criteria: design doc, source API notes, guard tests, localization, manual rows, disable gate, and no default-on promotion without evidence.
- Tests/commands: full build/test/format/diff-check; publish only if release artifacts intentionally refresh.
- Manual evidence: targeted live row plus regression rows for current active systems.

## Not Next

- Do not implement Ascension 21-30.
- Do not implement a custom character.
- Do not make Morvi default-on.
- Do not implement Lotha or Vakuu Fight.
- Do not implement Red Ink Overdraft or Death Reprieve until their source API blockers are resolved.

