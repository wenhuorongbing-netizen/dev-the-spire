using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class AncientPickupBalancePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ancient-pickup-balance";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Handle Ancient relic pickup effects (WarHammer, Sozu, Ectoplasm, SealOfGold, JeweledMask)";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.AfterObtained))];
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
