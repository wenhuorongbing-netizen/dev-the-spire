using STS2RitsuLib.Settings;

namespace EZMicroBalance.EZMicroBalanceCode.Config;

internal static partial class SpirePlusModConfig
{
    // This is intentionally a narrow preview-tool diagnostic toggle, not the
    // broad internal debug logging gate used for focused development sessions.
    private static void AddPreviewDebugLogsToggle(ModSettingsSectionBuilder section, string modId) =>
        section.AddToggle(
            ShowPreviewDebugLogsEntryId,
            Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", "Preview debug logs"),
            Binding(modId, state => state.ShowPreviewDebugLogs, (state, value) => state.ShowPreviewDebugLogs = value),
            Text("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.description", "Emit extra preview-tool diagnostics to the log."),
            () => true);
}
