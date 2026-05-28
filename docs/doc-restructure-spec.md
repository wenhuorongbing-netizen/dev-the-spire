# Documentation Restructure Spec

## Problem

207 docs files. 113 active (non-archive). Too many to scan, too many root-level
loose files, overlapping content, stale goal dumps, and no clear reading path
per task line.

## Current State

| Location | Files | Status |
| --- | --- | --- |
| `docs/archive/` | 94 | Already archived, leave alone |
| `docs/features/` | 45 | Organized by feature, mostly fine |
| Root-level `docs/` + loose subdirs | 68 | **Needs restructuring** |

## Active Task Lines

The project has 4 active work streams. All docs should route through these:

| # | Task Line | Current Docs | Status |
| --- | --- | --- | --- |
| 1 | **Ancient expansion** (Urda/Morvi/Lotha/Vakuu) | `features/ancient-expansion-v2.2/`, `features/ancient-expansion-urda/`, `features/ancients-rework-v4/` | Active |
| 2 | **Ascension 11-20** | `features/ascension-11-20/` | Active |
| 3 | **RitsuLib integration** (PR6+) | `integrations/ritsulib.md`, `migration.md` | Just started |
| 4 | **Release readiness** | `release-*.md`, `test-*.md`, `private-beta-*.md` | Validation pending |

## Target Structure

```text
docs/
  README.md                    <- index (keep, rewrite)
  PROJECT_MAP.md               <- path map (keep)

  features/                    <- task-line docs (keep as-is)
    ancient-expansion-v2.2/    <- Ancient task line
    ancient-expansion-urda/    <- Ancient task line
    ancients-rework-v4/        <- Ancient task line
    ascension-11-20/           <- Ascension task line
    preview-tools/             <- sub-feature
    forum/                     <- sub-feature
    future-peek/               <- sub-feature
    README.md                  <- feature index

  integrations/                <- RitsuLib task line
    ritsulib.md                <- keep (canonical RitsuLib record)

  release/                     <- NEW: release readiness task line
    release-checklist.md
    release-evidence-status.md
    release-scope-v1.md        <- moved from specs/
    release-traceability-matrix.md  <- moved from specs/
    test-plan.md
    test-ready-development-goal.md
    test-ready-completion-audit.md
    private-beta-verification-handoff.md
    private-beta-release-completion-audit.md
    platform-testing.md
    BETA_COMPATIBILITY.md

  ref/                         <- NEW: reference/resource materials
    architecture/
      bounded-contexts.md
      patch-boundaries.md
      save-state-contracts.md
    style/
      card-localization-style-guide.md
    skills/
      sts2-godot-mod-development.md
    source-research/
      run-room-event-reward.md
      multiplayer-save-rng.md
    adr/
      0000-template.md
    dev-environment.md
    patch-inventory.md

  pm/                          <- NEW: project management
    migration.md
    restructure.md
    refactor-map.md
    codex-workflow.md
    git-commit-push-policy.md
    REMOTE_DEVELOPMENT_SETUP.md

  codex-harness/               <- keep (agent workflow)
    README.md
    PROMPTS.md
    templates/

  archive/                     <- keep (already archived)
```

## Moves

### Root-level files -> release/

| From | To |
| --- | --- |
| `docs/release-checklist.md` | `docs/release/release-checklist.md` |
| `docs/release-evidence-status.md` | `docs/release/release-evidence-status.md` |
| `docs/test-plan.md` | `docs/release/test-plan.md` |
| `docs/test-ready-development-goal.md` | `docs/release/test-ready-development-goal.md` |
| `docs/test-ready-completion-audit.md` | `docs/release/test-ready-completion-audit.md` |
| `docs/private-beta-verification-handoff.md` | `docs/release/private-beta-verification-handoff.md` |
| `docs/private-beta-release-completion-audit.md` | `docs/release/private-beta-release-completion-audit.md` |
| `docs/platform-testing.md` | `docs/release/platform-testing.md` |
| `docs/BETA_COMPATIBILITY.md` | `docs/release/BETA_COMPATIBILITY.md` |
| `docs/specs/release-scope-v1.md` | `docs/release/release-scope-v1.md` |
| `docs/specs/release-traceability-matrix.md` | `docs/release/release-traceability-matrix.md` |

### Root-level files -> ref/

