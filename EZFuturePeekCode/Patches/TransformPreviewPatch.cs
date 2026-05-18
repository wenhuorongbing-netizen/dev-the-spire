using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EZFuturePeek.EZFuturePeekCode.Config;
using EZFuturePeek.EZFuturePeekCode.Diagnostics;
using EZFuturePeek.EZFuturePeekCode.Prediction;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Random;

namespace EZFuturePeek.EZFuturePeekCode.Patches;

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

        if (!EZFuturePeekConfig.EnableTransformPrediction || !EZFuturePeekConfig.TransformPredictionAlwaysOn)
        {
            return;
        }

        if (transformations.Count == 0)
        {
            return;
        }

        try
        {
            var realRng = transformations[0].Original.Owner.PlayerRng.Transformations;
            var fork = new Rng(realRng.Seed, realRng.Counter);
            var queue = new Queue<CardModel?>();

            foreach (var transformation in transformations)
            {
                if (transformation.Replacement != null)
                {
                    continue;
                }

                queue.Enqueue(TransformPredictionService.PredictReplacementModel(transformation, fork));
            }

            pendingPredictions = queue;
            FuturePeekLog.Debug($"Prepared {queue.Count} transform prediction(s).");
        }
        catch (Exception exception)
        {
            pendingPredictions = null;
            FuturePeekLog.Warn("Transform prediction skipped: " + exception.Message);
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
        if (!EZFuturePeekConfig.EnableTransformPrediction || !EZFuturePeekConfig.TransformPredictionAlwaysOn)
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
        __result = Task.CompletedTask;
        return false;
    }
}
