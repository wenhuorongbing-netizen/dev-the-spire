# Documentation Inventory

## A. Current entrypoints

- `README.md`
- `AGENTS.md`
- `PROJECT_STATE.md`
- `docs/README.md`
- `docs/PROJECT_MAP.md`
- `docs/worktree-cleanup-audit.md`
- `docs/issues.md`
- `docs/patch-inventory.md`
- `docs/release-evidence-status.md`
- `docs/specs/release-scope-v1.md`
- `docs/specs/release-traceability-matrix.md`
- `docs/specs/website-claim-audit.md`
- `docs/intro.zh.md`
- `docs/month-plan/baseline-2026-05-20.md`
- `docs/month-plan/commit-boundaries.md`
- `docs/test-ready-development-goal.md`
- `docs/features/ancient-expansion-v2.2/README.md`
- `docs/features/ritsulib-migration/README.md` (single active RitsuLib migration entry point; support files remain guarded but are not default reading path)
- `website/README.md`

Current support docs, not default reading path:

- `docs/dev-environment.md`
- `docs/release-checklist.md`
- `docs/private-beta-verification-handoff.md`
- `docs/private-beta-release-completion-audit.md`
- `docs/test-ready-completion-audit.md`
- `docs/issues/waiting-tests.md`
- `docs/adr/0000-template.md`
- `docs/source-research/run-room-event-reward.md`
- `docs/source-research/multiplayer-save-rng.md`
- `docs/architecture/bounded-contexts.md`
- `docs/architecture/patch-boundaries.md`
- `docs/architecture/save-state-contracts.md`
- `docs/migration.md` (compatibility stub only; migration work routes through `docs/features/ritsulib-migration/README.md`, `docs/goals/migration.md`, `docs/integrations/ritsulib.md`, and patch counts in `docs/patch-inventory.md`)
- `docs/goals/sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md` (compact historical-boundary stub; current StS1 event work routes through `docs/goals/event.md`)
- `.github/workflows/spire-plus-site.yml`

## B. Feature docs

- `docs/features/ancients-rework-v4/`
- `docs/features/ascension-11-20/`
- `docs/features/ancient-expansion-urda/` (Urda support evidence; current combined Ancient docs override conflicts)
- `docs/features/ancient-expansion-v2.2/` (current roadmap/prototype docs; Urda, Morvi, Lotha, and the single-player Vakuu fight are source-active/live-pending)
- `docs/features/ancients-rework-v4/reference-inputs/` (traceability input only; do not implement from it without checking current issues/goal/source-design)
- `docs/features/preview-tools/` (Crystal Sphere peek and transform preview integrated into Spire Plus)
- `docs/features/ritsulib-migration/` (RitsuLib-only migration entry and guarded runtime/migration support files)

## C. Style / agent docs

- `docs/style/card-localization-style-guide.md`
- `docs/skills/sts2-godot-mod-development.md`
- `docs/skills` (current skill notes and references)
- `docs/adr/` (architecture decision records; start new decisions from `0000-template.md`)

## C2. Public website

- `website/README.md`
- `website/index.html`
- `website/content-data.js`
- `website/styles.css`
- `website/app.js`
- `.github/workflows/spire-plus-site.yml`

## D. Archive

