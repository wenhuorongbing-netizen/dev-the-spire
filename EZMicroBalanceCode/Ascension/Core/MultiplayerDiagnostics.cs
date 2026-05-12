using HarmonyLib;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Gated multiplayer diagnostics for A11-A20 run-start/Neow/save-quit investigation.
/// Default off: set EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1 to enable.
/// Does not change gameplay, state, or network behavior.
/// </summary>
internal static class MultiplayerDiagnostics
{
    public static bool IsEnabled => AscensionFeatureGate.IsMultiplayerDiagnosticsEnabled;

    public static void LogLobbyState(StartRunLobby lobby, string phase)
    {
        if (!IsEnabled) return;

        var netType = lobby.NetService.Type;
        var gameMode = lobby.GameMode;
        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] Lobby state: phase={phase}; netType={netType}; gameMode={gameMode}; " +
            $"ascension={lobby.Ascension}; maxAscension={lobby.MaxAscension}; players={lobby.Players.Count}; " +
            $"localPlayerId={lobby.NetService.NetId}");

        for (var i = 0; i < lobby.Players.Count; i++)
        {
            var p = lobby.Players[i];
            MainFile.Logger.Info(
                $"[EZMicroBalance][MPDiag] LobbyPlayer[{i}]: id={p.id}; slot={p.slotId}; " +
                $"character={p.character?.Id?.Entry ?? "<null>"}; " +
                $"maxMultiplayerAscensionUnlocked={p.maxMultiplayerAscensionUnlocked}; isReady={p.isReady}");
        }
    }

    public static void LogRunStateHp(RunState runState, string phase)
    {
        if (!IsEnabled) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] RunState: phase={phase}; ascension={runState.AscensionLevel}; " +
            $"players={runState.Players.Count}; act={runState.CurrentActIndex}");

        foreach (var player in runState.Players)
        {
            var hp = player.Creature;
            MainFile.Logger.Info(
                $"[EZMicroBalance][MPDiag] Player[slot={runState.GetPlayerSlotIndex(player)}]: " +
                $"netId={player.NetId}; " +
                $"currentHp={hp.CurrentHp}; maxHp={hp.MaxHp}; isDead={hp.IsDead}; " +
                $"isActiveForHooks={player.IsActiveForHooks}");
        }
    }

    public static void LogSaveQuit(string phase, bool isHost, ulong localNetId)
    {
        if (!IsEnabled) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] SaveQuit: phase={phase}; isHost={isHost}; localNetId={localNetId}");
    }

    public static void LogInitialGameInfo(InitialGameInfoMessage message)
    {
        var localVersion = ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? GitHelper.ShortCommitId ?? "UNKNOWN";
        var localModelHash = ModelIdSerializationCache.Hash;
        var localMods = ModManager.GetGameplayRelevantModNameList() ?? [];
        var hostMods = message.mods ?? [];
        var versionMatch = string.Equals(message.version, localVersion, StringComparison.Ordinal);
        var modelHashMatch = message.idDatabaseHash == localModelHash;
        var missingOnHost = localMods.Except(hostMods).ToList();
        var missingOnLocal = hostMods.Except(localMods).ToList();
        var modListMatch = missingOnHost.Count == 0 && missingOnLocal.Count == 0;

        if (!IsEnabled && versionMatch && modelHashMatch && modListMatch) return;

        var summary =
            $"[EZMicroBalance][MPDiag] JoinFlow initial game info: " +
            $"hostVersion={message.version}; localVersion={localVersion}; versionMatch={versionMatch}; " +
            $"hostModelHash={message.idDatabaseHash}; localModelHash={localModelHash}; modelHashMatch={modelHashMatch}; " +
            $"gameMode={message.gameMode}; sessionState={message.sessionState}; " +
            $"hostFailure={message.connectionFailureReason?.ToString() ?? "<none>"}; " +
            $"hostMods=[{FormatList(hostMods)}]; localMods=[{FormatList(localMods)}]; " +
            $"missingOnHost=[{FormatList(missingOnHost)}]; missingOnLocal=[{FormatList(missingOnLocal)}]";

        if (versionMatch && !modelHashMatch)
        {
            MainFile.Logger.Warn(summary + "; visible game versions match, but the ModelDb hash does not; vanilla will report this as VersionMismatch.");
            return;
        }

        if (!versionMatch || !modListMatch)
        {
            MainFile.Logger.Warn(summary);
            return;
        }

        MainFile.Logger.Info(summary);
    }

    private static string FormatList(IEnumerable<string> values) =>
        string.Join(",", values.OrderBy(value => value, StringComparer.Ordinal));
}

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
            $"[EZMicroBalance][MPDiag] BeginRunForAllPlayers: preferred save skip={AscensionSelectionPatches.ShouldSkipVanillaPreferredAscensionSave(__instance)}");
    }

    private static void Finalizer(StartRunLobby __instance, Exception? __exception)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        if (__exception != null)
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance][MPDiag] BeginRunForAllPlayers exception: {__exception.Message}");
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
            $"[EZMicroBalance][MPDiag] BeginRunLocally: ascension={__instance.Ascension}; " +
            $"singleplayer={__instance.NetService.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Singleplayer}");
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

