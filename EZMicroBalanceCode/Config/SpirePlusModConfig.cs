using STS2RitsuLib;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
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
