# RitsuLib Migration — Monthly Development Spec

## Purpose

Define the 4-week plan for completing the RitsuLib migration of Spire Plus from raw Harmony patches to managed RitsuLib `IPatchMethod` patches, including evidence backlog reduction and architecture hardening.

## Current State (Runtime Proof + Governance Closure 2026-05-31)

- **25 patches migrated** to RitsuLib `IPatchMethod` (Batch 1 + 4a + 4b).
- **Raw Harmony remaining**: 142 source declarations tracked by the current inventory family; runtime migration beyond Batch 4b remains blocked.
- **Tracked patch units total**: 167 (`25` migrated `IPatchMethod` classes + `142` raw `[HarmonyPatch]` declarations).
- **Hybrid bootstrap active**: `ModPatcher.PatchAll()` for migrated patches, `Harmony.PatchAll()` for remaining raw patches.
- **Latest no-game validation**: 2026-05-31 Revision J validation at HEAD `6b149ba0` is 0 build errors / 89 Sts1Events nullable warnings, 464 passed / 0 failed / 21 skipped / 485 total for both full and no-build solution tests, format clean, and patch inventory fresh. See `docs/reviews/current-validation.md`.
- **Build warning debt**: 89 nullable warnings in `EZMicroBalanceCode/Sts1Events/Models/` (`CS8604` = 54, `CS8602` = 34, `CS8625` = 1). See `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md`.
- **Sts1Events**: compiled, feature-gated default Off, and source-guarded by a 5-mode safety matrix. CanaryOnly and AdditiveBatch1 are bounded source-test/prototype scopes. AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only, require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, and are not tester/release-safe. The mode env var is handled by `Sts1EventFeatureGate`, not by generic FeatureRegistry disable overrides.
- **FeatureRegistry hardened**: `IFeatureModule` metadata, `FeatureBootstrapRecord`, `LiveStatus` enum, unified truthy env key overrides before bootstrap record creation, metadata/override guard tests.
- **UrdaStateCodec V1**: encode/decode/legacy compat, including current full positional decode, legacy full positional decode, null-string encode behavior, and edge cases.
- **Architecture canary integration**: RewardPipeline diagnostics are wired into FeatureRegistry bootstrap events and low-risk Ascension reward/card-reward surfaces; CardPlayContext canary emits diagnostics through the existing Lotha extra-play allow-only adapter; active multiplayer policy records are registered for diagnostics. No gameplay behavior changes are claimed.
- **DeathProtectionService**: diagnostics-only, provider-testable service with request/result/check/provider attribution tests; not wired into gameplay death prevention.
- **MultiplayerPolicy**: diagnostics-only registry/taxonomy with active-system records and co-op evidence metadata; not a gameplay enforcement system.
- **Runtime smoke**: fresh K1 smoke at HEAD `8f2d79b4` on 2026-06-02 confirms clean Off-mode and CanaryOnly loader-gate evidence. Off-mode Steam smoke reached main menu in 40s with clean audit (0 Godot ERROR). CanaryOnly direct-launch smoke reached main menu in 22s with exactly 4 canary events registered and clean audit. AdditiveBatch1 evidence from earlier June 2 pass also clean. All three modes (Off=0, CanaryOnly=4, AdditiveBatch1=10/11) are runtime-proven at loader-gate level. Gameplay, save-load, Mod Settings UI, event encounter screenshots, independent QA, and versioned tester-package handoff remain pending.

## 4-Week Plan

### Week 1: Batch 4a/4b Truth Closure (DONE)

**Status**: Complete for source-level closure.

- Fixed Batch 4a count (9, not 10) and Batch 4b count (16).
- Fixed RitsuLibBootstrap comment (8 classes, not 7).
- Created migration guard tests for double-patch, source-level separation, manifest coverage, doc counts.
- Moved untracked Sts1Events files to archive.
- Historical validation totals remain historical only; current Revision J source validation at `6b149ba0` is 464 passed / 0 failed / 21 skipped / 485 total.

**Exit criteria status**: Source/doc counts match source and guard tests pass. Worktree cleanliness is not met in this local run; existing dirty edits are preserved.

### Week 2: Runtime Smoke + Full Test Truth

