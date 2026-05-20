using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EZMicroBalance.EZMicroBalanceCode.Config;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

[HarmonyPatch(typeof(NTransformPreview), nameof(NTransformPreview.Initialize))]
internal static class TransformPreviewInitializePatch
{
    private static void Prefix(
        ref IEnumerable<CardTransformation> cardTransformations,
        out List<CardTransformation> __state)
    {
        __state = cardTransformations.ToList();
        cardTransformations = __state;
        TransformPreviewCyclePatch.PreparePredictions(__state);
    }

    private static void Finalizer()
    {
        TransformPreviewCyclePatch.ClearPredictions();
    }
}

[HarmonyPatch]
internal static class TransformPreviewCyclePatch
{
    private static Queue<CardModel?>? pendingPredictions;

    internal static void PreparePredictions(IReadOnlyList<CardTransformation> transformations)
    {
        pendingPredictions = null;

        if (!EZMicroBalanceModConfig.EnableTransformPrediction || !EZMicroBalanceModConfig.TransformPredictionAlwaysOn)
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

            pendingPredictions = queue;
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
            pendingPredictions = null;
            ReleaseEvidenceLog.Log(
                "PreviewTransform",
                "prediction_skipped_exception",
                runState: RunManager.Instance?.DebugOnlyGetState(),
                data: new Dictionary<string, object?>
                {
                    ["exception"] = exception.GetType().Name
                });
            PreviewLog.Warn("Transform prediction skipped: " + exception.Message);
        }
    }

    internal static void ClearPredictions()
    {
        pendingPredictions = null;
    }

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NTransformPreview), "CycleThroughCards")!;
    }

    private static bool Prefix(NPreviewCardHolder holder, CardPile cardPile, ref Task __result)
    {
        if (!EZMicroBalanceModConfig.EnableTransformPrediction || !EZMicroBalanceModConfig.TransformPredictionAlwaysOn)
        {
            return true;
        }

        if (pendingPredictions == null || pendingPredictions.Count == 0)
        {
            return true;
        }

        var predicted = pendingPredictions.Dequeue();
        if (predicted == null)
        {
            return true;
        }

        holder.Hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
        holder.ReassignToCard(predicted, cardPile.Type, null, ModelVisibility.Visible);
        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "prediction_displayed",
            data: new Dictionary<string, object?>
            {
                ["card"] = predicted.Id.Entry
            });
        __result = Task.CompletedTask;
        return false;
    }
}
