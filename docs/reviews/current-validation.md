# Current Validation

Date: 2026-06-11; latest addendum: 2026-06-17

## June 17 Validation-Lane Script Addendum

- Source/test validation was rerun in a single active lane after stale duplicate `testhost` processes were cleared: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passed with 0 warnings / 0 errors; isolated `ReleaseEvidenceGateTests` passed 9 / 0 / 0 with `migration-ci-helper-release-evidence-final-tree-diag.log`; focused governance/compactness guards passed 49 / 0 / 0; the complementary no-build test-project lane excluding `ReleaseEvidenceGateTests` passed 448 / 0 / 39 / 487; `dotnet format`, patch inventory, worktree batch classification, and `git diff --check` passed with CRLF normalization warnings only.
- No publish, package refresh, runtime smoke, staging, release handoff, or release-ready claim was produced by the June 17 source/test lane.
- `scripts/ci-full-validation.ps1` now defaults to the beta.85 split no-build strategy: isolated `ReleaseEvidenceGateTests`, then the complementary test-project lane excluding that class. The same helper runs that split lane under `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` after publish/package. `-TestStrategy Solution` remains available only for a deliberate legacy one-shot comparison.
- Local game-source guard tests that read ignored `source code/src/Core/**` now use `LocalSourceFactAttribute` and are opt-in through `SPIREPLUS_RUN_LOCAL_SOURCE_GUARDS=1`. Normal test lanes no longer require that ignored source snapshot; the opt-in lane should be run only after refreshing local game source from the current installed version. This env-var rename is a static wiring change only until post-pause `dotnet test` validation confirms it; older diagnostic logs may mention the abandoned `SPIREPLUS_RUN_LOCAL_SOURCE_TESTS` name and should not be cited as current local-source lane proof.
- Pause-safe checks for this script repair: PowerShell parser check passed for `scripts/ci-full-validation.ps1`; `docs/test-ready-development-goal.md` remains under the 120-line compactness guard; touched-file `git diff --check` passed; `scripts/report-worktree-batches.ps1 -FailOnUnclassified` reported 0 unclassified entries.
- StS1 v20 static alignment follow-up stayed inside the same pause boundary: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 896 checks / 0 mismatches; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 66 checks / 0 mismatches across 15 roles; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches; `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 11 checks / 0 mismatches; `scripts/check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap. `docs/features/sts1-events/test-plan.md` now treats the no-launch runtime preflight as a prerequisite before future enabled-mode launches. This is no-launch/static evidence only and does not close O25, O33, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.
- StS1 runtime preflight was read-only only: `scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch` reported game `release_info.json` at `v0.107.0`, installed `STS2-RitsuLib\mod_manifest.json` version `0.4.16`, `STS2-RitsuLib\lib\0.107.0\compat-target.txt` at `0.107.0`, installed `EZMicroBalance\EZMicroBalance.json` at `v0.1.0-private-beta.85`, CanaryOnly source-only expected shape 6 calls / 4 types, AdditiveBatch1 source-only expected shape 14 calls / 10 types, and 23 checks / 0 mismatches. This is prerequisite/source-shape evidence only, not an enabled-mode launch or verifier packet.
- Follow-up no-game validation for the static-preflight/test split work: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passed with 0 warnings / 0 errors; focused Ancient/Ascension/Lotha/Vakuu touched guard classes plus governance/compactness passed 137 / 0 / 13 / 150 with local-source and release-artifact guards skipped by design.
- The updated full local CI script still needs a real post-pause run before it can be cited as current full validation evidence.

## June 15 Pause-Safe Static Verification Addendum

- No build, test, publish, package/release-evidence validation, runtime smoke, staging, commit, or push was started from this thread while the same-repo migration validation lane remains paused.
- Pause-safe static verification was rerun after adding active summary direct localization non-proof guards: `scripts/check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures, keeping the 33-key localization gap as known/non-failing; `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 715 checks / 0 mismatches in that pass, later superseded by the 872-check follow-up below; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches.
- Pause-safe subagent evidence-packet checklist hardening, replacement gate-range split hardening, O14/O15 source-identity classification, format-pause wording, direct-key localization non-proof mirroring, status-board remaining-gate split hardening, post-pause QA/release/owner row splitting, evidence-map runtime/static range splitting, active event-goal checkpoint guarding, active-goal stale-count scope guarding, subagent owner/final-handoff non-authorization guarding, direct enabled-mode copied-log `-AuditPath` hardening, verifier `-FailOnMismatch` command guards, verifier expected-target command guards, enabled-mode packet missing-state bypass guards, aggregate static-suite composition/fail-closed wrapper guards, retained `audit-godot-log.ps1` command guards, live-session prepare/restore command guards, runtime-smoke checklist live-session prerequisite guard, runtime-smoke checklist broad stale-scan inclusion guard, runtime-smoke checklist static-file hygiene scope guard, next-overnight runtime-plan stale-scan/static-hygiene scope guards, and RitsuLib monthly/Batch 4c static-file hygiene scope guards were added to `docs/goals/event.md`, `docs/features/sts1-events/v19-subagent-coverage.md`, `docs/features/sts1-events/test-plan.md`, `docs/features/sts1-events/status-board.md`, `docs/features/sts1-events/localization.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/ritsulib-migration/runtime-smoke-checklist.md`, `docs/features/ritsulib-migration/next-overnight-run.md`, `docs/features/ritsulib-migration/monthly-dev-spec.md`, `docs/features/ritsulib-migration/batch-4c-candidates.md`, `scripts/README.md`, and the v19 hard-stop report, then guarded by `scripts/check-sts1-v19-subagent-coverage.ps1`, `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch`, and `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch`. The follow-up static rerun returned `scripts/check-sts1-event-static-suite.ps1` 14 static steps / 0 suite failures, `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` 872 checks / 0 mismatches, `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` 11 checks / 0 mismatches, `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` 531 checks / 0 mismatches, `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` 63 checks / 0 mismatches, and `git diff --check --` exit 0 with CRLF warnings only.
- `git diff --check --` exited 0 and emitted only CRLF normalization warnings for `AGENTS.md` and `docs/goals/refactor.md`; no whitespace errors were reported.
- This static verification does not close O25, O33, enabled-mode, gameplay, save/load, replacement, multiplayer, image/render, QA, release, or handoff gates.

