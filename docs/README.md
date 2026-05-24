# Documentation Index

This folder is project memory for `Spire Plus` (`EZMicroBalance` manifest id). Keep the active reading path small: current development should start from the files below, not from archived prompts or old audit matrices.

## Read First: Current Path

| Need | Document | Purpose |
| --- | --- | --- |
| Current state snapshot | `../PROJECT_STATE.md` | Current status, blockers, and next action. |
| Human overview | `../README.md` | Short project overview, build commands, and release policy. |
| Agent rules | `../AGENTS.md` | Hard rules for manifest ids, source evidence, release claims, and validation. |
| Current development goal | `test-ready-development-goal.md` | Single active long-scope directive for the next implementation pass. |
| Open issues and TODO | `issues.md` | Compact active blocker and next-development issue index. |
| Ancient expansion v2.2 | `features/ancient-expansion-v2.2/README.md` | Current feature status and focused doc map. |
| Project map | `PROJECT_MAP.md` | Active, support, archive, and local-only path map. |
| Cleanup audit | `worktree-cleanup-audit.md` | Current cleanup/refactor scope, archive decisions, and owner-decision areas. |
| Architecture boundaries | `architecture/bounded-contexts.md` | Bounded contexts, dependency direction, extension rules, and refactor priorities. |
| Patch inventory | `patch-inventory.md` | Generated Harmony patch owner/risk inventory; refresh with `scripts/generate-patch-inventory.ps1`. |
| Release evidence dashboard | `release-evidence-status.md` | Compact live/manual evidence state for the current test package. |
| v0.106 source/API audit | `audits/v0.106-source-api-drift.md` | Current local Core evidence for high-risk patch surfaces and API drift. |
| Release scope | `specs/release-scope-v1.md` | Current release-candidate boundary and go/no-go rules from `goal.md`. |
| Traceability matrix | `specs/release-traceability-matrix.md` | Player promise to source/guard/live evidence mapping. |
| Baseline freeze | `month-plan/baseline-2026-05-20.md` | Current no-game baseline, package hashes, patch inventory, and blocker state. |
| Full local CI lane | `../.github/workflows/full-local-validation.yml` | Self-hosted Windows workflow for build/test/format/publish/package/artifact validation with explicit StS2 and Godot paths. |
| Public website | `../website/README.md` | Static GitHub Pages site for effect tables, validation status, and download links. |

Do not read archived prompt dumps or archived audit matrices by default. They are historical and only needed when investigating an old finding.

## Feature Records

Use `features/README.md` as the compact feature index.

| Feature | Entry Point | Notes |
| --- | --- | --- |
| Ancient expansion v2.2 | `features/ancient-expansion-v2.2/README.md` | Current Urda, Morvi, Lotha, and Vakuu roadmap/status. |
| Ancient reward rebalance v4 | `features/ancients-rework-v4/README.md` | Active Spire Plus core feature. v4.3 is current. |
| Ascension 11-20 | `features/ascension-11-20/README.md` | Active development track, default-on for private-beta multiplayer testing, not release-verified. |
| Urda ancient expansion support | `features/ancient-expansion-urda/README.md` | Support evidence for the current default-on Urda slice with eleven source-backed blessings; active goal/issues/v2.2/v3.3 docs override older behavior if they conflict. |
| Preview tools | `features/preview-tools/README.md` | Crystal Sphere peek and transform preview now ship inside the single Spire Plus mod. |
| Public forum | `features/forum/technical-spec.md` | GitHub Pages + Supabase anonymous text forum; go-live steps are in `features/forum/go-live-checklist.md`. |
| Independent mod architecture | `architecture-ez-micro-balance.md` | Why `Spire Plus` keeps the stable `EZMicroBalance` technical id. |

## Validation And Release Support

These files are current support records but are not part of the default next-development reading path.

