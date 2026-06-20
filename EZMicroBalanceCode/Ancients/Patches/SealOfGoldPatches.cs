using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SealOfGoldMaxEnergyPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "seal-of-gold-max-energy";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add SealOfGold energy bonus to max energy";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))];
    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, Player player, ref decimal __result)
    {
        if (__instance is SealOfGold sealOfGold && player == sealOfGold.Owner)
        {
            __result += sealOfGold.DynamicVars.Energy.BaseValue;
        }
    }
}

internal sealed class SealOfGoldTurnPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "seal-of-gold-turn";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Suppress vanilla SealOfGold after-side-turn-start behavior";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))];
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

