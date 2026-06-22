namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // Stable persisted/localization keys and package-facing status values stay
    // here. Page ids and UI bounds live in focused partials so future settings
    // work does not accidentally rename evidence anchors or change validation.
    private const string SettingsKey = "spire-plus-settings";
    private const string SettingsFileName = "spire-plus-settings.json";
    private const string SettingsLocalizationStem = "settings_ui";
    private const string SettingsLocalizationPckRoot = "res://EZMicroBalance/localization/settings_ui";

    private const string RequiredRuntimeDependency = "STS2-RitsuLib >= 0.4.33";
    private const string StableTechnicalId = "EZMicroBalance";
}
