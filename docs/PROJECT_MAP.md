# Project Map

`Spire Plus` is the single active private beta target. Its stable manifest id is `EZMicroBalance`. The old `EzDailyContent` scaffold and standalone `EZFuturePeek` prototype are no longer active root mod surfaces.

## Top-Level Layout

| Path | Status | Purpose |
| --- | --- | --- |
| `README.md` | Current | Short project overview, build/publish commands, and release policy. |
| `AGENTS.md` | Current | Agent rules and project hard constraints. |
| `EZMicroBalance.sln` | Current | Active solution for build/test/publish. |
| `EZMicroBalance.csproj` | Current | Active private beta C# project. |
| `EZMicroBalance.json` | Current | Active mod manifest, display name `Spire Plus`, id `EZMicroBalance`. |
| `EZMicroBalance/` | Current | Active Godot resources, images, and localization. |
| `EZMicroBalanceCode/` | Current | Active C# source. See `EZMicroBalanceCode/README.md`. |
| `EZMicroBalanceCode/Ancients/Common/` | Current | Shared Ancient saved state, card helpers, reward relic service, and feature-gate helper. |
| `EZMicroBalanceCode/Ancients/Expansion/Urda/` | Current | Urda Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Morvi/` | Current | Morvi Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Lotha/` | Current | Lotha Ancient expansion feature implementation and gate control. |
| `EZMicroBalanceCode/Ancients/Expansion/Vakuu/` | Current | Hidden-by-default Vakuu fight slice, encounter, monster, Temptation card, and fight gate. |
| `EZMicroBalanceCode/Preview/` | Current | Integrated preview tools: Crystal Sphere peek and transform preview. |
| `tests/EZMicroBalance.Tests/` | Current | Source, localization, docs, package, and runtime-evidence guards. |
| `export_presets.cfg` | Current | Selected-resource PCK export for active mod resources only. |
| `Directory.Build.props.example` | Current | Template for local machine paths. |
| `Sts2PathDiscovery.props` | Current | Local path discovery helper used by build props. |
| `docs/` | Current | Documentation index, current release docs, feature records, and archive. |
| `scripts/` | Current | Repository helper scripts. |
| `website/` | Current | GitHub Pages static site for the Spire Plus change log, effect tables, validation state, and download entry. |
| `.github/workflows/repository-hygiene.yml` | Current | Hosted CI-safe manifest, JSON, docs, patch inventory, and whitespace checks. |
| `.github/workflows/full-local-validation.yml` | Current | Self-hosted Windows full no-game validation lane; requires explicit StS2 and Godot paths. |
| `.github/workflows/spire-plus-site.yml` | Current | Publishes `website/` to GitHub Pages. |
| `docs/archive/` | Current | Historical planning, prompt material, release archaeology, archived audits, and implementation records. |
| `docs/archive/feature-inputs/` | Archive | Historical feature prompts, superseded implementation specs, and old source-design inputs moved out of active feature folders. |
| `docs/archive/feature-inputs/debug-goal-mojibake-intake-20260620.md` | Archive | Historical corrupted `docs/goals/debug.md` prompt dump; current debug governance remains in the compact active `docs/goals/debug.md`. |
| `docs/archive/feature-inputs/sts1-event-port-strict-audit-monthly-spec-v5-overnight-subagents-20260620.md` | Archive | Historical StS1 v5 overnight/subagent prompt dump; current StS1 event guidance remains in `docs/goals/event.md` and active StS1 feature docs. |
| `docs/archive/legacy-planning/m5-revision-l-*-20260610.md` | Archive | Historical beta.84/beta.85 Revision L owner-review packet, dirty ledger, runtime blocker/smoke plan, warning ledger, commit slices, and final report. |
| `docs/archive/legacy-planning/m5-revision-m-*-20260611.md` and `docs/archive/legacy-planning/m5-revision-m-runtime-drift-report-20260618.md` | Archive | Historical beta.85/beta.87 Revision M owner packet, runtime drift report, patch-failure ledger, version decision, commit slices, and final report. |
| `docs/archive/legacy-planning/m5-revision-n-*-20260619.md` | Archive | Historical beta.88/previous package Revision N owner packet, validation replay, runtime evidence plan, and final report; current routing lives in `docs/goals/historical-revision-boundaries.md`. |
| `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/` | Archive | Historical v2.2 source-audit matrices; do not use as default next-development input. |
| `docs/archive/feature-audits/review-pre-slim-20260518.md` | Archive | Full historical source-review log before the compact current `docs/review.md`. |
| `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md` | Archive | Full 2026-05-26 beta.41-beta.54 source/package pass history moved out of the compact current review. |
| `docs/archive/feature-audits/review-current-fixed-findings-history-20260622.md` | Archive | Full pre-compact current fixed-finding ledger moved out of active `docs/review.md`. |
| `docs/archive/feature-audits/current-validation-full-20260622.md` | Archive | Full historical validation ledger moved out of the compact active `docs/reviews/current-validation.md`. |
| `docs/archive/feature-audits/event-goal-full-20260622.md` | Archive | Full historical StS1 event-goal ledger moved out of the compact active `docs/goals/event.md`. |
| `docs/archive/feature-audits/overnight-run-20260529.md` | Archive | Historical StS1 overnight no-game/source-governance report; active `docs/reviews/overnight-run-20260529.md` is a compact guarded stub only. |
| `docs/archive/feature-audits/refactor-qa-20260602*.md` | Archive | Historical StS1 loader-gate QA reports; active `docs/reviews/refactor-qa-20260602*.md` files are compact guarded stubs only. |
| `docs/archive/feature-audits/red-team-goal-implementation-pass-1-20260520.md` | Archive | Full May 20 red-team goal-completion review; active `docs/reviews/red-team-goal-implementation-pass-1.md` is a compact boundary stub only. |
| `docs/archive/feature-audits/toreview-pre-slim-20260518.md` | Archive | Full historical retest queue before the compact current `docs/toreview.md`. |
| `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md` | Archive | Historical architecture/clean-code audit; superseded `EZFuturePeek` separation advice is not current direction. |
| `docs/archive/implementation-records/` | Archive | Compact implementation records moved out of the active reading path. |
| `docs/intro.zh.md` | Current support | Chinese Spire Plus feature summary kept under an ASCII path for stable Git/script output. |
| `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md` | Archive | Historical RC1 live-validation log for older package states; current evidence is in `docs/release-evidence-status.md`. |
| `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md` | Archive | Historical runtime-smoke detail archived from `docs/dev-environment.md`; current environment evidence stays compact. |
| `docs/archive/superseded/setup-spec-original-scaffold.md` | Archive | Historical original `EzDailyContent` setup specification; current setup starts from `README.md`, `docs/README.md`, and `docs/dev-environment.md`. |
| `docs/archive/project-state-history-20260516.md` | Archive | Pre-cleanup `PROJECT_STATE.md` snapshot preserving superseded per-pass validation/package history. |
| `docs/issues/` | Current support | Compact issue follow-up plus compact manual evidence queue retained for guard/reference support (`docs/issues/waiting-tests.md`). |
| `docs/worktree-cleanup-audit.md` | Current support | Current cleanup/refactor inventory, archive decisions, owner-decision areas, and clean-state worktree checks. |
| `docs/patch-inventory.md` | Current support | Generated Harmony patch owner/risk inventory. Regenerate with `scripts/generate-patch-inventory.ps1`. |
| `docs/release-evidence-status.md` | Current support | Compact current package and live/manual evidence dashboard. |
| `docs/adr/` | Current support | Architecture decision records. Start with `docs/adr/0000-template.md`. |
| `docs/specs/` | Current support | Release scope, website claim audit, and release traceability matrix. |
| `docs/source-research/` | Current support | Compact source evidence for high-risk run, room, reward, save, RNG, and multiplayer surfaces. |
| `docs/codex-harness/` | Current support | Thin Codex workflow templates adapted from codex-app-better-token harness. |
| `docs/integrations/` | Current support | Runtime integration records for active dependencies such as RitsuLib. |
| `docs/refactor-map.md` | Current support | Move-only folder refactor map for EZMicroBalanceCode (planning only). |
| `docs/migration.md` | Current support | Compatibility stub only; route migration work to `docs/features/ritsulib-migration/README.md`, `docs/goals/migration.md`, `docs/integrations/ritsulib.md`, and generated patch counts to `docs/patch-inventory.md`. |
| `harness/` | Current support | Live task-scoped state files for Codex workflow (project root). |
| `docs/architecture/` | Current support | Bounded context, patch boundary, and save-state contract maps. |
| `docs/month-plan/` | Current support | 30-day plan outputs such as baseline freeze and commit boundaries. |
| `docs/features/ancient-expansion-v2.2/` | Current planning/prototype | Current Urda stabilization, default-on Morvi and Lotha source slices, and the hidden-by-default dedicated Vakuu fight slice; live verification remains pending. |
| `docs/features/sts1-events/` | Current planning | StS1 events migration: public 52-event baseline tracked; 54 canonical rows, 50 registry identities, 48 model files, and 47 compiling models reconciled. RegisterAll is now 57 calls; AdditiveBatch1 is 10 event types / 14 calls after Big Fish, Golden Idol, The Cleric, and Shining Light moved to Act 1 bucket registration. Current beta.119 proof covers package parity and source validation for the 65-patch source state; previous beta.108 covers smoke-level clicked Ancient UI; beta.99 covers previous-package RitsuLib settings UI visibility and Off loader startup/loading; previous beta.93 RitsuLib-only `v0.107.1` AdditiveBatch1 loader/registration proof is clean for previous-package enabled-mode shape only. Use `v19-gate-evidence-map.md`, `v19-gate-ledger.csv`, `v20-final-gate-overlay.csv`, `hard-stop-blocker-report-v20-coordination-pause-20260617.md`, and `v19-subagent-coverage.md` for the current O0-O76 gate split, O76-O84 final documentation/handoff overlay, current v20 hard-stop/next-run point, and subagent split. |
| `docs/goals/sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md` | Current boundary stub | Compact pointer preserving the historical v5 StS1 prompt boundary while routing current work to `docs/goals/event.md` and active StS1 docs. |
| `docs/goals/event.md` | Current boundary summary | Compact active StS1 event prototype boundary; routes runtime/static proof through active StS1 gate docs and keeps historical ledger content archived. |
| `docs/goals/historical-revision-boundaries.md` | Current boundary index | Single compact pointer for old M5 Revision L/M/N boundaries; the former per-file `docs/goals/m5-revision-*` stubs were removed from the active goal directory. Full records live in `docs/archive/legacy-planning/`. |
| `docs/reviews/overnight-run-20260529.md` | Current boundary stub | Compact guarded pointer preserving historical overnight no-game/source-governance no-overclaim boundaries while the full record lives in `docs/archive/feature-audits/`. |
| `docs/reviews/refactor-qa-20260602*.md` | Current boundary stubs | Compact guarded pointers preserving historical StS1 QA no-overclaim boundaries while full records live in `docs/archive/feature-audits/`. |
| `docs/reviews/red-team-goal-implementation-pass-1.md` | Current boundary stub | Compact pointer preserving the May 20 source-only completion rejection while current status routes through `docs/review.md`, `docs/issues.md`, and `PROJECT_STATE.md`. |
| `docs/reviews/current-validation.md` | Current validation summary | Compact active validation entry for beta.108 RitsuLib-only proof boundaries; full historical validation ledger lives in `docs/archive/feature-audits/`. |
| `EZMicroBalanceCode/Sts1Events/` | Current implementation | StS1 event code: Runtime registry, Shared/Act1/Act2/Act3 event models. |
| `manifests/` | Current support | StS1 event manifest CSV/JSON and asset manifest for extraction scripts. |

