using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PrismaticGemPoolPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "prismatic-gem-pool-noop";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Disable vanilla Prismatic Gem pool broadening so Spire Plus can replace screen rewards deterministically";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(PrismaticGem),
            nameof(PrismaticGem.ModifyCardRewardCreationOptions),
            [typeof(Player), typeof(CardCreationOptions)])
    ];

    [HarmonyPrefix]
    private static bool Prefix(CardCreationOptions options, ref CardCreationOptions __result)
    {
        __result = options;
        return false;
    }
}

internal sealed class PrismaticGemRewardScreenContextPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "prismatic-gem-reward-screen-context";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Track the active CardReward screen while Prismatic Gem reward options are being populated";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardReward), nameof(CardReward.Populate))];

    [ThreadStatic]
    private static Stack<CardReward>? PopulateStack;

    internal static CardReward? CurrentReward =>
        PopulateStack is { Count: > 0 } ? PopulateStack.Peek() : null;

    [HarmonyPrefix]
    private static void Prefix(CardReward __instance)
    {
        (PopulateStack ??= new Stack<CardReward>()).Push(__instance);
    }

    [HarmonyFinalizer]
    private static void Finalizer(CardReward __instance)
    {
        if (PopulateStack is not { Count: > 0 })
        {
            return;
        }

        if (ReferenceEquals(PopulateStack.Peek(), __instance))
        {
            PopulateStack.Pop();
            return;
        }

        PopulateStack.Clear();
    }
}
