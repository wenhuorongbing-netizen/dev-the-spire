using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void AddMigrationStatusSection(ModSettingsPageBuilder page)
    {
        page.AddSection(MigrationStatusSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-MIGRATION_STATUS.title", "Migration Status"));
            section.WithDescription(Text(
                "SPIREPLUS-MIGRATION_STATUS.description",
                "Read-only status for the current RitsuLib-only settings surface."));
            AddRitsuLibSummaryParagraph(section);
            AddRequiredRuntimeDependencyCard(section);
            AddStableManifestIdCard(section);
            AddProofBoundaryCard(section);
        });
    }
}
