using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal sealed class TransformPredictionMorphicGroveNicheRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-morphic-grove-niche-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Morphic Grove's Niche RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(MorphicGrove), "Group")];

    [HarmonyPrefix]
    private static void Prefix(MorphicGrove __instance) =>
        TransformPredictionNicheRngSourcePatches.RegisterNicheRng(__instance.Owner, nameof(MorphicGrove));
}

internal sealed class TransformPredictionTrialNicheRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-trial-niche-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Trial's Niche RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(Trial), "NondescriptInnocent")];

    [HarmonyPrefix]
    private static void Prefix(Trial __instance) =>
        TransformPredictionNicheRngSourcePatches.RegisterNicheRng(__instance.Owner, nameof(Trial));
}

internal sealed class TransformPredictionNewLeafNicheRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-new-leaf-niche-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture New Leaf's Niche RNG before its transform-card selection preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(NewLeaf), nameof(NewLeaf.AfterObtained))];

    [HarmonyPrefix]
    private static void Prefix(NewLeaf __instance) =>
        TransformPredictionNicheRngSourcePatches.RegisterNicheRng(__instance.Owner, nameof(NewLeaf));
}

internal sealed class TransformPredictionAstrolabeNicheRngPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "transform-prediction-astrolabe-niche-rng";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Capture Astrolabe's Niche RNG before its upgraded transform-card preview opens";
    static ModPatchTarget[] IPatchMethod.GetTargets() => [new ModPatchTarget(typeof(Astrolabe), nameof(Astrolabe.AfterObtained))];

    [HarmonyPrefix]
    private static void Prefix(Astrolabe __instance) =>
        TransformPredictionNicheRngSourcePatches.RegisterNicheRng(
            __instance.Owner,
            nameof(Astrolabe),
            upgradeReplacementPreview: true);
}

internal static class TransformPredictionNicheRngSourcePatches
{
    // These sources all use RunState.Rng.Niche for the real transform. Preview
    // code receives only a forked snapshot so the actual RNG counter is untouched.
    internal static void RegisterNicheRng(
        Player? player,
        string sourceName,
        bool upgradeReplacementPreview = false) =>
        TransformPredictionRngContext.Register(
            player,
            player?.RunState.Rng.Niche,
            $"{sourceName}.RunState.Rng.Niche",
            upgradeReplacementPreview);
}
