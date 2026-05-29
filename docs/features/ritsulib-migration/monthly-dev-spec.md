# RitsuLib Migration — Monthly Development Spec

## Purpose

Define the 4-week plan for completing the RitsuLib migration of Spire Plus from raw Harmony patches to managed RitsuLib `IPatchMethod` patches, including evidence backlog reduction and architecture hardening.

## Current State (End of Architecture Stabilization 2026-05-29)

- **25 patches migrated** to RitsuLib `IPatchMethod` (Batch 1 + 4a + 4b)
- **142 raw Harmony patches remaining** (22 high-risk, 35 medium-risk, 85 low-risk)
- **Hybrid bootstrap active**: `ModPatcher.PatchAll()` for migrated, `Harmony.PatchAll()` for remaining
- **8 migration guard tests** + 22 Sts1Events guard tests + 18 UrdaStateCodec guards + 10 FeatureRegistry guards + 13 architecture skeleton guards active, 361 total tests passing (0 failed, 21 skipped)
- **Sts1Events**: compiled, feature-gated (default Off), 4-mode safety matrix validated (Off/CanaryOnly/AdditiveAllDraft/ReplaceUnknownEventsPrototype)
- **FeatureRegistry hardened**: IFeatureModule metadata (DisplayName, Category, DisableEnvKeys, ForceEnvKeys), FeatureBootstrapRecord status tracking, env key override
- **UrdaStateCodec V1**: encode/decode/legacy compat complete, 18 source-level guard tests
- **Architecture skeletons**: RewardPipeline, CardPlayContext, DeathProtectionService spec, MultiplayerPolicy taxonomy
- **Runtime smoke**: pending — STS2-RitsuLib not installed locally

## 4-Week Plan

### Week 1: Batch 4a/4b Truth Closure (DONE)

**Status**: Complete

- Fixed Batch 4a count (9, not 10) and Batch 4b count (16)
- Fixed RitsuLibBootstrap comment (8 classes, not 7)
- Created migration guard tests for double-patch, source-level separation, manifest coverage, doc counts
- Moved untracked Sts1Events files to archive
- Full test suite clean: 311 passed, 21 skipped, 0 failed
- Format clean, diff clean

**Exit criteria met**: All doc counts match source, all guard tests pass, no untracked files.

### Week 2: Runtime Smoke + Full Test Truth

**Status**: In Progress

**Goals**:
1. Execute runtime smoke checklist (requires manual game load)
2. Capture `godot.log` evidence for loader smoke
3. Verify Mod Settings UI renders correctly
4. Confirm SavedSpireFields count matches source (30)
5. Document any runtime regressions found

**Tasks**:
- [ ] Run manual loader smoke per `runtime-smoke-checklist.md`
- [ ] Capture `godot.log` and store in `docs/evidence/`
- [ ] Verify Mod Settings UI screenshot
- [ ] Update `docs/dev-environment.md` with runtime evidence
- [ ] Update `docs/release-checklist.md` with completed runtime items

**Exit criteria**: Loader smoke passes, Mod Settings UI verified, 0 release-blocking log hits.

### Week 3: Sts1Events Scope Closure + FeatureRegistry Hardening

**Status**: Complete

