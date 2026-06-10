namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]
internal static class SozuPotionGatePatch
{
    private static readonly HashSet<Player> InitialPotionFillOwners = [];

    public static void BeginInitialPotionFill(Player player)
    {
        InitialPotionFillOwners.Add(player);
    }

    public static void EndInitialPotionFill(Player player)
    {
        InitialPotionFillOwners.Remove(player);
    }

    [HarmonyPrefix]
    private static bool Prefix(Sozu __instance, Player player, ref bool __result)
    {
        if (InitialPotionFillOwners.Contains(player) && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))]
internal static class EctoplasmGoldGatePatch
{
    private static readonly HashSet<Player> InitialGoldOwners = [];

    public static void BeginInitialGold(Player player)
    {
        InitialGoldOwners.Add(player);
    }

    public static void EndInitialGold(Player player)
    {
        InitialGoldOwners.Remove(player);
    }

    [HarmonyPrefix]
    private static bool Prefix(Ectoplasm __instance, Player player, decimal amount, ref decimal __result)
    {
        if (InitialGoldOwners.Contains(player) && player == __instance.Owner)
        {
            __result = amount;
            return false;
        }

        return true;
    }
}
