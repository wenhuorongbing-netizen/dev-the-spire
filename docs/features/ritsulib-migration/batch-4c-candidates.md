# Batch 4c Low-Risk Localization Migration

Date: 2026-06-22
Static review recaptured: 2026-06-18
Dependency gate refreshed: 2026-06-22
Status: migrated localization fallback batch.

Owner decision recorded: 2026-06-22 continuation goal approved migrating the remaining six localization fallback candidates.

This migration is source/registration work only; it is not gameplay, save-load, co-op, release, or handoff proof.

## Gate

- STS2-RitsuLib `v0.4.34` is installed on the E-drive game root in direct NuGet runtime layout.
- The latest dependency recheck still reports `STS2.RitsuLib` `0.4.34` as the stable NuGet target.
- installed beta.118 package parity passed; previous beta.108 clicked Ancient UI smoke applied the then-current 64 migrated patch classes.
- This source pass moved the six localization fallback patches to RitsuLib `IPatchMethod` / `ModPatcher`; later visual-hover UI, rest-site UI, Act Ancient unlock-list UI, and Vakuu event-state UI passes raised the current source count to 119 migrated patches. The beta.108 clicked UI smoke covers only the earlier 64-patch state for forced Ancient UI.
- Gameplay, event screenshots, save-load, replacement functional proof, co-op proof, independent QA, and versioned tester-package handoff remain pending.

## Migration Self-Check

Checked: 2026-06-18.
Dependency gate checked: 2026-06-22.

- Migrated candidate count is 6 after the earlier UI/input subset was migrated through RitsuLib.
- All six candidates were classified as low risk in `docs/patch-inventory.md` before migration.
- No candidate touches run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, or reward-state mutation.
- Source inspection confirms the migrated candidates are scoped to ascension localization fallback.
- The 2026-06-18 recapture was static governance only; the 2026-06-22 continuation records owner approval for exactly the six localization fallback candidates.
- Current accepted no-build test lanes pass with 0 failures. If the solution-level lane destabilizes around `ReleaseEvidenceGateTests`, use the documented split lanes instead of treating runner instability as a source failure.

## Migrated Candidates

| # | File | Class | Target | Why low-risk | Targeted tests | Rollback |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationLocStringRawTextPatch` | `LocString.GetRawText` | A11-A20 localization fallback only; no gameplay state mutation. | `AscensionLocalizationBridgeCoversModdedOriginalAscensionTableKeys`; A20 selector localization manual row. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |
| 2 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationGetTablePatch` | `LocManager.GetTable` | Merges missing ascension entries into the ascension table only. | Same localization guard plus `dotnet test --filter AscensionFeatureGuardTests`. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |
| 3 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationRawTextPatch` | `LocTable.GetRawText` | Finalizer only recovers known ascension loc keys after `LocException`. | Same localization guard; verify no raw `ascension.LEVEL_20.*` text in selector. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |
| 4 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationLocStringPatch` | `LocTable.GetLocString` | Finalizer only returns a `LocString` for known ascension bridge keys. | Same localization guard; manual selector screenshot remains pending. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |
| 5 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationHasEntryPatch` | `LocTable.HasEntry` | Read-only table presence answer for known ascension bridge keys. | Same localization guard; build/test/fresh loader smoke. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |
| 6 | `EZMicroBalanceCode/Ascension/Patches/AscensionLocalizationTablePatches.cs` | `AscensionLocalizationIsLocalKeyPatch` | `LocTable.IsLocalKey` | Read-only local-key answer for known ascension bridge keys. | Same localization guard; build/test/fresh loader smoke. | Remove the RitsuLib registration and restore the class-level raw Harmony patch. |

## Per-Candidate Evidence

| # | Expected behavior unchanged | Source evidence |
| --- | --- | --- |
| 1 | `LocString.GetRawText` keeps Core behavior unless the request is for the `ascension` table and a known A11-A20 title/description key. | Prefix checks `LocTable == "ascension"`, `AscensionLocalizationBridge.IsAscensionLevelKey`, and `TryGetText`; it returns `true` for all other calls. |
| 2 | `LocManager.GetTable` still returns the Core table; Spire Plus only merges known ascension entries into that table after retrieval. | Postfix only runs for `name == "ascension"` and calls `AscensionLocalizationBridge.MergeIntoIfAscensionTable(__result)`. |
| 3 | `LocTable.GetRawText` behavior is unchanged for successful calls and for non-bridge exceptions. | Finalizer returns `null` when there is no exception and returns the original exception unless it is a `LocException` for a known bridge key. |
| 4 | `LocTable.GetLocString` behavior is unchanged for successful calls and for non-bridge exceptions. | Finalizer only constructs `new LocString("ascension", key)` after a `LocException` and successful `TryGetText` bridge lookup. |
| 5 | `LocTable.HasEntry` keeps existing `true` results and only upgrades missing known bridge keys to present. | Postfix checks `if (!__result && AscensionLocalizationBridge.TryGetText(...))`, so existing entries are untouched. |
| 6 | `LocTable.IsLocalKey` keeps existing `true` results and only upgrades missing known bridge keys to local. | Postfix checks `if (!__result && AscensionLocalizationBridge.TryGetText(...))`, so existing local-key results are untouched. |

## Stop Lines

- High-risk migration remains out of scope without a new owner decision: run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, and reward-state patches stay on their current guarded path.
- Before citing this source migration as current runtime coverage, rebuild/package and capture a fresh loader packet that proves the current 119 migrated Spire Plus patch classes apply in the installed game.
- Before any Batch 4c follow-up is cited as StS1 event runtime readiness, cite current enabled-mode evidence with retained verifier reports and add the missing gameplay evidence, or state that the Batch 4c claim is unrelated to StS1Events enabled modes.
- Release-ready remains blocked by gameplay, screenshot, save-load, image/render, replacement, multiplayer, independent QA, and tester-package handoff evidence.