/// <summary>
/// Patches NGame.StartNewMultiplayerRun to log player HP immediately after
/// RunState creation but before the run launches.
/// This catches the HP state before any mod hooks fire.
/// </summary>
[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.NGame), "StartNewMultiplayerRun")]
internal static class NGameStartNewMultiplayerRunDiagPatch
{
    private static void Postfix(MegaCrit.Sts2.Core.Nodes.NGame __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var runState = MegaCrit.Sts2.Core.Runs.RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(runState, "StartNewMultiplayerRun postfix");
        }
        else
        {
            MainFile.Logger.Info("[EZMicroBalance][MPDiag] StartNewMultiplayerRun postfix: RunState is null");
        }
    }
}

/// <summary>
/// Patches RunManager.EnterAct to log player HP before and after act entry.
/// </summary>
[HarmonyPatch(typeof(RunManager), "EnterAct")]
internal static class RunManagerEnterActDiagPatch
{
    private static void Prefix(RunManager __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var state = __instance.DebugOnlyGetState();
        if (state != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(state, "EnterAct prefix");
        }
    }

    private static void Postfix(RunManager __instance)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var state = __instance.DebugOnlyGetState();
        if (state != null)
        {
            MultiplayerDiagnostics.LogRunStateHp(state, "EnterAct postfix");
        }
    }
}

/// <summary>
/// Patches the Neow BeforeEventStarted to log player HP right before and after healing.
/// We patch AncientEventModel.BeforeEventStarted since Neow inherits it.
/// </summary>
[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.AncientEventModel), "BeforeEventStarted")]
internal static class AncientEventModelBeforeEventStartedDiagPatch
{
    private static void Prefix(MegaCrit.Sts2.Core.Models.AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] AncientEventModel.BeforeEventStarted prefix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }

    private static void Postfix(MegaCrit.Sts2.Core.Models.AncientEventModel __instance, bool isPreFinished)
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var player = __instance.Owner;
        if (player == null) return;

        MainFile.Logger.Info(
            $"[EZMicroBalance][MPDiag] AncientEventModel.BeforeEventStarted postfix: " +
            $"eventType={__instance.GetType().Name}; isPreFinished={isPreFinished}; " +
            $"playerNetId={player.NetId}; currentHp={player.Creature.CurrentHp}; maxHp={player.Creature.MaxHp}");
    }
}

/// <summary>
/// Patches the save/quit path to log when save/quit/diconnect is invoked.
/// NSaveAndQuitButton is the UI button; we hook SaveManager.SaveRun and
/// NGame.ReturnToMainMenu as additional save/quit surfaces.
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

    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.NGame), "ReturnToMainMenu")]
    [HarmonyPrefix]
    private static void ReturnToMainMenuPrefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var netService = MegaCrit.Sts2.Core.Runs.RunManager.Instance.NetService;
        var isHost = netService?.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Host;
        var netId = netService?.NetId ?? 0;
        MultiplayerDiagnostics.LogSaveQuit("ReturnToMainMenu prefix", isHost, netId);
    }

    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.NGame), "Quit")]
    [HarmonyPrefix]
    private static void NGameQuitPrefix()
    {
        if (!MultiplayerDiagnostics.IsEnabled) return;

        var netService = MegaCrit.Sts2.Core.Runs.RunManager.Instance.NetService;
        var isHost = netService?.Type == MegaCrit.Sts2.Core.Multiplayer.Game.NetGameType.Host;
        var netId = netService?.NetId ?? 0;
        MultiplayerDiagnostics.LogSaveQuit("NGame.Quit prefix", isHost, netId);
    }
}