**Status**: STS2-RitsuLib installed; controlled loader reaches main menu with Spire Plus loaded, but audit is not clean. Supplemental retry was invalid because Spire Plus did not load.

**Goals**:

1. Execute runtime smoke checklist after STS2-RitsuLib is installed.
2. Capture `godot.log` evidence for loader smoke.
3. Verify Mod Settings UI renders correctly.
4. Confirm SavedSpireFields count matches source (30).
5. Verify Sts1Events Off = 0 registrations and CanaryOnly = exactly 4 registrations.
6. Document any runtime regressions found.

**Tasks**:

- [x] Install STS2-RitsuLib v0.3.2+ at `<GameRoot>\mods\STS2-RitsuLib` (`v0.3.10` installed on E-drive).
- [x] Run loader smoke attempt per `runtime-smoke-checklist.md`.
- [x] Capture `godot.log` and store evidence under `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304`.
- [ ] Resolve/disposition the controlled-loader errors: `ritsulib-variants.json` manifest parsing and optional Spire Plus ModPatcher failures.
- [ ] Fix supplemental live-session retry causes before reuse: `EZMicroBalance` skipped as disabled, stale/duplicate mod manifest rows, and non-clean audit.
- [ ] Verify Mod Settings UI screenshot.
- [ ] Update `docs/dev-environment.md` with runtime evidence.
- [ ] Keep release checklist runtime rows pending until live evidence exists.

**Exit criteria**: Loader smoke passes, Off and CanaryOnly registration counts are proven in `godot.log`, Mod Settings UI verified, and 0 release-blocking log hits.

### Week 3: Sts1Events Scope Closure + FeatureRegistry Hardening

**Status**: Source-level governance complete; runtime proof blocked.

**Completed**:

- Sts1Events 5-mode safety matrix source-guarded: Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, ReplaceUnknownEventsPrototype.
- CanaryOnly registers Big Fish, Golden Idol, The Lab, and Divine Fountain only.
- AdditiveBatch1 registers 10 event types through 11 registration calls and is controlled prototype-only.
- AdditiveAllDraft registers all draft calls only when the unsafe dev override is set; it includes blocked/TODO and temporary-substitute events and remains unsafe/dev-only.
- ReplaceUnknownEventsPrototype is compile-symbol-gated, also requires the unsafe override, and is debug-only.
- Sts1Event guard tests cover default Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, replacement prototype governance, shared/combat event behavior, mode safety, and registry presence.
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` records the risk table.

**Runtime status**: Off, CanaryOnly, and AdditiveBatch1 are not runtime-verified until fresh `godot.log` evidence exists.

### Week 4: Post-Smoke Migration Decision Backlog

**Status**: Frozen until runtime smoke passes.

**Allowed before runtime smoke**:

- Catalog high-risk patch owners/read-only evidence.
- Document rollback strategy in general terms.
- Keep diagnostics-only canary tests and source guards current.
- Do not migrate patches.

**Not allowed before runtime smoke**:

- Batch 4c migration.
- High-risk patch migration.
- New `IPatchMethod` migration beyond the current 25.
- Any gameplay behavior expansion.
- Any runtime-safe/live-ready/release-ready claim.

**Conditional post-smoke tasks**:

- [x] Propose 5-10 low-risk Batch 4c candidates after runtime smoke passes. (10 candidates proposed: EnemyDamagePolish, AscensionLocalizationTable, VakuReward, VelvetChoker, AeonglassIntent, MeatCleaverCook, JewelryBox, PreservedFog, PaelsTooth, ToastyMittens+JeweledMask — 44 patches total.)
- [ ] Require explicit owner acceptance before migrating any candidate.
- [ ] Keep high-risk migration as a later planning item only.

**Exit criteria**: Runtime smoke is passed and independently reviewed before any Batch 4c decision advances.

## Architecture Decisions

### Hybrid Bootstrap

The current hybrid approach is intentional. It allows incremental migration without requiring all patches to move at once. The double-patch guard tests ensure no patch is applied twice.

### Double-Patch Guard

Source-level separation: migrated patch classes live in `RitsuLib/` namespace and implement `IPatchMethod`. Raw Harmony patches live in their original locations with `[HarmonyPatch]` attributes. The guard tests verify no class has both.

### Risk Classification

- **High-risk**: Run lifecycle, map generation, save/load, multiplayer, lobby. Requires runtime evidence before migration.
- **Medium-risk**: UI, card, relic, reward, combat model hooks. Requires targeted testing.
- **Low-risk**: Narrow local hooks with isolated blast radius. Can be proposed only after runtime smoke passes in this run family.

### Sts1Events Mode Safety

Sts1Events source code compiles and is registered in the feature registry with a gate that defaults to Off. The registration service is compiled, not compile-excluded. Five modes are source-guarded:

- **Off** (default): returns immediately, 0 events registered.
- **CanaryOnly**: registers 4 safe shared events: Big Fish, Golden Idol, The Lab, Divine Fountain.
- **AdditiveBatch1**: registers 10 event types through 11 registration calls; controlled prototype, runtime loader-gate proven.
- **AdditiveAllDraft**: registers all draft calls only with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; includes blocked/TODO and temporary-substitute events; unsafe/dev-only.
- **ReplaceUnknownEventsPrototype**: debug-only and compile-symbol-gated, with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` also required; unsafe/dev-only.

