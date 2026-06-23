using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class WitheredHuskTransformablePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-withered-husk-transformable";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Keep Urda Withered Husk out of player-facing transform selection";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(CardModel), nameof(CardModel.IsTransformable), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is WitheredHusk)
        {
            __result = false;
        }
    }
}

internal sealed class WitheredHuskTransformationOptionsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-withered-husk-transformation-options";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Remove Urda Withered Husk from default transform result pools";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(CardFactory),
            nameof(CardFactory.GetDefaultTransformationOptions),
            [typeof(CardModel), typeof(bool)])
    ];

    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<CardModel> __result)
    {
        __result = __result.Where(card => card is not WitheredHusk);
    }
}
