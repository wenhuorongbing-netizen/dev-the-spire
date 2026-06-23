# RitsuLib Migration

This is the compact entry point for RitsuLib migration and future
RitsuLib-first development. It intentionally does not repeat the full patch
inventory or dependency ledger; those details have one owner each.
This is the single entry point for RitsuLib migration and future RitsuLib-first
development.

## Current Boundary

- Spire Plus is RitsuLib-only for beta.135.
- Current source target: Slay the Spire 2 `v0.107.1`, `STS2.RitsuLib`
  `0.4.34`, and Spire Plus `v0.1.0-private-beta.135`.
- Compile dependency: NuGet `STS2.RitsuLib` `0.4.34`.
- Runtime dependency: manifest `STS2-RitsuLib >= 0.4.34`.
- Current package proof: beta.135 build, publish, package parity, runtime
  preflight, and source-workspace validation for the 169/0 source state.
- Current source inventory: 169 migrated `IPatchMethod` / `ModPatcher` patch
  classes and 0 raw Harmony declarations in `docs/patch-inventory.md`.
- Previous beta.128 evidence covers forced clicked Ancient UI smoke for Urda,
  Morvi, Lotha, and normal Vakuu with 152/152 default runtime patch
  registration from that older package. Recapture beta.135 runtime smoke before
  claiming current in-game patch coverage.
- Not proved: beta.135 enabled-mode registration, gameplay, save-load,
  replacement behavior, co-op, independent QA, release readiness, or tester
  handoff.

## Read Order

1. `PROJECT_STATE.md` for the current status and blockers.
2. `docs/goals/migration.md` for migration success criteria and validation
   commands.
3. `docs/integrations/ritsulib.md` for dependency/version/API evidence and the
   current RitsuLib API ownership plan.
4. `docs/patch-inventory.md` for generated patch counts and class ownership.
5. `docs/reviews/current-validation.md` for the latest validation record.
6. `runtime-smoke-checklist.md` only when preparing or reviewing runtime
   evidence.

Do not start future implementation from historical plans, archived prompt dumps, or old runtime reports.
Do not start future implementation from copied migration lists either.

## RitsuLib-First Rules

- Use unpacked local game source under `source code/src/Core/` as primary game
  API evidence before changing gameplay, save-load, reward, map, combat, or UI
  behavior.
- Use installed `STS2-RitsuLib.xml` and the public RitsuLib docs to confirm the
  RitsuLib API shape before adding wrappers.
- Run or review `scripts/check-ritsulib-latest-package.ps1` before claiming a
  RitsuLib package line is current. Do not infer package freshness from memory
  or an old validation note.
- Keep future developer guidance on the RitsuLib lane. The repository hygiene
  guard scans Git-tracked text files and rejects retired shared-runtime wording.
- Register mod content through `RitsuLibFramework.CreateContentPack(...)` and
  `SpirePlusContentRegistrationService`.
- Register settings data before the settings page: `BeginModDataRegistration`
  / `ModDataStore.Register` first, then `RegisterModSettings`.
- Use `SavedAttachedState<TKey, TValue>` for attached state that is known to
  flow through game saved properties; use `ModDataStore` for global mod
  settings.
- Migrate or add patches through explicit RitsuLib `IPatchMethod` classes and
  `ModPatcher` registration. Do not add broad discovery or raw Harmony
  attributes to the active source tree.

## Source Ownership Rules

- Patch bootstrap and ordered registration live in
  `EZMicroBalanceCode/Core/Integrations/RitsuLib`.
- `SpirePlusMigratedPatchRegistry.cs` keeps only the ordered entry point.
- `SpirePlusMigratedPatchRegistry.Ui.cs` owns event, clicked, map, settings,
  and selection UI registrations.
- `SpirePlusMigratedPatchRegistry.PreviewUi.cs` owns local-only preview-tool
  UI registrations.
- `SpirePlusMigratedPatchRegistry.DisplayUi.cs` owns display-only icon, hover,
  intent, and damage-number registrations.
- `SpirePlusMigratedPatchRegistry.Rewards.cs` owns card, relic, and reward-hook
  registrations.
