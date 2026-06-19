using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

/// <summary>
/// Gated multiplayer diagnostics for A11-A20 run-start/Neow/save-quit investigation.
/// Default off: set SPIREPLUS_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1 to enable.
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
            $"[Spire Plus][MPDiag] Lobby state: phase={phase}; netType={netType}; gameMode={gameMode}; " +
            $"ascension={lobby.Ascension}; maxAscension={lobby.MaxAscension}; players={lobby.Players.Count}; " +
            $"localPlayerId={lobby.NetService.NetId}");

        for (var i = 0; i < lobby.Players.Count; i++)
        {
            var p = lobby.Players[i];
            MainFile.Logger.Info(
                $"[Spire Plus][MPDiag] LobbyPlayer[{i}]: id={p.id}; slot={p.slotId}; " +
                $"character={p.character?.Id?.Entry ?? "<null>"}; " +
                $"maxMultiplayerAscensionUnlocked={p.maxMultiplayerAscensionUnlocked}; isReady={p.isReady}");
        }
    }

    public static void LogRunStateHp(RunState runState, string phase)
    {
        if (!IsEnabled) return;

        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] RunState: phase={phase}; ascension={runState.AscensionLevel}; " +
            $"players={runState.Players.Count}; act={runState.CurrentActIndex}");

        foreach (var player in runState.Players)
        {
            var hp = player.Creature;
            MainFile.Logger.Info(
                $"[Spire Plus][MPDiag] Player[slot={runState.GetPlayerSlotIndex(player)}]: " +
                $"netId={player.NetId}; " +
                $"currentHp={hp.CurrentHp}; maxHp={hp.MaxHp}; isDead={hp.IsDead}; " +
                $"isActiveForHooks={player.IsActiveForHooks}");
        }
    }

    public static void LogSaveQuit(string phase, bool isHost, ulong localNetId)
    {
        if (!IsEnabled) return;

        MainFile.Logger.Info(
            $"[Spire Plus][MPDiag] SaveQuit: phase={phase}; isHost={isHost}; localNetId={localNetId}");
    }

    public static void LogInitialGameInfo(InitialGameInfoMessage message)
    {
        var localVersion = ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? GitHelper.ShortCommitId ?? "UNKNOWN";
        var localModelHash = ModelIdSerializationCache.Hash;
        var localMods = ModManager.GetGameplayRelevantModNameList() ?? [];
        var localOtherMods = ModManager.GetNonGameplayRelevantModNameList() ?? [];
        var hostMods = message.gameplayAffectingMods ?? [];
        var hostOtherMods = message.otherMods ?? [];
        var versionMatch = string.Equals(message.version, localVersion, StringComparison.Ordinal);
        var modelHashMatch = message.idDatabaseHash == localModelHash;
        var missingOnHost = localMods.Except(hostMods).ToList();
        var missingOnLocal = hostMods.Except(localMods).ToList();
        var missingOtherOnHost = localOtherMods.Except(hostOtherMods).ToList();
        var missingOtherOnLocal = hostOtherMods.Except(localOtherMods).ToList();
        var modListMatch = missingOnHost.Count == 0 && missingOnLocal.Count == 0;
        var otherModListMatch = missingOtherOnHost.Count == 0 && missingOtherOnLocal.Count == 0;

        if (!IsEnabled && versionMatch && modelHashMatch && modListMatch && otherModListMatch) return;

        var summary =
            $"[Spire Plus][MPDiag] JoinFlow initial game info: " +
            $"hostVersion={message.version}; localVersion={localVersion}; versionMatch={versionMatch}; " +
            $"hostModelHash={message.idDatabaseHash}; localModelHash={localModelHash}; modelHashMatch={modelHashMatch}; " +
            $"gameMode={message.gameMode}; sessionState={message.sessionState}; " +
            $"hostFailure={message.connectionFailureReason?.ToString() ?? "<none>"}; " +
            $"hostGameplayMods=[{FormatList(hostMods)}]; localGameplayMods=[{FormatList(localMods)}]; " +
            $"missingOnHost=[{FormatList(missingOnHost)}]; missingOnLocal=[{FormatList(missingOnLocal)}]; " +
            $"hostOtherMods=[{FormatList(hostOtherMods)}]; localOtherMods=[{FormatList(localOtherMods)}]; " +
            $"missingOtherOnHost=[{FormatList(missingOtherOnHost)}]; missingOtherOnLocal=[{FormatList(missingOtherOnLocal)}]";

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

        if (!otherModListMatch)
        {
            MainFile.Logger.Warn(summary + "; non-gameplay relevant mod mismatch is allowed by vanilla, but mod authors still need to verify safety.");
            return;
        }

        MainFile.Logger.Info(summary);
    }

    private static string FormatList(IEnumerable<string> values) =>
        string.Join(",", values.OrderBy(value => value, StringComparer.Ordinal));
}
