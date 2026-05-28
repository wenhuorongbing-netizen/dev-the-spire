using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SozuPotionGatePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-o-z-u-p-o-t-i-o-n-g-a-t-e-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch Sozu.ShouldProcurePotion";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))];
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

internal sealed class EctoplasmGoldGatePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "e-c-t-o-p-l-a-s-m-g-o-l-d-g-a-t-e-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch Ectoplasm.ShouldGainGold";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Ectoplasm), nameof(Ectoplasm.ShouldGainGold))];
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
    private static bool Prefix(Ectoplasm __instance, Player player, ref bool __result)
    {
        if (InitialGoldOwners.Contains(player) && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}