## June 11 Revision M Runtime Drift Addendum

- M5 Revision M loader/runtime-drift blocker is closed for Off-mode loader proof only. The red root-cause packet remains `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`: beta.84 reached main menu with RitsuLib `v0.4.16` / compat branch `0.107.0`, but Spire Plus applied only 17/25 ModPatcher patches, logged 8 optional ModPatcher failures, and threw an `EctoplasmGoldGatePatch` initializer exception.
- The beta.85 source/package contains targeted fixes for that drift: ModPatcher getter targets use property names with `MethodType.Getter`, and `EctoplasmGoldGatePatch` targets `Ectoplasm.ModifyGoldGained(Player, decimal)`.
- Source-fix context under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/` applied 25/25 patches and audited clean on `v0.107.0`, but that log still reports beta.84.
- Current beta.85 Off proof is `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`: the log reports `v0.1.0-private-beta.85`, RitsuLib `0.4.16` with compat branch `0.107.0`, 25/25 Spire Plus ModPatcher patches applied, StS1Events default Off, main menu reached, and `godot-log-audit.json` is clean with 0 blocking signature hits.
- The same already-captured beta.85 Off packet has a retained no-launch verifier report at `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json`: Off packet checks=34 / mismatches=0 after adding the explicit RitsuLib package-version target, nested log verifier checks=10 / mismatches=0. This is evidence bookkeeping for default-Off proof only, not CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, image/render, or QA proof.
- Installed beta.85 package parity is recorded in `PROJECT_STATE.md` as passed via `scripts\check-installed-spire-plus-package.ps1`. The current beta.85 release/package artifact, artifact parity, Ascension milestone, and Ancient behavior subset also passed with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`: 67 passed / 0 failed / 0 skipped.
- `PROJECT_STATE.md` now records the latest beta.85 runtime-fix validation as 0 build errors, 0 warnings, the isolated `ReleaseEvidenceGateTests` class passing 9 passed / 0 failed / 0 skipped / 9 total, and the complementary no-build test-project lane excluding `ReleaseEvidenceGateTests` passing 466 passed / 0 failed / 21 skipped / 487 total, for split coverage of 475 passed / 0 failed / 21 skipped / 496 total after stale current-repo `testhost` locks were cleared. The dated June 10 command table below remains historical evidence for that lane.
- Final post-doc-refresh command evidence on 2026-06-11: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passed with 0 warnings / 0 errors; `ReleaseEvidenceGateTests` passed 9 / 0 / 0 / 9 with diag `migration-beta85-release-evidence-post-doc-final-diag.log`; the complementary no-build test-project lane passed 466 / 0 / 21 / 487 with diag `migration-beta85-non-release-evidence-post-doc-final-diag.log`; the opt-in installed-artifact lane passed 67 / 0 / 0 / 67 with `STS2_PATH=E:\Steam\steamapps\common\Slay the Spire 2`; `dotnet format`, patch inventory, worktree batch classifier, `git diff --check`, and installed package parity all passed.
- Runtime-ready/live-ready/release-ready remain blocked: no gameplay, clicked UI, save-load, co-op, event encounter, replacement, independent QA, or release handoff proof was produced by the loader smoke. Active same-repo `dotnet`/`testhost` processes were observed during the continuation, so do not start overlapping validation lanes.
- StS1 v19 validation coordination hard stop is recorded at `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md`. The current O0-O76 gate map is `docs/features/sts1-events/v19-gate-evidence-map.md`, and the per-gate ledger is `docs/features/sts1-events/v19-gate-ledger.csv` guarded by `scripts/check-sts1-v19-gate-ledger.ps1`: `O11-O20` has static source/doc coverage, `O21-O24` has beta.85 default-Off loader proof, and `O25-O76` remain current-pending or blocked except for source-guarded replacement/classification surfaces. Beta.85 Off proof must not be extended to CanaryOnly, AdditiveBatch1, gameplay, replacement, multiplayer, or QA gates.

