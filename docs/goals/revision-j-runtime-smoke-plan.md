# Revision J Runtime Smoke Plan — Achieved

Date: 2026-06-02
HEAD: `8f2d79b4 (HEAD -> main, origin/main, origin/HEAD) sprint3`

## Status

Runtime smoke evidence has been collected. Both Off and CanaryOnly diagnostic sessions produced clean logs.

## Achieved Evidence

### Off Mode

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` |
| Log file | `godot.log.after-launch` |
| Audit | Clean (0 Godot ERROR, 0 TypeLoadException, 0 MissingMethodException) |
| BaseLib | 217 patches applied, 0 failed |
| RitsuLib | v0.3.10 [0.106.1], 459 patches applied, 0 failed |
| Spire Plus | 25/25 ModPatcher patches applied |
| Sts1Events | Disabled (default Off) |
| StS1 registrations | 0 |
| SavedSpireFields | 30 |

### CanaryOnly Mode

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` |
| Log file | `godot.log.after-direct-launch` |
| Audit | Clean (0 Godot ERROR, 0 TypeLoadException, 0 MissingMethodException) |
| BaseLib | 217 patches applied, 0 failed |
| RitsuLib | v0.3.10 [0.106.1], 459 patches applied, 0 failed |
| Spire Plus | 25/25 ModPatcher patches applied |
| Sts1Events | Enabled (CanaryOnly mode) |
| Canary registrations | Big Fish, Golden Idol, Lab, Divine Fountain (exactly 4) |
| SavedSpireFields | 30 |

## Non-Achieved

- Gameplay verification (live run with Ancient UI, save-load, route traversal)
- Co-op verification
- Clicked UI verification
- Versioned tester-package handoff
- Independent QA rerun
