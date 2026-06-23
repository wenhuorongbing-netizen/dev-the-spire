using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SozuPotionGatePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sozu-initial-potion-gate";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Allow Spire Plus initial potion grants through Sozu while leaving normal blocking intact";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))];

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
    static string IPatchMethod.PatchId => "ectoplasm-initial-gold-gate";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Allow Spire Plus initial gold grants through Ectoplasm while leaving normal blocking intact";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))];

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
