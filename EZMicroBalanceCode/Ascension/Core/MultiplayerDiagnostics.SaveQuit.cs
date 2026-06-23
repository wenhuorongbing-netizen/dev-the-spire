using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Logs when save, quit, or main-menu return paths run during focused multiplayer diagnostics.
/// </summary>
internal sealed class SaveQuitSaveRunDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-save-run";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log SaveManager.SaveRun calls while multiplayer diagnostics are enabled";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(SaveManager), nameof(SaveManager.SaveRun), [typeof(AbstractRoom), typeof(bool)])];

    [HarmonyPrefix]
    private static void Prefix(AbstractRoom? preFinishedRoom, bool saveProgress)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogSaveQuit(
            $"SaveRun prefix; room={preFinishedRoom?.RoomType.ToString() ?? "<none>"}; saveProgress={saveProgress}",
            NetServiceSnapshot.IsHost,
            NetServiceSnapshot.NetId);
    }
}

internal sealed class SaveQuitReturnToMainMenuDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-return-to-main-menu";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log NGame.ReturnToMainMenu calls while multiplayer diagnostics are enabled";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NGame), nameof(NGame.ReturnToMainMenu))];

    [HarmonyPrefix]
    private static void Prefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogSaveQuit(
            "ReturnToMainMenu prefix",
            NetServiceSnapshot.IsHost,
            NetServiceSnapshot.NetId);
    }
}

internal sealed class SaveQuitNGameQuitDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-ngame-quit";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log NGame.Quit calls while multiplayer diagnostics are enabled";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NGame), nameof(NGame.Quit))];

    [HarmonyPrefix]
    private static void Prefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogSaveQuit(
            "NGame.Quit prefix",
            NetServiceSnapshot.IsHost,
            NetServiceSnapshot.NetId);
    }
}

internal static class NetServiceSnapshot
{
    public static bool IsHost
    {
        get
        {
            if (!MultiplayerDiagnostics.IsEnabled)
            {
                return false;
            }

            return RunManager.Instance.NetService?.Type == NetGameType.Host;
        }
    }

    public static ulong NetId
    {
        get
        {
            if (!MultiplayerDiagnostics.IsEnabled)
            {
                return 0;
            }

            return RunManager.Instance.NetService?.NetId ?? 0;
        }
    }
}