| Document | Purpose |
| --- | --- |
| `dev-environment.md` | Local versions, paths, latest build/publish/test/smoke evidence. |
| `release-checklist.md` | Private beta checklist and explicit pending gates. |
| `release-evidence-status.md` | Compact dashboard of current package hashes and live/manual proof rows. |
| `platform-testing.md` | Windows/macOS package hash, log path, and environment-variable testing notes. |
| `private-beta-verification-handoff.md` | Concise package, validation, and manual-test handoff. |
| `private-beta-release-completion-audit.md` | Release completion audit, strict release-readiness audit, and blocker evidence. |
| `test-ready-completion-audit.md` | Source/test-ready completion audit from the previous pass. |
| `test-plan.md` | Automated, release-artifact, localization, manual, save/load, and disable checks. |
| `BETA_COMPATIBILITY.md` | Version compatibility policy and compatibility log. |
| `archive/implementation-records/rc1-live-validation-log-20260508-20260513.md` | Historical RC1 source/live-validation notes for older packages. Current evidence is summarized in `release-evidence-status.md` and `dev-environment.md`. |
| `issues/waiting-tests.md` | Compact manual evidence queue retained for guard/reference support. Full historical rows are archived under `archive/issues/`. |
| `mod-changelog.md` | Player-facing change summary. Keep it short. |
| `介绍.md` | Detailed feature and change summary for downstream web documentation. |
| `../website/README.md` | Public static site maintenance notes. |

## Setup And Workflow

| Document | Status |
| --- | --- |
| `REMOTE_DEVELOPMENT_SETUP.md` | Current setup guide for another machine. |
| `archive/superseded/setup-spec-original-scaffold.md` | Historical setup specification from the original scaffold; keep for context, do not use as current status. |
| `codex-workflow.md` | Repeatable Codex workflow notes. |
| `style/card-localization-style-guide.md` | Source-backed card text, keyword, preview, and bilingual terminology rules. |
| `skills/sts2-godot-mod-development.md` | Repo-local StS2/Godot/.NET development reference. |
| `adr/0000-template.md` | Template for architecture, patch-surface, saved-state, and release-scope decisions. |

## Code And Helper Indexes

| Document | Purpose |
| --- | --- |
| `../EZMicroBalanceCode/README.md` | Active C# module map and extension rules. |
| `../tests/EZMicroBalance.Tests/README.md` | Test suite group map and commands. |
| `../scripts/README.md` | Repository helper script map. |
| `patch-inventory.md` | Generated Harmony patch owner/risk map. |
| `specs/release-scope-v1.md` | Current release boundary and product decisions. |
| `specs/website-claim-audit.md` | Archived website/public-claim audit. |
| `../website/content-data.js` | Current public website effect data and download metadata. |
| `specs/release-traceability-matrix.md` | Claim-to-evidence traceability matrix. |
| `source-research/run-room-event-reward.md` | Run, room, event, and reward source evidence. |
| `source-research/multiplayer-save-rng.md` | Multiplayer, save, and RNG source evidence. |
| `architecture/bounded-contexts.md` | Feature/context ownership map. |
| `architecture/patch-boundaries.md` | Harmony patch boundary rules. |
| `architecture/save-state-contracts.md` | Stateful feature save/load contract map. |
| `audits/v0.106-source-api-drift.md` | Source/API drift audit for the refreshed `v0.106.0` Core snapshot. |
| `../scripts/ci-full-validation.ps1` | Full no-game validation entry point for self-hosted CI or a local release machine. |
| `archive/legacy-planning/legacy-project-files/README.md` | Preserved migration artifact map. |

## Archived Support

| Archive | Purpose |
| --- | --- |
| `archive/prompts/2026-05/` | Historical nightly/development prompts. |
| `archive/feature-inputs/` | Historical feature prompts and superseded implementation specs moved out of active feature folders. |
| `archive/feature-audits/ancient-expansion-v2.2/2026-05-13/` | Historical v2.2 audit matrices moved out of the active reading path. |
| `archive/feature-audits/review-pre-slim-20260518.md` | Full historical source-review log before `docs/review.md` was reduced to current findings. |
| `archive/feature-audits/toreview-pre-slim-20260518.md` | Full historical retest queue before `docs/toreview.md` was reduced to the current manual queue. |
| `archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md` | Historical architecture/clean-code audit. It contains superseded Future Peek separation advice and should not be used as current implementation direction. |
| `archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md` | Compact record of the source/test-ready implementation pass. |
| `archive/implementation-records/rc1-live-validation-log-20260508-20260513.md` | Historical RC1 live-validation log for older package states. |
| `archive/project-state-history-20260516.md` | Historical `PROJECT_STATE.md` snapshot before the active file was reduced to current status. |

## Cleanup Rules

- Keep one current entry point for each concern.
- Move historical design, audit, or prompt material to `archive/` instead of leaving it in the active reading path.
- Do not delete source evidence that is still read by automated guards.
- Do not leave release-critical TODOs only in archived files.
- Update `PROJECT_MAP.md` and `doc-inventory.md` when moving documentation.
