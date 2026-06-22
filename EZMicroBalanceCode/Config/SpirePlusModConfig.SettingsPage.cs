using System.Globalization;
using STS2RitsuLib;
using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void RegisterSettingsPage(string modId)
    {
        RitsuLibFramework.RegisterModSettings(modId, page =>
        {
            page.WithModDisplayName(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithTitle(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithDescription(Text(
                "SPIREPLUS.settings_page.description",
                "RitsuLib settings page for Spire Plus private beta testing."));

            AddMigrationStatusSection(page);
            AddPreviewToolsSection(page, modId);
        });
    }

    private static void AddMigrationStatusSection(ModSettingsPageBuilder page)
    {
        // Keep these entry IDs stable. The manual screenshot checklist and
        // future automation use them to prove the RitsuLib-only settings page.
        page.AddSection(MigrationStatusSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-MIGRATION_STATUS.title", "Migration Status"));
            section.WithDescription(Text(
                "SPIREPLUS-MIGRATION_STATUS.description",
                "Read-only status for the current RitsuLib-only settings surface."));
            section.AddParagraph(
                RitsuLibSummaryEntryId,
                Text("SPIREPLUS-RITSULIB_ONLY_SUMMARY.title", "RitsuLib-only mod surface"),
                Text(
                    "SPIREPLUS-RITSULIB_ONLY_SUMMARY.description",
                    "This page is registered through RitsuLib. Spire Plus uses RitsuLib for settings persistence, content registration, patch metadata, and saved marker state."));
            section.AddInfoCard(
                RequiredRuntimeDependencyEntryId,
                Text("SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.title", "Runtime dependency"),
                LiteralText(RequiredRuntimeDependency),
                Text(
                    "SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.description",
                    "Install the runtime pack under the game mods/STS2-RitsuLib folder before enabling Spire Plus."));
            section.AddInfoCard(
                StableManifestIdEntryId,
                Text("SPIREPLUS-STABLE_MANIFEST_ID.title", "Technical id"),
                LiteralText(StableTechnicalId),
                Text(
                    "SPIREPLUS-STABLE_MANIFEST_ID.description",
                    "This id remains only for the manifest, install folder, saves, and compatibility. Player-facing UI should say Spire Plus."));
            section.AddInfoCard(
                ProofBoundaryEntryId,
                Text("SPIREPLUS-PROOF_BOUNDARY.title", "Evidence boundary"),
                Text("SPIREPLUS-PROOF_BOUNDARY.subtitle", "Settings screenshots prove UI visibility only."),
                Text(
                    "SPIREPLUS-PROOF_BOUNDARY.description",
                    "Gameplay, clicked Ancient screens, save/load, co-op, and release readiness still need separate evidence."));
        });
    }

    private static void AddPreviewToolsSection(ModSettingsPageBuilder page, string modId)
    {
        page.AddSection(PreviewToolsSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-PREVIEW_TOOLS.title", "Preview Tools"));
            section.WithDescription(Text(
                "SPIREPLUS-PREVIEW_TOOLS.description",
                "Controls for Crystal Sphere peek and transform prediction."));
            section.AddToggle(
                EnableCrystalSpherePeekEntryId,
                Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.title", "Crystal Sphere peek"),
                Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
                Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.description", "Show the peek overlay for Crystal Sphere when supported."),
                () => true);
            section.AddSlider(
                CrystalSphereMaskAlphaEntryId,
                Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.title", "Crystal Sphere mask alpha"),
                Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value)),
                CrystalSphereMaskAlphaMin,
                CrystalSphereMaskAlphaMax,
                CrystalSphereMaskAlphaStep,
                value => value.ToString("0.00", CultureInfo.InvariantCulture),
                Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.description", "Opacity of the Crystal Sphere peek mask."));
            section.AddToggle(
                EnableTransformPredictionEntryId,
                Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.title", "Transform prediction"),
                Binding(modId, state => state.EnableTransformPrediction, (state, value) => state.EnableTransformPrediction = value),
                Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.description", "Show predicted transform outcomes when supported."),
                () => true);
            section.AddToggle(
                TransformPredictionAlwaysOnEntryId,
                Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.title", "Always show transform prediction"),
                Binding(modId, state => state.TransformPredictionAlwaysOn, (state, value) => state.TransformPredictionAlwaysOn = value),
                Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.description", "Show transform prediction without requiring a modifier key."),
                () => EnableTransformPrediction);
            section.AddToggle(
                ShowPreviewDebugLogsEntryId,
                Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", "Preview debug logs"),
                Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
                Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.description", "Emit extra preview-tool diagnostics to the log."),
                () => true);
        });
    }

    private static ModSettingsText Text(string key, string fallback) =>
        settingsLocalization is { } i18n
            ? ModSettingsText.I18N(i18n, key, fallback)
            : LiteralText(fallback);

    private static ModSettingsText LiteralText(string value) => ModSettingsText.Literal(value);
}
