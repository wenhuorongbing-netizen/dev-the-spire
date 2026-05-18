namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]
internal static class PaelsToothPickupPatch
{
    [HarmonyPostfix]
    private static void Postfix(PaelsTooth __instance, ref Task __result)
    {
        __result = PaelsToothStoredCardService.ResetCounterAfterPickup(__instance, __result);
    }
}

[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]
internal static class PaelsToothCombatPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PaelsTooth __instance, CombatRoom room, ref Task __result)
    {
        __result = PaelsToothStoredCardService.AfterCombatEnd(__instance, room);
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]
internal static class PaelsToothActTransitionPatch
{
    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, ref Task __result)
    {
        if (__instance is not PaelsTooth paelsTooth)
        {
            return;
        }

        __result = ClearStoredCardsAfterOriginal(paelsTooth, __result);
    }

    private static async Task ClearStoredCardsAfterOriginal(PaelsTooth paelsTooth, Task original)
    {
        await original;
        if (paelsTooth.SerializableCards.Count > 0)
        {
            PaelsToothStoredCardService.ClearStoredCards(paelsTooth, "act transition");
        }
    }
}

