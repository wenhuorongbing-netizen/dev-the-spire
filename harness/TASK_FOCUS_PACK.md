# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- M5 Week 1: owner-review packet + runtime hard-blocker closure + governance decisions.
- Current HEAD: `3f01cb7e (HEAD -> main, origin/main, origin/HEAD) sprint 4`.
- Runtime proof achieved; owner-review packet prepared; owner decisions are the next gates.

## Acceptance Criteria

- RitsuLib runtime dependency verified: clean Off/CanaryOnly logs, 25/25 ModPatcher patches, v0.3.10 [0.106.1].
- Manifest dependency for `STS2-RitsuLib` remains present in `EZMicroBalance.json`.
- Build: 0 errors, 89 Sts1Events nullable warnings.
- Warning ledger: no unresolved warning rows; all 89 warnings classified by file/code/owner.
- Tests: 0 failed in no-game validation (464/0/21/485).
- Patch inventory: 142 raw + 25 migrated = 167 tracked units.
- Sts1Events formal/staging/remove recommendation recorded.
- Debug accept-scaffold/feature-complete/rollback recommendation recorded.
- RitsuLib runtime-validated status recorded.
- Dirty entries reconciled; commit slices prepared for owner review.
- Runtime smoke evidence collected for Off and CanaryOnly modes.

## Blocked Scope

- Batch 4c patch migration (pending gameplay proof and owner decision).
- Batch 5 / PR7 / high-risk patch migration.
- New gameplay behavior.
- Debug expansion.
- Sts1Events formalization (blocked by 89 warnings + gameplay proof).
- Longhaul audit (blocked pending owner-review acceptance).
- Release-ready claims (pending gameplay/UI/save-load/co-op proof).

## Related Files Or Modules

- `EZMicroBalance.csproj` (RitsuLib PackageReference)
- `EZMicroBalance.json` (manifest dependency)
- `scripts/spire-plus-live-session.ps1` (runtime smoke helper)
- `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` (Off mode evidence)
- `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` (CanaryOnly evidence)
- `docs/goals/m5-week1-owner-review-packet.md`
- `docs/goals/m5-week1-runtime-hard-blocker.md`
- `docs/goals/m5-week1-runtime-smoke-plan.md`
- `docs/goals/m5-week1-dirty-ledger.md`
- `docs/goals/m5-week1-warning-ledger.md`
- `docs/goals/m5-week1-commit-slices.md`
- `docs/goals/warning-ledger.md`

## Risks

- Sts1Events warning debt remains open while the feature is default-Off/prototype-gated.
- Gameplay verification has not been performed; runtime proof covers loader/startup only.
- Co-op verification has not been performed.
- Versioned tester-package handoff has not been performed.
- 25 dirty entries need owner-approved commit slices.
