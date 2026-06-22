# Archive

This folder preserves historical design, planning, and prompt material.

Archived documents are useful for context, but they are not current implementation truth unless a current document explicitly points back to them. Prefer current files under `docs/`, `docs/features/ancients-rework-v4/`, and `docs/features/ascension-11-20/` for release decisions.

## Layout

| Folder | Contents |
| --- | --- |
| `legacy-planning/` | Pre-`EZMicroBalance` planning, old Ancient reward research, old Ascension 11-30 roadmap, and future custom character concepts. |
| `superseded/` | Former current-facing docs that have been replaced by clearer indexes, checklists, feature records, or current setup docs. |
| `prompts/2026-05/` | Nightly/overhaul prompt files and one-off issue addenda archived during the 2026-05 doc-operations pass. |
| `feature-inputs/` | Historical feature prompts, superseded implementation specs, and old source-design inputs moved out of active feature folders. |
| `feature-audits/ancient-expansion-v2.2/2026-05-13/` | Historical v2.2 overnight source-audit matrices. Current work should start from `docs/test-ready-development-goal.md`, not this archive. |
| `feature-audits/review-pre-slim-20260518.md` | Full source-review history before the active review file was slimmed to current findings. |
| `feature-audits/review-2026-05-26-beta54-pass-history.md` | Full 2026-05-26 beta.41-beta.54 source/package fixed-finding history moved out of the active review file. |
| `feature-audits/current-validation-full-20260622.md` | Full historical validation ledger moved out of active `docs/reviews/current-validation.md`. |
| `feature-audits/event-goal-full-20260622.md` | Full historical StS1 event-goal ledger moved out of active `docs/goals/event.md`. |
| `feature-audits/overnight-run-20260529.md` | Historical StS1 overnight no-game/source-governance report; active `docs/reviews/` file is a compact guarded stub only. |
| `feature-audits/refactor-qa-20260602.md` and `feature-audits/refactor-qa-20260602-round2.md` | Historical StS1 loader-gate QA reports; active `docs/reviews/` files are compact guarded stubs only. |
| `feature-audits/red-team-goal-implementation-pass-1-20260520.md` | Full May 20 red-team goal-completion review; active `docs/reviews/` file is a compact boundary stub only. |
| `feature-audits/toreview-pre-slim-20260518.md` | Full retest-queue history before the active to-review file was slimmed to current manual proof gates. |
| `feature-audits/architecture-clean-code-management-audit-2026-05-19.md` | Historical architecture/clean-code audit. Superseded `EZFuturePeek` separation advice remains only as context. |
| `implementation-records/` | Compact records of completed implementation passes that should not stay in the active reading path. |
| `issues-archive.md` | Closed issue records and release-cycle traceability for previously active blocker items. |
| `project-state-history-20260516.md` | Pre-cleanup `PROJECT_STATE.md` snapshot preserving superseded per-pass validation and package history. |
| `legacy-planning/m5-revision-l-*-20260610.md` | Historical beta.84/beta.85 Revision L owner-review packet, dirty ledger, runtime blocker/smoke plan, warning ledger, commit slices, and final report. |
| `legacy-planning/m5-revision-m-*-20260611.md` and `legacy-planning/m5-revision-m-runtime-drift-report-20260618.md` | Historical beta.85/beta.87 Revision M owner packet, runtime drift report, patch-failure ledger, version decision, commit slices, and final report. |
| `legacy-planning/m5-revision-n-*-20260619.md` | Historical beta.88 previous-package owner packet, validation replay, runtime-evidence plan, and final report. Current migration truth is beta.108 RitsuLib-only. |

## Rules

- Archive instead of deleting useful historical research.
- Add a short historical note when moving a document here.
- Do not leave release-critical TODOs only in archived files.
- Do not cite archive files in release docs without also naming the current replacement or status document.

## Superseded Setup Records

- `superseded/setup-spec-original-scaffold.md`: original `EzDailyContent` setup specification. It is retained for setup archaeology only; current setup and release status are in `README.md`, `docs/README.md`, `docs/dev-environment.md`, and `docs/test-plan.md`.
- `project-state-history-20260516.md`: historical `PROJECT_STATE.md` snapshot retained after the active file was reduced to current status, blockers, commands, and next action.
- `implementation-records/2026-05-15-current-package-verification-note.md`: historical long-form package verification note archived from `docs/issues.md`; current blockers remain in `docs/issues.md` and `docs/issues/waiting-tests.md`.
- `legacy-planning/legacy-project-files/`: preserved old `EzDailyContent.csproj` metadata and migration note moved out of the repository root during cleanup.
- `legacy-planning/m5-revision-l-runtime-hard-blocker-20260610.md`, `legacy-planning/m5-revision-l-runtime-smoke-plan-20260610.md`, `legacy-planning/m5-revision-l-final-report-20260610.md`, `legacy-planning/m5-revision-l-owner-review-packet-20260610.md`, `legacy-planning/m5-revision-l-dirty-ledger-20260610.md`, `legacy-planning/m5-revision-l-commit-slices-20260610.md`, and `legacy-planning/m5-revision-l-warning-ledger-20260610.md`: previous beta.84/beta.85 planning records; current no-overclaim routing is consolidated in `docs/goals/historical-revision-boundaries.md`.
- `legacy-planning/m5-revision-m-final-report-20260611.md`, `legacy-planning/m5-revision-m-owner-review-packet-20260611.md`, `legacy-planning/m5-revision-m-runtime-drift-report-20260618.md`, `legacy-planning/m5-revision-m-patch-failure-ledger-20260611.md`, `legacy-planning/m5-revision-m-version-decision-20260611.md`, and `legacy-planning/m5-revision-m-commit-slices-20260611.md`: previous beta.85/beta.87 planning records; current no-overclaim routing is consolidated in `docs/goals/historical-revision-boundaries.md`.
- `legacy-planning/m5-revision-n-final-report-20260619.md`, `legacy-planning/m5-revision-n-owner-commit-packet-20260619.md`, `legacy-planning/m5-revision-n-validation-replay-20260619.md`, and `legacy-planning/m5-revision-n-runtime-evidence-plan-20260619.md`: previous beta.88 previous-package planning records; current beta.108 RitsuLib-only routing is consolidated in `docs/goals/historical-revision-boundaries.md`.
- `implementation-records/rc1-live-validation-log-20260508-20260513.md`: historical live-validation log for older package states. Current package evidence is tracked in `docs/release-evidence-status.md` and `docs/dev-environment.md`.
- `implementation-records/website-localization-qa-20260522.md`: historical website localization/render QA journal moved out of the public website source; current website metadata is in `website/content-data.js` and `website/README.md`.
