# Urda Issues

Last updated: 2026-05-12

## Scope

- `Urda, Loamweaver` is a vertical-scope Ancient expansion feature in the private-beta prototype lane.
- Refactor scope: keep source refactor clean, reduce future reading cost, and keep runtime claims truthful.

## Status: Prototype / Default-on test candidate with source gameplay slice

Urda is default-on for private-beta testing in this pass. Seedbed, Humus Pact, Molting, and Moss Map now have source-backed gameplay hooks. The current pass source-hardened Seedbed and Humus Pact reward timing, but Urda is not release-ready until live gameplay and save/load checks pass.

- Disable gate: set `EZMB_DISABLE_URDA=1` to hide Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- `UrdaFeatureGate.IsUrdaEnabled(...)` returns true unless the disable gate is truthy.
- No Morvi/Lotha/Vakuu content is currently enabled by default in this release candidate. Morvi has a separate default-off prototype gate for focused development testing; Lotha and Vakuu remain planning-only.
- NeowEpoch visibility is not a live gate for Urda; activation is limited to `Underdocks`/`Overgrowth` patches.
- Ancient Expansion v2.2 expands Urda into a ten-blessing future roadmap, but only the four ids below are active in the current source-backed slice.

## Active Urda blessing ids (current)

These ids are registered in the default-on Urda test candidate:

- `urda_seedbed`
- `urda_humus_pact`
- `urda_molting`
- `urda_moss_map`

Future v2.2 Urda blessings are planning-only: Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank.

## Open blockers

| ID | Severity | Status | Notes |
| --- | --- | --- | --- |
| URDA-BL-01 | P0 | open | Source gameplay slice exists for the four active blessings; live selection, reward-screen, room-entry, act-transition, and card behavior checks are still pending.
| URDA-BL-02 | P0 | open | Blessing implementations must be live save/load-tested before any release-ready claim; `SavedSpireField<Player,string>` is registered but not source-proven as persisted by this pass.
| URDA-BL-03 | P0 | open | Keep `Urda, Loamweaver` out of user-facing release-ready claims until the manual matrix passes or the feature is explicitly postponed.
| URDA-BL-04 | P1 | source-mitigated / live-pending | Humus Pact no longer uses `CardReward.OnSkipped`; third payoff pending state is kept until resolver success; verify the explicit `Compost Reward` flow does not reenter reward UI, duplicate/lost payoff, or fire from room-exit cleanup.
| URDA-BL-05 | P1 | source-mitigated / live-pending | Seedbed now counts accepted choices only and uses no-heal max HP gain; verify reroll, low max HP, and fourth-accept behavior live.

## Issue links

- `docs/issues.md` (active blocker index)
- `docs/features/ancient-expansion-urda/source-design.md`
- `docs/features/ancient-expansion-urda/implementation-plan.md`
- `docs/features/ancient-expansion-urda/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/README.md` (future roadmap only)
- `docs/issues/ancient-expansion-v2.2.md`

## Required evidence to remove prototype lock

- Active blessing gameplay behavior implemented for all live pool entries.
- Manual evidence that Seedbed, Humus Pact, Molting, and Moss Map match their source behavior.
- Manual tests for save/load + selection + combat-safe behavior.
- Runtime smoke and release checks updated.

