# Urda Ancient Implementation Plan

## 0. Context

Current dependency and architecture baseline:

- Base game source refresh target: `v0.106.0`.
- BaseLib package target: `v3.1.4`.
- Active private-beta project: `EZMicroBalance`.

Urda work remains independently disableable and independent from other ancient families.

Ancient Expansion v2.2 now promotes Urda to a ten-blessing default-on source slice. The slice is test-ready in source only: live gameplay, save/load, and co-op verification remain pending, and several blessings intentionally use narrower source-safe UI fallbacks.

## 1. Phase 0: Source evidence guard pass

Goal:

- Confirm local source APIs for Act 1 ancient registration and reward hooks.
- Confirm no immediate breakage from adding Urda classes and files.

Actions:

1. Read local `source code/src/Core` for `Ancient`, `AncientModel`, `AncientEvent`, and reward flow.
2. Confirm BaseLib command APIs for card/relic/add/remove and save fields.
3. Reconfirm current `EZMB_...` manifest and `docs/issues.md` Urda issue list.
4. Record findings in `api-research.md`.

Exit:

- Build remains green after docs-only changes.
- `api-research.md` has concrete pass/fail API items.

## 2. Phase 1: Urda framework

Goal:

- Make Urda selectable in Act 1 and expose an enabled blessing pool limited to implemented blessings.

Actions:

1. Implement or wire Urda identity registration path.
2. Add `urda` blessing registry with enabled flags.
3. Ensure inactive blessings are excluded from live pool.
4. Keep Urda independently disableable with `EZMB_DISABLE_URDA=1`; keep `EZMB_FORCE_URDA_BLESSING` default-off for diagnostics.
5. Add source-guarded fallback behavior when registration path is unavailable.

Exit:

- Source path exists for Urda Act 1 insertion and selection.
- Live run still needs to prove Urda appears and the configured blessing set is selectable.

## 3. Phase 2: Active blessing slice

Active blessing pool:

1. Seedbed.
2. Humus Pact.
3. Molting + `Withered Husk`.
4. Moss Map.
5. Trial Branch.
6. Shallow-Root Relic.
7. Rooted Route.
8. After the Rain.
9. Root-Sight.
10. Seed Bank.

For each blessing:

- Guarded source hooks are implemented for the first active slice.
- Progress is encoded in `AncientSavedStateFields.UrdaStateKey` and mirrored to deck cards through `AncientSavedStateFields.UrdaDeckStateKey` so reload testing can recover from a card-backed carrier if the Player field is empty.
- EN and ZHS localization keys are present for the active custom cards, Seedbed alternative, and Humus Pact alternative.
- Manual checklist rows remain open until tested in-game.
- Source guard tests cover the implementation shape.

Current hardening notes:

- Seedbed counters advance only on accepted Seedbed alternatives, not reward alternative generation.
- Seedbed is hidden when max HP cannot safely pay the 2 max HP cost.
- Humus Pact uses an explicit card reward alternative and resolves its third-trigger payoff after `AfterRewardTaken`.
- Humus Pact no longer patches `CardReward.OnSkipped` because local Core source shows skipped reward finalization can happen when a reward set is abandoned or a room is exited.
- Humus Pact keeps `HumusCompletionPending` until payoff resolution succeeds, and creates the payoff card before optional removals to avoid consuming removals if no payoff card can be generated.
- `UrdaStateKey` now includes a Humus completion-pending bit and keeps an eight-field migration read path. `AncientPlayerState` mirrors Urda progress to the card-backed `UrdaDeckStateKey`, but live save/load proof is still required.
- Trial Branch uses a 4-card source-safe selection grid, upgrades the chosen card, marks it with `UrdaTrialPlantCard`, tracks three combats, and keeps the card only if it was played in each of those combats.
- Shallow-Root Relic offers two common relics, grants 75 Gold, roots on an Act 1 elite kill for 35 Gold, and uses a deterministic Act 2 fallback that removes the pending relic and refunds 75 Gold. The `lose 6 Max HP to keep it` settlement remains a guarded design constant but is not exposed because no safe Act 2 choice UI was proven.
- Rooted Route automatically marks a reachable normal-combat node within the first seven floors, uses quest markers only, grants three card rewards plus a potion if available on success, and withers for 8 HP loss plus 25 Gold if the marked route becomes unreachable.
- After the Rain uses `ShouldDieLate` / `AfterPreventingDeath` for one Act 1 death prevention, then grants 15 Block, draws 1, adds two Wounds, loses 3 Max HP, and spends the blessing. If unused at Act 2, it heals 8 and grants 75 Gold. Before spending, up to two Act 1 elite kills grant 20 Gold.
- Root-Sight starts with 5 Root Eyes. Clicking the Root Eyes relic opens map selection, highlights future reachable Monster, Unknown, or Elite rooms, stores the chosen room's concrete enemy group or event, and grants the first-use potion if a slot exists.
- Seed Bank adds a `Store Seed` reward alternative to Act 1 normal combat card rewards. The source-safe slice stores a selected reward card by consuming that reward, caps at three seeds, and before the Act 1 Boss lets the player choose up to two seeds; the first selected card is upgraded and Seed Bank does not mark cards as Trial Plant.

## 4. Phase 3: Diagnostics and release hygiene

Goal:

- Add lightweight read-only diagnostics and keep unsafe content out of live path.

Actions:

- Add one-time logs for selection, blessing id, and state transitions.
- Add fallback notices for missing required references.
- Keep all non-live diagnostics in default-off mode.

## 5. Phase 4: Finalization

Goal:

- Close documentation and runtime evidence for private beta handoff.

Actions:

- Populate all Urda manual rows.
- Add source-guard tests if coverage is practical.
- Update `docs/issues.md` issue status as work lands.
- Run `dotnet build`.
- Publish only if resources or localization changed.
- Run smoke/load checks before any public readiness claim.

No milestone in this branch should claim release-ready Urda behavior if:

- any of the ten Urda blessing rows is incomplete or undocumented.
- save/load behavior is broken.
- live manual checks are still unexecuted.
