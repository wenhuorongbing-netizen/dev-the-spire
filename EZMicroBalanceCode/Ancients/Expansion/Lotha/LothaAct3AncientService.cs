using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static class LothaAct3AncientService
{
    public static void AddLothaToAct3(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!LothaFeatureGate.IsLothaEnabled(unlockState))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(LothaFeatureGate.ForcedAncient) &&
            !LothaFeatureGate.ShouldForceLotha)
        {
            return;
        }

        var lotha = ModelDb.AncientEvent<EzmbLotha>();
        if (LothaFeatureGate.ShouldForceLotha)
        {
            unlockedAncients = [lotha];
            MainFile.Logger.Info("[Spire Plus] Force Ancient gate selected Lotha as the Act 3 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == lotha.Id))
        {
            list.Add(lotha);
            MainFile.Logger.Info("[Spire Plus] Lotha added to Act 3 unlocked ancients for private-beta testing.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]
internal static class LothaGloryPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        LothaAct3AncientService.AddLothaToAct3(unlockState, ref __result);
}
