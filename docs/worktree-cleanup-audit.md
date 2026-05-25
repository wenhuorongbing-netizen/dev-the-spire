# Worktree Cleanup Audit

Last updated: 2026-05-24

This document tracks cleanup/refactor scope for the current dirty worktree. It is not a release-readiness claim.

## Objective

Clean and refactor the `D:\Game\FOTN\dev-the-spire` workspace by reducing redundant code and stale files, archiving uncertain material before deletion, and keeping active `Spire Plus` source/package work reviewable.

## Current Include Candidates

These paths are active private-beta source, resource, test, script, or documentation changes and should remain visible in `git status` until intentionally reviewed/staged.

| Path | Reason |
| --- | --- |
| `EZMicroBalance/` | Active mod resources, localization, event backgrounds, Vakuu encounter/monster assets, and export inputs. |
| `EZMicroBalanceCode/` | Active C# source, including Ancient feature gates, Vakuu source slice, diagnostics, and Ascension combat cleanup. |
| `tests/EZMicroBalance.Tests/` | Guard tests plus shared `TestRepo.cs` infrastructure cleanup. |
| `scripts/` | Current helper scripts for screenshot/dev-console/release-evidence validation. |
| `export_presets.cfg` | Active PCK export selection. |
| Current docs under `docs/` | Active project state, feature docs, release handoff, and cleanup/archive indexes. |
| `.gitignore` | Keeps local/generated drafts and browser-output clutter out of release-candidate diffs. |

## Archived Or Ignored Cleanup Results

| Material | Current location/status | Notes |
| --- | --- | --- |
| Original scaffold setup spec | `docs/archive/superseded/setup-spec-original-scaffold.md` | Replaced by current setup docs and retained for archaeology. |
| Feature-local prompt/spec archives | `docs/archive/feature-inputs/` | Moved out of active feature folders. |
| Superseded project-state validation history | `docs/archive/project-state-history-20260516.md` | Full pre-cleanup `PROJECT_STATE.md` snapshot retained; active `PROJECT_STATE.md` now carries current status, blockers, commands, and next action. |
| Superseded issues package verification note | `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md` | Long-form 2026-05-15 package verification detail was moved out of the active issues index; `docs/issues.md` keeps only the current hashes, evidence pointer, and pending manual gates. |
| Duplicate root mod surfaces | Removed from active root | `EzDailyContent*` and standalone `EZFuturePeek*` surfaces were removed after the project owner chose a single active mod surface. Historical metadata remains in `docs/archive/legacy-planning/`. |
| Superseded architecture audit | `docs/archive/feature-audits/architecture-clean-code-management-audit-2026-05-19.md` | Retained for historical governance context; no longer current because it recommends an independent `EZFuturePeek` path that the owner later folded into Spire Plus. |
| Historical RC1 live log | `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md` | Older v0.105 / pre-current-package evidence retained outside the active reading path. Current package evidence stays in `docs/release-evidence-status.md` and `docs/dev-environment.md`. |
| Top-level legacy project metadata | `docs/archive/legacy-planning/legacy-project-files/` | Former root `legacy/` folder moved into the existing legacy-planning archive. |
| Root local art/calibration folders | `.tools/archive/local-art-and-calibration-20260515/` | Ignored local archive. |
| Root local clutter archives | `.tools/archive/local-root-clutter-20260515/` | Ignored local archive. |
| Static website and Pages workflow | Current tracked `website/` and `.github/workflows/spire-plus-site.yml`; older snapshot remains under `.tools/archive/local-website-preview-20260516/` | The public site was deliberately promoted after cleanup; generated forum build output under `website/forum/` and Godot `.import` metadata under `website/**` remain ignored. |
| Transient Edge browser profiles | Deleted `.tools/edge-chatgpt-profile-clone-20260515/`, `.tools/edge-chatgpt-profile-clone2-20260515/`, `.tools/edge-chatgpt-profile/`, and `.tools/edge-chatgpt-pw-profile/` | These were unreferenced browser cache/profile folders; the retained evidence/art outputs remain under `.tools/art-generation/`, active resources, and current docs. |
| Stale redirected publish outputs | Deleted `.tools/publish-redirect/`, `.tools/publish-redirect2/`, and `.tools/publish-mods/` | These ignored workspace-local DLL/manifest/PCK outputs were historical validation artifacts, not the current `publish/` package or installed-artifact evidence. |
| Stale install backup | Deleted `.tools/install-backups/` | This was an unreferenced backup of older installed `EZMicroBalance` artifacts; current package evidence remains under `publish/` and runtime evidence. |
| Generated local tool/cache folders | Deleted `.tools/browser-automation/`, `.tools/godot-appdata/`, `.tools/godot-localappdata/`, and `.tools/godot-user/` | These were generated Playwright/Godot cache or scratch folders. The actual Godot executable, GDRE/decompilation tools, ILSpy, runtime evidence, and art provenance remain. |
| Local Playwright screenshots/logs | Ignored `/output/playwright/` | Already tracked evidence files remain reviewable, but new browser screenshots, HARs, logs, and PID files no longer flood the Spire Plus release-candidate status. |
| Stale feature-folder archive guidance | current docs now point historical prompts/specs to `docs/archive/feature-inputs/` | Guarded so active feature folders do not grow local `archive/` prompt/spec folders again. |
| Current worktree batch pathspecs | `.tools/worktree-batches/current/` | Ignored local staging aids generated by `scripts/report-worktree-batches.ps1`; current 2026-05-24 snapshot has 319 dirty entries and 0 unclassified paths after the test UID cleanup. |

