using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Logs the initial join handshake before vanilla collapses a ModelDb hash mismatch
/// into the same VersionMismatch UI used for a real release-version mismatch.
/// </summary>
internal sealed class JoinFlowHandleInitialGameInfoMessageDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-join-initial-game-info";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log the client join initial-game-info message before vanilla mismatch handling";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JoinFlow), "HandleInitialGameInfoMessage", [typeof(InitialGameInfoMessage), typeof(ulong)])];

    [HarmonyPrefix]
    private static void Prefix(InitialGameInfoMessage message)
    {
        MultiplayerDiagnostics.LogInitialGameInfo(message);
    }
}
