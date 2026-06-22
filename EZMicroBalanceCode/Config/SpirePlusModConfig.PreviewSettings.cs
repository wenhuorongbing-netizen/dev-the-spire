namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Preview runtime code reads these accessors instead of reaching into the
    // RitsuLib store or settings-page builders directly.
    public static bool EnableCrystalSpherePeek
    {
        get => State.EnableCrystalSpherePeek;
        set => UpdateState(state => state.EnableCrystalSpherePeek = value);
    }

    public static double CrystalSphereMaskAlpha
    {
        get => State.CrystalSphereMaskAlpha;
        set => UpdateState(state => state.CrystalSphereMaskAlpha = NormalizeCrystalSphereMaskAlpha(value));
    }

    public static bool EnableTransformPrediction
    {
        get => State.EnableTransformPrediction;
        set => UpdateState(state => state.EnableTransformPrediction = value);
    }

    public static bool TransformPredictionAlwaysOn
    {
        get => State.TransformPredictionAlwaysOn;
        set => UpdateState(state => state.TransformPredictionAlwaysOn = value);
    }

    public static bool ShowPreviewDebugLogs
    {
        get => State.ShowPreviewDebugLogs;
        set => UpdateState(state => state.ShowPreviewDebugLogs = value);
    }
}
