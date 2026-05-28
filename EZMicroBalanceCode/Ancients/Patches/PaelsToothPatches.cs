using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PaelsToothPickupPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-a-e-l-s-t-o-o-t-h-p-i-c-k-u-p-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch PaelsTooth.AfterObtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))];
{
    [HarmonyPostfix]
    private static void Postfix(PaelsTooth __instance, ref Task __result)
    {
        __result = PaelsToothStoredCardService.ResetCounterAfterPickup(__instance, __result);
    }
}

internal sealed class PaelsToothCombatPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-a-e-l-s-t-o-o-t-h-c-o-m-b-a-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch PaelsTooth.AfterCombatEnd";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))];
{
    [HarmonyPrefix]
    private static bool Prefix(PaelsTooth __instance, CombatRoom room, ref Task __result)
    {
        __result = PaelsToothStoredCardService.AfterCombatEnd(__instance, room);
        return false;
    }
}

internal sealed class PaelsToothActTransitionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-a-e-l-s-t-o-o-t-h-a-c-t-t-r-a-n-s-i-t-i-o-n-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch AbstractModel.AfterActEntered";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))];
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



