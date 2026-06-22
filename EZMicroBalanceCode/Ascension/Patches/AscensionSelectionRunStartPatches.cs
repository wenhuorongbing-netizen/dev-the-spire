using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class StartRunLobbyBeginRunLocallyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-selection-begin-run-locally";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Temporarily allow A11-A20 single-player run launch without saving vanilla progress";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "BeginRunLocally", [typeof(string), typeof(List<ModifierModel>)])];

    [HarmonyPrefix]
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

    [HarmonyFinalizer]
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

internal sealed class StartRunLobbyUpdateMaxMultiplayerAscensionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-selection-update-max-multiplayer";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Temporarily widen host multiplayer selector caps for deliberate A11-A20 debug runs";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "UpdateMaxMultiplayerAscension")];

    [HarmonyPrefix]
    private static void Prefix(
        StartRunLobby __instance,
        ref AscensionSelectionPatches.MultiplayerUnlockOverride? __state)
    {
        __state = AscensionSelectionPatches.TemporarilyExpandMultiplayerUnlocks(__instance);
    }

    [HarmonyFinalizer]
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

internal sealed class StartRunLobbyUpdatePreferredAscensionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-selection-update-preferred";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Block A11-A20 test selections from being persisted as vanilla preferred Ascension";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "UpdatePreferredAscension")];

    [HarmonyPrefix]
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

internal sealed class StartRunLobbySyncAscensionChangeA20WarningPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-selection-sync-warning";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Warn when host multiplayer selects A20 while co-op gameplay proof remains pending";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), nameof(StartRunLobby.SyncAscensionChange), [typeof(int)])];

    [HarmonyPostfix]
    private static void Postfix(StartRunLobby __instance)
    {
        AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(__instance, "host multiplayer ascension selection");
    }
}

internal sealed class StartRunLobbyBeginRunForAllPlayersA20WarningPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-selection-begin-run-for-all-warning";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Warn when host multiplayer starts at A20 while co-op gameplay proof remains pending";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(StartRunLobby), "BeginRunForAllPlayers", [typeof(string), typeof(List<ModifierModel>)])];

    [HarmonyPrefix]
    private static void Prefix(StartRunLobby __instance)
    {
        AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(__instance, "host multiplayer run start");
    }
}
