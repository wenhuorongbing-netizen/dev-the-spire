# RitsuLib Migration - Monthly Development Spec

## Purpose

Complete the RitsuLib migration safely by separating four different claims:

- historical `v0.106.1` loader-gate proof for the RitsuLib dependency and migrated patch bootstrap;
- current local runtime compatibility for the installed game and RitsuLib variant pack;
- live/manual gameplay proof for Sts1Events and Spire Plus feature surfaces;
- owner-approved patch migration beyond the current 25 migrated classes.

## Current State

- STS2-RitsuLib `v0.4.28` is installed locally with the `0.107.1` runtime variant; the prior `v0.4.24` install was backed up before the beta.91 dependency pass.
- The current local game install is Slay the Spire 2 `v0.107.1`; installed beta.91 package parity is recorded, and fresh Off plus AdditiveBatch1 direct smokes under `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` audited clean. Earlier beta.84/beta.85/beta.87/beta.88/beta.90 smokes are previous-package, previous-dependency, or root-cause history only.
- Spire Plus now compiles against NuGet `STS2.RitsuLib` `0.4.28`; the manifest dependency floor is `STS2-RitsuLib >= 0.4.28`, and no BaseLib or separate `STS2.RitsuLib.Compat.*` package is used.
- Dependency decision for this pass: the owner-approved final dependency migration intentionally bumped compile package, manifest minimum, and package version together, then refreshed build, publish, package, installed-package parity, and Off/AdditiveBatch1 loader evidence.
- 25 patch classes are migrated to RitsuLib `IPatchMethod`.
- 146 raw `[HarmonyPatch]` declarations remain.
- 171 patch units are tracked by `docs/patch-inventory.md`.
- Hybrid bootstrap is active: migrated patches use RitsuLib `ModPatcher`, remaining raw patches use Harmony.
- Off, CanaryOnly, and AdditiveBatch1 diagnostic smokes have historical `v0.106.1` loader-gate evidence with 25/25 migrated patches applied and 30 SavedSpireFields observed.
- Sts1Events defaults Off. CanaryOnly and AdditiveBatch1 are controlled prototype/test modes. AdditiveAllDraft and ReplaceUnknownEventsPrototype remain unsafe/dev-only.
- Current-runtime beta.91 Off loader smoke is clean with exactly STS2-RitsuLib and Spire Plus loaded. Current beta.91 AdditiveBatch1 enabled-mode smoke is clean with 10 event types / 14 registration calls and retained verifier reports. The earlier beta.85 AdditiveBatch1 13/14 verifier mismatch and the beta.87 `v0.107.1` BaseLib clean-audit failure remain root-cause history only. Gameplay, event screenshots, save-load, image/render, replacement functional proof, Mod Settings UI refresh, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits. Post-baseline no-game/doc-governance recaptures do not close runtime/manual gates.
- Release-ready and live-ready remain no.

Coordination boundary: do not run overlapping no-game validation commands, package/release-evidence work, runtime/game smoke, staging, commit, or push from this thread. Assign one controlled lane, verify `git status --short --branch` first, and record the resulting HEAD/worktree status before using evidence for handoff.

## Workstream 1 - Validation Truth

Keep `docs/reviews/current-validation.md` aligned with the actual HEAD and worktree.

Required no-game commands for an implementation pass:

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet clean EZMicroBalance.sln
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

If a command fails, record the exact failure and do not advance the migration claim.

## Workstream 2 - Runtime Evidence

Historical `v0.106.1` loader-gate proof is available, but it does not prove current `v0.107.1` gameplay. The next runtime work should upgrade evidence in this order:

1. Keep installed-package parity verified after any package/source change.
2. Preserve beta.85/beta.87 `v0.107.0` loader proof only as previous-package/game-version context; rerun package parity and loader smoke when the changed surface requires a current claim.
3. Keep the beta.91 dependency floor aligned at `STS2.RitsuLib` / `STS2-RitsuLib` `0.4.28` unless a future owner-approved versioned package deliberately changes it again.
4. Refresh loader smoke only after the current runtime and installed package can support the claim being made; do not advance to CanaryOnly, AdditiveBatch1, replacement, or gameplay proof from the Off smoke alone.
5. Use the retained CanaryOnly enabled-mode smoke at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` only as beta.85 / `v0.107.0` previous-package proof; recapture before making a current-version CanaryOnly claim.
6. Use the current AdditiveBatch1 enabled-mode smoke at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` as the beta.91 / `v0.107.1` 10 event types / 14 registration-call proof; recapture it after any package/source change before using it for a new claim.
7. Capture Mod Settings UI for the current package state. Use `scripts\collect-mod-settings-evidence.ps1 -NoLaunch` to prepare the focused row, then rerun it with `-Capture List` and `-Capture Page` once the game is manually navigated to those screens.
8. Only after those enabled-mode smokes match current source shape, capture CanaryOnly gameplay evidence for Big Fish, Golden Idol, The Lab, and Divine Fountain.
9. Capture save/load and EN/ZHS render evidence for the canary events.
10. Capture co-op/fail-closed evidence or document the blocker.
11. Keep ReplacementPrototype behind explicit unsafe/debug gates.

## Workstream 3 - Batch 4c Candidate Review

Batch 4c is proposal-only. The current candidate list is `docs/features/ritsulib-migration/batch-4c-candidates.md`.

Candidate rules:

- 5-10 low-risk patch classes only.
- No run lifecycle.
- No save/load.
- No map generation.
- No multiplayer/lobby.
- No death handling.
- No A20 boss flow.
- No reward mutation with player state.

Each candidate must include source target, risk reason, targeted tests, and rollback plan. Migration requires explicit owner approval and a fresh validation pass.

## Workstream 4 - Architecture Diagnostics

The existing FeatureRegistry, RewardPipeline, CardPlayContext, DeathProtectionService, and MultiplayerPolicy work remains diagnostic/canary infrastructure unless a separate implementation task explicitly promotes behavior. Do not claim that diagnostics-only systems enforce gameplay.

## Stop Decision

Current decision: both.

- Optimize: reconcile docs, validation truth, warning debt, and manual-evidence queues.
- Advance: review Batch 4c candidates only. The current proposal self-check passes the 5-10 candidate count and prohibited-surface rules, but owner approval is still pending.

Do not migrate Batch 4c or high-risk patches in this phase. Do not claim release-ready.

## References

- `docs/migration.md`
- `docs/integrations/ritsulib.md`
- `docs/reviews/current-validation.md`
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/features/ritsulib-migration/batch-4c-candidates.md`
- `docs/features/sts1-events/status-board.md`
- `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md`
