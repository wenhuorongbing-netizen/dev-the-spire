using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void RegisterSettingsStore(string modId)
    {
        // RitsuLib setting controls bind to this data key, so the store must
        // exist before the page builder wires UI entries to persisted values.
        // Keep the mutable payload as a class so RitsuLib store reads, writes,
        // and UI bindings all point at the same settings object shape.
        using (RitsuLibFramework.BeginModDataRegistration(modId))
        {
            var store = RitsuLibFramework.GetDataStore(modId);
            store.Register(SettingsKey, SettingsFileName, SaveScope.Global, () => new SettingsState(), true);
        }
    }
}
