namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    public static void Register(string modId)
    {
        registeredModId = modId;
        settingsLocalization = CreateSettingsLocalization(modId);

        // RitsuLib settings entries bind to persisted values while the page is
        // registered, so the backing store must exist before page construction.
        RegisterSettingsStore(modId);
        RegisterSettingsPage(modId);
    }
}