## June 11 StS1 Event Source/Guard Addendum

- The StS1 event source/resource/test/doc changes listed here are covered by the beta.85 split no-build validation recorded above. That validation is still no-game evidence; it does not prove enabled-mode loader registration, event encounter gameplay, UI render, save/load, replacement behavior, or multiplayer disposition.
- `Sts1DivineFountain` now overrides `IsAllowed(IRunState)` and requires every run participant to have at least one curse before the shared event is eligible. `Sts1EventFeatureGuardTests.DivineFountainRequiresEveryPlayerToHaveACurse` guards the source behavior.
- `Sts1BigFish` now uses the wiki-aligned Box option identity (`InitialOptionKey("BOX")`) with matching EN/ZHS localization keys. `Sts1EventFeatureGuardTests.BigFishUsesBoxOptionName` guards the source/localization shape.
- `Sts1GoldenIdol` now uses the Outrun / Smash / Hide trap branch identities and values in source/localization, while still marking the random-relic Take reward as a non-parity substitute for the missing Golden Idol relic model. Guard coverage is included in the beta.85 StS1 event source lanes.
- `Sts1TheLab` now exposes only the Open option, removes unused EN/ZHS Leave keys, and keeps the source 3-potion / A15+ 2-potion split. `Sts1EventFeatureGuardTests.TheLabHasOnlyOpenOption` guards the source/localization shape.
- `Sts1OldBeggar`, `Sts1ShiningLight`, `Sts1GoldenShrine`, and `Sts1TheCleric` have source/localization/doc guard coverage for the current AdditiveBatch1 source contracts: paid-removal affordability, random upgrades, Pray/Desecrate values, Act 1 bucket registration, and A15 Purify cost/eligibility behavior.
- Static resource parity is not source-complete localization coverage: `docs/features/sts1-events/localization-source-gap-scan-20260611.md` records 33 source-referenced StS1 result-page keys missing from both EN and ZHS. The static closure order is `docs/features/sts1-events/localization-gap-closure-plan.md`; it changes no shipped resources by itself.
- Static-only event evidence now includes `scripts/check-sts1-event-static-suite.ps1`, which wraps registry, enabled-log expected-shape, current-doc-claims, static-file hygiene, v19 gate-ledger, v19 subagent coverage, spec, feature-gate, parity, asset, multiplayer, localization source-key, and localization gap-baseline checks. The current expected summary is 14 static steps, 0 suite failures, and the 33-key localization gap reported as known/non-failing until those keys are intentionally closed in a versioned resource pass.
- The current beta.85 Off smoke proves only the default-Off loader path. It does not prove Big Fish Box UI render, Golden Idol trap result render, The Lab result render, Divine Fountain natural-pool eligibility, AdditiveBatch1 enabled-mode registration, save/load, replacement behavior, multiplayer disposition, or gameplay parity.

## June 10 Migration Reconciliation Addendum (Historical)

- June 10 source fix: `Sts1EventRegistrationService` registered Big Fish and Golden Idol into the StS2 Act 1 buckets (`Overgrowth` and `Underdocks`) for CanaryOnly, AdditiveBatch1, and RegisterAll, matching the Sts1Event guard-test contract and then-current status-board counts. Later beta.85/v19 docs add The Cleric and Shining Light Act 1 bucket count reconciliation.
- Validation commands completed in this pass:
  - `dotnet build EZMicroBalance.sln -m:1 --no-incremental`: PASS, 0 warnings, 0 errors.
  - `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~Sts1EventFeatureGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1`: PASS, 31 passed / 0 failed / 0 skipped.
  - `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --logger "console;verbosity=minimal" --diag tests\EZMicroBalance.Tests\TestResults\migration-final-testproject-diag.log -- RunConfiguration.MaxCpuCount=1`: PASS, 464 passed / 0 failed / 21 skipped / 485 total.
  - `dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" --diag tests\EZMicroBalance.Tests\TestResults\migration-final-solution-diag.log -- RunConfiguration.MaxCpuCount=1`: PASS, 464 passed / 0 failed / 21 skipped / 485 total.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: PASS.
  - `.\scripts\generate-patch-inventory.ps1 -Check`: PASS.
  - `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified`: PASS, 130 dirty entries, 0 unclassified.
  - `git diff --check`: PASS with CRLF normalization warnings only for `AGENTS.md`, `docs/goals/refactor.md`, and `docs/patch-inventory.md`.
- Runner note: an earlier test-project attempt in this pass reported 55 passed before a testhost abort and left VSTest processes alive; those PIDs were stopped, then the diagnostic reruns above passed cleanly.
- Runtime status was later superseded by the June 11 beta.85 Off loader smoke above. This June 10 lane itself produced no game launch, package refresh, or live/manual proof.

## Current Normal-Run Skipped Tests

The 21 skipped tests in normal `dotnet test` lanes are source-explained by 21 usages of `[ReleaseArtifactFact]`. `tests/EZMicroBalance.Tests/ReleaseArtifactFactAttribute.cs` skips these tests unless `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` or legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set, because they require ignored publish/package outputs, installed DLL/PCK files, or local runtime smoke-log artifacts.

