namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // RitsuLib section and entry ids are persisted UI bindings and evidence
    // anchors for screenshots/automation. Rename only with migration + version.
    private const string MigrationStatusSectionId = "migration_status";
    private const string PreviewToolsSectionId = "preview_tools";
    private const string RitsuLibSummaryEntryId = "ritsulib_only_summary";
    private const string RequiredRuntimeDependencyEntryId = "required_runtime_dependency";
    private const string StableManifestIdEntryId = "stable_manifest_id";
    private const string ProofBoundaryEntryId = "proof_boundary";
    private const string EnableCrystalSpherePeekEntryId = "enable_crystal_sphere_peek";
    private const string CrystalSphereMaskAlphaEntryId = "crystal_sphere_mask_alpha";
    private const string EnableTransformPredictionEntryId = "enable_transform_prediction";
    private const string TransformPredictionAlwaysOnEntryId = "transform_prediction_always_on";
    private const string ShowPreviewDebugLogsEntryId = "show_preview_debug_logs";
}