## Active Mod Surface

```text
EZMicroBalance/
  images/
  localization/
    eng/
    zhs/

EZMicroBalanceCode/
  MainFile.cs
  Config/
  Core/
    Features/
    Integrations/
      RitsuLib/              (current RitsuLib content registration bootstrap)
  Diagnostics/
  Map/
  Modding/
  Sts1Events/
    Runtime/                 (event registry and pool replacement)
    Models/
      Shared/                (shared events: Big Fish, Golden Idol, etc.)
      Act1/                  (Act 1 exclusive events)
      Act2/                  (Act 2 exclusive events)
      Act3/                  (Act 3 exclusive events)
  Ancients/
    Common/
    Rebalance/               (reserved for shared rebalance helpers)
    Patches/
    Expansion/
      Urda/
      Morvi/
      Lotha/
      Vakuu/
  Ascension/
    Cards/
    Combat/
    Core/
    Enchantments/
    Events/
    Map/
    Patches/
    Powers/
    Relics/
    Rewards/
    Save/                    (reserved for save/load code)
    Ui/                      (reserved for UI patches)
  Preview/
```

Published runtime output should be:

```text
<GameRoot>/mods/EZMicroBalance/
  EZMicroBalance.json
  EZMicroBalance.dll
  EZMicroBalance.pck
```

