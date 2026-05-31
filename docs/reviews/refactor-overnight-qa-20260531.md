# Refactor Overnight QA — 2026-05-31

## Verdict

FAIL / HARD BLOCKED. Green Stop is not allowed.

## Independent Audit Results

- Runtime proof is absent because `STS2-RitsuLib` is missing at the checked D-drive and E-drive game-root mod paths.
- No active `godot.log` exists for RitsuLib bootstrap, Off=0 registration proof, or CanaryOnly=4 registration proof.
- Batch 4c, high-risk migration, new gameplay expansion, runtime-safe, live-ready, and release-ready claims remain blocked.
- Warning debt is tracked as 89 Sts1Events nullable warnings with code-level counts and per-file triage.
- Sts1Events source-level governance is mostly guarded, but the audit found `SPIREPLUS_STS1_EVENT_MODE` was incorrectly wired as a generic feature disable override before this pass.

## Fix Applied In This Pass

- `Sts1EventsFeatureModule` no longer declares `SPIREPLUS_STS1_EVENT_MODE` as a `DisableEnvKey`; the mode variable is handled only by `Sts1EventFeatureGate`.
- Guard tests now assert the Sts1Events mode variable cannot be treated as a FeatureRegistry disable override.

## Required Owner Action

1. Install `STS2-RitsuLib` v0.3.2+ at the active game root, currently expected under `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`.
2. Run fresh Steam-client runtime smoke with only BaseLib, STS2-RitsuLib, and Spire Plus enabled.
3. Capture `godot.log` for default Off and `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`.
4. Rerun independent QA after runtime evidence exists.

## Evidence

- Runtime prerequisite check: `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt`
- Current validation summary: `docs/reviews/current-validation.md`
- Runtime checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