## Refactor Cleanup Completed

| Area | Result | Guard |
| --- | --- | --- |
| Repository path helpers | Centralized in `tests/EZMicroBalance.Tests/TestRepo.cs`. | `TestInfrastructureGuardTests.GuardTestsUseSharedRepositoryPathHelpers` |
| Source evidence assertions | `AssertSourceContains` centralized in `TestRepo.cs`. | Same infrastructure guard. |
| Mojibake assertions | `AssertNoMojibake` centralized in `TestRepo.cs`; feature-specific bad-fragment lists remain local to their tests. | Same infrastructure guard and test README. |
| Bilingual localization key assertions | `AssertLocalizedKeys` centralized in `TestRepo.cs`; feature tests can pass a feature-specific value validator such as Morvi mojibake checks. | Same infrastructure guard and test README. |
| JSON/source slicing/order/count helpers | `JsonStringMap`, `SliceFrom`, `SliceBetween`, `AssertBefore`, and `CountOccurrences` centralized in `TestRepo.cs`. | Same infrastructure guard and test README. |
| ZIP/PCK/hash/source-tree helpers | `ReadZipBytes`, `ReadZipText`, `ReadPckDirectory`, `ReadSourceTree`, `ReadAllTestSource`, and `Sha256` centralized in `TestRepo.cs`. | Same infrastructure guard and test README. |
| Recursive JSON values, manifest version, PNG byte/dimension, JSON normalization, and exception unwrap helpers | `JsonStringValues`, `ManifestVersion`, `ReadPngBytes`, `ReadPngDimensions`, `NormalizeJson`, and `Unwrap` centralized in `TestRepo.cs`; local duplicate helpers removed from release/art/ancient guard tests. | Same infrastructure guard and test README. |
| Export preset parsing | `ParseExportFiles` centralized in `TestRepo.cs` and removed from four release/art guard test files. | Same infrastructure guard and test README. |
| Active release resource predicates | `IsActiveExportResource` and `IsActiveReleaseResource` centralized in `TestRepo.cs`; duplicate package-boundary predicates removed from release guard tests. | Same infrastructure guard and test README. |
| Ascension source tree test reads | `AscensionFeatureGuardTests` now uses shared `ReadSourceTree` instead of a local source-tree reader. | Same infrastructure guard. |
| Ancient feature gate env parsing | Shared `AncientFeatureGate` helper under `EZMicroBalanceCode/Ancients/Common/`. | Release/source guards. |
| Ascension combat metadata refresh | Duplicated metadata lookup consolidated in `AscensionCombatModifierService`. | `AscensionV2MilestoneGuardTests`. |
| Stale Morvi v2.2 enable constant | Removed unused `LegacyEnableEnvironmentVariable` from `MorviFeatureGate`; Morvi is current default-on with disable/force gates. | Build/test validation. |
| Current display-name docs | Updated current-facing intro/audit/status wording to use `Spire Plus` with `EZMicroBalance` only as the stable manifest id. | Diff-check and release/name guards. |
| Current manual-test display-name wording | Active manual verification and waiting-test instructions now say `Spire Plus`; `EZMicroBalance` remains only for technical ids, paths, commands, and literal historical log evidence. | Default tests and targeted `rg` scan. |
| Ancient UI PNG size helper | `AncientUiReadinessGuardTests` now uses shared `ReadPngDimensions`; the local `ReadPngSize` duplicate was removed. | Same infrastructure guard. |
| Lotha source slicing helper | `LothaPolishGuardTests` now uses shared `SliceBetween`; the local `SourceSlice` duplicate was removed. | Same infrastructure guard. |
| Shared-readable log file helper | `ReleaseSafetyExpandedGuardTests` now uses `ReadSharedText` from `TestRepo.cs` instead of a local `ReadAllTextShared` helper. | Same infrastructure guard and test README. |
| Repository path existence assertions | `TestRepo.cs` now exposes shared file/directory/path-exclusion assertion helpers; `TestInfrastructureGuardTests`, `AncientUiReadinessGuardTests`, `MorviV22GuardTests`, `ReleaseCoverageGuardTests`, `ReleaseSafetyExpandedGuardTests`, and `ReleaseArtifactTests` use them for cleanup/resource boundary checks. | Same infrastructure guard and test README. |
| Release artifact assembly resolver | `ReleaseArtifactTests` now uses one class-local `CreateAssemblyResolver` helper for installed game/BaseLib/mod API checks instead of repeating the same `AssemblyResolve` search loop in each test. | Targeted `ReleaseArtifactTests` validation. |
| Release artifact no-op assertion | Removed a tautological self-hash assertion from `ReleaseArtifactTests` and kept the meaningful source-copy hash parity check. | Targeted `ReleaseArtifactTests` validation. |
| Release artifact localization key helper | `ReleaseArtifactTests` now reuses shared `JsonStringMap` for localization key parity instead of carrying a local `JsonKeys` JSON parser; `TestInfrastructureGuardTests` now prevents `JsonKeys` from being reintroduced as a copied helper. | Targeted `ReleaseArtifactTests` and infrastructure validation. |
| Active project-state doc length | Superseded per-pass validation/package history was archived from `PROJECT_STATE.md` into `docs/archive/project-state-history-20260516.md`; the active first-read state file now focuses on current status and pending gates. | `TestInfrastructureGuardTests.ProjectStateStaysCurrentAndHistoricalPassLogIsArchived`, release/source/doc guards, and default validation. |
| Root local clutter regression guard | `TestInfrastructureGuardTests` now checks that root `art_pipeline/` and `asset/` do not return, local archive folders exist, ignore rules are present, and docs map the archives. | Same infrastructure guard. |
| Owner-sensitive local material guard | `TestInfrastructureGuardTests` now checks that `source code/`, `publish/`, `.tools/`, local props, and binary/package outputs remain ignored and decision-tracked instead of silently committed or deleted. | Same infrastructure guard. |

