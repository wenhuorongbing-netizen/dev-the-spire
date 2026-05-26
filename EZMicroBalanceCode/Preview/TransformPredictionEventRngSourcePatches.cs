using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Events;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch]
internal static class TransformPredictionEventRngSourcePatches
{
    [HarmonyPatch(typeof(AromaOfChaos), "LetGo")]
    [HarmonyPrefix]
    private static void AromaOfChaosLetGo(AromaOfChaos __instance) =>
        RegisterEventRng(__instance, nameof(AromaOfChaos));

    [HarmonyPatch(typeof(EndlessConveyor), "JellyLiver")]
    [HarmonyPrefix]
    private static void EndlessConveyorJellyLiver(EndlessConveyor __instance) =>
        RegisterEventRng(__instance, nameof(EndlessConveyor));

    [HarmonyPatch(typeof(Symbiote), "KillWithFire")]
    [HarmonyPrefix]
    private static void SymbioteKillWithFire(Symbiote __instance) =>
        RegisterEventRng(__instance, nameof(Symbiote));

    [HarmonyPatch(typeof(WhisperingHollow), "Hug")]
    [HarmonyPrefix]
    private static void WhisperingHollowHug(WhisperingHollow __instance) =>
        RegisterEventRng(__instance, nameof(WhisperingHollow));

    private static void RegisterEventRng(EventModel source, string sourceName) =>
        TransformPredictionRngContext.Register(source.Owner, source.Rng, $"{sourceName}.Rng");
}
