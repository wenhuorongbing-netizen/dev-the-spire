namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]
internal static class AncientPickupBalancePatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref Task __result)
    {
        switch (__instance)
        {
            case WarHammer warHammer:
                __result = PickupRewardService.UpgradeTwoCardsOnWarHammerPickup(warHammer);
                return false;
            case Sozu sozu:
                __result = PickupRewardService.FillPotionSlotsForSozu(sozu);
                return false;
            case Ectoplasm ectoplasm:
                __result = PickupRewardService.GainInitialGoldForEctoplasm(ectoplasm);
                return false;
            case SealOfGold sealOfGold:
                __result = PickupRewardService.AddDebtsForSealOfGold(sealOfGold);
                return false;
            case JeweledMask jeweledMask:
                __result = PickupRewardService.ChoosePermanentFreePower(jeweledMask);
                return false;
            default:
                return true;
        }
    }
}