| File | Skipped methods | Why normal runs skip them |
|---|---:|---|
| `AncientBehaviorGuardTests.cs` | 1 | Versioned private-beta ZIP contents require refreshed package artifacts. |
| `AscensionV2MilestoneGuardTests.cs` | 1 | Current Ascension localization must be verified inside the packaged artifact. |
| `ReleaseArtifactParityGuardTests.cs` | 7 | Cover-art policy, installed/package PCK parity, release hash claims, runtime-log version/API-drift checks, smoke-claim support, and disabled-plug-off evidence require package or smoke evidence. |
| `ReleaseArtifactTests.cs` | 7 | Audited art, published PCK contents, installed DLL/manifest parity, installed-game Harmony target resolution, installed Urda asset paths, and Prismatic Gem installed API checks require publish/install artifacts. |
| `ReleasePackageArtifactGuardTests.cs` | 5 | Versioned ZIP/install hash parity, hash docs, handoff artifact claims, and installed/package PCK text checks require refreshed package staging and installed files. |

Exact skipped method list from source:

- `AncientBehaviorGuardTests.PrivateBetaZipContainsOnlyInstallableActiveModFiles`
- `AscensionV2MilestoneGuardTests.PackageContainsCurrentAscensionLocalization`
- `ReleaseArtifactParityGuardTests.ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage`
- `ReleaseArtifactParityGuardTests.ExportedResourcesInstalledPckAndPackagePckStayInParity`
- `ReleaseArtifactParityGuardTests.CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts`
- `ReleaseArtifactParityGuardTests.CurrentRuntimeLogVersionMustMatchManifest`
- `ReleaseArtifactParityGuardTests.RecentRuntimeLogMustNotContainV105ApiDriftOrBaseLibDependencyFailures`
- `ReleaseArtifactParityGuardTests.RecentSmokeLogSupportsControlledSmokeClaims`
- `ReleaseArtifactParityGuardTests.DisabledSpirePlusPlugOffEvidenceSupportsDocs`
- `ReleaseArtifactTests.ActiveReleaseArtMatchesAuditedNoTextNoLogoAsset`
- `ReleaseArtifactTests.PublishedPckContainsOnlyActiveReleaseResources`
- `ReleaseArtifactTests.InstalledDllMatchesABuildOutput`
- `ReleaseArtifactTests.InstalledManifestMatchesRepositoryManifest`
- `ReleaseArtifactTests.HarmonyPatchesResolveAgainstInstalledGameApi`
- `ReleaseArtifactTests.InstalledUrdaUsesCustomAncientAssetPaths`
- `ReleaseArtifactTests.PrismaticGemRewardBannerContractMatchesInstalledGameApi`
- `ReleasePackageArtifactGuardTests.PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes`
- `ReleasePackageArtifactGuardTests.CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals`
- `ReleasePackageArtifactGuardTests.PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers`
- `ReleasePackageArtifactGuardTests.InstalledAndPackagedPckCarrySereTalonTanxClawsSplit`
- `ReleasePackageArtifactGuardTests.InstalledAndPackagedPckCarryTrialBranchShortChoiceText`

## June 10 Refactor Validation (Historical)

