# RitsuLib Migration — Monthly Development Spec

## Purpose

Define the 4-week plan for completing the RitsuLib migration of Spire Plus from raw Harmony patches to managed RitsuLib `IPatchMethod` patches, including evidence backlog reduction and architecture hardening.

## Current State (Architecture Integration Validation 2026-05-31)

- **25 patches migrated** to RitsuLib `IPatchMethod` (Batch 1 + 4a + 4b)
- **Raw Harmony remaining**: 142 source declarations tracked by the current inventory family; runtime migration beyond Batch 4b remains blocked
- **Hybrid bootstrap active**: `ModPatcher.PatchAll()` for migrated, `Harmony.PatchAll()` for remaining
- **Latest validation**: 2026-05-31 clean build passed with 0 errors / 89 warnings; full tests passed with 452 passed / 0 failed / 21 skipped (473 total). Format and diff-check passed. See `docs/reviews/current-validation.md`.
- **Build**: 0 errors, 89 warnings (Sts1Events nullable CS8602/CS8604/CS8625 — accepted for prototype until StS1Events prototype hardening)
- **Sts1Events**: compiled, feature-gated (default Off), 5-mode safety matrix validated; CanaryOnly and AdditiveBatch1 are bounded test scopes, AdditiveAllDraft and replacement remain unsafe/dev-only
- **FeatureRegistry hardened**: IFeatureModule metadata, FeatureBootstrapRecord, LiveStatus enum, unified truthy env key overrides before bootstrap record creation, metadata/override guard tests
- **UrdaStateCodec V1**: encode/decode/legacy compat, 41 tests (18 source-structure + 15 behavioral + 8 edge-case)
- **Architecture canary integration**: RewardPipeline diagnostics are wired into FeatureRegistry bootstrap events, CardPlayContext canary is touched by Lotha extra-play through an allow-only adapter, and active multiplayer policy records are registered for diagnostics only. No gameplay behavior changes are intended.
- **DeathProtectionService stub**: diagnostics-only code stub with Request/Result/Priority, 21 tests (13 guard + 8 behavioral canary)
- **MultiplayerPolicy stub**: diagnostics-only registry with 6-category taxonomy, 14 tests (6 guard + 8 behavioral canary)
- **Runtime smoke**: blocked — STS2-RitsuLib not installed locally; Batch 4c blocked until runtime smoke passes

## 4-Week Plan

### Week 1: Batch 4a/4b Truth Closure (DONE)

**Status**: Complete

- Fixed Batch 4a count (9, not 10) and Batch 4b count (16)
- Fixed RitsuLibBootstrap comment (8 classes, not 7)
- Created migration guard tests for double-patch, source-level separation, manifest coverage, doc counts
- Moved untracked Sts1Events files to archive
- Historical full test suite clean: 444 passed, 0 failed, 21 skipped (465 total); current source now has 452 passed, 0 failed, 21 skipped (473 total)
- Format clean, diff clean

**Exit criteria met**: All doc counts match source, all guard tests pass, no untracked files.

### Week 2: Runtime Smoke + Full Test Truth

**Status**: Blocked (STS2-RitsuLib not installed locally; Batch 4c blocked until runtime smoke passes)

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
- Sts1Events 5-mode safety matrix validated: Off (default), CanaryOnly (4 safe shared events), AdditiveBatch1 (10 event types through 11 registration calls), AdditiveAllDraft (all draft calls), ReplaceUnknownEventsPrototype (compile-symbol-gated debug prototype)
- CanaryOnly registers: BigFish, GoldenIdol, TheLab, DivineFountain — all safe, no TODOs
- Sts1EventRegistrationService IS compiled, gated behind Sts1EventFeatureGate (default Off)
- Sts1Event guard tests cover default Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, and replacement prototype governance
- FeatureRegistry hardened: IFeatureModule metadata (DisplayName, Category, DisableEnvKeys, ForceEnvKeys), FeatureBootstrapRecord status tracking, ForceEnvKeys/DisableEnvKeys override evaluation, IsTruthyEnv helper
- 16 EngineeringGovernance guard tests (including 6 FeatureRegistry guards, 3 metadata guards, 6 module bootstrap guards)
- UrdaStateCodec V1 complete: encode/decode/legacy compat, 18 source-level guard tests + 15 behavioral tests
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` rewritten with mode safety matrix

### Week 4: High-Risk Migration Prep

**Status**: In Progress (architecture skeletons complete)

**Completed**:
- RewardPipeline skeleton: `RewardPhase` enum, `IRewardHandler` interface, `RewardPipeline` diagnostics-only orchestrator
- CardPlayContext skeleton: `ExtraPlayPolicy` enum, `CardPlayContext` with depth guard (MaxDepth=10), power fallback tracking
- DeathProtectionService spec: documents Lotha DeathReprieve lifecycle, inReprieve flag, forced death bypass, co-op owner attribution, future `IDeathProtectionProvider` interface
- MultiplayerPolicy taxonomy: 6 categories (LocalUiOnly, LocalPlayerOnly, HostAuthoritative, SharedRunState, CombatCommandReplicated, UnsafeInMultiplayer) mapped to existing MultiplayerFeaturePolicy
- 13 architecture skeleton guards + 15 UrdaStateCodec behavioral tests

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
- **AdditiveBatch1**: registers 10 event types through 11 registration calls; runtime unverified
- **AdditiveAllDraft**: registers all draft calls, including blocked/TODO and temporary-substitute events; unsafe/dev-only
- **ReplaceUnknownEventsPrototype**: debug-only and compile-symbol gated

Guard tests verify mode behavior. Resolution options:
1. Complete registration infrastructure and go live (requires runtime testing)
2. Archive permanently (reduces code surface)

### Runtime Log Plan — Off / CanaryOnly

When runtime smoke becomes available, the following log entries must be verified:

**Off mode (default, env var unset):**
- `[Spire Plus] Feature Sts1Events bootstrap gate: disabled (StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.).`
- No `[StS1 Events]` log lines (RegisterGated returns immediately)
- 0 StS1 events registered in RitsuLib content pack

**CanaryOnly mode (env var = `CanaryOnly`):**
- `[Spire Plus] Feature Sts1Events bootstrap gate: enabled (StS1 events CanaryOnly mode: registering 4 canary events.).`
- `[StS1 Events] Registering canary events (Big Fish, Golden Idol, Lab, Divine Fountain)...`
- `[StS1 Events] Canary events registered successfully.`
- Exactly 4 SharedEvent registrations visible in RitsuLib debug log
- No `ActEvent` registrations (canary events are shared-only)

**If RitsuLib not active:**
- `[StS1 Events] RitsuLib not active; skipping canary event registration.` (CanaryOnly)
- `[StS1 Events] RitsuLib not active; skipping event registration.` (AdditiveAllDraft/ReplaceUnknownEventsPrototype)

These log patterns are the required evidence for runtime smoke verification.

## Success Metrics

| Metric | Current | Target (End of Week 4) |
|--------|---------|------------------------|
| Migrated patches | 25 | 55-65 |
| Raw Harmony patches | 142 | 110-120 |
| Guard tests | 94 | 100+ |
| Total test suite | 452 passed | Keep 0 failed |
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