## Completion Audit Against Cleanup Goal

Goal restated as concrete deliverables:

1. Remove or refactor clearly redundant code without changing external behavior.
2. Move stale or superseded documentation out of the active reading path.
3. Preserve uncertain material in an archive before any deletion.
4. Delete only material that has an archive copy or is proven generated/local-only and safe to ignore.
5. Keep active `Spire Plus` source/resource/test/docs changes reviewable.
6. Record a completion audit based on actual files and command output.

| Requirement | Evidence inspected | Status |
| --- | --- | --- |
| Redundant code/test infrastructure reduced | `tests/EZMicroBalance.Tests/TestRepo.cs` exists; `TestInfrastructureGuardTests` guards duplicate repository/path/source/JSON/PCK/hash/export/Png helpers; `rg` over guarded duplicate helper names returns no active duplicate definitions outside `TestRepo.cs`. | Complete for the audited helper classes. |
| Feature-local historical prompt/spec archives moved out of active feature folders | `Get-ChildItem docs\features -Recurse -Directory -Filter archive` reports `0`; archived copies live under `docs/archive/feature-inputs/`. | Complete for the known feature-local archive folders. |
| Superseded setup spec moved out of active docs | `docs/SETUP_SPEC.md` no longer exists; `docs/archive/superseded/setup-spec-original-scaffold.md` exists; docs point current setup to `README.md`, `docs/README.md`, and `docs/dev-environment.md`. | Complete. |
| Superseded project-state validation history moved out of first-read docs | `PROJECT_STATE.md` now links to `docs/archive/project-state-history-20260516.md`; archive indexes map the snapshot. | Complete. |
| Uncertain/generated root clutter preserved before deletion | Root `art_pipeline/`, `asset/`, and website preview snapshots were moved to ignored `.tools/archive/...` locations and documented in `docs/doc-inventory.md` / `docs/PROJECT_MAP.md`. | Complete for the handled local clutter. |
| Current website source is visible, generated forum/editor output is ignored | `website/` and `.github/workflows/spire-plus-site.yml` are tracked current surfaces; `.gitignore` ignores only `/website/forum/` and `/website/**/*.import`. | Complete: source changes remain reviewable, local Vite build output and Godot sidecar metadata stay out of the release-candidate diff. |
| Local browser screenshots/logs kept out of release-candidate diff | `.gitignore` ignores `/output/playwright/`; existing tracked evidence stays visible, while new local browser output is ignored. | Complete unless the owner deliberately promotes a screenshot as evidence. |
| Broad ignored-file deletion is unsafe | `git clean -ndX` | Current dry run shows ignored build/editor caches plus `.tools/`, `publish/`, `source code/`, and `Directory.Build.props`; do not run broad `git clean -fdX` for this workspace. | Complete as a cleanup boundary. |
| Active release/source/test changes remain visible | `git status --short` still shows active `EZMicroBalance/`, `EZMicroBalanceCode/`, `tests/`, `scripts/`, and docs changes rather than hiding them. | Complete. |
| Owner-sensitive material not deleted blindly | `source code/`, `publish/`, retained `.tools` evidence/tool folders, and the promoted website are documented below. The old root `legacy/` folder was archived, duplicate root mod surfaces were removed after the owner rule change, the earlier website draft snapshot was preserved, and generated `.tools` clutter was pruned by exact path. | Complete: remaining retained areas have current evidence or hard-rule justification. |
| Validation covers cleanup edits | Build, default tests, format verification, and diff whitespace checks pass as recorded below. | Complete for current cleanup edits. |

