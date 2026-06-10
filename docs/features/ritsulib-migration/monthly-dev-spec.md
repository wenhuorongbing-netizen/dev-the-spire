# RitsuLib Migration - Monthly Development Spec

## Purpose

Complete the RitsuLib migration safely by separating four different claims:

- historical `v0.106.1` loader-gate proof for the RitsuLib dependency and migrated patch bootstrap;
- current local runtime compatibility for the installed game and RitsuLib variant pack;
- live/manual gameplay proof for Sts1Events and Spire Plus feature surfaces;
- owner-approved patch migration beyond the current 25 migrated classes.

## Current State

- STS2-RitsuLib `v0.4.16` is installed locally with the `0.107.0` runtime variant; the prior `v0.3.10` install was backed up under `%TEMP%\codex-ritsulib-backup-20260610-090338`.
- The current local game install is Slay the Spire 2 `v0.107.0`; installed beta.84 package parity was restored and a fresh Off smoke was captured, but it is non-clean. The game reached main menu with RitsuLib `v0.4.16` / compat `0.107.0`, while Spire Plus applied only 17/25 ModPatcher patches and hit an `EctoplasmGoldGatePatch` initializer exception from packaged API drift.
- Spire Plus still compiles against NuGet `STS2.RitsuLib` `0.3.2`; NuGet now has `STS2.RitsuLib` `0.4.16`, and no separate `STS2.RitsuLib.Compat.0.107.0` package is published.
- Dependency decision for this dirty source state: do not bump the compile package or manifest minimum in place. A future owner-approved `v0.107.0` tester package should bump both to `0.4.16` with the required package-version, publish/package, artifact-test, and loader-smoke work.
- 25 patch classes are migrated to RitsuLib `IPatchMethod`.
- 142 raw `[HarmonyPatch]` declarations remain.
- 167 patch units are tracked by `docs/patch-inventory.md`.
- Hybrid bootstrap is active: migrated patches use RitsuLib `ModPatcher`, remaining raw patches use Harmony.
- Off, CanaryOnly, and AdditiveBatch1 diagnostic smokes have historical `v0.106.1` loader-gate evidence with 25/25 migrated patches applied and 30 SavedSpireFields observed.
- Sts1Events defaults Off. CanaryOnly and AdditiveBatch1 are controlled prototype/test modes. AdditiveAllDraft and ReplaceUnknownEventsPrototype remain unsafe/dev-only.
- Clean current-runtime Off smoke, gameplay, event screenshots, save-load, image/render, replacement functional proof, Mod Settings UI, co-op/fail-closed proof, independent QA, clean-worktree decision, current-source package decision, and versioned tester-package handoff remain pending.
- Release-ready and live-ready remain no.

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

Historical `v0.106.1` loader-gate proof is available, but it does not prove current `v0.107.0` compatibility or gameplay. The next runtime work should upgrade evidence in this order:

1. Keep installed-package parity verified after any package/source change.
2. Resolve the non-clean `v0.107.0` Off smoke by fixing the package/source runtime drift or by cutting an owner-approved current-source tester package.
3. If owner approves a new `v0.107.0` tester package, bump the repo compile package and manifest minimum from `0.3.2` to `0.4.16` in that versioned package pass.
4. Refresh loader smoke only after the current runtime and installed package can support the claim being made; do not advance to CanaryOnly, AdditiveBatch1, replacement, or gameplay proof until Off is clean.
5. Capture Mod Settings UI for the current package state.
6. Capture CanaryOnly gameplay evidence for Big Fish, Golden Idol, The Lab, and Divine Fountain.
7. Capture save/load and EN/ZHS render evidence for the canary events.
8. Capture co-op/fail-closed evidence or document the blocker.
9. Keep ReplacementPrototype behind explicit unsafe/debug gates.

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
