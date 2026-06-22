using STS2RitsuLib;
using STS2RitsuLib.Utils;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // RitsuLib settings labels use a dedicated I18N root. Keeping the PCK path
    // and assembly anchor here prevents page/entry builders from carrying
    // localization bootstrap details.
    private static I18N CreateSettingsLocalization(string modId) =>
        RitsuLibFramework.CreateModLocalization(
            modId,
            SettingsLocalizationStem,
            Array.Empty<string>(),
            Array.Empty<string>(),
            [SettingsLocalizationPckRoot],
            typeof(SpirePlusModConfig).Assembly);
}