- `docs/archive/README.md`
- `docs/archive/legacy-planning/`
- `docs/archive/legacy-planning/legacy-project-files/`
- `docs/archive/superseded/`
- `docs/archive/superseded/setup-spec-original-scaffold.md`
- `docs/archive/issues-archive.md`
- `docs/archive/prompts/2026-05/`
- `docs/archive/feature-inputs/`
- `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`
- `docs/archive/feature-audits/current-validation-full-20260622.md`
- `docs/archive/feature-audits/event-goal-full-20260622.md`
- `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`
- `docs/archive/feature-audits/overnight-run-20260529.md`
- `docs/archive/feature-audits/refactor-qa-20260602.md`
- `docs/archive/feature-audits/refactor-qa-20260602-round2.md`
- `docs/archive/feature-audits/red-team-goal-implementation-pass-1-20260520.md`
- `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md`
- `docs/archive/implementation-records/`
- `docs/archive/project-state-history-20260516.md`
- `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md`
- `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`
- `docs/archive/implementation-records/website-localization-qa-20260522.md`
- `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`
- `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`
- `docs/archive/implementation-records/bugfix-notes-seedbed-draw-reentrancy-20260527.md`
- `docs/archive/implementation-records/bugfix-report-seedbed-draw-reentrancy-20260527.md`
- `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md`
- `docs/archive/feature-inputs/sts1-event-port-strict-audit-monthly-spec-v5-overnight-subagents-20260620.md`
- `docs/archive/legacy-planning/m5-revision-l-runtime-hard-blocker-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-runtime-smoke-plan-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-final-report-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-owner-review-packet-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-dirty-ledger-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-commit-slices-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-warning-ledger-20260610.md`
- `docs/archive/legacy-planning/m5-revision-m-final-report-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-owner-review-packet-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-runtime-drift-report-20260618.md`
- `docs/archive/legacy-planning/m5-revision-m-patch-failure-ledger-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-version-decision-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-commit-slices-20260611.md`
- `docs/archive/legacy-planning/m5-revision-n-final-report-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-owner-commit-packet-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-validation-replay-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-runtime-evidence-plan-20260619.md`

## E. Clutter candidates handled

