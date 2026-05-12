# Urda Issues

Last updated: 2026-05-12

## Scope

- `Urda, Loamweaver` is a vertical-scope Ancient expansion feature in the private-beta prototype lane.
- Refactor scope: keep source refactor clean, reduce future reading cost, and keep runtime claims truthful.

## Status: Prototype / Default-on test candidate with source gameplay slice

Urda is default-on for private-beta testing in this pass. Seedbed, Humus Pact, Molting, and Moss Map now have source-backed gameplay hooks, but Urda is not release-ready until live gameplay and save/load checks pass.

- Disable gate: set `EZMB_DISABLE_URDA=1` to hide Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- `UrdaFeatureGate.IsUrdaEnabled(...)` returns true unless the disable gate is truthy.
- No Morvi/Lotha/Vakuu active content is currently implemented in this release candidate.
- NeowEpoch visibility is not a live gate for Urda; activation is limited to `Underdocks`/`Overgrowth` patches.

## Active Urda blessing ids (current)

These ids are registered in the default-on Urda test candidate:

- `urda_seedbed`
- `urda_humus_pact`
- `urda_molting`
- `urda_moss_map`

## Open blockers

| ID | Severity | Status | Notes |
| --- | --- | --- | --- |
| URDA-BL-01 | P0 | open | Source gameplay slice exists for the four active blessings; live selection, reward-screen, room-entry, act-transition, and card behavior checks are still pending.
| URDA-BL-02 | P0 | open | Blessing implementations must be live save/load-tested before any release-ready claim.
| URDA-BL-03 | P0 | open | Keep `Urda, Loamweaver` out of user-facing release-ready claims until the manual matrix passes or the feature is explicitly postponed.

## Issue links

- `docs/issues.md` (active blocker index)
- `docs/features/ancient-expansion-urda/source-design.md`
- `docs/features/ancient-expansion-urda/implementation-plan.md`
- `docs/features/ancient-expansion-urda/manual-test-checklist.md`

## Required evidence to remove prototype lock

- Active blessing gameplay behavior implemented for all live pool entries.
- Manual evidence that Seedbed, Humus Pact, Molting, and Moss Map match their source behavior.
- Manual tests for save/load + selection + combat-safe behavior.
- Runtime smoke and release checks updated.

