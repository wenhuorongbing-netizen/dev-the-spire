using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Nodes.Relics;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaOptionRelicClickPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-option-relic-click";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Urda option relic clicks to Root Sight or Seed Bank actions before vanilla relic inspection";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NRelicInventory), "OnRelicClicked", [typeof(RelicModel)])];

    [HarmonyPrefix]
    private static bool Prefix(RelicModel model)
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
