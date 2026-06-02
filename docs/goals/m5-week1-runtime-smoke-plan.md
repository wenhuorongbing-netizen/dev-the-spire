# M5 Week 1 Runtime Smoke Plan — Achieved

Date: 2026-06-02
HEAD: `3f01cb7e (HEAD -> main, origin/main, origin/HEAD) sprint 4`
Spec: `docs/goals/debug.md` M5 July 2026

## Status

Runtime smoke evidence has been collected at sprint3. Both Off and CanaryOnly diagnostic sessions produced clean logs.

## Achieved Evidence

### Off Mode

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` |
| Audit | Clean (0 Godot ERROR) |
| RitsuLib | v0.3.10 [0.106.1], all patches applied |
| Spire Plus | 25/25 ModPatcher patches |
| Sts1Events | Disabled (default Off) |
| StS1 registrations | 0 |
| SavedSpireFields | 30 |

### CanaryOnly Mode

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` |
| Audit | Clean (0 Godot ERROR) |
| RitsuLib | v0.3.10 [0.106.1], all patches applied |
| Spire Plus | 25/25 ModPatcher patches |
| Sts1Events | Enabled (CanaryOnly mode) |
| Canary registrations | Big Fish, Golden Idol, Lab, Divine Fountain (exactly 4) |
| SavedSpireFields | 30 |

## Non-Achieved

- Gameplay verification (live run with Ancient UI, save-load, route traversal)
- Co-op verification
- Clicked UI verification
- Versioned tester-package handoff
- Independent QA rerun
