# Revision J Dirty Ledger

Date: 2026-05-31
HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`

## Final Revision J Dirty State

After Revision J packet and concurrent runtime-hardening updates, `git status --short --branch --untracked-files=all` reports 42 modified tracked files and 7 untracked owner-review/status docs.

The final batch classifier reported 49 dirty entries and 0 unclassified:

| Batch | Count | Meaning |
|---|---:|---|
| 1 | 7 | Status and release docs |
| 2 | 6 | Governance and architecture docs |
| 3 | 10 | Ancient/Sts1Events source, tests, and docs |
| 5 | 9 | RitsuLib runtime docs, scripts, and validation tests |
| 8 | 17 | Goal docs and Revision I/J artifacts |
| -1 | 0 | Unclassified |

## Decisions

| Group | Decision | Risk | Rollback |
|---|---|---|---|
| Runtime dependency docs | Keep for owner review; they record installed RitsuLib and non-clean loader proof | Medium, can be mistaken for runtime-ready | Revert docs to previous dependency-missing blocker wording |
| Live-session helper script | Keep for owner review; preserves `STS2-RitsuLib` during `-MoveOtherMods` smoke | Medium, changes local smoke helper behavior | Restore previous allowed mod list and do not use `-MoveOtherMods` for RitsuLib smoke |
| Sts1Events docs | Keep for owner review; blocker is now clean-audit/runtime proof, not dependency path | Low-medium | Restore old blocker wording |
| Goal docs | Keep for owner review only; high-churn governance material | High docs churn | Compact or revert selected goal docs |
| Revision I artifacts | Treat as historical artifacts with later local-state corrections | Medium history-blurring risk | Restore Revision I files if owner wants immutable historical packets |

## Revision J Artifact Decision

The required `docs/goals/revision-j-*.md` files are owner-review artifacts. They should remain uncommitted until owner explicitly authorizes commit slices. If `git status` shows them as untracked, the decision is keep/uncommitted for owner review, not delete and not commit automatically.

## Commit Readiness

Not commit-ready without owner approval. No commit, push, stash, checkout, reset, restore, or broad clean is authorized.
