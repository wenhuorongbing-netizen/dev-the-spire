using System.Collections.Generic;
using System.Linq;

using BaseLib.Utils;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaAct1AncientService
{
    public static void AddUrdaToAct1(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!UrdaFeatureGate.IsUrdaEnabled(unlockState))
        {
            return;
        }

        if (RunManager.Instance.DebugOnlyGetState() is { } runState &&
            MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                runState,
                "UrdaAncientOffer",
                "Urda Ancient reward selection mutates per-player reward state and is disabled in co-op until two-client proof exists."))
        {
            return;
        }

        var urda = ModelDb.AncientEvent<EzmbUrda>();
        if (UrdaFeatureGate.ShouldForceUrda)
        {
            unlockedAncients = [urda];
            MainFile.Logger.Info("[Spire Plus] SPIREPLUS_FORCE_ANCIENT forced Urda as the Act 1 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == urda.Id))
        {
            list.Add(urda);
            MainFile.Logger.Info("[Spire Plus] Urda added to Act 1 unlocked ancients.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Overgrowth), nameof(Overgrowth.GetUnlockedAncients))]
internal static class UrdaOvergrowthPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        UrdaAct1AncientService.AddUrdaToAct1(unlockState, ref __result);
}

[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]
internal static class UrdaUnderdocksPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        UrdaAct1AncientService.AddUrdaToAct1(unlockState, ref __result);
}