- `SpirePlusMigratedPatchRegistry.Localization.cs` owns localization fallback
  and RitsuLib compatibility registrations.
- `SpirePlusMigratedPatchRegistry.Gameplay.cs` owns gameplay and diagnostic
  registrations whose runtime behavior still needs live proof.
- StS1 event registration stays mode-specific: the dispatcher belongs in
  `Sts1EventRegistrationService.cs`, and each mode's RitsuLib content-pack
  calls belong in its matching partial file under
  `EZMicroBalanceCode/Sts1Events/Runtime`.
- StS1 event id lists are reporting and validation metadata only; they do not
  register content unless the matching RitsuLib registration partial also
  contains the explicit content-pack calls.

## Settings Ownership Rules

- Keep settings entry ids stable; screenshots and future automation use them as
  evidence anchors. The current ids live in
  `SpirePlusModConfig.SettingsPage.Ids.cs`.
- Keep Crystal Sphere preview defaults and RitsuLib slider bounds in
  `SpirePlusModConfig.PreviewDefaults.cs`; preview normalization and UI
  construction should share those constants instead of duplicating numbers.
- Keep preview value normalization in
  `SpirePlusModConfig.PreviewNormalization.cs`.
- Keep RitsuLib settings localization bootstrap in
  `SpirePlusModConfig.SettingsLocalization.cs`; registration and page/entry
  files should not call `CreateModLocalization` directly.
- Keep preview-tool runtime reads behind `SpirePlusModConfig.PreviewSettings.cs`;
  preview code should not call RitsuLib stores or settings-page builders
  directly.
- Keep RitsuLib bootstrap runtime cache and fallback settings in
  `SpirePlusModConfig.SettingsRuntimeState.cs`.
- Keep RitsuLib store availability and lookup in
  `SpirePlusModConfig.SettingsStoreResolution.cs`; fallback-aware reads/writes
  belong in `SpirePlusModConfig.SettingsAccess.cs`.
- Keep RitsuLib settings text construction in
  `SpirePlusModConfig.SettingsText.cs`; page and entry files should call
  `Text(...)` / `LiteralText(...)` instead of constructing `ModSettingsText`
  directly.
- Keep RitsuLib settings page assembly separate from individual entry builders:
  the preview section orders entries, while
  `SpirePlusModConfig.SettingsPage.PreviewToolEntries.*.cs` files own
  feature-specific toggle/slider calls and their stable entry ids.
- Keep read-only migration status UI split the same way: the section file
  orders entries, while `MigrationStatusEntries` owns the paragraph/info-card
  calls and their stable entry ids.

## Support Files

| File | Role |
| --- | --- |
| `docs/integrations/ritsulib.md` | Dependency version, installed runtime, public-doc/API evidence, and detailed RitsuLib ownership plan. |
| `docs/patch-inventory.md` | Generated migrated/raw patch counts and patch class ownership. |
| `runtime-smoke-checklist.md` | Runtime evidence checklist and verifier command source. |

Historical migration-process files were moved out of the active feature folder:

| Archive file | Role |
| --- | --- |
| `docs/archive/feature-audits/ritsulib-migration/batch-4c-candidates-20260623.md` | Completed localization fallback migration record. |
| `docs/archive/feature-audits/ritsulib-migration/next-runtime-qa-run-20260623.md` | Superseded controlled runtime QA run plan. |
| `docs/archive/feature-audits/ritsulib-migration/runtime-hard-block-report-20260531.md` | Historical May 31 runtime hard-block boundary. |
| `docs/archive/feature-audits/ritsulib-migration/monthly-dev-spec-stub-20260623.md` | Superseded monthly spec compatibility stub. |

## Stop Lines

- Do not add any runtime dependency besides STS2-RitsuLib without owner
  approval and a same-pass package/docs/guard update.
- Do not treat loader proof or settings screenshots as gameplay, save-load,
  multiplayer, QA, release, or handoff proof.
- Do not migrate high-risk run/map/reward/save/multiplayer patches without
  explicit owner approval.
