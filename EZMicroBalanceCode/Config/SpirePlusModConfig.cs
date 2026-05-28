using BaseLib.Config;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal sealed class SpirePlusModConfig : SimpleModConfig
{
    public static bool EnableDebugLogs { get; set; } = false;

    public static bool EnableCrystalSpherePeek { get; set; } = true;

    [ConfigSlider(0.05, 0.95, 0.05, Format = "{0:0.00}")]
    public static double CrystalSphereMaskAlpha { get; set; } = 0.32;

    public static bool EnableTransformPrediction { get; set; } = true;

    public static bool TransformPredictionAlwaysOn { get; set; } = true;

    public static bool ShowPreviewDebugLogs { get; set; } = false;
}
