using STS2RitsuLib.Utils;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // RitsuLib owns the persisted store. This in-memory fallback only keeps
    // preview settings readable before RitsuLib is active or after a store read
    // fails, so preview code never reaches into page builders or raw stores.
    private static readonly SettingsState FallbackState = new();

    // Registration caches the active mod id and I18N table once. Later partials
    // use them to bind values and render labels without carrying bootstrap code.
    private static I18N? settingsLocalization;
    private static string? registeredModId;
}
