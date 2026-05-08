# Documentation Index

This folder is the project memory for `EZ Micro Balance`. Use this page as the first stop before opening individual planning, validation, or feature documents.

## Read First

| Need | Document | Purpose |
| --- | --- | --- |
| Current project overview | `../README.md` | Short human-facing state, build commands, and release policy. |
| Agent rules | `../AGENTS.md` | Hard rules for manifest ids, source evidence, release claims, and validation. |
| Directory map | `PROJECT_MAP.md` | Current repo layout, active/legacy boundaries, ignored local scratch folders. |
| Current machine state | `dev-environment.md` | Local versions, paths, latest build/publish/test/smoke evidence. |
| Release gates | `release-checklist.md` | Private beta checklist and explicit pending gates. |
| Tester handoff | `private-beta-verification-handoff.md` | Concise package, validation, and manual-test handoff. |
| Open problems | `issues.md` | Runtime/player-reported issues and unresolved validation work. |

## Feature Records

Use `features/README.md` as the compact feature index.

| Feature | Entry Point | Notes |
| --- | --- | --- |
| Ancient reward rebalance v4 | `features/ancients-rework-v4/README.md` | Active EZ Micro Balance core feature. v4.3 is current. |
| Ascension 11-20 | `features/ascension-11-20/README.md` | Active development track, default-on for private-beta multiplayer testing, not release-verified. |
| Independent mod architecture | `architecture-ez-micro-balance.md` | Why `EZMicroBalance` exists separately from `EzDailyContent`. |

## Validation And Release

| Document | Purpose |
| --- | --- |
| `test-plan.md` | Automated, release-artifact, localization, manual, save/load, and disable checks. |
| `BETA_COMPATIBILITY.md` | Version compatibility policy and compatibility log. |
| `rc1-live-validation-log.md` | RC1 source/live-validation notes and remaining live gates. |
| `mod-changelog.md` | Player-facing change summary. Keep it short. |

## Setup And Workflow

| Document | Status |
| --- | --- |
| `REMOTE_DEVELOPMENT_SETUP.md` | Current setup guide for another machine. |
| `SETUP_SPEC.md` | Historical setup specification from the original scaffold; keep for context, do not use as current status. |
| `codex-workflow.md` | Repeatable Codex workflow notes. |
| `skills/sts2-godot-mod-development.md` | Repo-local StS2/Godot/.NET development reference. |

## Code And Helper Indexes

| Document | Purpose |
| --- | --- |
| `../EZMicroBalanceCode/README.md` | Active C# module map and extension rules. |
| `../tests/EZMicroBalance.Tests/README.md` | Test suite group map and commands. |
| `../scripts/README.md` | Repository helper script map. |
| `../legacy/README.md` | Preserved migration artifact map. |

## Archive Policy

`archive/` contains preserved historical planning material. Do not cite archived documents as current implementation truth unless a current document explicitly re-promotes that content.

Use this rule when cleaning up duplicates:

- Keep one current entry point for each concern.
- Move historical design or prompt material to `archive/` instead of deleting it.
- Keep chronological work logs, but treat newer feature README/checklist/audit files as the current summary.
- When a document becomes historical, say so at the top and link to the current replacement.

## Update Rules

- Update `dev-environment.md` after build, publish, package, smoke, version, or dependency changes.
- Update `release-checklist.md` and `private-beta-verification-handoff.md` after release-gate evidence changes.
- Update the relevant feature README when scope, owner files, or current behavior changes.
- Update `PROJECT_MAP.md` when top-level folders, active modules, or important entry points change.