- `docs/codex-repo-overhaul-refactor-prompt.md` -> `docs/archive/prompts/2026-05/codex-repo-overhaul-refactor-prompt.md`
- `docs/codex-urda-overnight-prompt.md` -> `docs/archive/prompts/2026-05/codex-urda-overnight-prompt.md`
- `docs/issues-urda-overnight-addendum.md` -> `docs/archive/prompts/2026-05/issues-urda-overnight-addendum.md`
- `docs/issues-waiting-tests.md` moved to `docs/issues/waiting-tests.md`
- `docs/features/ancient-expansion-v2.2/next-development-prompt.md` -> `docs/archive/prompts/2026-05/codex-ancient-expansion-v22-next-development-prompt.md`
- `docs/features/ancient-expansion-v2.2/audit/` -> `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`
- Completed source/test-ready implementation summary -> `docs/archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md`
- Historical original scaffold setup spec -> `docs/archive/superseded/setup-spec-original-scaffold.md`
- `docs/features/ancients-rework-v4/archive/` -> `docs/archive/feature-inputs/ancients-rework-v4/`
- `docs/features/ascension-11-20/archive/` -> `docs/archive/feature-inputs/ascension-11-20/`
- root `art_pipeline/` and `asset/` local generated/calibration folders -> ignored `.tools/archive/local-art-and-calibration-20260515/`
- root local-only package/source-analysis/website zip clutter -> ignored `.tools/archive/local-root-clutter-20260515/`
- older website draft snapshot -> `.tools/archive/local-website-preview-20260516/`; current public site source is tracked under root `website/` with `.github/workflows/spire-plus-site.yml`, while generated `website/forum/` output is ignored.
- superseded `PROJECT_STATE.md` per-pass validation history -> `docs/archive/project-state-history-20260516.md`
- superseded `docs/issues.md` long-form 2026-05-15 package verification note -> `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md`
- top-level `legacy/` migration project metadata -> `docs/archive/legacy-planning/legacy-project-files/`
- top-level `EzDailyContent*` and `EZFuturePeek*` mod surfaces removed from the active root; the single active deliverable is `Spire Plus`.
- stale root architecture audit with superseded `EZFuturePeek` separation advice -> `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md`
- historical RC1 live-validation log for older package states -> `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`
- historical runtime-smoke detail from active `docs/dev-environment.md` -> `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`
- `docs/介绍.md` -> `docs/intro.zh.md` to keep the Chinese intro active while avoiding quoted non-ASCII paths in Git/script output.
- root `web_issue.md` -> `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`; current forum follow-up remains in `website/web_issue.md` and `docs/features/forum/`.
- `website/localization_qa.md` -> `docs/archive/implementation-records/website-localization-qa-20260522.md`; current website package/download state is guarded by `website/content-data.js`, `website/README.md`, and website tests.
- Detailed 2026-05-26 source/package fixed-finding list from `docs/review.md` -> `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`; the later pre-compact current fixed-finding ledger now lives at `docs/archive/feature-audits/review-current-fixed-findings-history-20260622.md`. Current review now keeps only current conclusions, migration evidence, and manual-proof focus.
- Historical StS1 overnight no-game/source-governance report `docs/reviews/overnight-run-20260529.md` -> `docs/archive/feature-audits/overnight-run-20260529.md`; active review file is a compact guarded stub retaining no-overclaim boundaries.
- Historical StS1 loader-gate QA reports `docs/reviews/refactor-qa-20260602.md` and `docs/reviews/refactor-qa-20260602-round2.md` -> `docs/archive/feature-audits/refactor-qa-20260602.md` and `docs/archive/feature-audits/refactor-qa-20260602-round2.md`; active review files are compact guarded stubs retaining no-overclaim boundaries.
- Historical May 20 goal-completion red-team review `docs/reviews/red-team-goal-implementation-pass-1.md` -> `docs/archive/feature-audits/red-team-goal-implementation-pass-1-20260520.md`; active review file is a compact boundary stub routing current status to `docs/review.md`, `docs/issues.md`, and `PROJECT_STATE.md`.
- Full historical validation ledger `docs/reviews/current-validation.md` -> `docs/archive/feature-audits/current-validation-full-20260622.md`; active validation file is a compact current beta.131/RitsuLib evidence summary.
- Full historical StS1 event-goal ledger `docs/goals/event.md` -> `docs/archive/feature-audits/event-goal-full-20260622.md`; active event-goal file is a compact current prototype boundary summary.
- Root `BUGFIX_NOTES.md` and `BUGFIX_REPORT.md` -> `docs/archive/implementation-records/bugfix-notes-seedbed-draw-reentrancy-20260527.md` and `docs/archive/implementation-records/bugfix-report-seedbed-draw-reentrancy-20260527.md`; current package/runtime state now lives in `PROJECT_STATE.md` and current release docs.
- ignored `forum/node_modules/` dependency cache deleted; restore with `npm ci` under `forum/` when forum validation needs dependencies.
- ignored local zip residue `docs/STS2-RitsuLib.0.3.3.variant-pack.zip` and `docs/codex-app-better-token-main.zip` deleted after confirming neither file was tracked nor referenced; use official RitsuLib release URLs instead of copied archives.
- `docs/BETA_COMPATIBILITY.md` historical dependency log compressed into a current RitsuLib-only compatibility policy; detailed old compatibility archaeology remains available through Git history and archived implementation records, not the active reading path.
- `docs/features/ritsulib-migration/runtime-hard-block-report-20260531.md` historical May 31 runtime table compressed into a current RitsuLib runtime-boundary stub; old environment archaeology remains in Git history, not the active migration reading path.
- `docs/test-plan.md` current artifact and manual load checks now require the RitsuLib-only lane (`STS2-RitsuLib >= 0.4.34` plus `EZMicroBalance`) and are guarded against reintroducing old dependency-package setup instructions.
- `docs/features/ritsulib-migration/runtime-smoke-checklist.md` active launch checklist compressed into beta.96 RitsuLib-only prerequisites, verifier commands, UI-proof status, and still-pending loader/gameplay gates; old smoke-history tables are no longer in the active checklist.
- `docs/migration.md` active PR-sequencing table compressed again into a compatibility stub; current migration rules live in `docs/features/ritsulib-migration/README.md`, goal/evidence routes through `docs/goals/migration.md` and `docs/integrations/ritsulib.md`, and patch counts stay in `docs/patch-inventory.md`.
- `docs/goals/debug.md` mojibake prompt dump -> `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md`; active `docs/goals/debug.md` is now a compact debug-governance note.
- `docs/goals/sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md` long StS1 v5 overnight/subagent prompt dump -> `docs/archive/feature-inputs/sts1-event-port-strict-audit-monthly-spec-v5-overnight-subagents-20260620.md`; active file is now a compact historical-boundary stub.
- Former per-file `docs/goals/m5-revision-l-*`, `docs/goals/m5-revision-m-*`, and `docs/goals/m5-revision-n-*` active stubs were consolidated into `docs/goals/historical-revision-boundaries.md`; full records remain in `docs/archive/legacy-planning/m5-revision-l-*`, `docs/archive/legacy-planning/m5-revision-m-*`, and `docs/archive/legacy-planning/m5-revision-n-*-20260619.md`.