Guard tests verify mode behavior. Runtime promotion requires live smoke and manual proof.

### Runtime Log Plan — Off / CanaryOnly

When runtime smoke becomes available, the following log entries must be verified:

**Off mode (default, env var unset):**

- `[Spire Plus] Feature Sts1Events bootstrap gate: disabled (StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.).`
- No `[StS1 Events]` registration lines.
- 0 StS1 events registered in RitsuLib content pack.

**CanaryOnly mode (`SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`):**

- `[Spire Plus] Feature Sts1Events bootstrap gate: enabled (StS1 events CanaryOnly mode: registering 4 canary events.).`
- `[StS1 Events] Registering canary events (Big Fish, Golden Idol, Lab, Divine Fountain)...`
- `[StS1 Events] Canary events registered successfully.`
- Exactly 4 SharedEvent registrations visible in RitsuLib debug log.
- No `ActEvent` registrations.

**If RitsuLib is not active:**

- `[StS1 Events] RitsuLib not active; skipping canary event registration.` (CanaryOnly)
- `[Spire Plus] Feature Sts1Events bootstrap gate: disabled (...unsafe/dev-only...SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1...)` for unsafe modes without the explicit override.
- `[StS1 Events] RitsuLib not active; skipping event registration.` only after an unsafe all-draft/debug mode is explicitly allowed.

These log patterns are required evidence for runtime smoke verification.

## Success Metrics

| Metric | Current | Gate Target |
|--------|---------|-------------|
| Migrated patches | 25 | Hold at 25 until runtime smoke passes |
| Raw Harmony declarations | 142 | Hold at 142 until runtime smoke passes |
| Tracked patch units | 167 | Reconcile on every inventory refresh |
| Total test suite | 464 passed / 0 failed / 21 skipped / 485 total | Keep 0 failed |
| Runtime smoke | Loader-gate PASS: Off=0, CanaryOnly=4, AdditiveBatch1=10/11 with clean audits at HEAD `3f01cb7` | Gameplay, save-load, Mod Settings UI, event screenshots, independent QA pending |
| Sts1Events status | Compiled, gated Off, 5-mode matrix source-guarded | Runtime proof required before activation/archive decision |
| High-risk migration plan | Frozen | Catalog-only until runtime smoke passes |
| Architecture skeletons | Diagnostics-only canary integration | No gameplay enforcement claim |

## References

- `docs/migration.md` — PR sequencing and batch tracking
- `docs/patch-inventory.md` — full patch inventory with risk classification
- `docs/integrations/ritsulib.md` — RitsuLib integration staging record
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md` — runtime smoke verification
- `docs/features/ritsulib-migration/next-overnight-run.md` — next automated run instructions
- `docs/issues/ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` — Sts1Events safety issue
- `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md` — warning debt triage
- `docs/architecture/death-protection-spec.md` — DeathProtectionService contract and Lotha DeathReprieve lifecycle
- `docs/architecture/multiplayer-policy-taxonomy.md` — 6-category multiplayer safety classification
- `docs/architecture/patch-boundaries.md` — high-risk surface owners and service seams
- `docs/architecture/save-state-contracts.md` — durable vs transient state contracts
