# Overnight Run Ledger — M5 Week 1

Date: 2026-06-02

## M5 Week 1 Sprint4 Summary

| Area | Finding |
| --- | --- |
| Git forensics | Current HEAD is `3f01cb7e` (sprint4); sprint3 `8f2d79b4` is historical. |
| Dirty state | 25 entries (4 modified goal docs, 21 deleted revision docs); 0 unclassified. |
| Validation replay | All 8 terminal commands pass; 0 errors, 89 warnings, 464/0/21/485 tests. |
| Warning recount | 89 warnings, all Sts1Events nullable staging debt (CS8604=54, CS8602=34, CS8625=1). |
| Patch inventory | 142 raw Harmony declarations + 25 migrated `IPatchMethod` classes = 167 tracked runtime patch units. |
| Sts1Events | staging-only recommended. Off=0 and CanaryOnly=4 runtime proof achieved. |
| Debug | accept-scaffold recommended. |
| RitsuLib | runtime-validated. Clean Off/CanaryOnly logs, 25/25 ModPatcher patches, v0.3.10 [0.106.1]. |
| Runtime | hard-blocker resolved. Clean Off and CanaryOnly diagnostic logs exist with 0 Godot ERROR hits. |
| Commit slices | prepared for owner review only; no commit authorized. |

## M5 Week 1 Runtime Evidence

- Off mode: `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` — clean audit, 0 Godot ERROR hits, Sts1Events disabled, 0 StS1 registrations, 25/25 ModPatcher patches, 30 SavedSpireFields.
- CanaryOnly mode: `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` — clean audit, 0 Godot ERROR hits, exactly 4 canary registrations (Big Fish, Golden Idol, Lab, Divine Fountain), 25/25 ModPatcher patches, 30 SavedSpireFields.

## Remaining Gates

- Gameplay verification (live run, Ancient UI, save-load, route traversal)
- Co-op verification
- Clicked UI verification
- Versioned tester-package handoff
- Independent QA rerun
- Sts1Events formalization (blocked by 89 warnings + gameplay proof)
- Debug feature completion (blocked by settings exposure, behavioral tests, Warn policy)
- Batch 4c / Batch 5 / PR7 (blocked pending gameplay proof and owner decision)
- Longhaul audit (blocked pending owner-review acceptance and governance decisions)

---

# Historical Overnight Run Ledger — Revision J Sprint3 (2026-06-02)

| Area | Finding |
| --- | --- |
| Git forensics | HEAD `8f2d79b4` (sprint3). |
| Dirty state | Worktree clean (0 entries). |
| Validation replay | All 8 terminal commands pass; 0 errors, 89 warnings, 464/0/21/485 tests. |
| RitsuLib | runtime-validated. |
| Runtime | hard-blocker resolved. |

---

# Historical Overnight Run Ledger — Revision J Sprint2 (2026-05-31)

| Area | Finding |
| --- | --- |
| Git forensics | HEAD `6b149ba0` (sprint2). |
| Dirty state | 49 dirty entries, 0 unclassified. |
| RitsuLib | compile/manifest attempted; runtime unverified. |
| Runtime | hard blocked; 11 Godot ERROR hits. |

---

# Historical Overnight Run Ledger — M3 Week 1 (2026-05-29)

Agent: Kilo (mimo-v2.5-pro)
Spec: `docs/goals/debug.md` M3 Week 1 Commit Readiness Gate

## Final Verdict

```text
NOT COMPLETE at M3/M4. Runtime hard blocker resolved at sprint3 (2026-06-02). M5 Week 1 owner-review packet prepared at sprint4 (2026-06-02).
```
