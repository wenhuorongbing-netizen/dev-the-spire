using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    private static void AddPreviewToolsSection(ModSettingsPageBuilder page, string modId)
    {
        page.AddSection(PreviewToolsSectionId, section =>
        {
            section.WithTitle(Text("SPIREPLUS-PREVIEW_TOOLS.title", "Preview Tools"));
            section.WithDescription(Text(
                "SPIREPLUS-PREVIEW_TOOLS.description",
                "Controls for Crystal Sphere peek and transform prediction."));
            AddCrystalSpherePeekToggle(section, modId);
            AddCrystalSphereMaskAlphaSlider(section, modId);
            AddTransformPredictionToggle(section, modId);
            AddTransformPredictionAlwaysOnToggle(section, modId);
            AddPreviewDebugLogsToggle(section, modId);
        });
    }
}