- HEAD before this pass: `f32c6767 (HEAD -> main, origin/main, origin/HEAD) update refactor.md with implementation results and Green Stop check`.
- Worktree: dirty before this pass with existing goal/migration doc edits and deleted goal files; those pre-existing edits were preserved.
- Source compatibility fix: adapted the then-current code to the installed game DLL API by using `AbstractModel.ModifyPowerAmountGivenAdditive(...)`, `Ectoplasm.ModifyGoldGained(...)`, and `CookRestSiteOption.get_IsEnabled`.
- Warning burn-down: expanded Sts1Events owner guards now cover the compile-included Sts1Events model set.
- June 10 forced build validation: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passed after stale `testhost` locks were cleared, with **0 errors and 0 warnings**. This cleared the prior 70-warning Sts1Events nullable staging debt in that source snapshot.
- June 10 test-project validation: `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --logger "console;verbosity=normal" -- RunConfiguration.MaxCpuCount=1` passed with **464 passed / 0 failed / 21 skipped / 485 total**. The formerly problematic stale-loader handoff test and the full handoff pair passed after isolating the PowerShell handoff runner from VSTest host I/O.
- June 10 solution-level test status: **PASS**. Exact rerun after clearing overlapping validation processes: `dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" --diag tests\EZMicroBalance.Tests\TestResults\solution-after-zero-warning-build-diag.log -- RunConfiguration.MaxCpuCount=1` passed with **464 passed / 0 failed / 21 skipped / 485 total**. Earlier `testhost` crashes during same-repo cross-thread validation overlap remain runner-contamination evidence, not current beta.85 validation truth.
- June 10 hygiene validation: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `.\scripts\generate-patch-inventory.ps1 -Check`, and `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` passed. `git diff --check` emitted only the existing CRLF normalization warning for `docs/patch-inventory.md`; dirty goal docs and deleted goal files were preserved pre-existing/concurrent work.
- Runtime/live status for this June 10 lane: no gameplay, event UI, save-load, co-op, release package, or live-ready evidence was produced in this pass. The local installed game is now `v0.107.0`; installed RitsuLib was updated to official `v0.4.16` with `lib\0.107.0`. On 2026-06-10 the installed beta.84 DLL was restored from package staging, changing the installed Spire Plus DLL SHA256 from stale `69DEB870A226FD58EC9AF9D8895EEDC832B5D9A8903A2D79B1D6CEDC2E114EB1` to packaged `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766`; `scripts\check-installed-spire-plus-package.ps1` then passed. The fresh `v0.107.0` beta.84 Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` reached main menu but failed clean runtime proof: 11 Godot ERROR hits, 1 Spire Plus error/exception hit, 8 optional ModPatcher failures, and a `TargetInvocationException` rooted in stale `EctoplasmGoldGatePatch` target API drift. This beta.84 result is now root-cause evidence superseded by the clean beta.85 Off loader smoke recorded in the June 11 addendum.

Historical sections below are retained as dated evidence records. Do not use their older warning counts, dirty counts, runtime version, or pass/fail status as current validation truth without comparing them to the June 11 beta.85 addendum above.

Date: 2026-06-02

## Sprint 4 Canonical Validation

- HEAD: `f20dd230 (HEAD -> main) fix nullable warnings in 4 canary event files`
- Branch: `main...origin/main`
- Worktree: **CLEAN** (0 dirty entries). All prior dirty entries committed.
- Historical runtime smoke: Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 loader-gate evidence was valid for the June 2 source/runtime state. Current beta.85 keeps only default-Off `v0.107.0` loader proof clean; current CanaryOnly/AdditiveBatch1 enabled-mode proof remains pending.

### Sprint 4 Required Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, **79** Sts1Events nullable warnings (CS8602, CS8604, CS8625). Reduced from 89 by fixing 4 canary event files. |
| `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `.\scripts\generate-patch-inventory.ps1 -Check` | PASS | Patch inventory is fresh. |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | PASS | 0 dirty entries, 0 unclassified. |
| `dotnet publish EZMicroBalance.sln` | PASS | Published to local installed mod folder for runtime smoke. |

### June 2 Runtime Path Check

