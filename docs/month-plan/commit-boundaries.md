# Commit Boundaries

Purpose: keep future cleanup/refactor work reviewable by routing dirty paths into explicit owner batches before any release handoff.

Current status: `GOV-WIP-SPLIT` is source-fixed for the committed `main` baseline. Keep this file as the operating guide for future dirty work; it is not a release-readiness claim.

Refresh command:

```powershell
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

The script reads `git status --short`, classifies every dirty path into the batches below, and fails if a new path does not have an explicit owner batch.

To prepare review/staging inputs without staging anything:

```powershell
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified -PathspecDirectory .tools\worktree-batches\current
```

That writes `batch-0.pathspec` through `batch-8.pathspec` and `manifest.json`. The manifest includes the exact `git add --pathspec-from-file=<pathspec>` command for each batch. Review a file first, then use that command only when intentionally staging that batch.

## Current Clean Baseline

Latest clean-state check: `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified`, 2026-05-26, reported `Total dirty entries: 0` and `Unclassified: 0`.

Rerun `git status --short` and the batch reporter before staging, committing, or release handoff. If any path is unclassified, update the classifier and this map before staging.

## Batch Ownership Map

| Batch | Primary paths | Review risk | Focused validation |
| --- | --- | --- | --- |
| 0 | `.gitignore`, `output/.gdignore`, tracked `output/playwright/` evidence | Local browser/Godot output hygiene can hide useful evidence if over-broadened. | `git check-ignore -v output/playwright/new.log output/.gdignore` plus default tests. |
| 1 | `PROJECT_STATE.md`, `README.md`, `docs/intro.zh.md`, compact status/release docs | Stale hashes or release-ready wording can mislead testers. | `DocumentationCompactnessGuardTests`, `ReleaseHashGuardTests`. |
| 2 | `docs/architecture/**`, `docs/specs/**`, `docs/month-plan/**`, archive/index docs, implementation-record archives | Governance docs can drift from actual patch/source state. | `EngineeringGovernanceGuardTests`, patch inventory freshness checks. |
| 3 | `EZMicroBalanceCode/Ancients/**`, Ancient support docs, Ancient shared evidence/tests | Ancient reward behavior, save state, and option relic visibility are high-risk. | Ancient/Urda/Morvi/Lotha/Vakuu focused guards plus full build/test. |
| 4 | `EZMicroBalanceCode/Ascension/**`, `EZMicroBalance/localization/*/ascension.json`, Ascension docs/tests | A11-A20 map/combat/save/co-op paths have broad runtime surface. | `AscensionV2MilestoneGuardTests`, `BossDedicatedAbilityV41GuardTests`, full build/test. |
| 5 | `scripts/**`, settings UI localization, `EZMicroBalanceCode/Diagnostics/**`, `EZMicroBalanceCode/Preview/**`, release/CI/test-infrastructure tests, and generated sidecar policy | Validation tooling, evidence logs, and preview helpers can create false confidence if hashes, RNG guards, or gates drift. | preview guards, release package/artifact guards, `check-installed-spire-plus-package.ps1`, script syntax checks. |
| 6 | Ancient art/resource docs, active image/export resources, and waiting-test docs | Art/resource claims can outpace live UI proof. | art/resource guards plus manual screenshot queue remains open. |
| 7 | `website/**`, `forum/**` | Public-info surfaces can accidentally imply release readiness. | website/forum syntax check plus website claim/localization guards. |
| 8 | `EZMicroBalanceCode/README.md`, `docs/audits/**` | Stray docs can escape the planned review batches. | docs index/map/inventory guards and reviewer read-through. |

Minimum split order for future broad work: land batches 0, 1, 2, and 5 before gameplay batches, then split Ancient batch 3 from Ascension batch 4, then package/website evidence. Do not mix live-proof closure into these batches unless the matching screenshots/logs/two-client notes are included.

## Proposed Order

| Batch | Scope | Files | Validation |
| --- | --- | --- | --- |
| 1 | Release planning docs | `docs/goal.md`, `docs/month-plan/**`, `docs/specs/**`, `docs/source-research/**`, `docs/architecture/**` | Docs guards, build/test not required unless tests change. |
| 2 | Governance guards | `.github/**`, `scripts/**`, `tests/*Governance*`, `docs/README.md`, `docs/PROJECT_MAP.md` | `dotnet test EZMicroBalance.sln --no-build --filter EngineeringGovernanceGuardTests`. |
| 3 | Preview tools | `EZMicroBalanceCode/Preview/**`, related localization/tests/docs | Spire Plus build/test/format. |
| 4 | Urda and Ancient reward fixes | `EZMicroBalanceCode/Ancients/Expansion/Urda/**`, `Ancients/Patches/**`, Urda docs/tests | Spire Plus build/test plus focused Ancient/Urda guards. |
| 5 | Morvi/Lotha/Vakuu | `Ancients/Expansion/Morvi/**`, `Lotha/**`, `Vakuu/**`, related tests/docs | Spire Plus build/test plus save-risk/Vakuu guards. |
| 6 | Ascension and Rootdeck | `EZMicroBalanceCode/Ascension/**`, related localization/tests/docs | Spire Plus build/test plus Ascension and Rootdeck guards. |
| 7 | Package/release evidence | `publish/` generated output, release docs, handoff docs | Publish, package, artifact tests, release evidence verifier. |

## Rules

- Keep preview-tool changes reviewable as their own Spire Plus batch.
- Do not mix docs-only planning with gameplay fixes unless the doc is the acceptance record for the same fix.
- Do not close live/manual rows in a commit that has no live evidence folder.
- Regenerate patch inventory in the same batch that adds, deletes, or moves patches.
- Keep `EZMicroBalance` manifest id unchanged.

## Current Next Action

Keep this plan linked from `docs/issues.md` as the batch-classifier operating guide. Future work should keep `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` green before staging.
