using System.Collections.Generic;
using System.Linq;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(NTransformPreview), nameof(NTransformPreview.Initialize))]
internal static class TransformPreviewInitializePatch
{
    private static void Prefix(
        NTransformPreview __instance,
        ref IEnumerable<CardTransformation> cardTransformations,
        out List<CardTransformation> __state)
    {
        __state = cardTransformations.ToList();
        cardTransformations = __state;
        TransformPreviewCyclePatch.PreparePredictions(__instance, __state);
    }

    private static void Finalizer(NTransformPreview __instance)
    {
        TransformPreviewCyclePatch.ClearPredictions(__instance);
    }
}

[HarmonyPatch]
internal static partial class TransformPreviewCyclePatch
{
    internal static void PreparePredictions(NTransformPreview preview, IReadOnlyList<CardTransformation> transformations)
    {
        ClearPredictions(preview);

        if (!SpirePlusModConfig.EnableTransformPrediction || !SpirePlusModConfig.TransformPredictionAlwaysOn)
        {
            return;
        }

        if (transformations.Count == 0)
        {
            return;
        }

        try
        {
            var owner = transformations[0].Original.Owner;
            if (!MultiplayerFeaturePolicy.IsSingleplayer(owner.RunState))
            {
                MultiplayerFeaturePolicy.LogCoopEvidence(
                    "PreviewTransform",
                    "prediction_prepared_multiplayer_ui_only",
                    owner.RunState,
                    new Dictionary<string, object?>
                    {
                        ["reason"] = "Transform prediction displays only the local preview card. It does not create a PlayerChoice, reward alternative, or advance the real transform RNG.",
                        ["netMode"] = MultiplayerFeaturePolicy.DescribeNetMode(owner.RunState)
                    });
            }

            if (!TransformPredictionRngContext.TryConsume(
                    owner,
                    out var fork,
                    out var sourceName,
                    out var upgradeReplacementPreview))
            {
                ReleaseEvidenceLog.Log(
                    "PreviewTransform",
                    "prediction_skipped_no_source",
                    owner);
                PreviewLog.Debug("Transform prediction skipped: no verified transform RNG source.");
                return;
            }

            var queue = new Queue<CardModel?>();

            foreach (var transformation in transformations)
            {
                if (transformation.Replacement != null)
                {
                    continue;
                }

                queue.Enqueue(TransformPredictionService.PredictReplacementModel(
                    transformation,
                    fork,
                    upgradeReplacementPreview));
            }

            StorePredictions(preview, queue);
            ReleaseEvidenceLog.Log(
                "PreviewTransform",
                "prediction_prepared",
                owner,
                new Dictionary<string, object?>
                {
                    ["count"] = queue.Count,
                    ["source"] = sourceName,
                    ["upgradePreview"] = upgradeReplacementPreview
                });
            PreviewLog.Debug($"Prepared {queue.Count} transform prediction(s) from {sourceName}.");
        }
        catch (Exception exception)
        {
            ClearPredictions(preview);
            ReleaseEvidenceLog.Log(
                "PreviewTransform",
                "prediction_skipped_exception",
                runState: MultiplayerFeaturePolicy.CurrentRunStateOrNull(),
                data: new Dictionary<string, object?>
                {
                    ["exception"] = exception.GetType().Name
                });
            PreviewLog.Warn("Transform prediction skipped: " + exception.Message);
        }
    }
}
