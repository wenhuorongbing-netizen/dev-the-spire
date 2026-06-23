using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PaelsToothPickupPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "paels-tooth-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Pael's Tooth stored-card counter after pickup";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))];

    [HarmonyPostfix]
    private static void Postfix(PaelsTooth __instance, ref Task __result)
    {
        __result = PaelsToothStoredCardService.ResetCounterAfterPickup(__instance, __result);
    }
}

internal sealed class PaelsToothCombatPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "paels-tooth-after-combat-end";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Resolve Pael's Tooth stored-card behavior after combat";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))];

    [HarmonyPrefix]
    private static bool Prefix(PaelsTooth __instance, CombatRoom room, ref Task __result)
    {
        __result = PaelsToothStoredCardService.AfterCombatEnd(__instance, room);
        return false;
    }
}

internal sealed class PaelsToothActTransitionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "paels-tooth-act-transition";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Clear Pael's Tooth stored cards after act transition";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))];

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
