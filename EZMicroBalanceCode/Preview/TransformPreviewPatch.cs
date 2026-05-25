using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
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
internal static class TransformPreviewCyclePatch
{
    private static readonly ConditionalWeakTable<NTransformPreview, PredictionQueue> PredictionsByPreview = new();

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
            if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopPreviewTool(
                    owner.RunState,
                    "PreviewTransform",
                    "transform prediction preview is single-player only until co-op selection, RNG, and reconnect paths have live proof"))
            {
                return;
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

            PredictionsByPreview.Add(preview, new PredictionQueue(queue));
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
                runState: RunManager.Instance?.DebugOnlyGetState(),
                data: new Dictionary<string, object?>
                {
                    ["exception"] = exception.GetType().Name
                });
            PreviewLog.Warn("Transform prediction skipped: " + exception.Message);
        }
    }

    internal static void ClearPredictions(NTransformPreview preview)
    {
        PredictionsByPreview.Remove(preview);
    }

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NTransformPreview), "CycleThroughCards")!;
    }

    private static bool Prefix(NTransformPreview __instance, NPreviewCardHolder holder, CardPile cardPile, ref Task __result)
    {
        if (!SpirePlusModConfig.EnableTransformPrediction || !SpirePlusModConfig.TransformPredictionAlwaysOn)
        {
            return true;
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopPreviewTool(
                RunManager.Instance?.DebugOnlyGetState(),
                "PreviewTransform",
                "transform prediction preview is single-player only until co-op selection, RNG, and reconnect paths have live proof"))
        {
            ClearPredictions(__instance);
            return true;
        }

        if (!PredictionsByPreview.TryGetValue(__instance, out var predictions) ||
            predictions.Pending.Count == 0)
        {
            return true;
        }

        var predicted = predictions.Pending.Dequeue();
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

    private sealed class PredictionQueue
    {
        public PredictionQueue(Queue<CardModel?> pending)
        {
            Pending = pending;
        }

        public Queue<CardModel?> Pending { get; }
    }
}
