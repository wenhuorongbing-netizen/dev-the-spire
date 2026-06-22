using System.Globalization;
using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void AddCrystalSpherePeekToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            EnableCrystalSpherePeekEntryId,
            Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.title", "Crystal Sphere peek"),
            Binding(modId, state => state.EnableCrystalSpherePeek, (state, value) => state.EnableCrystalSpherePeek = value),
            Text("SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.description", "Show the peek overlay for Crystal Sphere when supported."),
            () => true);

    private static void AddCrystalSphereMaskAlphaSlider(ModSettingsSectionBuilder section, string modId) =>
        section.AddSlider(
            CrystalSphereMaskAlphaEntryId,
            Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.title", "Crystal Sphere mask alpha"),
            Binding(modId, state => state.CrystalSphereMaskAlpha, (state, value) => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value)),
            CrystalSphereMaskAlphaMin,
            CrystalSphereMaskAlphaMax,
            CrystalSphereMaskAlphaStep,
            value => value.ToString("0.00", CultureInfo.InvariantCulture),
            Text("SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.description", "Opacity of the Crystal Sphere peek mask."));

    private static void AddTransformPredictionToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            EnableTransformPredictionEntryId,
            Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.title", "Transform prediction"),
            Binding(modId, state => state.EnableTransformPrediction, (state, value) => state.EnableTransformPrediction = value),
            Text("SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.description", "Show predicted transform outcomes when supported."),
            () => true);

    private static void AddTransformPredictionAlwaysOnToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            TransformPredictionAlwaysOnEntryId,
            Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.title", "Always show transform prediction"),
            Binding(modId, state => state.TransformPredictionAlwaysOn, (state, value) => state.TransformPredictionAlwaysOn = value),
            Text("SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.description", "Show transform prediction without requiring a modifier key."),
            () => EnableTransformPrediction);

    private static void AddPreviewDebugLogsToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            ShowPreviewDebugLogsEntryId,
            Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", "Preview debug logs"),
            Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
            Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.description", "Emit extra preview-tool diagnostics to the log."),
            () => true);
}
