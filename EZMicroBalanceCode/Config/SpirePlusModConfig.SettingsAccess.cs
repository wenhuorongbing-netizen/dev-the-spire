using STS2RitsuLib;
using STS2RitsuLib.Data;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static SettingsState State
    {
        get
        {
            if (registeredModId is null || !RitsuLibFramework.IsActive)
            {
                return FallbackState;
            }

            try
            {
                return Store.Get<SettingsState>(SettingsKey);
            }
            catch
            {
                return FallbackState;
            }
        }
    }

    private static ModDataStore Store => RitsuLibFramework.GetDataStore(registeredModId ?? MainFile.ModId);

    private static void UpdateState(Action<SettingsState> update)
    {
        if (registeredModId is null || !RitsuLibFramework.IsActive)
        {
            update(FallbackState);
            return;
        }

        try
        {
            Store.Modify(SettingsKey, update);
        }
        catch
        {
            update(FallbackState);
        }
    }
}
