# Runtime Smoke Checklist

## Purpose

Verify that the RitsuLib migration does not change runtime behavior by loading the game with Spire Plus enabled and checking for regressions in the loader log, Mod Settings UI, and basic gameplay flow.

## Status

**HARD BLOCKED** — STS2-RitsuLib is not installed at `<GameRoot>\mods\STS2-RitsuLib`. The installed Spire Plus package (`v0.1.0-private-beta.84`) declares a dependency on `STS2-RitsuLib >= 0.3.2`. Runtime smoke cannot proceed until STS2-RitsuLib is installed. Batch 4c, high-risk patch migration, Off/CanaryOnly runtime claims, live-ready, and release-ready claims remain blocked.

2026-05-31 Runtime Proof + Governance Closure check:

| Path | Result |
| --- | --- |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Missing |

No Off, CanaryOnly, or AdditiveBatch1 `godot.log` runtime smoke was captured. The source emits additional FeatureRegistry/RewardPipeline, CardPlayContext, DeathProtection, and multiplayer-policy diagnostics, but those diagnostics remain source-level until live loader proof exists.

## Prerequisites

1. Clean Steam client install with Slay the Spire 2 v0.106.1
2. BaseLib v3.1.4 installed at `<GameRoot>\mods\BaseLib`
3. STS2-RitsuLib v0.3.2+ installed at `<GameRoot>\mods\STS2-RitsuLib`
4. Spire Plus package from `publish/SpirePlus-v0.1.0-private-beta.84.zip` installed at `<GameRoot>\mods\EZMicroBalance`
5. No other mods enabled
6. If using `scripts\spire-plus-live-session.ps1`, invoke it with the E-drive `-GameRoot` and `-SteamExe`, pass the chosen `-SteamUserId`, and ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Checklist

### Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.3.2` | [BLOCKED] Missing locally |
| 2 | Launch game via Steam | Main menu loads without crash | [PENDING] |
| 3 | Check `godot.log` for EZMicroBalance init | Single Spire Plus initialization line, no errors | [PENDING] |
| 4 | Check `godot.log` for BaseLib init | BaseLib initializes before Spire Plus | [PENDING] |
| 5 | Check `godot.log` for STS2-RitsuLib init | RitsuLib initializes, no errors | [PENDING] |
| 6 | Check `godot.log` for RitsuLib bootstrap | Spire Plus RitsuLib bootstrap starts | [PENDING] |
| 7 | Check `godot.log` for ModPatcher count | 25 ModPatcher patches applied; remaining raw Harmony patches load without dependency failures | [PENDING] |
| 8 | Check `godot.log` for release-blocking log hits | 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failure, or release-blocking audit hits | [PENDING] |
| 9 | Check SavedSpireFields count | 30 SavedSpireFields registered | [PENDING] |

### Sts1Events Runtime Gates

| Mode | Required env | Expected | Evidence |
| --- | --- | --- | --- |
| Off | unset / empty / invalid `SPIREPLUS_STS1_EVENT_MODE` | 0 Sts1Events registrations, no `[StS1 Events]` registration lines | [BLOCKED] Missing STS2-RitsuLib/runtime log |
| CanaryOnly | `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` | Exactly 4 canary registrations: Big Fish, Golden Idol, The Lab, Divine Fountain | [BLOCKED] Missing STS2-RitsuLib/runtime log |
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | Controlled prototype only: 11 registration calls / 10 event types, no TODO/BLOCKED events | [PENDING] Only after Off + CanaryOnly smoke passes |
| AdditiveAllDraft | `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft` | Not release-safe; dev-only all-draft mode includes TODO/BLOCKED content | [DO NOT USE for tester/release paths] |
| ReplaceUnknownEventsPrototype | `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` plus compile symbol | Not release-safe; debug-only replacement prototype | [DO NOT USE for tester/release paths] |

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
| 1 | Attempt co-op with Spire Plus enabled | Co-op fails closed for unverified shared-state gameplay | [PENDING] |
| 2 | Check multiplayer diagnostics log | No crash, clean fail-closed message | [PENDING] |

## Exit Criteria

- All loader smoke items pass
- Off mode proves 0 Sts1Events registrations in `godot.log`
- CanaryOnly proves exactly 4 canary registrations in `godot.log`
- Mod Settings UI verified
- At least 3 of 5 basic gameplay items pass, with shop and save/load mandatory
- Multiplayer disposition confirmed fail-closed
- `godot.log` contains 0 release-blocking hits

## Notes

- This checklist supplements `docs/test-plan.md` and `docs/release-checklist.md`.
- Evidence should be captured as screenshots or log excerpts and stored in `docs/evidence/` or a documented runtime-evidence folder.
- If any loader smoke item fails, do not proceed to gameplay items; diagnose first.
- Runtime smoke is the Batch 4c decision gate. No Batch 4c patch migration is allowed while this checklist is blocked.