| From | To |
| --- | --- |
| `docs/architecture/bounded-contexts.md` | `docs/ref/architecture/bounded-contexts.md` |
| `docs/architecture/patch-boundaries.md` | `docs/ref/architecture/patch-boundaries.md` |
| `docs/architecture/save-state-contracts.md` | `docs/ref/architecture/save-state-contracts.md` |
| `docs/style/card-localization-style-guide.md` | `docs/ref/style/card-localization-style-guide.md` |
| `docs/skills/sts2-godot-mod-development.md` | `docs/ref/skills/sts2-godot-mod-development.md` |
| `docs/source-research/run-room-event-reward.md` | `docs/ref/source-research/run-room-event-reward.md` |
| `docs/source-research/multiplayer-save-rng.md` | `docs/ref/source-research/multiplayer-save-rng.md` |
| `docs/adr/0000-template.md` | `docs/ref/adr/0000-template.md` |
| `docs/dev-environment.md` | `docs/ref/dev-environment.md` |
| `docs/patch-inventory.md` | `docs/ref/patch-inventory.md` |
| `docs/architecture-ez-micro-balance.md` | `docs/ref/architecture/ez-micro-balance.md` |

### Root-level files -> pm/

| From | To |
| --- | --- |
| `docs/migration.md` | `docs/pm/migration.md` |
| `docs/restructure.md` | `docs/pm/restructure.md` |
| `docs/refactor-map.md` | `docs/pm/refactor-map.md` |
| `docs/codex-workflow.md` | `docs/pm/codex-workflow.md` |
| `docs/git-commit-push-policy.md` | `docs/pm/git-commit-push-policy.md` |
| `docs/REMOTE_DEVELOPMENT_SETUP.md` | `docs/pm/REMOTE_DEVELOPMENT_SETUP.md` |

### Merge duplicates

| Source | Target | Action |
| --- | --- | --- |
| `docs/review.md` + `docs/reviews/red-team-goal-implementation-pass-1.md` | `docs/review.md` | Merge into one, delete `reviews/` |
| `docs/toreview.md` + `docs/issues/waiting-tests.md` | `docs/toreview.md` | Merge, delete `issues/waiting-tests.md` |
| `docs/issues.md` + `docs/issues/*.md` | `docs/issues.md` | Merge all issue files into one, delete `issues/` |
| `docs/goal.md` + `docs/goals/*.md` | `docs/pm/goals.md` | Consolidate into one goals file, delete `goals/` |
| `docs/month-plan/*.md` | `docs/pm/month-plan.md` | Consolidate, delete `month-plan/` |
| `docs/audits/v0.106-source-api-drift.md` | `docs/ref/audits/v0.106-source-api-drift.md` | Move to ref |

### Archive (move to docs/archive/)

| File | Reason |
| --- | --- |
| `docs/longhaul-audit/` (8 files) | Stale audit process, not active |
| `docs/doc-inventory.md` | Superseded by this restructure |
| `docs/worktree-cleanup-audit.md` | Cleanup done, historical record |
| `docs/private-beta-release-completion-audit.md` | Historical audit |
| `docs/specs/website-claim-audit.md` | Historical audit |
| `docs/intro.zh.md` | Downstream web doc, not active dev reference |

### Delete (truly useless)

| File | Reason |
| --- | --- |
| `docs/goals/sts1_event_port_master_plan_summary.md` | StS1 port plan, out of scope this cycle |
| `docs/goals/debug.md` | Chinese prompt dump, not a doc |
| `docs/goals/event.md` | ChatGPT dump, not a doc |
| `docs/goals/devspire_longhaul_file_audit_prompts.md` | Prompt dump for longhaul audit (archived process) |

## Result: Root-Level docs/ After Restructure

```text
docs/
  README.md
  PROJECT_MAP.md
  issues.md
  review.md
  toreview.md
  mod-changelog.md
  features/
  integrations/
  release/
  ref/
  pm/
  codex-harness/
  archive/
```

**Root-level files: 7** (down from 28+)

## Execution Order

1. Create new directories: `release/`, `ref/`, `ref/architecture/`, `ref/style/`, `ref/skills/`, `ref/source-research/`, `ref/adr/`, `ref/audits/`, `pm/`
2. Move files (git mv)
3. Merge duplicates (consolidate content, delete sources)
4. Archive stale files
5. Delete useless files
6. Rewrite `docs/README.md` index
7. Update all internal cross-references
8. Verify build, tests, format
