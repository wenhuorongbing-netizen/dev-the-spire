using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Patches StartRunLobby.BeginRunForAllPlayers to log lobby state before run starts.
/// Also logs preferred-ascension-save skip behavior if our patch fires.
/// </summary>
internal sealed class StartRunLobbyBeginRunForAllPlayersDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-lobby-begin-run-for-all";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log host lobby state around StartRunLobby.BeginRunForAllPlayers";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "BeginRunForAllPlayers", [typeof(string), typeof(List<ModifierModel>)])];

    [HarmonyPrefix]
    private static void Prefix(StartRunLobby __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogLobbyState(__instance, "BeginRunForAllPlayers prefix");
        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] BeginRunForAllPlayers: preferred save skip={AscensionSelectionPatches.ShouldSkipVanillaPreferredAscensionSave(__instance)}");
    }

    [HarmonyFinalizer]
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
internal sealed class StartRunLobbyBeginRunLocallyDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-lobby-begin-run-locally";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log local lobby state before StartRunLobby.BeginRunLocally creates the run";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "BeginRunLocally", [typeof(string), typeof(List<ModifierModel>)])];

    [HarmonyPrefix]
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
internal sealed class StartRunLobbyUpdateMaxMultiplayerAscensionDiagPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "multiplayer-diagnostics-lobby-update-max-ascension";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Log lobby state after StartRunLobby.UpdateMaxMultiplayerAscension";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")];

    [HarmonyPostfix]
    private static void Postfix(StartRunLobby __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        MultiplayerDiagnostics.LogLobbyState(__instance, "UpdateMaxMultiplayerAscension postfix");
    }
}
