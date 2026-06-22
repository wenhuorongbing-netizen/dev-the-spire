using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Transform entries only control prediction visibility. The preview
    // services keep ownership of RNG snapshots and displayed predicted cards.
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
}
