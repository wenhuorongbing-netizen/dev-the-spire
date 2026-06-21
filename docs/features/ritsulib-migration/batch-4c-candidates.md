# Batch 4c Low-Risk Candidate Proposal

Date: 2026-06-10
Static review recaptured: 2026-06-18
Dependency gate refreshed: 2026-06-21
Status: proposal only. Do not migrate these patches without explicit owner approval.

## Gate

The historical `v0.106.1` RitsuLib loader gate is good enough to propose low-risk candidates, but not to approve a migration or claim current `v0.107.1` runtime proof:

- STS2-RitsuLib `v0.4.32` is installed on the E-drive game root in direct NuGet runtime layout.
- Historical `v0.106.1` Off, CanaryOnly, and AdditiveBatch1 diagnostic smokes reached main menu with clean audits.
- Spire Plus applied 25/25 migrated ModPatcher patches in those historical loader smokes.
- The current local game install is `v0.107.1`; installed beta.98 package parity passes. Previous beta.96 Off proof exists under `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621/`, and previous beta.93 AdditiveBatch1 loader proof exists under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`.
- Previous beta.93 AdditiveBatch1 retained log and packet verifiers passed with 10 event types / 14 registration lines and exact tuple parity. This is loader/registration proof only and is not Batch 4c approval, gameplay proof, or handoff proof. Retained beta.85/beta.87 `v0.107.0` smokes remain previous-package/game-version context.
- Gameplay, event screenshots, save-load, replacement functional proof, co-op proof, independent QA, and versioned tester-package handoff remain pending.

This list is not a migration approval. It excludes run lifecycle, save/load, map generation, multiplayer/lobby, death handling, A20 boss-flow, and reward-state patches. If an owner later approves any candidate for a `v0.107.1` tester package, the approval must be paired with the package-version, dependency-metadata, publish/package, artifact-test, and clean Off-smoke work documented in `docs/migration.md`.

## Proposal Self-Check

Checked: 2026-06-18.
Dependency gate checked: 2026-06-21.

- Candidate count is 10, within the required 5-10 range.
- All candidates are currently classified as low risk in `docs/patch-inventory.md`.
- No candidate touches run lifecycle, save/load, map generation, multiplayer/lobby, death handling, A20 boss flow, or reward-state mutation.
- Source inspection confirms the candidates are scoped to localization fallback, Sere Talon UI icon refresh, stale-hand input crash suppression, and Crystal Sphere local preview cleanup.
- This recapture was static governance only: no source migration, package refresh, loader smoke, gameplay proof, or owner approval was performed.
- Owner decision remains pending; this self-check is not approval to migrate.

## Candidates

| # | File | Class | Target | Why low-risk | Targeted tests | Rollback |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationLocStringRawTextPatch` | `LocString.GetRawText` | A11-A20 localization fallback only; no gameplay state mutation. | `AscensionLocalizationBridgeCoversModdedOriginalAscensionTableKeys`; A20 selector localization manual row. | Move the class back to raw Harmony registration. |
| 2 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationGetTablePatch` | `LocManager.GetTable` | Merges missing ascension entries into the ascension table only. | Same localization guard plus `dotnet test --filter AscensionFeatureGuardTests`. | Move the class back to raw Harmony registration. |
| 3 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationRawTextPatch` | `LocTable.GetRawText` | Finalizer only recovers known ascension loc keys after `LocException`. | Same localization guard; verify no raw `ascension.LEVEL_20.*` text in selector. | Move the class back to raw Harmony registration. |
| 4 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationLocStringPatch` | `LocTable.GetLocString` | Finalizer only returns a `LocString` for known ascension bridge keys. | Same localization guard; manual selector screenshot remains pending. | Move the class back to raw Harmony registration. |
| 5 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationHasEntryPatch` | `LocTable.HasEntry` | Read-only table presence answer for known ascension bridge keys. | Same localization guard; build/test/fresh loader smoke. | Move the class back to raw Harmony registration. |
| 6 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationIsLocalKeyPatch` | `LocTable.IsLocalKey` | Read-only local-key answer for known ascension bridge keys. | Same localization guard; build/test/fresh loader smoke. | Move the class back to raw Harmony registration. |
| 7 | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | `SereTalonAncientEventOptionButtonPatch` | `NEventOptionButton._Ready` | UI icon reroute for Sere Talon option buttons only; no reward logic. | `VakuuSereTalonAndTanxClawsStayOnSeparateSourceRoutes`; clicked UI proof remains pending. | Move the class back to raw Harmony registration. |
| 8 | `EZMicroBalanceCode/Ancients/Patches/SereTalonVisualPatches.cs` | `SereTalonRelicNodeReloadPatch` | `NRelic.Reload` | UI icon reroute for Sere Talon relic nodes only; guarded against Tanx Claws. | Sere Talon/Tanx Claws route guards; relic-bar manual screenshot remains pending. | Move the class back to raw Harmony registration. |
| 9 | `EZMicroBalanceCode/Ascension/Patches/CombatHandInputSafetyPatches.cs` | `CombatHandInputSafetyPatch` | `NPlayerHand._UnhandledInput` | Finalizer only suppresses the observed stale-hand `ArgumentOutOfRangeException`; other exceptions pass through. | `CombatHandInputIgnoresOnlyTheObservedStaleIndexCrash`; combat manual proof remains pending. | Move the class back to raw Harmony registration. |
| 10 | `EZMicroBalanceCode/Preview/CrystalSpherePeekPatch.cs` | `CrystalSpherePeekFinishedPatch` | `NCrystalSphereScreen.OnMinigameFinished` | UI cleanup for the local Crystal Sphere peek button only; no reveal, reward, or cell-resolution API. | `PreviewToolsGuardTests`; Crystal Sphere live proof remains pending. | Move the class back to raw Harmony registration. |

## Per-Candidate Evidence

| # | Expected behavior unchanged | Source evidence |
| --- | --- | --- |
| 1 | `LocString.GetRawText` keeps Core behavior unless the request is for the `ascension` table and a known A11-A20 title/description key. | Prefix checks `LocTable == "ascension"`, `AscensionLocalizationBridge.IsAscensionLevelKey`, and `TryGetText`; it returns `true` for all other calls. |
| 2 | `LocManager.GetTable` still returns the Core table; Spire Plus only merges known ascension entries into that table after retrieval. | Postfix only runs for `name == "ascension"` and calls `AscensionLocalizationBridge.MergeIntoIfAscensionTable(__result)`. |
| 3 | `LocTable.GetRawText` behavior is unchanged for successful calls and for non-bridge exceptions. | Finalizer returns `null` when there is no exception and returns the original exception unless it is a `LocException` for a known bridge key. |
| 4 | `LocTable.GetLocString` behavior is unchanged for successful calls and for non-bridge exceptions. | Finalizer only constructs `new LocString("ascension", key)` after a `LocException` and successful `TryGetText` bridge lookup. |
| 5 | `LocTable.HasEntry` keeps existing `true` results and only upgrades missing known bridge keys to present. | Postfix checks `if (!__result && AscensionLocalizationBridge.TryGetText(...))`, so existing entries are untouched. |
| 6 | `LocTable.IsLocalKey` keeps existing `true` results and only upgrades missing known bridge keys to local. | Postfix checks `if (!__result && AscensionLocalizationBridge.TryGetText(...))`, so existing local-key results are untouched. |
| 7 | Event option button behavior is unchanged unless the option relic is exactly `SereTalon`. | `SereTalonVisualNodeRoutes.TryApplyEventOptionButton` immediately returns when `button.Option?.Relic is not SereTalon`. |
| 8 | Relic node reload behavior is unchanged unless the node model is exactly `SereTalon` and required icon nodes are present. | `TryApplyRelicNode` returns when the node is not ready, icon/outline nodes are missing, model lookup fails, or `model is not SereTalon`. |
| 9 | Combat input exceptions still propagate except for the observed stale hand index crash. | Finalizer returns `null` only for `ArgumentOutOfRangeException`; every other exception is returned unchanged. |
| 10 | Crystal Sphere finish behavior is unchanged except hiding/resetting the local peek UI state. | Postfix calls `CrystalSpherePeekPatch.HideForFinishedScreen`; the state helper hides the button and restores mask alpha without reveal, reward, or cell-resolution calls. |

## Required Acceptance Before Migration

Before any Batch 4c source migration:

1. Owner accepts this exact candidate list or a smaller subset.
2. `dotnet build EZMicroBalance.sln` passes with 0 errors.
3. Current accepted no-build test lanes pass with 0 failures. If the solution-level lane destabilizes around `ReleaseEvidenceGateTests`, use the documented split lanes instead of treating runner instability as a source failure.
4. `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passes.
5. `git diff --check` passes.
6. `scripts/generate-patch-inventory.ps1 -Check` passes after any migration.
7. A loader smoke is rerun if the migrated patch registration path changes.
8. Previous `v0.107.1` beta.93 AdditiveBatch1 loader/registration proof is clean, but this proposal is not a substitute for any new loader smoke required by changed patch registration paths or for gameplay proof.
9. Before any Batch 4c follow-up is cited as StS1 event runtime readiness, cite the retained current AdditiveBatch1 10 event types / 14 registration-line smoke with retained verifier reports and add the missing gameplay evidence, or state that the Batch 4c claim is unrelated to StS1Events enabled modes.

Release-ready remains blocked by gameplay, screenshot, save-load, image/render, replacement, multiplayer, independent QA, and tester-package handoff evidence.
