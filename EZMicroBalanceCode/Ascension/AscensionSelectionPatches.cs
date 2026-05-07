using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionSelectionPatches
{
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
            lobby.GameMode != GameMode.Daily;
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
                MainFile.Logger.Warn("[EZMicroBalance] Ascension selector expansion skipped: StartRunLobby MaxAscension backing field was not found.");
            }

            return;
        }

        MaxAscensionBackingField.SetValue(lobby, AscensionFeatureGate.MaxSupportedAscensionLevel);
        lobby.LobbyListener.MaxAscensionChanged();

        var lobbyKind = lobby.NetService.Type == NetGameType.Host
            ? "multiplayer host"
            : "singleplayer";
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension selector expanded to A{AscensionFeatureGate.MaxSupportedAscensionLevel} for local {lobbyKind} testing.");
    }

    public static bool ShouldSkipVanillaPreferredAscensionSave(StartRunLobby lobby)
    {
        return ShouldExpandSelection(lobby) &&
            lobby.Ascension > 10 &&
            lobby.Players.Count > 0;
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

[HarmonyPatch(typeof(StartRunLobby), "BeginRunLocally")]
internal static class StartRunLobbyBeginRunLocallyPatch
{
    private static void Prefix(StartRunLobby __instance, ref ProgressMaxAscensionOverride? __state)
    {
        __state = null;

        if (!AscensionSelectionPatches.ShouldExpandSingleplayerSelection(__instance) ||
            __instance.Ascension <= 10 ||
            __instance.Players.Count == 0)
        {
            return;
        }

        var stats = SaveManager.Instance.Progress.GetOrCreateCharacterStats(__instance.Players[0].character.Id);
        if (stats.MaxAscension >= __instance.Ascension)
        {
            AscensionSelectionPatches.ExpandMaxAscension(__instance);
            return;
        }

        __state = new ProgressMaxAscensionOverride(stats, stats.MaxAscension);
        stats.MaxAscension = Math.Min(
            AscensionFeatureGate.MaxSupportedAscensionLevel,
            Math.Max(__instance.Ascension, stats.MaxAscension));

        AscensionSelectionPatches.ExpandMaxAscension(__instance);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Temporarily raised local MaxAscension to {stats.MaxAscension} so A{__instance.Ascension} can start.");
    }

    private static Exception? Finalizer(ProgressMaxAscensionOverride? __state, Exception? __exception)
    {
        RestoreProgressMaxAscension(__state);
        return __exception;
    }

    private static void RestoreProgressMaxAscension(ProgressMaxAscensionOverride? __state)
    {
        if (__state == null)
        {
            return;
        }

        __state.Stats.MaxAscension = __state.OriginalMaxAscension;
    }

    private sealed class ProgressMaxAscensionOverride(CharacterStats stats, int originalMaxAscension)
    {
        public CharacterStats Stats { get; } = stats;

        public int OriginalMaxAscension { get; } = originalMaxAscension;
    }
}

[HarmonyPatch(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")]
internal static class StartRunLobbyUpdateMaxMultiplayerAscensionPatch
{
    private static void Prefix(
        StartRunLobby __instance,
        ref AscensionSelectionPatches.MultiplayerUnlockOverride? __state)
    {
        __state = AscensionSelectionPatches.TemporarilyExpandMultiplayerUnlocks(__instance);
    }

    private static Exception? Finalizer(
        StartRunLobby __instance,
        AscensionSelectionPatches.MultiplayerUnlockOverride? __state,
        Exception? __exception)
    {
        AscensionSelectionPatches.RestoreMultiplayerUnlocks(__instance, __state);
        AscensionSelectionPatches.ExpandMaxAscension(__instance);
        return __exception;
    }
}

[HarmonyPatch(typeof(StartRunLobby), "UpdatePreferredAscension")]
internal static class StartRunLobbyUpdatePreferredAscensionPatch
{
    private static bool Prefix(StartRunLobby __instance)
    {
        if (!AscensionSelectionPatches.ShouldSkipVanillaPreferredAscensionSave(__instance))
        {
            return true;
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Keeping A{__instance.Ascension} as a launch-only test selection; not writing it to vanilla progress.");
        return false;
    }
}
