# Refactor Overnight QA — 2026-05-31

## Verdict

FAIL / HARD BLOCKED. Green Stop is not allowed.

## Independent Audit Results

- Runtime proof is absent. The original QA saw `STS2-RitsuLib` missing at the checked D-drive and E-drive game-root mod paths; the dependency is now installed on E-drive, and v15 captured a loader log, but the audit is not clean.
- Active `godot.log` evidence now exists for RitsuLib bootstrap and StS1Events default-Off bootstrap state, but it has 11 `Godot ERROR` hits and does not close Off=0, CanaryOnly=4, gameplay, save-load, replacement, or multiplayer proof.
- Batch 4c, high-risk migration, new gameplay expansion, runtime-safe, live-ready, and release-ready claims remain blocked.
- Warning debt is tracked as 89 Sts1Events nullable warnings with code-level counts and per-file triage.
- Sts1Events source-level governance is mostly guarded, but the audit found `SPIREPLUS_STS1_EVENT_MODE` was incorrectly wired as a generic feature disable override before this pass.

## Fix Applied In This Pass

- `Sts1EventsFeatureModule` no longer declares `SPIREPLUS_STS1_EVENT_MODE` as a `DisableEnvKey`; the mode variable is handled only by `Sts1EventFeatureGate`.
- Guard tests now assert the Sts1Events mode variable cannot be treated as a FeatureRegistry disable override.

## Required Owner Action

1. Keep `STS2-RitsuLib` v0.3.10 installed at the active game root under `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`.
2. Resolve or explicitly disposition the v15 loader audit errors, then rerun fresh Steam-client runtime smoke with only previous package, STS2-RitsuLib, and Spire Plus enabled.
3. Capture clean/accepted `godot.log` evidence for default Off and `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`.
4. Rerun independent QA after clean/accepted runtime evidence exists.

## Evidence

- Runtime prerequisite check: `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt`
- Current validation summary: `docs/reviews/current-validation.md`
- Runtime checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
