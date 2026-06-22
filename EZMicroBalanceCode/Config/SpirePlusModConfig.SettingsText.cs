using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // RitsuLib renders these labels inside its settings page. Literal fallbacks
    // keep the page readable if localization is not ready during early startup.
    private static ModSettingsText Text(string key, string fallback) =>
        settingsLocalization is { } i18n
            ? ModSettingsText.I18N(i18n, key, fallback)
            : LiteralText(fallback);

    private static ModSettingsText LiteralText(string value) => ModSettingsText.Literal(value);
}
