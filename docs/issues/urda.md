# Urda Issues

Last updated: 2026-05-13

## Scope

- `Urda, Loamweaver` is a vertical-scope Ancient expansion feature in the private-beta prototype lane.
- Refactor scope: keep source refactor clean, reduce future reading cost, and keep runtime claims truthful.
- Authority note, 2026-05-22: this folder is Urda support evidence. Current combined Ancient behavior is governed by `docs/issues.md`, `docs/test-ready-development-goal.md`, `docs/features/ancient-expansion-v2.2/source-design.md`, and `docs/issues/v3.3-design-review.md`; older Seedbed reward-alternative rows here are historical unless repeated there.

## Status: Prototype / Default-on ten-blessing source candidate

Urda is default-on for private-beta testing in this pass. All ten v2.2 Urda blessing ids now have source-backed gameplay hooks, option relics/icons, localization, and guard coverage. The original four blessings remain intact, while Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank are promoted with documented source-safe deviations. A headless installed-PCK check still resolves the custom Urda scene/icon with 0 log errors or warnings, but Urda is not release-ready until live gameplay and save/load checks pass.

- Disable gate: set `EZMB_DISABLE_URDA=1` to hide Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- `UrdaFeatureGate.IsUrdaEnabled(...)` returns true unless the disable gate is truthy.
- Morvi is default-on in the active private-beta test slice with its own disable/force gates. Lotha is now default-on in the active test slice with its own disable/force gates. Vakuu fight is hidden by default, has a dedicated source enemy/scene, and still needs live victory proof before normal exposure.
- NeowEpoch visibility is not a live gate for Urda; activation is limited to `Underdocks`/`Overgrowth` patches.
- Ancient Expansion v2.2 now uses a ten-blessing Urda source slice. Runtime proof is still pending.

## Active Urda blessing ids (current)

These ids are registered in the default-on Urda test candidate:

- `urda_seedbed`
- `urda_humus_pact`
- `urda_molting`
- `urda_moss_map`
- `urda_trial_branch`
- `urda_shallow_root_relic`
- `urda_rooted_route`
- `urda_after_rain`
- `urda_root_sight`
- `urda_seed_bank`

Source-safe deviations:

- Trial Branch uses a simple 4-card rare-card source-safe grid, upgrades the chosen card, applies a visible Trial Branch enchantment, and removes the card if it is missed in any one of the next three combats.
- Shallow-Root Relic offers two common relics and grants 75 Gold; if it is not rooted by an Act 1 elite, Act 2 removes the pending relic and refunds 75 Gold instead of opening the unproven `lose 6 Max HP to keep it` settlement UI.
- Rooted Route automatically marks a reachable normal-combat node within the first seven floors and does not mutate the map graph.
- Root-Sight uses the Root Eyes relic as its map control: clicking the Root Eyes relic opens map selection, highlights future reachable Monster, Unknown, or Elite rooms, stores the chosen room's concrete enemy group or event, and spends one Root Eye.
- Seed Bank deliberately uses the current test-slice path: store a selected reward card by consuming that reward. The player clicks the Seed Bank relic later to choose up to two stored cards; the first chosen card is upgraded. This is the active source-safe behavior, not an open promise to store an unchosen card after also taking another reward.

## Open blockers

| ID | Severity | Status | Notes |
| --- | --- | --- | --- |
| URDA-BL-01 | P0 | open | Source gameplay slice exists for ten active blessings; live selection, reward-screen, room-entry, act-transition, death-prevention, map-marker, and card behavior checks are still pending.
| URDA-BL-02 | P0 | open | Blessing implementations must be live save/load-tested before any release-ready claim; `SavedSpireField<Player,string>` is registered but not source-proven as persisted by this pass, and the card-backed `UrdaDeckStateKey` mirror is source mitigation rather than live proof.
| URDA-BL-03 | P0 | open | Keep `Urda, Loamweaver` out of user-facing release-ready claims until the manual matrix passes or the feature is explicitly postponed.
| URDA-BL-04 | P1 | source-mitigated / live-pending | Humus Pact no longer uses `CardReward.OnSkipped`; third payoff pending state is kept until resolver success; verify the explicit `Compost Reward` flow does not reenter reward UI, duplicate/lost payoff, or fire from room-exit cleanup.
| URDA-BL-05 | P1 | source-mitigated / live-pending | Seedbed now counts accepted choices only and uses no-heal max HP gain; verify reroll, low max HP, and fourth-accept behavior live.
| URDA-BL-06 | P1 | source/package/resource-load mitigated / live-pending | Urda now derives from BaseLib `CustomAncientModel` with custom icon/background-scene paths, packages `ezmb_urda.tscn`, and the headless installed-PCK check at `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` resolves the custom scene/icon with 0 `ERROR` / `WARNING` lines; rerun live Urda selection and Rootblight visual/gameplay checks to confirm the pre-fix missing asset errors are gone in-game.

## Issue links

- `docs/issues.md` (active blocker index)
- `docs/features/ancient-expansion-urda/source-design.md`
- `docs/features/ancient-expansion-urda/implementation-plan.md`
- `docs/features/ancient-expansion-urda/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/README.md` (future roadmap only)
- `docs/issues/ancient-expansion-v2.2.md`

## Required evidence to remove prototype lock

- Active blessing gameplay behavior implemented for all live pool entries.
- Manual evidence that all ten Urda blessings match their documented source behavior and deviations.
- Manual tests for save/load + selection + combat-safe behavior.
- Runtime smoke and release checks updated.
