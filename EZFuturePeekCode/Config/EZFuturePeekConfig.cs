using BaseLib.Config;

namespace EZFuturePeek.EZFuturePeekCode.Config;

internal sealed class EZFuturePeekConfig : SimpleModConfig
{
    public static bool EnableCrystalSpherePeek { get; set; } = true;

    [ConfigSlider(0.05, 0.95, 0.05, Format = "{0:0.00}")]
    public static double CrystalSphereMaskAlpha { get; set; } = 0.32;

    public static bool EnableTransformPrediction { get; set; } = true;

    public static bool TransformPredictionAlwaysOn { get; set; } = true;

    public static bool ShowDebugLogs { get; set; } = false;
}
