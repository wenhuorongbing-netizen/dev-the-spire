namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private const string SettingsKey = "spire-plus-settings";
    private const string SettingsFileName = "spire-plus-settings.json";
    private const string SettingsLocalizationStem = "settings_ui";
    private const string SettingsLocalizationPckRoot = "res://EZMicroBalance/localization/settings_ui";

    private const string MigrationStatusSectionId = "migration_status";
    private const string PreviewToolsSectionId = "preview_tools";
    private const string RitsuLibSummaryEntryId = "ritsulib_only_summary";
    private const string RequiredRuntimeDependencyEntryId = "required_runtime_dependency";
    private const string StableManifestIdEntryId = "stable_manifest_id";
    private const string ProofBoundaryEntryId = "proof_boundary";

    // These RitsuLib setting-entry ids are persisted bindings and evidence anchors.
    // Rename them only with an explicit migration and package-version pass.
    private const string EnableCrystalSpherePeekEntryId = "enable_crystal_sphere_peek";
    private const string CrystalSphereMaskAlphaEntryId = "crystal_sphere_mask_alpha";
    private const string EnableTransformPredictionEntryId = "enable_transform_prediction";
    private const string TransformPredictionAlwaysOnEntryId = "transform_prediction_always_on";
    private const string ShowPreviewDebugLogsEntryId = "show_preview_debug_logs";

    private const string RequiredRuntimeDependency = "STS2-RitsuLib >= 0.4.33";
    private const string StableTechnicalId = "EZMicroBalance";

    private const double DefaultCrystalSphereMaskAlpha = 0.32;
    private const double CrystalSphereMaskAlphaMin = 0.05;
    private const double CrystalSphereMaskAlphaMax = 0.95;
    private const double CrystalSphereMaskAlphaStep = 0.05;
}
