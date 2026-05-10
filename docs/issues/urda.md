# Urda Issues

Last updated: 2026-05-10

## Scope

- `Urda, Loamweaver` is a vertical-scope Ancient expansion feature in the private-beta prototype lane.
- Refactor scope: keep source refactor clean, reduce future reading cost, and keep runtime claims truthful.

## Status: Prototype / Debug-only

Urda is prototype/debug-only and default-off in this pass. It is not release-active.

- Gate: `EZMB_FORCE_ANCIENT=URDA` only.
- `UrdaFeatureGate.IsUrdaEnabled(...)` only returns true when the gate matches.
- No Morvi/Lotha/Vakuu active content is currently implemented in this release candidate.
- NeowEpoch visibility is not a live gate for Urda; activation is limited to `Underdocks`/`Overgrowth` patches behind `EZMB_FORCE_ANCIENT=URDA`.

## Active Urda blessing ids (current)

These ids are registered only behind the Urda gate:

- `urda_seedbed`
- `urda_humus_pact`
- `urda_molting`
- `urda_moss_map`

## Open blockers

| ID | Severity | Status | Notes |
| --- | --- | --- | --- |
| URDA-BL-01 | P0 | open | `UrdaAncient` currently records chosen blessing state only; gameplay blessing effects are not yet fully implemented.
| URDA-BL-02 | P0 | open | Blessing implementations must be source-verified + save/load-tested before any release-active enablement.
| URDA-BL-03 | P0 | open | Keep `Urda, Loamweaver` out of user-facing release-active claims until this prototype is either fully implemented or explicitly postponed.

## Issue links

- `docs/issues.md` (active blocker index)
- `docs/features/ancient-expansion-urda/source-design.md`
- `docs/features/ancient-expansion-urda/implementation-plan.md`
- `docs/features/ancient-expansion-urda/manual-test-checklist.md`

## Required evidence to remove prototype lock

- Active blessing gameplay behavior implemented for all live pool entries.
- Manual tests for save/load + selection + combat-safe behavior.
- Runtime smoke and release checks updated.