The private beta zip should contain only those three installable files plus `README_INSTALL.txt` under an `EZMicroBalance/` folder.

## Legacy And Local-Only Material

| Path | Status | Notes |
| --- | --- | --- |
| `docs/archive/legacy-planning/legacy-project-files/` | Archive | Preserved legacy project metadata from earlier migration work; top-level `legacy/` was removed during cleanup. |
| `source code/` | Ignored local scratch | Local recovered source reference; it may be historical. Require `scripts/check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot -FailOnMismatch` before current-source API claims. Do not commit or package. |
| `.tools/` | Ignored local tools | Downloaded GDRETools, Godot, ILSpy, and local helper binaries. |
| `.godot/` | Ignored generated output | Godot import/build cache. |
| `publish/` | Ignored release output | Package staging, versioned package, and private beta zip. |
| `website/` | Promoted current site | Root static site was deliberately promoted from local-draft status for public reading and download entry. |
| `.github/workflows/spire-plus-site.yml` | Promoted current workflow | Pages workflow was deliberately promoted with `website/` as the only uploaded artifact path. |
| `.tools/archive/local-art-and-calibration-20260515/` | Ignored local archive | Former root `art_pipeline/` and `asset/` generated art/calibration material; not part of the active PCK unless explicitly revalidated and promoted later. |
| `.tools/archive/local-root-clutter-20260515/` | Ignored local archive | Former root local-only package/source-analysis/website zip clutter preserved before any deletion decision. |
| `.tools/archive/local-website-preview-20260516/` | Ignored local archive | Older snapshot of the pre-promotion website draft and Pages workflow; the current tracked source lives at root `website/` and `.github/workflows/spire-plus-site.yml`. |
| `docs/archive/implementation-records/forum-public-integration-qa-20260526.md` | Archive | Former root `web_issue.md` forum QA note moved out of the root reading path; current forum follow-up lives in `website/web_issue.md` and `docs/features/forum/`. |
| `docs/archive/implementation-records/website-localization-qa-20260522.md` | Archive | Former `website/localization_qa.md` historical website QA journal moved out of the public website surface; current website state is guarded by `website/content-data.js`, `website/README.md`, and website tests. |

