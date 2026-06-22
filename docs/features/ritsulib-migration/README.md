# RitsuLib Migration

This is the single entry point for RitsuLib migration work. Start here, then
read only the support file that matches the task.

## Current Boundary

- Spire Plus is RitsuLib-only for beta.107.
- Compile dependency: NuGet `STS2.RitsuLib` `0.4.33`.
- Runtime dependency: manifest `STS2-RitsuLib >= 0.4.33`.
- Installed game target: Slay the Spire 2 `v0.107.1`.
- Current packaged proof: beta.107 package parity, runtime preflight,
  source-workspace validation, and smoke-level clicked Ancient UI for Urda,
  Morvi, Lotha, and normal Vakuu.
- Current source migration: Urda option-relic, Root Sight map-click, Root Sight
  map-visual, shared map-hover, Ascension map-icon/boss-hover, Sere Talon
  event-option/relic-node, Crystal Sphere peek, transform preview,
  Prismatic Gem reward-screen hint, A20 reward-screen wording, and Spire Plus
  mod-info localization UI patches plus combat hand stale-input safety are
  registered through RitsuLib
  `IPatchMethod` / `ModPatcher`, not broad Harmony discovery.
- Batch 4c localization fallback patches for A11-A20 ascension table text now
  also use RitsuLib `IPatchMethod` / `ModPatcher`.
- Batch 4c localization fallback patches have moved to RitsuLib; this is still
  source/registration work, not runtime gameplay proof.
- Current inventory: 52 migrated patch classes and 119 raw Harmony declarations
  remain in `docs/patch-inventory.md`.
- Boundary: beta.107 was rebuilt, published, packaged, and runtime-smoked after
  the UI/input migration, and that retained smoke applied the then-current 46
  Spire Plus ModPatcher patches. It predates the source-only Batch 4c
  localization migration, so a future package smoke must recapture 52 migrated
  patches before claiming runtime coverage for this source state.
- Previous-package proof: beta.99 RitsuLib settings UI visibility and direct Off
  loader startup/default-Off evidence.
- Not proved: beta.107 enabled-mode registration, gameplay, save-load,
  replacement behavior, co-op, independent QA, release readiness, or tester
  handoff.

## Read Order

1. `PROJECT_STATE.md` for the current status and blockers.
2. `docs/goals/migration.md` for migration success criteria and validation
   commands.
3. `docs/integrations/ritsulib.md` for dependency/version/API evidence.
4. `docs/reviews/current-validation.md` for the latest validation record.
5. `runtime-smoke-checklist.md` only when preparing or reviewing runtime
   evidence.
6. `batch-4c-candidates.md` only when reviewing the completed Batch 4c
   localization fallback migration and its remaining proof boundaries.

Do not start future implementation from historical plans, archived prompt dumps, or old runtime reports.

## RitsuLib-First Rules

- Use unpacked local game source under `source code/src/Core/` as primary game
  API evidence before changing gameplay, save/load, reward, map, combat, or UI
  behavior.
- Use installed `STS2-RitsuLib.xml` and the public RitsuLib docs to confirm the
  RitsuLib API shape before adding wrappers.
- Keep future developer guidance on the RitsuLib lane. The repository hygiene
  guard scans Git-tracked text files and rejects retired shared-runtime wording.
- Register mod content through `RitsuLibFramework.CreateContentPack(...)` and
  `SpirePlusContentRegistrationService`.
- Keep StS1 event registration mode-specific: the dispatcher belongs in
  `Sts1EventRegistrationService.cs`, and each mode's RitsuLib content-pack
  calls belong in its matching partial file under
  `EZMicroBalanceCode/Sts1Events/Runtime`.
- Treat StS1 event id lists as reporting and validation metadata only; they do
  not register content unless the matching RitsuLib registration partial also
  contains the explicit content-pack calls.
- Register settings data before the settings page: `BeginModDataRegistration`
  / `ModDataStore.Register` first, then `RegisterModSettings`.
- Keep settings entry ids stable; screenshots and future automation use them as
  evidence anchors. The current ids live in
  `SpirePlusModConfig.SettingsPage.Ids.cs`.
- Keep Crystal Sphere preview defaults and RitsuLib slider bounds in
  `SpirePlusModConfig.PreviewDefaults.cs`; preview normalization and UI
  construction should share those constants instead of duplicating numbers.
- Keep preview value normalization in
  `SpirePlusModConfig.PreviewNormalization.cs`; RitsuLib binding construction
  should stay focused on `ModSettingsValueBinding`.
- Keep RitsuLib settings localization bootstrap in
  `SpirePlusModConfig.SettingsLocalization.cs`; registration and page/entry
  files should not call `CreateModLocalization` directly.
- Keep preview-tool runtime reads behind `SpirePlusModConfig.PreviewSettings.cs`;
  preview code should not call RitsuLib stores or settings-page builders
  directly.
- Keep RitsuLib bootstrap runtime cache and fallback settings in
  `SpirePlusModConfig.SettingsRuntimeState.cs`; registration, store resolution,
  fallback-aware access, page assembly, and entry builders should stay in their
  own partial files.
- Keep RitsuLib store availability and lookup in
  `SpirePlusModConfig.SettingsStoreResolution.cs`; fallback-aware reads/writes
  belong in `SpirePlusModConfig.SettingsAccess.cs`.
- Keep settings persistence split by role: store registration, fallback-aware
  access, UI bindings, and persisted state shape each live in their matching
  `SpirePlusModConfig.Settings*.cs` partial.
- Keep RitsuLib settings text construction in `SpirePlusModConfig.SettingsText.cs`;
  page and entry files should call `Text(...)` / `LiteralText(...)` instead of
  constructing `ModSettingsText` directly.
- Keep RitsuLib settings page assembly separate from individual entry builders:
  the preview section orders entries, while
  `SpirePlusModConfig.SettingsPage.PreviewToolEntries.*.cs` files own
  feature-specific toggle/slider calls and their stable entry ids.
- Keep read-only migration status UI split the same way: the section file
  orders entries, while `MigrationStatusEntries` owns the paragraph/info-card
  calls and their stable entry ids.
- Use `SavedAttachedState<TKey, TValue>` for attached state that is known to
  flow through game saved properties; use `ModDataStore` for global mod
  settings.
- Migrate Harmony patches to RitsuLib `IPatchMethod` only with owner-approved
  scope, source evidence, tests, and fresh validation.

## Support Files

| File | Role |
| --- | --- |
| `monthly-dev-spec.md` | Compatibility stub retained for guarded historical references. Not a default entry point. |
| `runtime-smoke-checklist.md` | Runtime evidence checklist and verifier command source. |
| `next-overnight-run.md` | Future controlled runtime QA run order. |
| `batch-4c-candidates.md` | Completed Batch 4c localization fallback migration record and proof boundary. |
| `runtime-hard-block-report-20260531.md` | Current pointer for the old runtime hard-block lane; the original blocker is historical. |

## Stop Lines

- Do not add any runtime dependency besides STS2-RitsuLib without owner
  approval and a same-pass package/docs/guard update.
- Do not treat loader proof or settings screenshots as gameplay, save-load,
  multiplayer, QA, release, or handoff proof.
- Do not migrate high-risk run/map/reward/save/multiplayer patches without explicit owner approval.