Conclusion: the cleanup/refactor pass is validated. Remaining large ignored/local areas are retained intentionally because they are current package output, source evidence, runtime evidence, art provenance, or local tool installations rather than confirmed redundant clutter.

## Prompt-To-Artifact Checklist

This maps the cleanup prompt to concrete artifacts and current evidence. Passing tests are not treated as sufficient by themselves; each row records what was actually inspected.

| Prompt requirement / deliverable | Concrete artifact or command | Current evidence | Status |
| --- | --- | --- | --- |
| Clean `D:\Game\FOTN\dev-the-spire`, not another workspace | `pwd` | Output path is `D:\Game\FOTN\dev-the-spire`. | Complete. |
| Remove or refactor redundant implementation/test code | `tests/EZMicroBalance.Tests/TestRepo.cs`, `TestInfrastructureGuardTests`, `AncientFeatureGate`, `AscensionCombatModifierService` | Shared test helpers now centralize repository paths, source reads, JSON/string helpers, PCK/ZIP/hash helpers, PNG dimension reads, export parsing, and active resource predicates; Ancient feature gates share env parsing; Ascension combat metadata refresh duplication was reduced. | Complete for the audited duplication classes; additional feature-specific helpers may remain by design. |
| Do not change external behavior while cleaning | Default build/test/format/diff validation | `dotnet build`, `dotnet test --no-build`, `dotnet format --verify-no-changes`, and `git diff --check` pass for the cleanup edits recorded below. | Complete for static validation; live gameplay behavior is outside this cleanup pass. |
| Move stale or superseded docs out of the active reading path | `docs/archive/superseded/setup-spec-original-scaffold.md`; `docs/archive/feature-inputs/`; `docs/archive/project-state-history-20260516.md`; `docs/archive/implementation-records/2026-05-15-current-package-verification-note.md` | `Test-Path docs\SETUP_SPEC.md` is `False`; archived setup spec exists; `Get-ChildItem docs\features -Recurse -Directory -Filter archive` reports `0`; current docs point to central archives; active `PROJECT_STATE.md` keeps current status while the historical pass log is archived; active `docs/issues.md` keeps the package snapshot compact and links the archived long note. | Complete for identified stale setup, project-state history, issue-note history, and feature prompt/spec docs. |
| Archive uncertain material before deletion | `.tools/archive/local-art-and-calibration-20260515/`, `.tools/archive/local-root-clutter-20260515/`, `.tools/archive/local-website-preview-20260516/`, `docs/archive/**` | Local clutter and website draft snapshots are documented under archive locations; historical prompt/spec files were moved to tracked `docs/archive/feature-inputs/`; original setup spec was moved to tracked `docs/archive/superseded/`. | Complete for handled uncertain material. |
| Delete only material proven safe or already archived | Deleted active-path copies of feature-local archive files, `docs/SETUP_SPEC.md`, the ignored website/Pages workflow draft, transient Edge browser profile/cache folders, stale redirected publish-output folders, an old install backup, and generated Playwright/Godot cache folders; archived root `legacy/` into `docs/archive/legacy-planning/legacy-project-files/` | Deleted tracked docs and moved legacy project metadata have archive copies; website and Pages workflow were ignored local drafts and now exist only in the cleanup snapshot; Edge profile/cache directories were unreferenced local browser state, while `.tools/art-generation/` and active resources preserve the actual art evidence; redirected publish folders, install backup, and generated cache folders were local outputs separate from the current `publish/` package and runtime evidence. | Complete for performed deletions; no scaffold/source/current-package/runtime-evidence directories were deleted. |
| Keep active `Spire Plus` source/resource/test/docs reviewable | `git status --short` | Active changes remain visible in `EZMicroBalance/`, `EZMicroBalanceCode/`, `tests/`, `scripts/`, docs, and `export_presets.cfg`; only local/generated drafts are ignored. | Complete. |
| Prevent regression into feature-local archive clutter | `TestInfrastructureGuardTests.FeaturePromptArchivesStayInCentralArchive` | Guard checks that historical prompt/spec archives live under `docs/archive/feature-inputs` and current docs/tests do not reference old feature-local archive paths. | Complete. |
| Keep current website source reviewable while ignoring generated forum/editor output | `.gitignore`, `website/`, `.github/workflows/spire-plus-site.yml`, `.tools/archive/local-website-preview-20260516/` | Root `website/` and the Pages workflow are tracked current surfaces; older snapshot remains archived; generated `website/forum/` output and `website/**/*.import` metadata are ignored. | Complete unless owner demotes the website again. |
| Preserve project hard rules and active manifest boundary | `README.md`, `AGENTS.md`, `docs/architecture-ez-micro-balance.md`, release guard tests | Active mod id remains `EZMicroBalance`; duplicate root mod surfaces are blocked from returning by repository hygiene guards. | Complete. |
| Confirm whether every uncertain area is useless before permanent deletion | Owner Deletion Decision Checklist below | `legacy/`, duplicate root mod surfaces, website draft, transient browser profiles, stale redirected publish outputs, old install backup, and generated cache folders were archived/deleted after exact-path checks. `source code`, current `publish`, and retained `.tools` evidence/tool folders are kept with explicit justification. | Complete. |
| Final completion audit based on actual state | This document plus validation commands | Audit records inspected files, commands, status, and retained boundaries. | Complete as an audit artifact. |
| Avoid broad ignored-file deletion | `git clean -ndX` dry run | Current dry run includes `.godot/`, `.tools/`, `Directory.Build.props`, `publish/`, `source code/`, and test `bin/obj`; cleanup must stay targeted. | Complete. |

