# Historical Revision Boundaries

Status: compact active boundary index replacing the former per-file M5 Revision L/M/N stubs. Full records live under `docs/archive/legacy-planning/`; current work starts from `PROJECT_STATE.md`, `docs/goals/migration.md`, `docs/goals/event.md`, and `docs/test-ready-development-goal.md`.

## Revision L

Archived records:

- `docs/archive/legacy-planning/m5-revision-l-runtime-hard-blocker-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-runtime-smoke-plan-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-final-report-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-owner-review-packet-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-dirty-ledger-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-commit-slices-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-warning-ledger-20260610.md`

Boundary: Revision L beta.84/beta.85 planning and warning cleanup are historical. The historical log used the then-current 10 event types / 11 registration-call shape; current source expects 10 event types / 14 calls. Do not use the historical `v0.106.1` loader smokes or the red beta.84 smoke as current runtime proof. Runtime smoke does not prove gameplay.

## Revision M

Archived records:

- `docs/archive/legacy-planning/m5-revision-m-final-report-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-owner-review-packet-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-runtime-drift-report-20260618.md`
- `docs/archive/legacy-planning/m5-revision-m-patch-failure-ledger-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-version-decision-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-commit-slices-20260611.md`

Boundary: Revision M was the Off loader runtime-drift closure lane; it is not live-ready or release-ready. Accept as loader-smoke package, not gameplay/release proof. May run only after process coordination, and do not start overlapping validation lanes. Do not use beta.85 version docs as gameplay, live-ready, or release-ready proof. This should remain default-Off/staged until runtime event proof exists.

## Revision N

Archived records:

- `docs/archive/legacy-planning/m5-revision-n-final-report-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-owner-commit-packet-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-validation-replay-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-runtime-evidence-plan-20260619.md`

Boundary: Revision N is beta.88 previous-package context. Current migration truth is beta.134 RitsuLib-only. The current beta.134 RitsuLib-only routing is consolidated in `docs/goals/historical-revision-boundaries.md`. Official `STS2-RitsuLib` remains `0.4.34`. Do not use Revision N as gameplay, release, commit, push, tester-handoff, or current package proof.
