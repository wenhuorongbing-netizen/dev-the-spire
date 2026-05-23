using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Common;

internal static class AncientSelectionEvidenceLog
{
    public static void LogBlessingSelected(
        Player player,
        string ancientId,
        string blessingId,
        string relicType,
        bool forced)
    {
        ReleaseEvidenceLog.Log(
            "AncientSelection",
            "blessing_selected",
            player,
            new Dictionary<string, object?>
            {
                ["ancient"] = ancientId,
                ["blessing"] = blessingId,
                ["relic"] = relicType,
                ["forced"] = forced
            });
        MainFile.Logger.Info(
            $"[EZMicroBalance] {ancientId} blessing selected: {blessingId}; " +
            $"playerSlot={PlayerSlot(player)}, run={RunId(player)}, forced={forced}, relic={relicType}.");
    }

    public static void LogBlessingSelectionFailed(
        Player player,
        string ancientId,
        string blessingId,
        string reason,
        bool forced)
    {
        ReleaseEvidenceLog.Log(
            "AncientSelection",
            "blessing_selection_failed",
            player,
            new Dictionary<string, object?>
            {
                ["ancient"] = ancientId,
                ["blessing"] = blessingId,
                ["reason"] = reason,
                ["forced"] = forced
            });
        MainFile.Logger.Warn(
            $"[EZMicroBalance] {ancientId} blessing selection failed before completion: {blessingId}; " +
            $"playerSlot={PlayerSlot(player)}, run={RunId(player)}, forced={forced}, reason={reason}.");
    }

    public static void LogOptionSelected(
        Player player,
        string ancientId,
        string optionId,
        string relicType,
        bool forced)
    {
        ReleaseEvidenceLog.Log(
            "AncientSelection",
            "option_selected",
            player,
            new Dictionary<string, object?>
            {
                ["ancient"] = ancientId,
                ["option"] = optionId,
                ["relic"] = relicType,
                ["forced"] = forced
            });
        MainFile.Logger.Info(
            $"[EZMicroBalance] {ancientId} option selected: {optionId}; " +
            $"playerSlot={PlayerSlot(player)}, run={RunId(player)}, forced={forced}, relic={relicType}.");
    }

    private static string RunId(Player player)
    {
        var runState = player.RunState;
        return $"{runState.Rng.Seed}:{runState.CurrentActIndex}:{runState.ActFloor}";
    }

    private static string PlayerSlot(Player player)
    {
        try
        {
            return player.RunState.GetPlayerSlotIndex(player).ToString();
        }
        catch
        {
            return "unknown";
        }
    }
}
