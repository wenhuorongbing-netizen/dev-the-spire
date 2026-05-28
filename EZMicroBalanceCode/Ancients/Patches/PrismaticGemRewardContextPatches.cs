using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PrismaticGemPoolPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-r-i-s-m-a-t-i-c-g-e-m-p-o-o-l-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch PrismaticGem.ModifyCardRewardCreationOptions";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))];
{
    [HarmonyPrefix]
    private static bool Prefix(CardCreationOptions options, ref CardCreationOptions __result)
    {
        __result = options;
        return false;
    }
}

internal sealed class PrismaticGemRewardScreenContextPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-r-i-s-m-a-t-i-c-g-e-m-r-e-w-a-r-d-s-c-r-e-e-n-c-o-n-t-e-x-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch CardReward.Populate";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardReward), nameof(CardReward.Populate))];
{
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