**Completed**:
- Sts1Events 4-mode safety matrix validated: Off (default), CanaryOnly (4 safe shared events), AdditiveAllDraft (all events), ReplaceUnknownEventsPrototype (all events)
- CanaryOnly registers: BigFish, GoldenIdol, TheLab, DivineFountain — all safe, no TODOs
- Sts1EventRegistrationService IS compiled, gated behind Sts1EventFeatureGate (default Off)
- 22 Sts1Event guard tests (including 4 mode-safety guards)
- FeatureRegistry hardened: IFeatureModule metadata (DisplayName, Category, DisableEnvKeys, ForceEnvKeys), FeatureBootstrapRecord status tracking, IsTruthyEnv helper
- 10 EngineeringGovernance guard tests (including 3 FeatureRegistry guards)
- UrdaStateCodec V1 complete: encode/decode/legacy compat, 18 source-level guard tests
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` rewritten with mode safety matrix

### Week 4: High-Risk Migration Prep

**Status**: In Progress (architecture skeletons complete)

**Completed**:
- RewardPipeline skeleton: `RewardPhase` enum, `IRewardHandler` interface, `RewardPipeline` diagnostics-only orchestrator
- CardPlayContext skeleton: `ExtraPlayPolicy` enum, `CardPlayContext` with depth guard (MaxDepth=10), power fallback tracking
- DeathProtectionService spec: documents Lotha DeathReprieve lifecycle, inReprieve flag, forced death bypass, co-op owner attribution, future `IDeathProtectionProvider` interface
- MultiplayerPolicy taxonomy: 6 categories (LocalUiOnly, LocalPlayerOnly, HostAuthoritative, SharedRunState, CombatCommandReplicated, UnsafeInMultiplayer) mapped to existing MultiplayerFeaturePolicy
- 13 architecture skeleton guard tests

**Remaining**:
- [ ] Catalog high-risk patches (22 total) by subsystem
- [ ] Prioritize by evidence availability and blast radius
- [ ] Create migration plan for top 5 high-risk patches
- [ ] Establish rollback strategy: feature flags, compile-exclude guards
- [ ] Document expected behavior changes for each high-risk migration
- [ ] Update `docs/migration.md` with Batch 5 plan

**Exit criteria**: High-risk migration plan documented, rollback strategy established, top 5 patches ready for migration.

## Architecture Decisions

### Hybrid Bootstrap

The current hybrid approach (ModPatcher for migrated, raw Harmony for remaining) is intentional. It allows incremental migration without requiring all patches to move at once. The double-patch guard tests ensure no patch is applied twice.

### Double-Patch Guard

Source-level separation: migrated patch classes live in `RitsuLib/` namespace and implement `IPatchMethod`. Raw Harmony patches live in their original locations with `[HarmonyPatch]` attributes. The guard tests verify no class has both.

### Risk Classification

- **High-risk**: Run lifecycle, map generation, save/load, multiplayer, lobby — requires runtime evidence before migration
- **Medium-risk**: UI, card, relic, reward, combat model hooks — requires targeted testing
- **Low-risk**: Narrow local hooks with isolated blast radius — can migrate with guard tests only

### Sts1Events Mode Safety

Sts1Events source code compiles and is registered in the feature registry with a gate that defaults to Off. The registration service is compiled (not compile-excluded). Four modes validated:

- **Off** (default): returns immediately, 0 events registered
- **CanaryOnly**: registers 4 safe shared events (BigFish, GoldenIdol, TheLab, DivineFountain)
- **AdditiveAllDraft**: registers all 51 events, including DeadAdventurer (TODO elite) and Joust (no gold guard)
- **ReplaceUnknownEventsPrototype**: debug-only, replaces unknown events

Guard tests verify mode behavior. Resolution options:
1. Complete registration infrastructure and go live (requires runtime testing)
2. Archive permanently (reduces code surface)

## Success Metrics

| Metric | Current | Target (End of Week 4) |
|--------|---------|------------------------|
| Migrated patches | 25 | 55-65 |
| Raw Harmony patches | 142 | 110-120 |
| Guard tests | 71 | 80+ |
| Total test suite | 361 passed | 370+ passed |
| Runtime smoke | Pending | Complete |
| Sts1Events status | Compiled, gated Off, 4-mode matrix validated | Resolved (activate or archive) |
| High-risk migration plan | Architecture skeletons done | Documented with rollback strategy |
| Architecture skeletons | Complete (RewardPipeline, CardPlayContext, DeathProtectionService, MultiplayerPolicy) | N/A (done) |

## References

- `docs/migration.md` — PR sequencing and batch tracking
- `docs/patch-inventory.md` — full patch inventory with risk classification
- `docs/integrations/ritsulib.md` — RitsuLib integration staging record
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md` — runtime smoke verification
- `docs/features/ritsulib-migration/next-overnight-run.md` — next automated run instructions
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` — Sts1Events safety issue
- `docs/architecture/death-protection-spec.md` — DeathProtectionService contract and Lotha DeathReprieve lifecycle
- `docs/architecture/multiplayer-policy-taxonomy.md` — 6-category multiplayer safety classification
- `docs/architecture/patch-boundaries.md` — high-risk surface owners and service seams
- `docs/architecture/save-state-contracts.md` — durable vs transient state contracts
