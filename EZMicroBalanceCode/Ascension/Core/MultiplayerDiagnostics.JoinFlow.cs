using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Logs the initial join handshake before vanilla collapses a ModelDb hash mismatch
/// into the same VersionMismatch UI used for a real release-version mismatch.
/// </summary>
[HarmonyPatch(typeof(JoinFlow), "HandleInitialGameInfoMessage")]
internal static class JoinFlowHandleInitialGameInfoMessageDiagPatch
{
    private static void Prefix(InitialGameInfoMessage message)
    {
        MultiplayerDiagnostics.LogInitialGameInfo(message);
    }
}
