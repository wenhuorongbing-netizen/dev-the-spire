# Ancient Expansion v2.2 Risk Register

Status: Urda, Morvi, and Lotha are default-on and source-backed for private-beta testing. Normal Vakuu clicked UI smoke exists, but the optional Vakuu fight stays hidden behind explicit gates until gated fight-option UI, victory return, save/load, death/failure, and co-op evidence exist. Do not make a release-ready claim from this file.

State mirror coverage: encoded `SavedAttachedState<Player, string>` and deck-mirrored `SavedAttachedState<CardModel, string>` values are mediated through `AncientPlayerState`; source guards reject direct field bypasses. This is source evidence only, not live save/load proof.

## Current P0/P1 Risks

| Risk | Severity | Area | Required mitigation |
| --- | --- | --- | --- |
| Power-card extra-play exploit | P0 | Morvi/Lotha card effects | Keep Attack/Skill-only behavior unless a source-backed fallback explicitly permits a Power card. Generated or extra-played cards must not recursively trigger the same blessing. |
| Death-interrupt complexity | P0 | Lotha Death Reprieve | Source uses `ShouldDieLate` / `AfterPreventingDeath` and only calls `CreatureCmd.Kill(..., force: true)` for failed reprieve resolution. Manual lethal-path checks are still required before runtime readiness. |
| Death Reprieve pending/active restore | P0 | Lotha Death Reprieve | Source persists selected blessing, `DeathReprieveUsed`, and `DeathReprievePhase` through player/deck state and can hydrate pending/active protection state. Exact active-turn save/load remains not proven because Core run saves do not source-prove full hand, energy, pile, and power recovery here. |
| Extra-play recursion | P0 | Misprint Press, Mirror Hall Echo | Mark generated and extra-played cards so the same blessing cannot retrigger itself. |
| Debt accounting and HP fallback rounding | P1 | Morvi Red Ink / Debt Settlement | Source uses deterministic rounding where applicable and caps Red Ink and Debt Settlement HP fallback as nonlethal. Live combat-end and save/load rows remain pending. |
| Reward UI softlock | P1 | Morvi/Vakuu reward alternatives | Use proven reward-completion paths and manually test accept, cancel, skip, and save/load. |
| Custom Ancient event art/background missing | P1 | Urda/Morvi/Lotha/Vakuu visuals | Urda, Morvi, and Lotha have custom Control scenes and exported assets. Static guards enforce clicked-background, map/run-history, option-art, and manifest export coverage. Final live UI proof remains pending. |
| Humus Pact reward reentry | P0 | Urda | Use explicit reward alternatives plus `AfterRewardTaken` follow-up instead of reward-screen skip postfix flow. Live reward-screen verification remains pending. |
| Humus Pact third payoff loss | P0 | Urda | Keep `HumusCompletionPending` set until payoff resolution succeeds and generate the payoff card before optional removals. Live reward-screen and save/load verification remain pending. |
| Seedbed generation accounting | P1 | Urda | Count accepted Seedbed choices only; reroll and reopen checks remain pending. |
| Player-field persistence not source-proven | P1 | Urda/Morvi/Lotha and Ascension-style player state | RitsuLib `SavedAttachedState<Player, string>` is source-guarded here, but live persistence still depends on actual game save/load behavior. Urda, Morvi, and Lotha mirror encoded player state onto card-backed `SavedAttachedState<CardModel, string>` deck markers through `AncientPlayerState`; guards cover helper use, owner/removed-card filters, recurrent `SyncDeck` calls, and direct field bypasses. Keep save/load rows open until live evidence proves recovery. |
| Card storage and save/load | P1 | Archive Pages, Waste Paper, Open Book, Vakuu Contracts | Morvi generated cards and Vakuu Contract cards use cleanup wrappers for not-in-combat and empty-result paths, and Open Book sealed cards have a saved card marker. Live restore rows remain pending. |
| Active button implementation | P1 | Red Ink Overdraft | Implemented as a temporary card. Visible Overdraft power now supports combat-end debt fallback, full-hand skip generation, wrong-pile generated-card cleanup, and nonlethal unpaid-debt HP fallback. Live restore and cleanup rows remain pending. |
| Public Evidence debuff policy source-closed / live-pending | P1 | Lotha Public Evidence | Source policy is Lotha-only and direction-gated: use `PowerModel.GetTypeForAmount(amount) == PowerType.Debuff` as the base, allow non-damaging negative statuses including `WeakPower`, `VulnerablePower`, and `FrailPower`, and exclude source-proven damage/kill Debuffs (`PoisonPower`, `ConstrictPower`, `DemisePower`, `DisintegrationPower`, `DoomPower`, `MagicBombPower`, `StranglePower`, and `TheGambitPower`). Live runtime verification of doubled stacks and Enlightenment gain/loss remains pending. |
| Vakuu child combat save/restore | P0 | Vakuu optional fight | Current source clears the parent event `Node`, uses direct `EnterRoomWithoutExitingCurrentRoom(...)`, sets `ShouldResumeParentEventAfterCombat`, and does not store `ParentEventId` while the combat room is active. Prefinished restore records the parent only for the reconstructed parent event. Live save/load must still prove active-fight behavior, prefinished no-reward restore, and parent resume. |
| Vakuu live victory return | P0 | Vakuu optional fight | Hidden by default. Source uses a dedicated Vakuu monster and encounter scene, but the reported post-victory black-screen path still needs live victory, failure/death, and save/load proof before normal exposure. |
| Vakuu post-victory black screen | P0 | Vakuu optional fight | Source clears the parent event `Node` before child combat, matching the intended direct transition shape. Live victory must prove parent event resume without black screen. |
| Vakuu fight death/failure path | P1 | Vakuu optional fight | Source communicates lethal risk and resumes parent event on victory. Live failure/death testing must prove it does not corrupt room, reward, or combat state. |
| Multiplayer ownership/desync | P1 | All future Ancient systems | Keep Ancient state player-owned or host-authoritative and run the host/client reward plus save/load matrix before co-op claims. |
| Save/load persistence | P1 | All future blessings | Add serialization evidence and manual reload rows before release claims. Source guards for deck-card markers and once-per-run flags do not close live restore rows. |
| Morvi Misprint target fidelity | P1 | Misprint Press | Generated-copy cleanup, clone/reentry guards, and Power-card exclusion are source-guarded. Live test must still verify random and targeted cards through `CardCmd.AutoPlay`. |
| Morvi Debt combat-end settlement | P1 | Debt Settlement | Source pays Gold first, falls back to nonlethal HP, and decreases Debt by the due amount. Live combat-end and save/load testing remain pending. |

## Readable Risk Anchors

- Power-card extra-play exploit: Morvi and Lotha must not copy, extra-play, or replay Power cards unless a source-backed fallback explicitly says so.
- Death-interrupt complexity: Death Reprieve remains live-pending for lethal-path runtime proof.
- Reward UI softlock: reward alternatives must use proven completion paths and still need accept/cancel/skip/save-load manual checks.
- Multiplayer ownership/desync: Ancient state must stay player-owned or host-authoritative before any co-op claim.
- Save/load persistence: deck-card marker source guards are not live save/load proof.
