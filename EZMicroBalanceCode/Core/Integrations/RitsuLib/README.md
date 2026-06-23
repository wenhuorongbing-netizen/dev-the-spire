# Core/Integrations/RitsuLib

RitsuLib bootstrap integration lives here.

Current source target: Slay the Spire 2 `v0.107.1`, `STS2.RitsuLib`
`0.4.34`, and Spire Plus `v0.1.0-private-beta.128`.

This directory owns the RitsuLib bootstrap, migrated patch registration,
content-pack registration, and SavedAttachedState field registration helper.
`SpirePlusMigratedPatchRegistry` owns the explicit migrated patch list so
`RitsuLibBootstrap` can stay focused on startup order and legacy Harmony
fallback boundaries. `SpirePlusMigratedPatchRegistry.Ui.cs` owns click, hover,
settings, and other UI-facing migrated patch registrations separately from
reward/content patch groups so the completed clicked-UI migration has a compact
source audit surface.
`RitsuLibBootstrap` applies the migrated registry with
`RitsuLibFramework.ApplyRequiredPatcher(...)`; if that required patcher fails,
startup stops before saved-state, content, settings, or feature initialization
can create a half-booted Spire Plus session.
`SpirePlusContentRegistrationService` creates and applies the RitsuLib content
pack; its sibling partial files own the Ancient/encounter, card, relic, power,
and enchantment registration lists so future content has a direct RitsuLib
home instead of drifting into scattered registration code.
StS1 event mode registration lives in `EZMicroBalanceCode/Sts1Events/Runtime`:
`Sts1EventRegistrationService.cs` owns mode dispatch, while
`Sts1EventRegistrationService.Canary.cs`,
`Sts1EventRegistrationService.AdditiveBatch1.cs`, and
`Sts1EventRegistrationService.AllDraft.cs` own the explicit RitsuLib content
pack calls for each mode. `Sts1EventRegistry.*` is metadata and validation
support only; changing that registry does not register content unless the
matching RitsuLib content-pack call is also added.
`Sts1EventFeatureGate.EventIds.cs` owns reporting/validation id lists only;
mode resolution remains in `Sts1EventFeatureGate.cs`.
Settings UI registration lives in `EZMicroBalanceCode/Config`: the entry file
keeps only registration order, `SpirePlusModConfig.Constants.cs` owns stable
persisted keys, localization roots, and package-facing status values,
`SpirePlusModConfig.SettingsPage.Ids.cs` owns stable page/entry ids,
`SpirePlusModConfig.PreviewDefaults.cs` owns preview defaults and slider bounds,
`SpirePlusModConfig.PreviewNormalization.cs` owns preview value normalization,
`SpirePlusModConfig.SettingsLocalization.cs` owns RitsuLib I18N creation,
`SpirePlusModConfig.PreviewSettings.cs` owns the public runtime accessors used
by preview code, `SpirePlusModConfig.SettingsStore.cs` owns RitsuLib data-store
registration, `SpirePlusModConfig.SettingsStoreResolution.cs` owns RitsuLib
store activation/lookup,
`SpirePlusModConfig.SettingsAccess.cs` owns fallback-aware store reads/writes,
`SpirePlusModConfig.SettingsBinding.cs` owns RitsuLib settings bindings,
`SpirePlusModConfig.SettingsRuntimeState.cs` owns the RitsuLib bootstrap
runtime cache and in-memory fallback, `SpirePlusModConfig.SettingsState.cs` owns
the persisted settings shape, `SpirePlusModConfig.SettingsPage.cs` owns page
registration, `SpirePlusModConfig.SettingsText.cs` owns RitsuLib settings text
construction and localization fallbacks,
`SpirePlusModConfig.SettingsPage.MigrationStatus.cs` owns the
read-only migration status section assembly,
`SpirePlusModConfig.SettingsPage.MigrationStatusEntries.cs` owns the
individual read-only RitsuLib paragraph/info-card entry builders,
`SpirePlusModConfig.SettingsPage.PreviewTools.cs` owns preview-tool section
assembly, while the `SpirePlusModConfig.SettingsPage.PreviewToolEntries.*.cs`
partials own Crystal Sphere, transform, and preview-diagnostic RitsuLib entry
builders separately.

Packaged beta.123 evidence covers build, publish, package parity, runtime
preflight, source-workspace validation, and forced clicked Ancient UI smoke for
Urda, Morvi, Lotha, and normal Vakuu. That proves forced clicked UI visibility only for the current package;
gameplay, save-load, gated Vakuu fight-option and victory return, co-op, release,
and handoff proof remain separate gates.

Previous beta.99 settings/off proof, beta.96 direct Off proof, and beta.93
AdditiveBatch1 proof are retained only as previous-package context. Use
`docs/integrations/ritsulib.md` for the current RitsuLib record and
`docs/features/ritsulib-migration/README.md` as the migration entry point.
