namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]
internal static class PrismaticGemPoolPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCreationOptions options, ref CardCreationOptions __result)
    {
        __result = options;
        return false;
    }
}

[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]
internal static class PrismaticGemRewardScreenContextPatch
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
