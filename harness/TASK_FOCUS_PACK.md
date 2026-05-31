# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- Revision J runtime hard-blocker closure and owner-review packet.
- Current HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`.
- Runtime verification is the gate; Batch 4c, Batch 5, PR7, high-risk migration, debug expansion, Sts1Events formalization, and longhaul audit remain blocked.

## Acceptance Criteria

- RitsuLib compile dependency remains present in `EZMicroBalance.csproj`.
- Manifest dependency for `STS2-RitsuLib` remains present in `EZMicroBalance.json`.
- Runtime dependency paths are verified: game root, BaseLib, STS2-RitsuLib, EZMicroBalance, runtime DLLs, and active `godot.log` availability/absence.
- Build: 0 errors, warning count recaptured from clean build.
- Warning ledger: no unresolved warning rows; all 89 warnings classified by file/code/owner.
- Tests: 0 failed in no-game validation.
- Patch inventory: raw/migrated/tracked unit relationship explained.
- Sts1Events formal/staging/remove recommendation recorded.
- Debug accept-scaffold/feature-complete/rollback recommendation recorded.
- RitsuLib attempted/runtime-validated/release-ready/rollback status recorded.
- Dirty entries and untracked files have owner-review decisions.
- Commit slices exist; no unauthorized commit.
- Runtime smoke remains blocked unless fresh `godot.log` proves Off=0 and CanaryOnly=4.

## Blocked Scope

- Batch 4c patch migration.
- Batch 5 / PR7 / high-risk patch migration.
- New gameplay behavior.
- Debug expansion.
- Sts1Events formalization.
- Longhaul audit.
- Runtime-ready, live-ready, or release-ready claims.

## Related Files Or Modules

- `EZMicroBalance.csproj` (RitsuLib PackageReference)
- `EZMicroBalance.json` (manifest dependency)
- `scripts/spire-plus-live-session.ps1` (runtime smoke helper)
- `docs/goals/revision-j-owner-review-packet.md`
- `docs/goals/revision-j-runtime-hard-blocker.md`
- `docs/goals/revision-j-runtime-smoke-plan.md`
- `docs/goals/revision-j-dirty-ledger.md`
- `docs/goals/revision-j-commit-slices.md`
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
- `docs/features/ritsulib-migration/monthly-dev-spec.md`
- `docs/reviews/current-validation.md`
- `docs/goals/warning-ledger.md`

## Risks

- Active `godot.log` is missing, so RitsuLib runtime behavior for 25 migrated patches is unverified.
- Process-level `SPIREPLUS_STS1_EVENT_MODE` may not propagate through an already-running Steam client.
- Sts1Events warning debt remains open while the feature is default-Off/prototype-gated.
- Dirty worktree state includes docs/scripts/tests changes that need owner-approved slicing before any commit.
