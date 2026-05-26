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
| `feature-audits/toreview-pre-slim-20260518.md` | Full retest-queue history before the active to-review file was slimmed to current manual proof gates. |
| `feature-audits/architecture-clean-code-management-audit-2026-05-19.md` | Historical architecture/clean-code audit. Superseded `EZFuturePeek` separation advice remains only as context. |
| `implementation-records/` | Compact records of completed implementation passes that should not stay in the active reading path. |
| `issues-archive.md` | Closed issue records and release-cycle traceability for previously active blocker items. |
| `project-state-history-20260516.md` | Pre-cleanup `PROJECT_STATE.md` snapshot preserving superseded per-pass validation and package history. |

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
- `implementation-records/rc1-live-validation-log-20260508-20260513.md`: historical live-validation log for older package states. Current package evidence is tracked in `docs/release-evidence-status.md` and `docs/dev-environment.md`.
- `implementation-records/website-localization-qa-20260522.md`: historical website localization/render QA journal moved out of the public website source; current website metadata is in `website/content-data.js` and `website/README.md`.
