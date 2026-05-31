# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- Runtime Proof + Governance Closure. Runtime verification is the gate; continued PR 6/Batch 4c migration is blocked.

## Acceptance Criteria

- RitsuLib compile/package dependency remains present.
- Manifest dependency for STS2-RitsuLib remains present.
- Build: 0 errors, current warning count recaptured from clean build.
- Tests: 0 failed in no-game validation.
- Runtime: STS2-RitsuLib installed and fresh `godot.log` captured.
- Off mode: 0 Sts1Events registrations proven in `godot.log`.
- CanaryOnly mode: exactly 4 canary registrations proven in `godot.log`.
- Format and `git diff --check` clean before handoff.

## Blocked Scope

- Batch 4c patch migration.
- High-risk patch migration.
- New gameplay behavior.
- Runtime-ready, live-ready, or release-ready claims.

## Related Files Or Modules

- `EZMicroBalance.csproj` (RitsuLib PackageReference)
- `EZMicroBalance.json` (manifest dependency)
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md`
- `docs/features/ritsulib-migration/monthly-dev-spec.md`
- `docs/reviews/current-validation.md`
- `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md`

## Risks

- STS2-RitsuLib is not installed in the active E-drive game root.
- Runtime ModPatcher behavior for 25 migrated patches is unverified.
- Sts1Events warning debt remains open while the feature is default Off and prototype-gated.
