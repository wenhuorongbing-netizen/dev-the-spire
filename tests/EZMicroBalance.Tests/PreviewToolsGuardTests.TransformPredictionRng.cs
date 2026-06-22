using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class PreviewToolsGuardTests
{
    [Fact]
    public void TransformPredictionDoesNotCreateRealCardsOrAdvanceRealRng()
    {
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");
        var displaySource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewCyclePatch.Display.cs");
        var queueSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPredictionQueue.cs");
        var predictionSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionService.cs");
        var combined = patchSource + Environment.NewLine + displaySource + Environment.NewLine + queueSource + Environment.NewLine + predictionSource;

        Assert.Contains("TransformPredictionRngContext.TryConsume", patchSource, StringComparison.Ordinal);
        Assert.Contains("IPatchMethod.PatchId => \"transform-preview-initialize\"", patchSource, StringComparison.Ordinal);
        Assert.Contains("IPatchMethod.PatchId => \"transform-preview-cycle-display\"", displaySource, StringComparison.Ordinal);
        Assert.Contains("no verified transform RNG source", patchSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<NTransformPreview, PredictionQueue>", queueSource, StringComparison.Ordinal);
        Assert.Contains("PreparePredictions(__instance", patchSource, StringComparison.Ordinal);
        Assert.Contains("ClearPredictions(__instance)", patchSource, StringComparison.Ordinal);
        Assert.Contains("StorePredictions(preview, queue)", patchSource, StringComparison.Ordinal);
        Assert.Contains("TryDequeuePrediction(preview", displaySource, StringComparison.Ordinal);
        Assert.Contains("PredictionsByPreview.TryGetValue(preview", queueSource, StringComparison.Ordinal);
        Assert.Contains("predictions.Pending.Count == 0", queueSource, StringComparison.Ordinal);
        Assert.Contains("return true;", displaySource, StringComparison.Ordinal);
        Assert.Contains("holder.ReassignToCard", displaySource, StringComparison.Ordinal);
        Assert.Contains("CardFactory.GetDefaultTransformationOptions", predictionSource, StringComparison.Ordinal);
        Assert.Contains("rng.NextItem(optionArray)", predictionSource, StringComparison.Ordinal);
        Assert.Contains("predicted.ToMutable()", predictionSource, StringComparison.Ordinal);
        Assert.Contains("preview.UpgradeInternal()", predictionSource, StringComparison.Ordinal);
        Assert.Contains("preview.FinalizeUpgradeInternal()", predictionSource, StringComparison.Ordinal);

        Assert.DoesNotContain("GetReplacement(", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateRandomCardForTransform", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("RunState.CreateCard", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("CombatState.CreateCard", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("Original.Owner.PlayerRng.Transformations", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("realRng.Next", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("realRng.FastForward", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("private static Queue<CardModel?>? pendingPredictions", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch", displaySource, StringComparison.Ordinal);
        Assert.DoesNotContain("AccessTools.Method", displaySource, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionOnlyUsesSourceBackedRngContexts()
    {
        var contextSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionRngContext.cs");
        var eventSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionEventRngSourcePatches.cs");
        var nicheSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionNicheRngSourcePatches.cs");
        var lifetimeSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionSelectionLifetimePatch.cs");
        var combinedSource = string.Join(Environment.NewLine, contextSource, eventSource, nicheSource, lifetimeSource);

        foreach (var eventRngSource in new[]
        {
            "AromaOfChaos",
            "EndlessConveyor",
            "Symbiote",
            "WhisperingHollow"
        })
        {
            Assert.Contains(eventRngSource, eventSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterEventRng(__instance", eventSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.Rng\"", eventSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionAromaOfChaosRngPatch : IPatchMethod", eventSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionEndlessConveyorRngPatch : IPatchMethod", eventSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSymbioteRngPatch : IPatchMethod", eventSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionWhisperingHollowRngPatch : IPatchMethod", eventSource, StringComparison.Ordinal);
        Assert.Contains("IPatchMethod.PatchId => \"transform-prediction-aroma-of-chaos-rng\"", eventSource, StringComparison.Ordinal);
        Assert.Contains("new ModPatchTarget(typeof(AromaOfChaos), \"LetGo\")", eventSource, StringComparison.Ordinal);

        foreach (var nicheRngSource in new[]
        {
            "MorphicGrove",
            "Trial",
            "NewLeaf",
            "Astrolabe"
        })
        {
            Assert.Contains(nicheRngSource, nicheSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterNicheRng(__instance.Owner", nicheSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.RunState.Rng.Niche\"", nicheSource, StringComparison.Ordinal);
        Assert.Contains("upgradeReplacementPreview: true", nicheSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionMorphicGroveNicheRngPatch : IPatchMethod", nicheSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionTrialNicheRngPatch : IPatchMethod", nicheSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionNewLeafNicheRngPatch : IPatchMethod", nicheSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionAstrolabeNicheRngPatch : IPatchMethod", nicheSource, StringComparison.Ordinal);
        Assert.Contains("IPatchMethod.PatchId => \"transform-prediction-astrolabe-niche-rng\"", nicheSource, StringComparison.Ordinal);
        Assert.Contains("new ModPatchTarget(typeof(Astrolabe), nameof(Astrolabe.AfterObtained))", nicheSource, StringComparison.Ordinal);

        Assert.Contains("new Rng(snapshot.Seed, snapshot.Counter)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.TryGetValue(player", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Dictionary<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSelectionLifetimePatch", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSelectionLifetimePatch : IPatchMethod", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("IPatchMethod.PatchId => \"transform-prediction-selection-lifetime\"", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("new ModPatchTarget(", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("nameof(CardSelectCmd.FromDeckForTransformation)", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("typeof(Func<CardModel, CardTransformation>)", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("ClearContextWhenSelectionCompletes", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", lifetimeSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch", eventSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch", nicheSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch", lifetimeSource, StringComparison.Ordinal);
        Assert.DoesNotContain("NextItem", combinedSource, StringComparison.Ordinal);
        Assert.DoesNotContain("FastForward", combinedSource, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionContextCannotBeReusedAfterSelection()
    {
        var contextSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionRngContext.cs");
        var lifetimeSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionSelectionLifetimePatch.cs");

        var clearSlice = SliceFrom(lifetimeSource, "private static async Task<IEnumerable<CardModel>> ClearContextWhenSelectionCompletes");
        Assert.Contains("try", clearSlice, StringComparison.Ordinal);
        Assert.Contains("finally", clearSlice, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", clearSlice, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.Remove(player)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        var staleSourceSlice = SliceFrom(contextSource, "if (snapshot.Source.Seed != snapshot.Seed || snapshot.Source.Counter != snapshot.Counter)");
        Assert.Contains("Clear(player)", staleSourceSlice, StringComparison.Ordinal);
        Assert.Contains("Transform prediction skipped: stale RNG source", staleSourceSlice, StringComparison.Ordinal);
    }
}