## F. Archive entrypoints

- `docs/archive/prompts/2026-05/codex-repo-overhaul-refactor-prompt.md`
- `docs/archive/prompts/2026-05/codex-urda-overnight-prompt.md`
- `docs/archive/prompts/2026-05/issues-urda-overnight-addendum.md`
- `docs/archive/prompts/2026-05/codex-ancient-expansion-v22-next-development-prompt.md`
- `docs/issues/waiting-tests.md`
- `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/README.md`
- `docs/archive/feature-audits/current-validation-full-20260622.md`
- `docs/archive/feature-audits/event-goal-full-20260622.md`
- `docs/archive/feature-audits/review-pre-slim-20260518.md`
- `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`
- `docs/archive/feature-audits/review-current-fixed-findings-history-20260622.md`
- `docs/archive/feature-audits/overnight-run-20260529.md`
- `docs/archive/feature-audits/refactor-qa-20260602.md`
- `docs/archive/feature-audits/refactor-qa-20260602-round2.md`
- `docs/archive/feature-audits/red-team-goal-implementation-pass-1-20260520.md`
- `docs/archive/feature-audits/toreview-pre-slim-20260518.md`
- `docs/archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md`
- `docs/archive/superseded/setup-spec-original-scaffold.md`
- `docs/archive/feature-inputs/README.md`
- `docs/archive/project-state-history-20260516.md`
- `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md`
- `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`
- `docs/archive/implementation-records/website-localization-qa-20260522.md`
- `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`
- `docs/archive/implementation-records/bugfix-notes-seedbed-draw-reentrancy-20260527.md`
- `docs/archive/implementation-records/bugfix-report-seedbed-draw-reentrancy-20260527.md`
- `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md`
- `docs/archive/legacy-planning/m5-revision-l-runtime-hard-blocker-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-runtime-smoke-plan-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-final-report-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-owner-review-packet-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-dirty-ledger-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-commit-slices-20260610.md`
- `docs/archive/legacy-planning/m5-revision-l-warning-ledger-20260610.md`
- `docs/archive/legacy-planning/m5-revision-m-final-report-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-owner-review-packet-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-runtime-drift-report-20260618.md`
- `docs/archive/legacy-planning/m5-revision-m-patch-failure-ledger-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-version-decision-20260611.md`
- `docs/archive/legacy-planning/m5-revision-m-commit-slices-20260611.md`
- `docs/archive/legacy-planning/m5-revision-n-final-report-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-owner-commit-packet-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-validation-replay-20260619.md`
- `docs/archive/legacy-planning/m5-revision-n-runtime-evidence-plan-20260619.md`
- `docs/archive/legacy-planning/legacy-project-files/README.md`
- `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md`
- `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`

## Validation note

- Keep `Current`, `Archive`, and `Clutter` sections aligned with any future documentation moves.
