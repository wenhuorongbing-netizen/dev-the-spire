namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsTransformable), MethodType.Getter)]
internal static class WitheredHuskTransformablePatch
{
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is WitheredHusk)
        {
            __result = false;
        }
    }
}

[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.GetDefaultTransformationOptions))]
internal static class WitheredHuskTransformationOptionsPatch
{
    private static void Postfix(ref IEnumerable<CardModel> __result)
    {
        __result = __result.Where(card => card is not WitheredHusk);
    }
}
