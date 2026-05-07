namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))]
internal static class SealOfGoldMaxEnergyPatch
{
    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, Player player, ref decimal __result)
    {
        if (__instance is SealOfGold sealOfGold && player == sealOfGold.Owner)
        {
            __result += sealOfGold.DynamicVars.Energy.BaseValue;
        }
    }
}

[HarmonyPatch(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))]
internal static class SealOfGoldTurnPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

