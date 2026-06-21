# Overnight Run Ledger

Date: 2026-06-10

Revision M supersession note, 2026-06-11: this ledger is historical Revision L owner-review context. Current beta.85 has clean `v0.107.0` default-Off loader proof only; current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, clean-worktree, and release-ready proof remain pending. Use `PROJECT_STATE.md`, `docs/goals/event.md`, and the Revision M docs for current proof claims.

## Timeline

| Step | Result |
|---|---|
| Read current state docs | `PROJECT_STATE.md`, `docs/test-ready-development-goal.md`, active goal docs, harness docs, RitsuLib docs reviewed. |
| Git and runtime dependency audits | Confirmed dirty worktree, installed RitsuLib/previous package/EZMicroBalance paths, and stale Week 1 references. |
| Build/API check | Initial stale API concern was resolved by building against the installed target DLL; current project build passes. |
| Warning recount | Initial pass counted 70 unique Sts1Events nullable warnings after AdditiveBatch1 owner guards; later expanded owner guards cleared the current build to 0 warnings. |
| Patch inventory audit | `generate-patch-inventory.ps1 -Check` reported stale row-level inventory; regenerated the document. |
| Localization audit | Static EN/ZHS localization pairing is clean; runtime render proof remains pending. |
| Revision L docs | Created owner-review, runtime blocker, smoke plan, dirty ledger, warning ledger, commit slices, and final report docs. |

## Notes

- Some uploaded/debug source text still contains historical `87820303`, missing-RitsuLib, and 89-warning context. Current packet docs supersede that historical input.
- No destructive git operation was used.
- No game launch was attempted.
