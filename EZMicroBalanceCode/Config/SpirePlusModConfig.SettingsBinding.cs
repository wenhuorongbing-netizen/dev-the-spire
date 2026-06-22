using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static IModSettingsValueBinding<TValue> Binding<TValue>(
        string modId,
        Func<SettingsState, TValue> getter,
        Action<SettingsState, TValue> setter) =>
        new ModSettingsValueBinding<SettingsState, TValue>(modId, SettingsKey, SaveScope.Global, getter, setter);
}
