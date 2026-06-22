namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    public static void Register(string modId)
    {
        registeredModId = modId;
        settingsLocalization = CreateSettingsLocalization(modId);
        RegisterSettingsStore(modId);
        RegisterSettingsPage(modId);
    }
}
