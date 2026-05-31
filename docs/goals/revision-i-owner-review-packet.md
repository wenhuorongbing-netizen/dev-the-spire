# Revision I Owner Review Packet

Date: 2026-05-31

## Verdict

Ready for owner review, not ready for commit or release.

## Current Truth

| Area | Status |
| --- | --- |
| HEAD | `87820303 sprint 1` |
| Validation | project clean/build passed; no-build tests passed after retry with 464 passed, 0 failed, 21 skipped |
| Warnings | 89 nullable warnings, all Sts1Events staging code |
| Runtime smoke | historical Revision I hard block; superseded by Revision J loader evidence that reaches main menu but is clean-audit blocked |
| Sts1Events | staging-only recommended |
| Debug | accept-scaffold recommended |
| RitsuLib | compile/manifest dependency attempted; runtime unverified |
| Batch 4c | blocked |
| Release-ready | no |

## Owner Decisions Needed

1. Approve or reject the Sts1Events gate fix that removes `SPIREPLUS_STS1_EVENT_MODE` from generic disable-key handling.
2. Decide whether to keep the large goal-doc rewrites or compact them into active status docs.
3. Decide whether Revision I artifacts should be committed as a docs packet.
4. Keep installed `STS2-RitsuLib` and resolve or disposition the current loader audit errors before runtime claims.
5. Choose whether to fix Sts1Events nullable warnings now or leave them as staging debt while default Off remains true.

## Recommended Decisions

| Topic | Recommendation |
| --- | --- |
| Sts1Events | keep staging-only |
| Debug | accept scaffold, do not expand |
| RitsuLib | keep compile/manifest integration, block runtime claims |
| Patch migration | do not start Batch 4c |
| Commit | wait for owner approval and use slices |

## Validation Evidence

Commands replayed:

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

Results:

- Clean: pass.
- Build: pass, 0 errors, 89 warnings.
- No-build tests: final rerun passed with 464 passed, 0 failed, 21 skipped, 485 total.
- Format: pass.
- Diff check: pass.
- Batch classifier: historical Revision I pass with 55 dirty entries, 0 unclassified; superseded by Revision J classifier at 49 dirty entries, 0 unclassified.

## Runtime Evidence

No live runtime evidence was captured during Revision I. Later Revision J evidence confirms `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` is installed and a loader log reaches main menu with Spire Plus, but clean-audit runtime proof remains blocked.

## Stop Condition

This packet satisfies the owner-review packet path, not the release path. Runtime proof remains the hard blocker.
