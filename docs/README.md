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
| Patch inventory | `patch-inventory.md` | Generated Harmony patch owner/risk inventory; refresh with `scripts/generate-patch-inventory.ps1`. |
| Release evidence dashboard | `release-evidence-status.md` | Compact live/manual evidence state for the current test package. |
| Release scope | `specs/release-scope-v1.md` | Current release-candidate boundary and go/no-go rules from `goal.md`. |
| Traceability matrix | `specs/release-traceability-matrix.md` | Player promise to source/guard/live evidence mapping. |
| Baseline freeze | `month-plan/baseline-2026-05-20.md` | Current no-game baseline, package hashes, patch inventory, and blocker state. |
| Full local CI lane | `../.github/workflows/full-local-validation.yml` | Self-hosted Windows workflow for build/test/format/publish/package/artifact validation with explicit StS2 and Godot paths. |

Do not read archived prompt dumps or archived audit matrices by default. They are historical and only needed when investigating an old finding.

## Feature Records

Use `features/README.md` as the compact feature index.

| Feature | Entry Point | Notes |
| --- | --- | --- |
| Ancient expansion v2.2 | `features/ancient-expansion-v2.2/README.md` | Current Urda, Morvi, Lotha, and Vakuu roadmap/status. |
| Ancient reward rebalance v4 | `features/ancients-rework-v4/README.md` | Active Spire Plus core feature. v4.3 is current. |
| Ascension 11-20 | `features/ascension-11-20/README.md` | Active development track, default-on for private-beta multiplayer testing, not release-verified. |
| Urda ancient expansion | `features/ancient-expansion-urda/README.md` | Current default-on Urda slice with ten source-backed blessings; live verification pending. |
| Preview tools | `features/preview-tools/README.md` | Crystal Sphere peek and transform preview now ship inside the single Spire Plus mod. |
| Independent mod architecture | `architecture-ez-micro-balance.md` | Why `EZMicroBalance` is the single active deliverable. |

## Validation And Release Support

These files are current support records but are not part of the default next-development reading path.

| Document | Purpose |
| --- | --- |
| `dev-environment.md` | Local versions, paths, latest build/publish/test/smoke evidence. |
| `release-checklist.md` | Private beta checklist and explicit pending gates. |
| `release-evidence-status.md` | Compact dashboard of current package hashes and live/manual proof rows. |
| `private-beta-verification-handoff.md` | Concise package, validation, and manual-test handoff. |
| `private-beta-release-completion-audit.md` | Release completion audit, strict release-readiness audit, and blocker evidence. |
| `test-ready-completion-audit.md` | Source/test-ready completion audit from the previous pass. |
| `test-plan.md` | Automated, release-artifact, localization, manual, save/load, and disable checks. |
| `BETA_COMPATIBILITY.md` | Version compatibility policy and compatibility log. |
| `rc1-live-validation-log.md` | RC1 source/live-validation notes and remaining live gates. |
| `issues/waiting-tests.md` | Compact manual evidence queue retained for guard/reference support. Full historical rows are archived under `archive/issues/`. |
| `mod-changelog.md` | Player-facing change summary. Keep it short. |
| `介绍.md` | Detailed feature and change summary for downstream web documentation. |

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
| `specs/release-traceability-matrix.md` | Claim-to-evidence traceability matrix. |
| `source-research/run-room-event-reward.md` | Run, room, event, and reward source evidence. |
| `source-research/multiplayer-save-rng.md` | Multiplayer, save, and RNG source evidence. |
| `architecture/bounded-contexts.md` | Feature/context ownership map. |
| `architecture/patch-boundaries.md` | Harmony patch boundary rules. |
| `architecture/save-state-contracts.md` | Stateful feature save/load contract map. |
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
| `archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md` | Compact record of the source/test-ready implementation pass. |
| `archive/project-state-history-20260516.md` | Historical `PROJECT_STATE.md` snapshot before the active file was reduced to current status. |

## Cleanup Rules

- Keep one current entry point for each concern.
- Move historical design, audit, or prompt material to `archive/` instead of leaving it in the active reading path.
- Do not delete source evidence that is still read by automated guards.
- Do not leave release-critical TODOs only in archived files.
- Update `PROJECT_MAP.md` and `doc-inventory.md` when moving documentation.
