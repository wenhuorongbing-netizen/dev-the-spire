using STS2RitsuLib;
using STS2RitsuLib.Utils;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // RitsuLib owns the persisted settings store. The fallback is only used while
    // the framework is unavailable during early startup or when a store read fails.
    private static readonly SettingsState FallbackState = new();
    private static I18N? settingsLocalization;
    private static string? registeredModId;

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

    public static void Register(string modId)
    {
        registeredModId = modId;
        settingsLocalization = RitsuLibFramework.CreateModLocalization(
            modId,
            SettingsLocalizationStem,
            Array.Empty<string>(),
            Array.Empty<string>(),
            [SettingsLocalizationPckRoot],
            typeof(SpirePlusModConfig).Assembly);

        RegisterSettingsStore(modId);
        RegisterSettingsPage(modId);
    }
}
