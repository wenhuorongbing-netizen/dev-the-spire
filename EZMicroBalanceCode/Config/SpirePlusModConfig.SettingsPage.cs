using STS2RitsuLib;

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
}