## Owner Deletion Decision Checklist

Use this checklist before any permanent deletion. Do not delete a row unless the owner explicitly chooses `Delete after verification` and the verification column passes. If the owner is unavailable, use the default recommendation.

| Path / Area | Current evidence | Default recommendation | Owner choices | Verification before delete/promote |
| --- | --- | --- | --- | --- |
| Removed duplicate root mod surfaces | `EzDailyContent*`, `EZFuturePeek*`, `tests/EZFuturePeek.Tests/`, and `scripts/export-future-peek.ps1` are absent from the active root. | Keep removed. | Restore only through a deliberate new-mod decision with a stable manifest id. | Repository hygiene guards must keep these paths absent; any future new mod needs its own architecture decision before files return. |
| Former root `legacy/` | Top-level historical migration area contained only `.gdignore`, a README, and the old `EzDailyContent.csproj.legacy.xml`. | Archived. | Keep archived unless project history policy changes. | Moved to `docs/archive/legacy-planning/legacy-project-files/`; update docs/tests that referenced the old root path; run `rg -n "legacy/EzDailyContent|../legacy/README|`legacy/`" README.md PROJECT_STATE.md docs tests scripts --glob "!docs/archive/**"` and validation. |
| `source code/` | `docs/test-ready-development-goal.md` names `source code/src/Core/**` as primary source evidence; multiple tests read files from this path directly. | Keep as ignored local reference. | Keep; refresh from current game version; delete after replacing evidence path. | First provide an alternate local game-source path or fixture strategy; update all `ReadRepoText("source code", ...)` tests and source-evidence docs; run `rg -n "source code" tests docs README.md PROJECT_STATE.md`; run `dotnet test EZMicroBalance.sln` without relying on the deleted tree. |
| `publish/` | Package refresh scripts and opt-in release-artifact tests use `publish/SpirePlus-v0.1.0-private-beta.4.zip` and hash parity evidence. | Keep generated output ignored. | Keep; clean and rebuild; delete after package rebuild. | Run `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`; confirm package hashes/docs are refreshed before deleting stale artifacts. |
| `.tools/` | Remaining subfolders are runtime evidence, generated art provenance, local archives, downloaded/decompiled game tooling, Godot, or ILSpy. | Keep remaining subfolders; never delete wholesale. | Future targeted prune only for newly proven generated clutter. | For a proposed subfolder, run `rg -n "<subfolder-name>|.tools/" README.md PROJECT_STATE.md docs tests scripts`; keep current release evidence, art provenance, owner-accepted image sources, and source-inspection tools; after pruning, run relevant script/test guards plus `git diff --check`. |
| `website/` and `.github/workflows/spire-plus-site.yml` | Current tracked public site source and GitHub Pages workflow. `.tools/archive/local-website-preview-20260516/` remains as historical pre-promotion snapshot only. | Keep tracked and reviewable. | Keep current; demote back to archive only with owner approval. | If demoting later, archive the current tree, restore ignore rules for root `website/` and the workflow, update `docs/PROJECT_MAP.md`, `docs/doc-inventory.md`, and run website/source guard tests. |

