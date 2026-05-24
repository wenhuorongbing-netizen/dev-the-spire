using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Saves;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

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
            $"[Spire Plus] Temporarily raised local MaxAscension to {stats.MaxAscension} so A{__instance.Ascension} can start.");
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
            $"[Spire Plus] Keeping A{__instance.Ascension} as a launch-only test selection; not writing it to vanilla progress.");
        return false;
    }
}

[HarmonyPatch(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange))]
internal static class StartRunLobbySyncAscensionChangeA20WarningPatch
{
    private static void Postfix(StartRunLobby __instance)
    {
        AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(__instance, "host multiplayer ascension selection");
    }
}

[HarmonyPatch(typeof(StartRunLobby), "BeginRunForAllPlayers")]
internal static class StartRunLobbyBeginRunForAllPlayersA20WarningPatch
{
    private static void Prefix(StartRunLobby __instance)
    {
        AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(__instance, "host multiplayer run start");
    }
}
