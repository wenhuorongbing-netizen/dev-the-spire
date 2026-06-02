# M5 Week 1 Runtime Hard Blocker — Resolved

Date: 2026-06-02
HEAD: `3f01cb7e (HEAD -> main, origin/main, origin/HEAD) sprint 4`
Spec: `docs/goals/debug.md` M5 July 2026

## Status

**Resolved.** Clean Off and CanaryOnly runtime evidence exists from sprint3.

## Resolution

The runtime hard blocker (missing STS2-RitsuLib runtime and clean godot.log) was resolved at sprint3 (`8f2d79b4`). Two controlled diagnostic sessions produced clean loader logs with 0 Godot ERROR hits.

### Off Mode Evidence

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` |
| Audit result | Clean (0 Godot ERROR, 0 TypeLoadException, 0 MissingMethodException) |
| RitsuLib version | 0.3.10 [compat branch: 0.106.1] |
| Spire Plus ModPatcher | 25 patches applied (25 registered) |
| Sts1Events bootstrap | Disabled (default Off) |
| StS1 registration lines | 0 |
| SavedSpireFields | 30 |

### CanaryOnly Mode Evidence

| Item | Value |
|---|---|
| Evidence dir | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` |
| Audit result | Clean (0 Godot ERROR, 0 TypeLoadException, 0 MissingMethodException) |
| RitsuLib version | 0.3.10 [compat branch: 0.106.1] |
| Spire Plus ModPatcher | 25 patches applied (25 registered) |
| Sts1Events bootstrap | Enabled (CanaryOnly mode) |
| Canary registrations | Big Fish, Golden Idol, Lab, Divine Fountain (exactly 4) |
| SavedSpireFields | 30 |

## Remaining Gates

Runtime hard-blocker closure does not mean release-ready:

- Gameplay verification (live run, Ancient UI, save-load, route traversal)
- Co-op verification
- Clicked UI verification
- Versioned tester-package handoff
- Independent QA rerun
