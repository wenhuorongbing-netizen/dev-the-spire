using MegaCrit.Sts2.Core.Hooks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]
internal static class UrdaSeedbedAfterCardDrawnPatch
{
    private static bool Prefix(CardModel card)
    {
        if (!UrdaBlessingService.WasPlantedBySeedbed(card))
        {
            return true;
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed skipped AfterCardDrawn hooks for planted card {card.Id.Entry}.");
        return false;
    }
}