## Documentation Map

Start at `docs/README.md`.

| Area | Entry Point |
| --- | --- |
| Current environment and evidence | `docs/dev-environment.md` |
| Release gates | `docs/release-checklist.md` |
| Tester handoff | `docs/private-beta-verification-handoff.md` |
| Open issues | `docs/issues.md` |
| Patch inventory | `docs/patch-inventory.md` |
| Release evidence dashboard | `docs/release-evidence-status.md` |
| Release scope | `docs/specs/release-scope-v1.md` |
| Traceability matrix | `docs/specs/release-traceability-matrix.md` |
| Baseline freeze | `docs/month-plan/baseline-2026-05-20.md` |
| Commit boundaries | `docs/month-plan/commit-boundaries.md` |
| Full local CI lane | `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` |
| Public change log website | `website/README.md` and `.github/workflows/spire-plus-site.yml` |
| Chinese feature summary | `docs/intro.zh.md` |
| Test-ready development goal | `docs/test-ready-development-goal.md` |
| Manual evidence queue | `docs/issues/waiting-tests.md` (compact support queue only; full historical rows are archived, and next development starts from `docs/test-ready-development-goal.md`) |
| Runtime monkey / AutoSlay stability methodology | `docs/testing/runtime-monkey-stability.md` |
| Ancient reward rebalance | `docs/features/ancients-rework-v4/README.md` |
| Ancient expansion v2.2 | `docs/features/ancient-expansion-v2.2/README.md` |
| RitsuLib migration | `docs/features/ritsulib-migration/README.md` |
| RitsuLib migration runtime smoke | `docs/features/ritsulib-migration/runtime-smoke-checklist.md` |
| RitsuLib migration next run | `docs/features/ritsulib-migration/next-overnight-run.md` |
| Debug governance | `docs/goals/debug.md` |
| Ancient expansion v2.2 source audit archive | `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/README.md` |
| Urda support evidence | `docs/features/ancient-expansion-urda/README.md` |
| Ascension 11-20 | `docs/features/ascension-11-20/README.md` |
| Preview tools | `docs/features/preview-tools/README.md` |
| StS1 event prototype | `docs/features/sts1-events/README.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, and `docs/features/sts1-events/v19-subagent-coverage.md` |
| Architecture decision | `docs/architecture-ez-micro-balance.md` |
| Historical planning | `docs/archive/README.md` |

Authority note: `docs/features/ancient-expansion-urda/` and `docs/features/ancients-rework-v4/reference-inputs/` are retained for tests and traceability. If they conflict with `docs/issues.md`, `docs/test-ready-development-goal.md`, or current combined Ancient docs, use the current docs.

## Extension Landmarks

- Add Ancient behavior under `EZMicroBalanceCode/Ancients/`, with shared state/helpers in `Ancients/Common/` and patch families in `Ancients/Patches/`.
- Add Ascension behavior under `EZMicroBalanceCode/Ascension/`; use the existing `Core/`, `Map/`, `Combat/`, `Rewards/`, `Enchantments/`, `Patches/`, `Cards/`, `Powers/`, `Relics/`, and `Events/` boundaries, and keep public/multiplayer selection disableable with live-readiness claims gated.
- Add preview helper behavior under `EZMicroBalanceCode/Preview/`; do not create a second manifest or publish path for it.
- Add user-facing text in both `EZMicroBalance/localization/eng/` and `EZMicroBalance/localization/zhs/`.
- Add or update tests in `tests/EZMicroBalance.Tests/` whenever source shape, localization, docs, package contents, or release evidence changes.
- Update feature README files and `docs/dev-environment.md` when implementation status, validation evidence, versions, or package hashes change.
- Promoted website source is tracked at `website/` with `.github/workflows/spire-plus-site.yml`; generated forum build output under `website/forum/` stays ignored.

## Milestones

| Milestone | Meaning |
| --- | --- |
| M0 | Local setup and baseline build/publish established. |
| M1 | Ancient reward rebalance implemented from the original scaffold. |
| M2 | Independent `Spire Plus` project created on the stable `EZMicroBalance` technical id and root build succeeds. |
| M3 | Historical v0.106.1 source context remains available; current `v0.107.1` / STS2-RitsuLib `v0.4.34` beta.119 build, publish, package, runtime preflight, source-workspace validation, and installed parity are refreshed; previous beta.99/beta.96/beta.93 loader/settings evidence is retained only as previous-package context, and enabled-mode/gameplay live smoke remains pending for beta.119. |
| M4 | Private beta release after normal Steam-client Mod Settings, live gameplay/manual matrix, clean handoff, validated commit, and pushed branch. |
