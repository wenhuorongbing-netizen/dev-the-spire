# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**BLOCKED** — STS2-RitsuLib is not installed at `<GameRoot>\mods\STS2-RitsuLib`. The installed Spire Plus package (`v0.1.0-private-beta.84`) declares a dependency on `STS2-RitsuLib >= 0.3.2`. Runtime smoke cannot proceed until STS2-RitsuLib is installed. Batch 4c is blocked until runtime smoke passes.

2026-05-31 blocker check: `Test-Path` returned `False` for `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`, `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`, `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`. No Off, CanaryOnly, or AdditiveBatch1 `godot.log` runtime smoke was captured in this pass. The source now emits additional FeatureRegistry/RewardPipeline and multiplayer-policy diagnostics, but those diagnostics still require live loader proof.

## Prerequisites

1. Clean Steam client install with Slay the Spire 2 v0.106.1
2. BaseLib v3.1.4 installed at `<GameRoot>\mods\BaseLib`
3. STS2-RitsuLib installed at `<GameRoot>\mods\STS2-RitsuLib`
4. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.N.zip` installed at `<GameRoot>\mods\EZMicroBalance`
5. No other mods enabled

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Launch game via Steam | Main menu loads without crash | [PENDING] |
| 2 | Check `godot.log` for EZMicroBalance init | Single initialization line, no errors | [PENDING] |
| 3 | Check `godot.log` for BaseLib init | BaseLib initializes before Spire Plus | [PENDING] |
| 4 | Check `godot.log` for STS2-RitsuLib init | RitsuLib initializes, no errors | [PENDING] |
| 5 | Check `godot.log` for Harmony patch count | 25 ModPatcher patches + remaining raw patches applied | [PENDING] |
| 6 | Check `godot.log` for release-blocking log hits | 0 release-blocking hits | [PENDING] |
| 7 | Check SavedSpireFields count | 30 SavedSpireFields registered | [PENDING] |

### Mod Settings UI

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Navigate to Mod Settings | Spire Plus appears in mod list | [PENDING] |
| 2 | Open Spire Plus settings | Settings UI renders without errors | [PENDING] |
| 3 | Verify feature toggles | All default-on features listed, toggles functional | [PENDING] |

### Basic Gameplay

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Start new run | Run starts without errors | [PENDING] |
| 2 | Play first combat | Combat resolves normally | [PENDING] |
| 3 | Visit first shop | Shop renders, no errors | [PENDING] |
| 4 | Check Ancient reward visibility | Default-on Ancients show rebalanced rewards | [PENDING] |
| 5 | Save and reload | Save/load succeeds, no data loss | [PENDING] |

### Multiplayer Disposition

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Attempt co-op with Spire Plus enabled | Co-op fails closed (A11-A20 disabled by default) | [PENDING] |
| 2 | Check multiplayer diagnostics log | No crash, clean fail-closed message | [PENDING] |

## Exit Criteria

- All loader smoke items pass
- All Mod Settings UI items pass
- At least 3 of 5 basic gameplay items pass (shop and save/load are mandatory)
- Multiplayer disposition confirmed fail-closed
- `godot.log` contains 0 release-blocking hits

## Notes

- This checklist supplements the existing `docs/test-plan.md` and `docs/release-checklist.md`
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/`
- If any loader smoke item fails, do not proceed to gameplay items — diagnose first
