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
- `.github/workflows/spire-plus-site.yml`

## B. Feature docs

- `docs/features/ancients-rework-v4/`
- `docs/features/ascension-11-20/`
- `docs/features/ancient-expansion-urda/` (Urda support evidence; current combined Ancient docs override conflicts)
- `docs/features/ancient-expansion-v2.2/` (current roadmap/prototype docs; Urda, Morvi, Lotha, and the single-player Vakuu fight are source-active/live-pending)
- `docs/features/ancients-rework-v4/reference-inputs/` (traceability input only; do not implement from it without checking current issues/goal/source-design)
- `docs/features/preview-tools/` (Crystal Sphere peek and transform preview integrated into Spire Plus)

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
- `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md`
- `docs/archive/implementation-records/`
- `docs/archive/project-state-history-20260516.md`
- `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md`
- `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`
- `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`
- `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`

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
- ignored `forum/node_modules/` dependency cache deleted; restore with `npm ci` under `forum/` when forum validation needs dependencies.

## F. Archive entrypoints

- `docs/archive/prompts/2026-05/codex-repo-overhaul-refactor-prompt.md`
- `docs/archive/prompts/2026-05/codex-urda-overnight-prompt.md`
- `docs/archive/prompts/2026-05/issues-urda-overnight-addendum.md`
- `docs/archive/prompts/2026-05/codex-ancient-expansion-v22-next-development-prompt.md`
- `docs/issues/waiting-tests.md`
- `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/README.md`
- `docs/archive/feature-audits/review-pre-slim-20260518.md`
- `docs/archive/feature-audits/toreview-pre-slim-20260518.md`
- `docs/archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md`
- `docs/archive/superseded/setup-spec-original-scaffold.md`
- `docs/archive/feature-inputs/README.md`
- `docs/archive/project-state-history-20260516.md`
- `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md`
- `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`
- `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`
- `docs/archive/legacy-planning/legacy-project-files/README.md`
- `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md`
- `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`

## Validation note

- Keep `Current`, `Archive`, and `Clutter` sections aligned with any future documentation moves.
