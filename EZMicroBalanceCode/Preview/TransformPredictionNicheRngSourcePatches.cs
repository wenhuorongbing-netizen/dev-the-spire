using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch]
internal static class TransformPredictionNicheRngSourcePatches
{
    [HarmonyPatch(typeof(MorphicGrove), "Group")]
    [HarmonyPrefix]
    private static void MorphicGroveGroup(MorphicGrove __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(MorphicGrove));

    [HarmonyPatch(typeof(Trial), "NondescriptInnocent")]
    [HarmonyPrefix]
    private static void TrialNondescriptInnocent(Trial __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(Trial));

    [HarmonyPatch(typeof(NewLeaf), nameof(NewLeaf.AfterObtained))]
    [HarmonyPrefix]
    private static void NewLeafAfterObtained(NewLeaf __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(NewLeaf));

    [HarmonyPatch(typeof(Astrolabe), nameof(Astrolabe.AfterObtained))]
    [HarmonyPrefix]
    private static void AstrolabeAfterObtained(Astrolabe __instance) =>
        RegisterNicheRng(__instance.Owner, nameof(Astrolabe), upgradeReplacementPreview: true);

    private static void RegisterNicheRng(
        Player? player,
        string sourceName,
        bool upgradeReplacementPreview = false) =>
        TransformPredictionRngContext.Register(
            player,
            player?.RunState.Rng.Niche,
            $"{sourceName}.RunState.Rng.Niche",
            upgradeReplacementPreview);
}
