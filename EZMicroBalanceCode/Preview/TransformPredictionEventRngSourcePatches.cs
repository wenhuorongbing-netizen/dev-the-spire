using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Events;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal sealed class TransformPredictionAromaOfChaosRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-aroma-of-chaos-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Aroma of Chaos' event RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(AromaOfChaos), "LetGo")];

    [HarmonyPrefix]
    private static void Prefix(AromaOfChaos __instance) =>
        TransformPredictionEventRngSourcePatches.RegisterEventRng(__instance, nameof(AromaOfChaos));
}

internal sealed class TransformPredictionEndlessConveyorRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-endless-conveyor-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Endless Conveyor's event RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(EndlessConveyor), "JellyLiver")];

    [HarmonyPrefix]
    private static void Prefix(EndlessConveyor __instance) =>
        TransformPredictionEventRngSourcePatches.RegisterEventRng(__instance, nameof(EndlessConveyor));
}

internal sealed class TransformPredictionSymbioteRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-symbiote-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Symbiote's event RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(Symbiote), "KillWithFire")];

    [HarmonyPrefix]
    private static void Prefix(Symbiote __instance) =>
        TransformPredictionEventRngSourcePatches.RegisterEventRng(__instance, nameof(Symbiote));
}

internal sealed class TransformPredictionWhisperingHollowRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-whispering-hollow-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Whispering Hollow's event RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(WhisperingHollow), "Hug")];

    [HarmonyPrefix]
    private static void Prefix(WhisperingHollow __instance) =>
        TransformPredictionEventRngSourcePatches.RegisterEventRng(__instance, nameof(WhisperingHollow));
}

internal static class TransformPredictionEventRngSourcePatches
{
    // Capture the source RNG before vanilla opens the selector; the preview later
    // reads a forked snapshot, while the real event still consumes the live RNG.
    internal static void RegisterEventRng(EventModel source, string sourceName) =>
        TransformPredictionRngContext.Register(source.Owner, source.Rng, $"{sourceName}.Rng");
}
