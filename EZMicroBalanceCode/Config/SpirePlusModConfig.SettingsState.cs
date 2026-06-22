namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private sealed class SettingsState
    {
        public bool EnableCrystalSpherePeek { get; set; } = true;

        public double CrystalSphereMaskAlpha { get; set; } = DefaultCrystalSphereMaskAlpha;

        public bool EnableTransformPrediction { get; set; } = true;

        public bool TransformPredictionAlwaysOn { get; set; } = true;

        public bool ShowPreviewDebugLogs { get; set; }
    }
}
