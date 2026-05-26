using System.Collections.Generic;
using System.Reflection;
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

internal static partial class TransformPreviewCyclePatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NTransformPreview), "CycleThroughCards")!;
    }

    private static bool Prefix(NTransformPreview __instance, NPreviewCardHolder holder, CardPile cardPile, ref Task __result)
    {
        try
        {
            if (!TryDisplayPrediction(__instance, holder, cardPile, out var result))
            {
                return true;
            }

            __result = result;
            return false;
        }
        catch (Exception exception)
        {
            LogDisplayFailure(__instance, exception);
            return true;
        }
    }

    private static bool TryDisplayPrediction(
        NTransformPreview preview,
        NPreviewCardHolder holder,
        CardPile cardPile,
        out Task result)
    {
        result = Task.CompletedTask;
        if (!SpirePlusModConfig.EnableTransformPrediction || !SpirePlusModConfig.TransformPredictionAlwaysOn)
        {
            return false;
        }

        if (!TryDequeuePrediction(preview, out var predicted))
        {
            return false;
        }

        if (predicted == null)
        {
            return false;
        }

        holder.Hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
        holder.ReassignToCard(predicted, cardPile.Type, null, ModelVisibility.Visible);
        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "prediction_displayed",
            data: new Dictionary<string, object?>
            {
                ["card"] = predicted.Id.Entry,
                ["netMode"] = MultiplayerFeaturePolicy.DescribeNetMode(MultiplayerFeaturePolicy.CurrentRunStateOrNull())
            });
        return true;
    }

    private static void LogDisplayFailure(NTransformPreview preview, Exception exception)
    {
        ClearPredictions(preview);
        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "prediction_display_failed_fallback_vanilla",
            runState: MultiplayerFeaturePolicy.CurrentRunStateOrNull(),
            data: new Dictionary<string, object?>
            {
                ["exception"] = exception.GetType().Name
            });
        PreviewLog.Warn("Transform prediction display failed; falling back to vanilla cycling: " + exception.Message);
    }
}
