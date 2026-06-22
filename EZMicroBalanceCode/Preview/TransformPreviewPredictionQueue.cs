using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal sealed partial class TransformPreviewCyclePatch
{
    private static readonly ConditionalWeakTable<NTransformPreview, PredictionQueue> PredictionsByPreview = new();

    internal static void ClearPredictions(NTransformPreview preview)
    {
        PredictionsByPreview.Remove(preview);
    }

    private static void StorePredictions(NTransformPreview preview, Queue<CardModel?> pending)
    {
        PredictionsByPreview.Add(preview, new PredictionQueue(pending));
    }

    private static bool TryDequeuePrediction(NTransformPreview preview, out CardModel? predicted)
    {
        predicted = null;
        if (!PredictionsByPreview.TryGetValue(preview, out var predictions) ||
            predictions.Pending.Count == 0)
        {
            return false;
        }

        predicted = predictions.Pending.Dequeue();
        return true;
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
