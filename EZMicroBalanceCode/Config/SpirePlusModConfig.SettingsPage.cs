using STS2RitsuLib;
using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void RegisterSettingsPage(string modId)
    {
        RitsuLibFramework.RegisterModSettings(modId, page =>
        {
            page.WithModDisplayName(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithTitle(Text("SPIREPLUS.mod_title", "Spire Plus"));
            page.WithDescription(Text(
                "SPIREPLUS.settings_page.description",
                "RitsuLib settings page for Spire Plus private beta testing."));

            AddMigrationStatusSection(page);
            AddPreviewToolsSection(page, modId);
        });
    }

    private static ModSettingsText Text(string key, string fallback) =>
        settingsLocalization is { } i18n
            ? ModSettingsText.I18N(i18n, key, fallback)
            : LiteralText(fallback);

    private static ModSettingsText LiteralText(string value) => ModSettingsText.Literal(value);
}
