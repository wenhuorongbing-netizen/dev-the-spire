# RitsuLib Migration — Monthly Development Spec

## Purpose

Define the 4-week plan for completing the RitsuLib migration of Spire Plus from raw Harmony patches to managed RitsuLib `IPatchMethod` patches, including evidence backlog reduction and architecture hardening.

## Current State (End of Overnight Run 2026-05-28)

- **25 patches migrated** to RitsuLib `IPatchMethod` (Batch 1 + 4a + 4b)
- **141 raw Harmony patches remaining** (22 high-risk, 35 medium-risk, 84 low-risk)
- **Hybrid bootstrap active**: `ModPatcher.PatchAll()` for migrated, `Harmony.PatchAll()` for remaining
- **8 migration guard tests** + 1 Sts1Events guard test active, 311 total tests passing
- **Sts1Events**: compiled, feature-gated (default Off), dormant by default; guard tests active
- **Runtime smoke**: pending — no local game environment available

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

**Status**: Pending

**Goals**:
1. Resolve Sts1Events dormant skeleton: either complete registration infrastructure or archive permanently
2. Harden FeatureRegistry with explicit feature ordering and fail-closed guards
3. Migrate low-risk patches from raw Harmony to RitsuLib (target: 20-30 patches)

**Tasks**:
- [ ] Decide Sts1Events fate: complete registration or permanent archive
- [ ] If completing: implement `Sts1EventRegistrationService.RegisterAll()` call in `MainFile.cs`
- [ ] If archiving: remove Sts1Events source, update csproj, update guard tests
- [ ] Add FeatureRegistry ordering assertions to guard tests
- [ ] Identify and migrate next batch of low-risk patches (Batch 4c)
- [ ] Run full test suite after each migration batch

**Exit criteria**: Sts1Events resolved, FeatureRegistry hardened, Batch 4c complete with guard tests.

### Week 4: High-Risk Migration Prep

**Status**: Pending

**Goals**:
1. Prepare for high-risk patch migration (Batch 5) by reducing evidence backlog
2. Document migration strategy for run/map/reward/save/multiplayer patches
3. Establish rollback plan for high-risk migrations

**Tasks**:
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

### Sts1Events Dormant Risk

Sts1Events source code compiles and is registered in the feature registry with a gate that defaults to Off. The registration service is compiled (not compile-excluded). Guard tests verify the gate behavior. This is safe for now but creates maintenance burden. Resolution options:
1. Complete registration infrastructure and go live (requires testing)
2. Archive permanently (reduces code surface)

## Success Metrics

| Metric | Current | Target (End of Week 4) |
|--------|---------|------------------------|
| Migrated patches | 25 | 55-65 |
| Raw Harmony patches | 141 | 110-120 |
| Guard tests | 9 | 15-20 |
| Total test suite | 311 passed | 320+ passed |
| Runtime smoke | Pending | Complete |
| Sts1Events status | Compiled, gated Off | Resolved |
| High-risk migration plan | Not started | Documented |

## References

- `docs/migration.md` — PR sequencing and batch tracking
- `docs/patch-inventory.md` — full patch inventory with risk classification
- `docs/integrations/ritsulib.md` — RitsuLib integration staging record
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md` — runtime smoke verification
- `docs/features/ritsulib-migration/next-overnight-run.md` — next automated run instructions
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` — Sts1Events safety issue