| Path | Exists |
| --- | --- |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | True (`v0.3.10`, includes `lib\0.106.1`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True |

### Historical June 2 K1 Runtime Smoke (HEAD `8f2d79b4`)

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot.log.after-launch` | HISTORICAL PASS | Historical Off-mode Steam smoke reached main menu in 40s. Loaded exactly 3 mods (BaseLib v3.1.4, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84). Applied 25/25 Spire Plus ModPatcher patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=disabled, live=Disabled` (default Off). FeatureRegistry diagnostics observed for all 6 features. All features default-on except Sts1Events. |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot-log-audit.json` | HISTORICAL PASS | Historical clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. The `[ERROR] ritsulib-variants.json` line is a known RitsuLib internal variant-manifest issue (ignored by audit). |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot.log.after-launch` | HISTORICAL PASS | Historical CanaryOnly direct launch (with `steam_appid.txt` + `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` env var) reached main menu in 22s. Loaded exactly 3 mods. Applied 25/25 patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=enabled, live=Enabled` (CanaryOnly mode). Registered exactly 4 canary events: `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`. No other events registered. This is not current beta.85 enabled-mode proof. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot-log-audit.json` | HISTORICAL PASS | Historical clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. |

### Historical June 2 CanaryOnly Fresh Smoke (HEAD `f20dd230`, with mod isolation)

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\live-spire-plus-session-20260602-174656\godot.log.after-launch` | HISTORICAL PASS | Historical CanaryOnly Steam launch with mod isolation (25 other mods moved). Reached main menu. Loaded exactly 3 mods (BaseLib, RitsuLib, Spire Plus). Applied 25/25 patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=enabled, live=Enabled` (CanaryOnly mode). Registered exactly 4 canary events: `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`. Additional mods still loaded from cached mod list (RouteSuggest, heybox, etc.); isolation moved files but game cached mod list before isolation. This is not current beta.85 enabled-mode proof. |

### Historical June 2 AdditiveBatch1 Runtime Evidence

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot.log.after-launch` | HISTORICAL PASS | Historical AdditiveBatch1 direct launch reached main menu in 42s. Loaded exactly 3 mods (BaseLib v3.1.4, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84). Applied 25/25 Spire Plus ModPatcher patches. Registered exactly 10 event types via 11 calls: Sts1BigFish (Shared), Sts1GoldenIdol (Shared), Sts1TheLab (Shared), Sts1DivineFountain (Shared), Sts1Purifier (Shared), Sts1UpgradeShrine->Glory (Act), Sts1GoldenShrine (Shared), Sts1TheCleric (Shared), Sts1OldBeggar (Shared), Sts1ShiningLight->Overgrowth (Act), Sts1ShiningLight->Underdocks (Act). This is not current beta.85 AdditiveBatch1 proof. |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot-log-audit.json` | HISTORICAL PASS | Historical clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. The single `[ERROR] ritsulib-variants.json` line is a RitsuLib internal variant-manifest issue (C# logger), not a Godot engine error. |

### June 2 Warning Triage

- Warning triage matrix written to `docs/reviews/warning-triage-matrix.md`.
- **79 warnings** remain (reduced from 89 by fixing all 4 canary event files: BigFish, GoldenIdol, TheLab, DivineFountain).
- All remaining warnings trace to single root cause: `EventModel.Owner` typed `Player?` from game base class.
- Recommended fix: early-exit guard `if (Owner is not { } owner) return;` at top of each handler method.
- CanaryOnly event files now have 0 nullable warnings.

### June 2 Diagnostics Architecture Audit

| Component | Required Posture | Actual Posture | Compliant? |
|---|---|---|---|
| RewardPipeline | Diagnostics-only | Diagnostics-only | YES |
| CardPlayContext | Allow-only | Allow-only | YES |
| DeathProtectionService | No-op / diagnostics-only | No-op (zero production callers) | YES |
| MultiplayerPolicy (registry) | Taxonomy / diagnostics-only | Taxonomy store | YES |
| MultiplayerFeaturePolicy (coop gates) | Behavioral safety gate | Active feature suppression in co-op | YES (intentional) |

### Historical June 2 Stop Decision (Superseded by June 11 beta.85 addendum)

- Status then: PARTIAL PASS / RELEASE STILL BLOCKED.
- No-game validation then: **HISTORICAL PASS** (build 0 errors / **79 warnings**, 464 passed / 0 failed / 21 skipped / 485 total, format clean, diff clean).
- Runtime dependency path then: **HISTORICAL PASS** (STS2-RitsuLib v0.3.10 installed, BaseLib v3.1.4 and EZMicroBalance present).
- Historical runtime loader gate then: **HISTORICAL PASS** (Off=0, CanaryOnly=4, AdditiveBatch1=10/11 with clean audits).
- Historical Sts1Events Off runtime proof then: **HISTORICAL PASS** (0 StS1 registrations, clean audit).
- Historical Sts1Events CanaryOnly runtime proof then: **HISTORICAL PASS** (exactly 4 canary events registered, clean audit, fresh at HEAD `f20dd230` with mod isolation).
- FeatureRegistry runtime diagnostics then: **HISTORICAL PASS** (all 6 features with bootstrap/live status in runtime log).
- RewardPipeline diagnostics then: **HISTORICAL PASS** (bootstrap events observed for all features in runtime log).
- AdditiveBatch1 runtime proof then: **HISTORICAL PASS** for the historical 10 event types / 11 registration calls source shape, clean audit. Current source now expects 10 event types / 14 registration calls and needs fresh `v0.107.0` enabled-mode proof.
- Worktree: **CLEAN** (0 dirty entries).
- Warning debt: **ACCEPTED** (79 warnings remaining, 10 fixed in canary events, single root cause, fix pattern documented).
- Independent QA: **PENDING** (needs rerun against current state).
- Gameplay proof: **PENDING** (game launched and reached main menu, but no interactive gameplay, save-load, or Mod Settings UI evidence captured).
- Event encounter screenshots: **PENDING** (require in-game event encounters).
- Save/load proof: **PENDING** (require save during/after event, reload, state stable).
- Versioned tester-package handoff: **PENDING**.
- Batch 4c status then: **READY FOR LOW-RISK CANDIDATE PROPOSAL** (historical runtime smoke passed; current beta.85 enabled-mode proof remains pending).
- Release-ready / live-ready: **NO**.

---

## Historical Revision J Snapshot (superseded by June 11 beta.85 Off-only addendum)

- HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`
- Branch: `main...origin/main`
- Worktree: dirty before this pass and still dirty. Existing source/docs/harness edits were preserved; no commit, push, stash, checkout, reset, restore, or broad clean was performed.
- Historical target-fix follow-up smoke evidence under `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\` and `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\` reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. Historical Off mode proved 0 StS1 registration lines; historical CanaryOnly proved exactly 4 canary content registrations for that source/runtime state. Current beta.85 keeps only default-Off `v0.107.0` proof clean; current CanaryOnly/AdditiveBatch1 enabled-mode proof, live gameplay, UI, save-load, co-op, independent QA rerun, clean worktree, versioned tester-package handoff, live-ready, and release-ready claims remain blocked.

## Revision J Required Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean .\EZMicroBalance.csproj` | PASS | 0 warnings, 0 errors. |
| `dotnet build .\EZMicroBalance.csproj` | PASS | 0 errors, 89 warnings. Warnings remain Sts1Events nullable staging debt (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj` | PASS | Test assembly builds against the current project. |
| `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `.\scripts\generate-patch-inventory.ps1 -Check` | PASS | Patch inventory is fresh. |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | PASS | Revision J classifier reports 49 dirty entries, 0 unclassified. |
| `dotnet publish EZMicroBalance.sln` | PASS | Published the target-fix build to the local installed mod folder for diagnostic runtime smoke; no new versioned tester package was created. |

## v15 Continuation Validation Rerun

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 Sts1Events nullable warnings. |
| `dotnet test EZMicroBalance.sln --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet test --filter Sts1EventFeatureGuardTests --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 31 passed, 0 failed, 0 skipped. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |

## Revision J Runtime Attempt

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\godot.log.after-launch` | FAIL / reaches menu with errors | BaseLib, RitsuLib, and Spire Plus loaded and reached main menu, but audit is not clean: 11 Godot ERROR hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. |
| `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\audit-godot-log.after-launch.json` | FAIL | Not clean; 11 Godot ERROR lines. No `MissingMethodException` or `TypeLoadException` hits. |
| `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\godot.log.after-launch` | HISTORICAL PASS | Historical Off-mode Steam smoke reached main menu, loaded exactly BaseLib/RitsuLib/Spire Plus, applied 25/25 Spire Plus patches, found 30 SavedSpireFields, and logged Sts1Events disabled/default Off. |
| `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\godot-log-audit.json` | HISTORICAL PASS | Historical clean audit with 0 release-blocking signature hits. |
| `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\godot.log.after-direct-launch` | HISTORICAL PASS | Historical CanaryOnly direct smoke reached main menu, loaded exactly 3 mods, applied 25/25 patches, found 30 SavedSpireFields, and registered `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, and `Sts1DivineFountain`. This is not current beta.85 enabled-mode proof. |
| `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\godot-log-audit.json` | HISTORICAL PASS | Historical clean audit with 0 release-blocking signature hits. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-direct-exe-steam-init-fail.log` | FAIL | Direct executable launch failed Steam initialization before mod loading. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch.log` | FAIL / invalid Spire Plus proof | RitsuLib `0.3.10` loaded with compat branch `0.106.1`; RitsuLib framework patches reported 0 failed; BaseLib `3.1.4` loaded. `EZMicroBalance` was skipped as disabled in settings, so Spire Plus initialization, 30 SavedSpireFields, and Spire Plus ModPatcher proof were not established. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch-audit.json` | FAIL | Audit was not clean: 3 Godot ERROR lines. No `MissingMethodException` or `TypeLoadException` hits were found. |
| Cleanup | PASS | Stopped `SlayTheSpire2`; restored `settings.save` for Steam user `76561199353211250` with matching before/after SHA256. |

## StS1 Unsafe-Gate Continuation Validation

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet test --filter Sts1EventFeatureGuardTests` | PASS | 31 passed, 0 failed, 0 skipped after adding unsafe-mode and replacement fail-closed guards. |
| `dotnet test --filter PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable` | PASS | 1 passed; active player-facing markdown naming guard passed. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS after clearing stale testhost locks | Final rerun passed with 0 errors and 89 Sts1Events nullable warnings. Earlier attempts failed only because stale `testhost` processes locked `EZMicroBalance.Tests.dll`. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS after retry | Latest default no-build rerun passed with 464 passed, 0 failed, 21 skipped, 485 total after stale testhost locks were absent. Earlier normal reruns intermittently aborted with the known testhost crash and no assertion failures; `RunConfiguration.MaxCpuCount=1` remains the documented fallback if needed. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors; PowerShell emitted a CRLF normalization warning for existing `docs/patch-inventory.md`. |

## Revision J Runtime Path Check

| Path | Exists |
| --- | --- |
| `E:\Steam\steamapps\common\Slay the Spire 2` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | True (`v0.3.10`, includes `lib\0.106.1`) |
| `D:\Steam\steamapps\common\Slay the Spire 2` | False |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | False |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | True, but current content is from runtime smoke attempts with non-clean audit; use copied evidence logs for review |

## Revision J Stop Decision

- Status: PARTIAL PASS / RELEASE STILL BLOCKED.
- Runtime dependency path blocker: cleared locally by installed STS2-RitsuLib `v0.3.10`.
- Historical Revision J loader gate then: Off and CanaryOnly diagnostic smokes passed for that source/runtime state. Current beta.85 `v0.107.0` proof remains default-Off only; CanaryOnly/AdditiveBatch1 enabled-mode proof is still pending.
- Commit readiness: not complete because the worktree is dirty and no commit/push was requested.
- Batch 4c: remains blocked until independent QA reruns against the new evidence and the owner accepts the dirty-worktree/package state.
- Release-ready: no.

## Governance Closure Validation Snapshot (Supersedes 24d4fe9a)

- HEAD: `87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1`
- Branch: `main...origin/main`
- Worktree: dirty before and after this pass; existing goal-doc edits were preserved. This continuation fixed the Sts1Events mode bootstrap override, refreshed runtime prerequisite evidence, and did not attempt to normalize unrelated dirty files.

## Prior M4 Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean EZMicroBalance.sln -m:1` | PASS | Exited `0`; clean completed with 0 warnings and 0 errors. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 warnings. Warnings are existing Sts1Events nullable warnings (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | Exited `0`; no formatting changes required. |
| `git diff --check` | PASS | Exited `0`; no whitespace errors. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## Validation Fixes Applied

- Replayed clean/build/test/format/diff-check on the current `87820303` HEAD after doc and test-guard reconciliation.
- Removed the trailing whitespace that previously blocked `git diff --check` in goal/status docs.
- Cleared stale `testhost` locks before final validation so clean/build/test could rebuild the current source and test assembly.
- Fixed the current `docs/issues.md` compactness guard regression and updated its guard to assert the active dirty-worktree truth instead of the obsolete beta.84 clean-worktree phrase.
- Kept the test assembly serialized by default to reduce prior default test-host instability; the latest default `dotnet test EZMicroBalance.sln --no-build` rerun passes, while earlier crash logs remain historical/intermittent evidence rather than assertion failures.
- Earlier full and `--no-build` test runs both passed with 462/0/21/483; the final current project no-build rerun passed with 464/0/21/485.
- Ran independent QA/Red-Team review. Verdict: FAIL / HARD BLOCKED because runtime proof is absent; completion stop is not allowed.
- Fixed QA-flagged stale wording in `docs/goals/refactor.md` and `docs/goals/event.md` without changing the runtime hard-block decision.
- Reconciled active RitsuLib/Sts1Events validation docs and doc guard tests to the 464/0/21/485 project no-build test count and current runtime dependency blocker.
- Fixed `Sts1EventsFeatureModule` so `SPIREPLUS_STS1_EVENT_MODE` is no longer treated as a generic FeatureRegistry disable override; CanaryOnly/AdditiveBatch1 can now reach `Sts1EventFeatureGate` in the source-level bootstrap path.
- Tightened StS1 unsafe modes so AdditiveAllDraft requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, and ReplaceUnknownEventsPrototype reports disabled/fail-closed in normal builds unless `REPLACEMENT_PROTOTYPE_ENABLED` and the unsafe override are present.
- Added independent refactor QA report at `docs/reviews/refactor-overnight-qa-20260531.md`; verdict is FAIL / HARD BLOCKED until runtime evidence exists.
- Added StS1 v14 hard-stop report at `docs/features/sts1-events/hard-stop-blocker-report-v14.md`; blocked runtime gates remain open.
- Preserved existing goal-doc edits that were outside this continuation.

## Warning Truth

- Current clean build warning count: 89.
- Warning codes: `CS8602`, `CS8604`, `CS8625`.
- Scope: all warnings are in `EZMicroBalanceCode/Sts1Events/Models/` staging code.
- Decision: warnings are issue-worthy and remain accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes.

## Historical Runtime Smoke

- Status then: LOADER/GATE PASS AT HEAD `8f2d79b4`, RELEASE BLOCKED. Current beta.85 truth is in the June 11 addendum at the top of this file.
- Historical K1 evidence (2026-06-02): Off-mode Steam smoke and CanaryOnly direct-launch smoke both reached main menu with clean audits, 25/25 Spire Plus patches, 30 SavedSpireFields, and BaseLib + RitsuLib + Spire Plus loaded. Off mode proved Sts1Events disabled (0 registrations). CanaryOnly proved exactly 4 canary event registrations (`Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`) for that source/runtime state.
- Historical runtime dependency path: `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` (`v0.3.10`), `BaseLib` (`v3.1.4`), `EZMicroBalance` (`v0.1.0-private-beta.84`) all present.
- Historical decision: loader/runtime gate proof was available for Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 at HEAD `8f2d79b4`. Current beta.85 keeps only default-Off `v0.107.0` proof clean; current CanaryOnly/AdditiveBatch1 enabled-mode, gameplay, save-load, rendering, replacement, multiplayer, independent QA, live-ready, and release-ready proof remain pending.

## Historical Independent QA

- Target-fix QA/Red-Team verdict: CONDITIONAL PASS for loader gates, not release-ready.
- QA-supported historical proof: Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 loader-gate evidence is supported by the June 2 smoke logs and clean audits for that source/runtime state. Current beta.85 enabled-mode proof remains pending.
- QA fixes applied after review: removed stale hard-block wording that still claimed no CanaryOnly proof or non-clean loader audit in active docs.
- Stop decision: release/live completion stop remains disallowed until event encounter screenshots, save/load proof, image rendering, replacement functional proof, multiplayer fail-closed, independent QA rerun, clean worktree or owner decision, and versioned tester-package handoff are complete.

## Architecture Status

- RewardPipeline diagnostics are wired into `FeatureRegistry` bootstrap events and the low-risk `AscensionRewardService` reward/card-reward surfaces as no-mutation diagnostics.
- `ArchitectureCanaryBootstrap` registers FeatureRegistry and Ascension reward diagnostic handlers, a no-op DeathProtection provider, and multiplayer policy records for preview tools, Ancients, Ascension, reward surfaces, and combat hooks.
- Lotha extra-play paths touch `CardPlayContextCanary` through a single-depth adapter that returns `Allow`; play counts and gameplay branches are unchanged.
- Existing co-op gates still make the same allow/disable decisions; their evidence payloads now include policy registration/category/env/verification metadata.
- Guard coverage was added for reward-surface diagnostics, multiplayer policy metadata, no-op DeathProtection registration, architecture wiring, multiplayer policy records, and source-manifest coverage.
