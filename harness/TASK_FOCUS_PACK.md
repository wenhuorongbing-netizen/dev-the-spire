# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- M5 Revision L: owner-review packet + runtime hard-blocker closure + governance decisions.
- Current baseline HEAD: `f32c6767 (HEAD -> main, origin/main, origin/HEAD) update refactor.md with implementation results and Green Stop check`.
- The old missing-runtime-folder blocker is closed for local review. Historical diagnostic loader proof exists for the `v0.106.1` setup; the current local game is `v0.107.0`, and official RitsuLib `v0.4.16` with `lib\0.107.0` is installed.

## Acceptance Criteria

- RitsuLib historical loader-gate verified only: clean Off/CanaryOnly/AdditiveBatch1 logs, 25/25 ModPatcher patches, historical v0.3.10 / `lib\0.106.1` evidence.
- Manifest dependency for `STS2-RitsuLib` remains present in `EZMicroBalance.json`.
- Build: 0 errors, 0 warnings.
- Warning ledger: prior 70-warning ledger is superseded by the current zero-warning build; keep it only as historical triage context.
- Tests: 0 failed in current no-game validation; test-project and exact solution-level no-build lanes both report 464/0/21/485.
- Patch inventory: 142 raw + 25 migrated = 167 tracked units.
- Sts1Events formal/staging/remove recommendation recorded.
- Debug accept-scaffold/feature-complete/rollback recommendation recorded.
- RitsuLib status recorded as historical loader-gate validated, not current `v0.107.0` runtime-validated.
- Dirty entries reconciled; commit slices prepared for owner review.
- Runtime smoke evidence collected historically for Off, CanaryOnly, and AdditiveBatch1 diagnostic modes.
- Current runtime proof remains blocked until fresh `v0.107.0` loader smoke is captured with installed RitsuLib `v0.4.16` / `lib\0.107.0` and installed-package parity is resolved.

## Blocked Scope

- Batch 4c patch migration (pending gameplay proof and owner decision).
- Batch 5 / PR7 / high-risk patch migration.
- New gameplay behavior.
- Debug expansion.
- Sts1Events formalization (blocked by gameplay/render/save-load/image/replacement/multiplayer proof, not current build warnings).
- Longhaul audit (blocked pending owner-review acceptance).
- Release-ready claims (pending gameplay/UI/save-load/co-op proof).
- Current-runtime-ready claims (pending fresh `v0.107.0` loader smoke and installed-package parity).

## Related Files Or Modules

- `EZMicroBalance.csproj` (RitsuLib PackageReference)
- `EZMicroBalance.json` (manifest dependency)
- `scripts/spire-plus-live-session.ps1` (runtime smoke helper)
- `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` (Off mode evidence)
- `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` (CanaryOnly evidence)
- `docs/goals/m5-revision-l-owner-review-packet.md`
- `docs/goals/m5-revision-l-runtime-hard-blocker.md`
- `docs/goals/m5-revision-l-runtime-smoke-plan.md`
- `docs/goals/m5-revision-l-dirty-ledger.md`
- `docs/goals/m5-revision-l-warning-ledger.md`
- `docs/goals/m5-revision-l-commit-slices.md`
- `docs/goals/warning-ledger.md`

## Risks

- Sts1Events warning debt remains open while the feature is default-Off/prototype-gated.
- Gameplay verification has not been performed; runtime proof covers historical loader/startup only.
- Current `v0.107.0` runtime compatibility has not been proven.
- Co-op verification has not been performed.
- Versioned tester-package handoff has not been performed.
- Dirty source/docs/test entries need owner-approved commit slices.
