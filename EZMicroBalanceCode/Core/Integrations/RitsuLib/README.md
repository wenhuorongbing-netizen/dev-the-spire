# Core/Integrations/RitsuLib

RitsuLib bootstrap integration lives here.

Current source target: Slay the Spire 2 `v0.107.1`, `STS2.RitsuLib`
`0.4.33`, and Spire Plus `v0.1.0-private-beta.105`.

This directory owns the RitsuLib bootstrap, migrated patch registration,
content-pack registration, and SavedAttachedState field registration helper.
`SpirePlusMigratedPatchRegistry` owns the explicit migrated patch list so
`RitsuLibBootstrap` can stay focused on startup order and legacy Harmony
fallback boundaries.
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
keeps only localization/bootstrap order, `SpirePlusModConfig.Constants.cs` owns
stable page/entry ids and slider bounds,
`SpirePlusModConfig.PreviewSettings.cs` owns the public runtime accessors used
by preview code, `SpirePlusModConfig.SettingsStore.cs` owns RitsuLib data-store
registration, `SpirePlusModConfig.SettingsAccess.cs` owns fallback-aware store
reads/writes, `SpirePlusModConfig.SettingsBinding.cs` owns RitsuLib settings
bindings and value normalization, `SpirePlusModConfig.SettingsState.cs` owns
the persisted settings shape, `SpirePlusModConfig.SettingsPage.cs` owns page
registration, `SpirePlusModConfig.SettingsPage.MigrationStatus.cs` owns the
read-only migration status section,
`SpirePlusModConfig.SettingsPage.PreviewTools.cs` owns preview-tool section
assembly, and `SpirePlusModConfig.SettingsPage.PreviewToolEntries.cs` owns the
individual RitsuLib toggle/slider entry builders.

Current beta.105 evidence covers package parity, runtime preflight,
source-workspace validation, and smoke-level clicked Ancient UI for Urda,
Morvi, Lotha, and normal Vakuu. That proves forced clicked UI visibility only;
gameplay, save-load, gated Vakuu fight-option/victory return, co-op, release,
and handoff proof remain separate gates.

Previous beta.99 settings/off proof, beta.96 direct Off proof, and beta.93
AdditiveBatch1 proof are retained only as previous-package context. Use
`docs/integrations/ritsulib.md` for the current RitsuLib record and
`docs/features/ritsulib-migration/README.md` as the migration entry point.