Owner decision statuses:

| Path / Area | Status |
| --- | --- |
| Removed duplicate root mod surfaces | Removed after the 2026-05-20 owner rule change; keep absent from active root. |
| Former root `legacy/` | Archived to `docs/archive/legacy-planning/legacy-project-files/`; top-level folder removed. |
| `source code/` | Default keep because current tests/docs require it. |
| `publish/` | Retained current package/staging/cover-source output; stale old-name package folder was deleted. Future prune should happen only after a new package rebuild/hash refresh. |
| `.tools/` | Unreferenced Edge browser profile/cache folders, stale redirected publish outputs, an old install backup, and generated Playwright/Godot cache folders were deleted; remaining `.tools/` subfolders are retained as current evidence, art provenance, local archives, or local tool installations. Wholesale deletion is not recommended. |
| `output/playwright/` | Default ignored for new browser screenshots/logs/HARs/PID files. Existing tracked evidence remains visible until reviewed in its own batch. |
| `website/` and `.github/workflows/spire-plus-site.yml` | Promoted and tracked as current public site source and Pages workflow. Generated `website/forum/` output and `website/**/*.import` metadata remain ignored. |

## Latest Validation For Cleanup Changes

- `dotnet build EZMicroBalance.sln --no-restore`: pass, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~TestInfrastructureGuardTests`: pass, 8 passed.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~AncientUiReadinessGuardTests`: pass, 11 passed.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~MorviV22GuardTests`: pass, 5 passed.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseCoverageGuardTests`: pass, 23 passed / 3 skipped.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseSafetyExpandedGuardTests`: pass, 12 passed / 6 skipped.
- `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseSafetyExpandedGuardTests`: pass, 18 passed / 0 skipped.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseArtifactTests`: pass, 9 passed / 7 skipped.
- `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseArtifactTests`: pass, 16 passed / 0 skipped.
- Combined current-doc guard filter (`ReleaseCoverageGuardTests`, `ReleaseSafetyExpandedGuardTests`, `AncientStateMirrorGuardTests`, `AncientUiReadinessGuardTests`, `AncientArtAssetHygieneGuardTests`, `TestInfrastructureGuardTests`): pass, 68 passed / 9 skipped.
- `PROJECT_STATE.md` cleanup check: active file is 81 lines; `docs/archive/project-state-history-20260516.md` exists; stale `Latest ...` per-pass history scan reports no active `PROJECT_STATE.md` hits.
- `docs/issues.md` package-note cleanup guard filter (`ReleaseCoverageGuardTests`, `AncientStateMirrorGuardTests`, `AncientUiReadinessGuardTests`, `AncientArtAssetHygieneGuardTests`, `VakuuLothaSaveRiskGuardTests`, `VakuuTemptationGuardTests`, `TestInfrastructureGuardTests`): pass, 70 passed / 3 skipped.
- Website promotion check: `git ls-files website .github\workflows\spire-plus-site.yml` lists the tracked current source and workflow; `.tools\archive\local-website-preview-20260516\website\index.html` remains as historical snapshot; `git check-ignore -v website\forum\index.html website\assets\relics\relic.png.import` confirms generated forum output and Godot sidecar metadata remain ignored.
- Edge browser profile/cache deletion check: `Test-Path .tools\edge-chatgpt-profile-clone-20260515`, `Test-Path .tools\edge-chatgpt-profile-clone2-20260515`, `Test-Path .tools\edge-chatgpt-profile`, and `Test-Path .tools\edge-chatgpt-pw-profile` return `False`; `rg -n "edge-chatgpt-profile|edge-chatgpt-pw-profile" README.md PROJECT_STATE.md docs tests scripts --glob "!docs/archive/**"` returns only cleanup-audit references after deletion; `.tools` size scan no longer lists those browser profile/cache directories.
- Redirected publish output deletion check: `Test-Path .tools\publish-redirect`, `Test-Path .tools\publish-redirect2`, and `Test-Path .tools\publish-mods` return `False`; references are limited to historical work-log entries that explicitly describe redirected local publish output and do not replace installed/current package evidence.
- Install backup deletion check: `Test-Path .tools\install-backups` returns `False`; `rg -n "install-backups" README.md PROJECT_STATE.md docs tests scripts --glob "!docs/archive/**"` returns no references.
- Generated local tool/cache deletion check: `Test-Path .tools\browser-automation`, `Test-Path .tools\godot-appdata`, `Test-Path .tools\godot-localappdata`, and `Test-Path .tools\godot-user` return `False`; the only remaining active-doc reference among these names is a historical work-log line for `.tools/godot-user/` paired with now-deleted redirected publish output.
- Legacy root archive check: `Test-Path legacy` returns `False`; `docs/archive/legacy-planning/legacy-project-files/EzDailyContent/EzDailyContent.csproj.legacy.xml` and `docs/archive/legacy-planning/legacy-project-files/README.md` exist; current docs point to the archive path.
- Final ignored-file dry run: `git clean -ndX` now lists only `.godot/`, `.tools/`, `Directory.Build.props`, `publish/`, `source code/`, and test `bin/obj`; previously deleted website, Pages workflow, browser profile/cache, redirected publish, install-backup, and root `legacy/` clutter do not appear.
- Final targeted cleanup/path guard filter (`TestInfrastructureGuardTests`, `ReleaseArtifactTests`, `ReleaseCoverageGuardTests`, `AncientBehaviorGuardTests`): pass, 59 passed / 11 skipped.
- Post-cleanup guard update: `dotnet test EZMicroBalance.sln --filter "FullyQualifiedName~TestInfrastructureGuardTests|FullyQualifiedName~ReleaseCoverageGuardTests|FullyQualifiedName~AncientStateMirrorGuardTests|FullyQualifiedName~AncientUiReadinessGuardTests|FullyQualifiedName~AncientArtAssetHygieneGuardTests|FullyQualifiedName~VakuuLothaSaveRiskGuardTests|FullyQualifiedName~VakuuTemptationGuardTests"` passed, 70 passed / 3 skipped.
- `git clean -ndX`: dry-run only; confirms broad ignored-file deletion would hit owner-sensitive/local-evidence paths and must not be used as cleanup.
- Final `dotnet test EZMicroBalance.sln --no-build`: pass, 170 passed / 18 skipped.
- Final `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: pass.
- Final `git diff --check`: pass, with existing CRLF normalization warnings only.

Re-run validation after every subsequent cleanup batch.
