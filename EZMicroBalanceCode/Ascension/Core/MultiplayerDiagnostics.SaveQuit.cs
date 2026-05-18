using MegaCrit.Sts2.Core.Nodes;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Patches save/quit paths to log when save, quit, or disconnect is invoked.
/// </summary>
[HarmonyPatch]
internal static class SaveQuitDiagPatches
{
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveRun), typeof(AbstractRoom), typeof(bool))]
    [HarmonyPrefix]
    private static void SaveRunPrefix(AbstractRoom? preFinishedRoom, bool saveProgress)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var netService = RunManager.Instance.NetService;
        var isHost = netService?.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Host;
        var netId = netService?.NetId ?? 0;
        MultiplayerDiagnostics.LogSaveQuit(
            $"SaveRun prefix; room={preFinishedRoom?.RoomType.ToString() ?? "<none>"}; saveProgress={saveProgress}",
            isHost,
            netId);
    }

    [HarmonyPatch(typeof(NGame), "ReturnToMainMenu")]
    [HarmonyPrefix]
    private static void ReturnToMainMenuPrefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var netService = RunManager.Instance.NetService;
        var isHost = netService?.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Host;
        var netId = netService?.NetId ?? 0;
        MultiplayerDiagnostics.LogSaveQuit("ReturnToMainMenu prefix", isHost, netId);
    }

    [HarmonyPatch(typeof(NGame), "Quit")]
    [HarmonyPrefix]
    private static void NGameQuitPrefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var netService = RunManager.Instance.NetService;
        var isHost = netService?.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Host;
        var netId = netService?.NetId ?? 0;
        MultiplayerDiagnostics.LogSaveQuit("NGame.Quit prefix", isHost, netId);
    }
}
