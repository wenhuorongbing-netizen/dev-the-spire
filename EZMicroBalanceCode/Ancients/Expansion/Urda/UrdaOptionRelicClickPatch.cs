using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(NRelicInventory), "OnRelicClicked")]
internal static class UrdaOptionRelicClickPatch
{
    [HarmonyPrefix]
    private static bool ExtractStoredSeedInsteadOfInspecting(RelicModel model)
    {
        if (model is UrdaRootSightOptionRelic rootSight && rootSight.Owner != null)
        {
            return !UrdaBlessingService.TryBeginRootSightSelection(rootSight.Owner);
        }

        if (model is not UrdaSeedBankOptionRelic seedBank ||
            seedBank.Owner == null ||
            seedBank.IsUsedUp ||
            UrdaBlessingService.GetSeedBankStoredCount(seedBank.Owner) == 0)
        {
            return true;
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
            seedBank.Owner.RunState,
            "UrdaSeedBank",
            "Seed Bank relic extraction opens unsynced shared reward selection"))
        {
            return true;
        }

        _ = TaskHelper.RunSafely(UrdaBlessingService.TryExtractSeedBankFromRelicClick(seedBank.Owner));
        return false;
    }
}
