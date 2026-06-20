using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionSelectionPatches
{
    public const string MultiplayerA20DowngradeWarning =
        "Multiplayer A11-A20 gameplay is fail-closed by default after crash logs. " +
        $"Set {MultiplayerFeaturePolicy.AllowUnverifiedCoopGameplayEnvironmentVariable}=1 only for focused two-client debugging.";

    private static readonly FieldInfo? MaxAscensionBackingField =
        AccessTools.Field(typeof(StartRunLobby), "<MaxAscension>k__BackingField");
    private static bool _missingMaxAscensionFieldLogged;

    public static bool ShouldExpandSingleplayerSelection(StartRunLobby lobby)
    {
        return AscensionFeatureGate.IsPublicSelectionEnabled &&
            lobby.NetService.Type == NetGameType.Singleplayer &&
            lobby.GameMode != GameMode.Daily;
    }

    public static bool ShouldExpandMultiplayerSelection(StartRunLobby lobby)
    {
        return AscensionFeatureGate.IsPublicSelectionEnabled &&
            !AscensionFeatureGate.IsMultiplayerSelectionDisabled &&
            lobby.NetService.Type == NetGameType.Host &&
            lobby.GameMode != GameMode.Daily &&
            !MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                null,
                "AscensionMultiplayerSelection",
                "A11-A20 co-op selection is disabled by default because run-state, map, reward, and combat mutations do not yet have two-client proof.");
    }

    public static bool ShouldExpandSelection(StartRunLobby lobby)
    {
        return ShouldExpandSingleplayerSelection(lobby) ||
            ShouldExpandMultiplayerSelection(lobby);
    }

    public static void ExpandMaxAscension(StartRunLobby lobby)
    {
        if (!ShouldExpandSelection(lobby) ||
            lobby.MaxAscension >= AscensionFeatureGate.MaxSupportedAscensionLevel)
        {
            return;
        }

        if (MaxAscensionBackingField == null)
        {
            if (!_missingMaxAscensionFieldLogged)
            {
                _missingMaxAscensionFieldLogged = true;
                MainFile.Logger.Warn("[Spire Plus] Ascension selector expansion skipped: StartRunLobby MaxAscension backing field was not found.");
            }

            return;
        }

        MaxAscensionBackingField.SetValue(lobby, AscensionFeatureGate.MaxSupportedAscensionLevel);
        lobby.LobbyListener.MaxAscensionChanged();

        var lobbyKind = lobby.NetService.Type == NetGameType.Host
            ? "multiplayer host"
            : "singleplayer";
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension selector expanded to A{AscensionFeatureGate.MaxSupportedAscensionLevel} for local {lobbyKind} testing.");
    }

    public static bool ShouldSkipVanillaPreferredAscensionSave(StartRunLobby lobby)
    {
        return ShouldExpandSelection(lobby) &&
            lobby.Ascension > 10 &&
            lobby.Players.Count > 0;
    }

    public static void WarnIfA20MultiplayerDowngraded(StartRunLobby lobby, string surface)
    {
        if (!ShouldWarnA20MultiplayerDowngrade(lobby))
        {
            return;
        }

        MainFile.Logger.Warn(
            $"[Spire Plus] {MultiplayerA20DowngradeWarning} Surface: {surface}; selected A{lobby.Ascension}; players: {lobby.Players.Count}.");
    }

    private static bool ShouldWarnA20MultiplayerDowngrade(StartRunLobby lobby)
    {
        return ShouldExpandMultiplayerSelection(lobby) &&
            lobby.Ascension >= AscensionFeatureGate.DoubleRoyalBrandLevel;
    }

    public static MultiplayerUnlockOverride? TemporarilyExpandMultiplayerUnlocks(StartRunLobby lobby)
    {
        if (!ShouldExpandMultiplayerSelection(lobby) ||
            lobby.Players.Count == 0)
        {
            return null;
        }

        var snapshots = new List<MultiplayerUnlockSnapshot>();
        for (var i = 0; i < lobby.Players.Count; i++)
        {
            var player = lobby.Players[i];
            if (player.maxMultiplayerAscensionUnlocked >= AscensionFeatureGate.MaxSupportedAscensionLevel)
            {
                continue;
            }

            snapshots.Add(new MultiplayerUnlockSnapshot(player.id, player.maxMultiplayerAscensionUnlocked));
            player.maxMultiplayerAscensionUnlocked = AscensionFeatureGate.MaxSupportedAscensionLevel;
            lobby.Players[i] = player;
        }

        return snapshots.Count == 0
            ? null
            : new MultiplayerUnlockOverride(snapshots);
    }

    public static void RestoreMultiplayerUnlocks(StartRunLobby lobby, MultiplayerUnlockOverride? state)
    {
        if (state == null)
        {
            return;
        }

        foreach (var snapshot in state.Snapshots)
        {
            var index = lobby.Players.FindIndex(player => player.id == snapshot.PlayerId);
            if (index < 0)
            {
                continue;
            }

            var player = lobby.Players[index];
            player.maxMultiplayerAscensionUnlocked = snapshot.OriginalMaxAscension;
            lobby.Players[index] = player;
        }
    }

    internal sealed class MultiplayerUnlockOverride(IReadOnlyList<MultiplayerUnlockSnapshot> snapshots)
    {
        public IReadOnlyList<MultiplayerUnlockSnapshot> Snapshots { get; } = snapshots;
    }

    internal readonly record struct MultiplayerUnlockSnapshot(ulong PlayerId, int OriginalMaxAscension);
}

[HarmonyPatch(typeof(StartRunLobby), "SetSingleplayerAscensionAfterCharacterChanged")]
internal static class StartRunLobbySetSingleplayerAscensionPatch
{
    private static void Postfix(StartRunLobby __instance, ModelId characterId)
    {
        AscensionSelectionPatches.ExpandMaxAscension(__instance);
    }
}
