namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // All preview-facing reads funnel through this property. If RitsuLib has
    // not finished bootstrap, or if its store throws during an early-access API
    // edge case, previews keep using the in-memory fallback instead of reaching
    // into page builders or raw persistence APIs.
    private static SettingsState State
    {
        get
        {
            if (!CanUseRitsuLibStore)
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

    private static void UpdateState(Action<SettingsState> update)
    {
        if (!CanUseRitsuLibStore)
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
