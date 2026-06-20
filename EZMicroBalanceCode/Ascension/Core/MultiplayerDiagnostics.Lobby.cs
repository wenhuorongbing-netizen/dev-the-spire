using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Patches StartRunLobby.BeginRunForAllPlayers to log lobby state before run starts.
/// Also logs preferred-ascension-save skip behavior if our patch fires.
/// </summary>
[HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]
internal static class StartRunLobbyBeginRunForAllPlayersDiagPatch
{
    private static void Prefix(StartRunLobby __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogLobbyState(__instance, "BeginRunForAllPlayers prefix");
        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] BeginRunForAllPlayers: preferred save skip={AscensionSelectionPatches.ShouldSkipVanillaPreferredAscensionSave(__instance)}");
    }

    private static void Finalizer(StartRunLobby __instance, Exception? __exception)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        if (__exception != null)
        {
            MainFile.Logger.Warn(
                $"[Spire Plus][MPDiag] BeginRunForAllPlayers exception: {__exception.Message}");
        }

        MultiplayerDiagnostics.LogLobbyState(__instance, "BeginRunForAllPlayers finalizer");
    }
}

/// <summary>
/// Patches StartRunLobby.BeginRunLocally to log ascension/player HP state before
/// the local run starts. This is the point where RunState is about to be created.
/// </summary>
[HarmonyPatch(typeof(StartRunLobby), "BeginRunLocally")]
internal static class StartRunLobbyBeginRunLocallyDiagPatch
{
    private static void Prefix(StartRunLobby __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogLobbyState(__instance, "BeginRunLocally prefix");
        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] BeginRunLocally: ascension={__instance.Ascension}; " +
            $"singleplayer={__instance.NetService.Type == NetGameType.Singleplayer}");
    }
}

/// <summary>
/// Patches StartRunLobby.UpdateMaxMultiplayerAscension to log the multiplayer
/// ascension cap computation.
/// </summary>
[HarmonyPatch(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")]
internal static class StartRunLobbyUpdateMaxMultiplayerAscensionDiagPatch
{
    private static void Postfix(StartRunLobby __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogLobbyState(__instance, "UpdateMaxMultiplayerAscension postfix");
    }
}
