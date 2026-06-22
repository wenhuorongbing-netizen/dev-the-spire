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
