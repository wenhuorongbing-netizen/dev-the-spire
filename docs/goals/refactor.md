# Spire Plus Refactoring Plan

Last updated: 2026-05-28

## Current Status

| Phase | Status | Notes |
| --- | --- | --- |
| Phase 0: Stop Bleeding | Done | Patch count drift fixed, PR template updated, guard tests added |
| Phase 1: Bootstrap Cleanup | Done | FeatureOrders, named feature modules, registry refactor, VakuuFightInitializer split |
| Phase 2: Patch Adapter Rule | Not Started | Next phase |
| Phase 3: Extract Seams | Not Started | PreviewTransformPolicy, BannerCombatPolicy, etc. |
| Phase 4: State/Save Cleanup | Not Started | UrdaProgress sub-states |
| Phase 5: Test Upgrade | Not Started | Behavior tests alongside source-shape guards |

## Validation Results (2026-05-28)

```text
Build:     dotnet build EZMicroBalance.sln           → 0 errors
Tests:     dotnet test EZMicroBalance.sln --no-build  → 303 pass / 0 fail / 21 skip
Format:    dotnet format --verify-no-changes           → clean
Diff:      git diff --check                            → clean
Skipped:   21 release artifact tests (require local StS2 install)
```

## Phase 0: Stop Bleeding (Done)

| Item | Status |
| --- | --- |
| Fix patch count drift in docs | Done |
| Add high-risk patch seam checklist to PR template | Done |
| Add source-only/live-proof checklist to PR template | Done |
| Update guard tests for new PR checklist items | Done |

## Phase 1: Bootstrap Cleanup (Done)

| Item | Status |
| --- | --- |
| Create FeatureOrders.cs with named constants | Done |
| Create named feature modules (Lotha, Morvi, Urda, Vakuu, Ascension) | Done |
| Refactor SpirePlusFeatureRegistry to use named modules | Done |
| Move VakuuFightInitializer to its own file | Done |
| Add compatibility fallback comment to AscensionInitializer | Done |
| Update active source manifest tests | Done |

## Phase 2: Patch Adapter Rule (Not Started)

### Goal

All high-risk patches should be thin adapters: gate, source object lookup, call service, return. No business logic in patches.

### Tasks

| Task | Risk | Acceptance Criteria |
| --- | --- | --- |
| Add adapter checklist to high-risk patch files | Medium | Each high-risk patch group has owner and service seam |
| Low-risk patch move-only refactor | Low | Patch inventory regenerates cleanly |
| High-risk patches: no move + behavior change in same PR | High | PR checklist enforces separation |

### High-Risk Patch Owners

| Owner | Patch Surface | Required Boundary |
| --- | --- | --- |
| Vakuu | CombatRoom, EventRoom, reward patches | Child-combat flow in VakuuFightService |
| Urda Root Eyes | RunManager, map click/hover UI | Preview storage/commit in UrdaBlessingService |
| A20 dual boss | RunManager, reward screen patches | Boss-chain in A20 services |
| Ascension selector | StartRunLobby private methods | Selector expansion gated |
| Ascension map | ActModel.CreateMap | Map graph in AscensionMapService |
| Ascension combat | combat/card/power hooks | Banner/Firemark/boss in combat services |
| Preview tools | NCrystalSphereScreen, NTransformPreview | Preview-only, no real mutation |

## Phase 3: Extract Highest-Value Seams (Not Started)

Priority order:
1. PreviewTransformPolicy (lowest risk)
2. BannerCombatPolicy / FiremarkWindowPolicy
3. RootSightPreviewPolicy (high value, high risk)
4. VakuuFightFlow (extreme risk, needs characterization tests)
5. Ascension selection scope

## Phase 4: State/Save Cleanup (Not Started)

| Task | Risk | Acceptance Criteria |
| --- | --- | --- |
| Split UrdaProgress into sub-state objects | Medium-High | Wire format unchanged |
| Add UrdaStateCodec round-trip tests | Medium | Legacy/current state stable |
| Align save field documentation | Medium | save-state-contracts updated |
| No field reordering without migration | High | Guard test coverage |

## Phase 5: Test Upgrade (Not Started)

Add behavior tests alongside existing source-shape guards:

| Test | Target | Priority |
| --- | --- | --- |
| RootSightPreview_DoesNotAdvanceLiveRng | RootSightPreviewPolicy | P0 |
| VakuuFightFlow_VictoryWithParent | VakuuFightFlow | P0 |
| VakuuFightFlow_NoParentFallback | VakuuFightFlow | P0 |
| UrdaStateCodec_RoundTrips | UrdaStateCodec | P0 |
| AscensionSelection_RestoreOnException | AscensionSelectionPolicy | P1 |
| BannerCombatPolicy_ShieldwallTiming | BannerCombatPolicy | P1 |
| FiremarkWindowPolicy_SuppressScope | FiremarkWindowPolicy | P1 |
| PreviewTransformPolicy_UsesForkedRng | PreviewTransformPolicy | P1 |

## Constraints

- Do not rename EZMicroBalance manifest id, DLL, PCK, or install folder
- Do not move high-risk patches in the same PR as behavior changes
- RitsuLib migration must not be mixed with large folder moves
- Source-only pass cannot close live proof gates
- StS1Events is prototype, not complete feature

## References

- Patch inventory: `docs/patch-inventory.md`
- Patch boundaries: `docs/architecture/patch-boundaries.md`
- Refactor map: `docs/refactor-map.md`
- Issues tracker: `docs/issues.md`
