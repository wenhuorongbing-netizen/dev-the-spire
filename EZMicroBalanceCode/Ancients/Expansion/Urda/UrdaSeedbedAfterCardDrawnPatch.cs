using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using System.Threading.Tasks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]
internal static class UrdaSeedbedAfterCardDrawnPatch
{
    private static bool Prefix(CardModel card, ref Task __result)
    {
        if (!UrdaBlessingService.WasPlantedBySeedbed(card))
        {
            return true;
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed skipped AfterCardDrawn hooks for planted card {card.Id.Entry}.");
        __result = Task.CompletedTask;
        return false;
    }
}
