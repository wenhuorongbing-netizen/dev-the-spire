using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class PreviewToolsGuardTests
{
    [Fact]
    public void SingleManifestCarriesPreviewFeaturesInsideSpirePlus()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;

        Assert.Equal("EZMicroBalance", root.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", root.GetProperty("name").GetString());
        Assert.Contains("preview tools", root.GetProperty("description").GetString(), StringComparison.Ordinal);
        Assert.True(root.GetProperty("affects_gameplay").GetBoolean());
        Assert.True(root.GetProperty("has_pck").GetBoolean());
        Assert.True(root.GetProperty("has_dll").GetBoolean());

        foreach (var removedRootSurface in new[]
        {
            "EZFuturePeek",
            "EZFuturePeekCode",
            "EZFuturePeek.csproj",
            "EZFuturePeek.json",
            "EZFuturePeek.sln",
            Path.Combine("tests", "EZFuturePeek.Tests")
        })
        {
            Assert.False(Directory.Exists(RepoPath(removedRootSurface)), $"{removedRootSurface} should not remain as a separate mod directory.");
            Assert.False(File.Exists(RepoPath(removedRootSurface)), $"{removedRootSurface} should not remain as a separate mod file.");
        }
    }

    [Fact]
    public void FuturePeekManifestDecisionIsFoldedIntoGameplayImpactingSpirePlus()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;
        var releaseScope = ReadRepoText("docs", "specs", "release-scope-v1.md");
        var previewReadme = ReadRepoText("docs", "features", "preview-tools", "README.md");

        Assert.True(root.GetProperty("affects_gameplay").GetBoolean());
        Assert.Contains("preview tools", root.GetProperty("description").GetString(), StringComparison.Ordinal);
        Assert.Contains("Preview tools are no longer a separate mod", releaseScope, StringComparison.Ordinal);
        Assert.Contains("information advantage", previewReadme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("multiplayer", previewReadme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("live verification remains pending", previewReadme, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CrystalSpherePatchOnlyTouchesTheMaskAndButton()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Preview", "CrystalSpherePeekPatch.cs");

        Assert.Contains("NCrystalSphereScreen", source, StringComparison.Ordinal);
        Assert.Contains("%ScryMask", source, StringComparison.Ordinal);
        Assert.Contains("GetPeekButtonText()", source, StringComparison.Ordinal);
        Assert.Contains("Modulate", source, StringComparison.Ordinal);
        Assert.Contains("ToggleMode = true", source, StringComparison.Ordinal);
        Assert.Contains("\"预知\"", source, StringComparison.Ordinal);
        Assert.Contains("OnMinigameFinished", source, StringComparison.Ordinal);
        Assert.Contains("HideForFinishedScreen", source, StringComparison.Ordinal);
        Assert.Contains("OriginalMaskAlpha", source, StringComparison.Ordinal);

        Assert.DoesNotContain("ClearCell", source, StringComparison.Ordinal);
        Assert.DoesNotContain("RevealItem", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CellClicked", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddReward", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsHidden = false", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionQueueOrderIsGuardedAgainstExplicitReplacementSlots()
    {
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");
        var vanillaSource = ReadRepoText("source code", "src", "Core", "Nodes", "Cards", "NTransformPreview.cs");

        Assert.Contains("if (transformation.Replacement != null)", patchSource, StringComparison.Ordinal);
        Assert.Contains("continue;", SliceFrom(patchSource, "if (transformation.Replacement != null)"), StringComparison.Ordinal);
        Assert.Contains("cardTransformation.Replacement == null", vanillaSource, StringComparison.Ordinal);
        Assert.Contains("CycleThroughCards", SliceFrom(vanillaSource, "if (cardTransformation.Replacement == null)"), StringComparison.Ordinal);
        Assert.DoesNotContain(
            "queue.Enqueue(transformation.Replacement",
            patchSource,
            StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionDoesNotCreateRealCardsOrAdvanceRealRng()
    {
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");
        var predictionSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionService.cs");
        var combined = patchSource + Environment.NewLine + predictionSource;

        Assert.Contains("TransformPredictionRngContext.TryConsume", patchSource, StringComparison.Ordinal);
        Assert.Contains("no verified transform RNG source", patchSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<NTransformPreview, PredictionQueue>", patchSource, StringComparison.Ordinal);
        Assert.Contains("PreparePredictions(__instance", patchSource, StringComparison.Ordinal);
        Assert.Contains("ClearPredictions(__instance)", patchSource, StringComparison.Ordinal);
        Assert.Contains("PredictionsByPreview.TryGetValue(__instance", patchSource, StringComparison.Ordinal);
        Assert.Contains("predictions.Pending.Count == 0", patchSource, StringComparison.Ordinal);
        Assert.Contains("return true;", patchSource, StringComparison.Ordinal);
        Assert.Contains("holder.ReassignToCard", patchSource, StringComparison.Ordinal);
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
    }

    [Fact]
    public void TransformPredictionOnlyUsesSourceBackedRngContexts()
    {
        var contextSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionRngContext.cs");

        foreach (var eventRngSource in new[]
        {
            "AromaOfChaos",
            "EndlessConveyor",
            "Symbiote",
            "WhisperingHollow"
        })
        {
            Assert.Contains(eventRngSource, contextSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterEventRng(__instance", contextSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.Rng\"", contextSource, StringComparison.Ordinal);

        foreach (var nicheRngSource in new[]
        {
            "MorphicGrove",
            "Trial",
            "NewLeaf",
            "Astrolabe"
        })
        {
            Assert.Contains(nicheRngSource, contextSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterNicheRng(__instance.Owner", contextSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.RunState.Rng.Niche\"", contextSource, StringComparison.Ordinal);
        Assert.Contains("upgradeReplacementPreview: true", contextSource, StringComparison.Ordinal);

        Assert.Contains("new Rng(snapshot.Seed, snapshot.Counter)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.TryGetValue(player", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Dictionary<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSelectionLifetimePatch", contextSource, StringComparison.Ordinal);
        Assert.Contains("ClearContextWhenSelectionCompletes", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("NextItem", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("FastForward", contextSource, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionContextCannotBeReusedAfterSelection()
    {
        var contextSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionRngContext.cs");

        var clearSlice = SliceFrom(contextSource, "private static async Task<IEnumerable<CardModel>> ClearContextWhenSelectionCompletes");
        Assert.Contains("try", clearSlice, StringComparison.Ordinal);
        Assert.Contains("finally", clearSlice, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", clearSlice, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.Remove(player)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        var staleSourceSlice = SliceFrom(contextSource, "if (snapshot.Source.Seed != snapshot.Seed || snapshot.Source.Counter != snapshot.Counter)");
        Assert.Contains("Clear(player)", staleSourceSlice, StringComparison.Ordinal);
        Assert.Contains("Transform prediction skipped: stale RNG source", staleSourceSlice, StringComparison.Ordinal);
    }

    [Fact]
    public void ConfigLocalizationContainsReadablePreviewRows()
    {
        var english = TestRepo.JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json");
        var simplifiedChinese = TestRepo.JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json");
        var requiredKeys = new[]
        {
            "EZMICROBALANCE.mod_title",
            "EZMICROBALANCE-CRYSTAL_SPHERE_PEEK_BUTTON.title",
            "EZMICROBALANCE-ENABLE_CRYSTAL_SPHERE_PEEK.title",
            "EZMICROBALANCE-CRYSTAL_SPHERE_MASK_ALPHA.title",
            "EZMICROBALANCE-ENABLE_TRANSFORM_PREDICTION.title",
            "EZMICROBALANCE-TRANSFORM_PREDICTION_ALWAYS_ON.title",
            "EZMICROBALANCE-SHOW_PREVIEW_DEBUG_LOGS.title"
        };

        foreach (var key in requiredKeys)
        {
            Assert.True(english.TryGetValue(key, out var englishValue), $"Missing English key: {key}");
            Assert.True(simplifiedChinese.TryGetValue(key, out var zhsValue), $"Missing zhs key: {key}");
            Assert.False(string.IsNullOrWhiteSpace(englishValue), $"Empty English key: {key}");
            Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs key: {key}");
        }

        Assert.Equal("Spire Plus", simplifiedChinese["EZMICROBALANCE.mod_title"]);
        Assert.Equal("预知", simplifiedChinese["EZMICROBALANCE-CRYSTAL_SPHERE_PEEK_BUTTON.title"]);
        Assert.Equal("水晶球预知按钮", simplifiedChinese["EZMICROBALANCE-ENABLE_CRYSTAL_SPHERE_PEEK.title"]);
        Assert.Equal("水晶球雾层透明度", simplifiedChinese["EZMICROBALANCE-CRYSTAL_SPHERE_MASK_ALPHA.title"]);
        Assert.Equal("Preview diagnostics logs", english["EZMICROBALANCE-SHOW_PREVIEW_DEBUG_LOGS.title"]);
        Assert.Equal("预览诊断日志", simplifiedChinese["EZMICROBALANCE-SHOW_PREVIEW_DEBUG_LOGS.title"]);

        var combinedZhs = string.Join('\n', simplifiedChinese.Values);
        var combinedEnglish = string.Join('\n', english.Values);
        Assert.DoesNotContain("debug", combinedEnglish, StringComparison.OrdinalIgnoreCase);
        foreach (var mojibake in new[] { "妫", "閸", "閺", "閼", "鐠", "鈧", "棰勭煡" })
        {
            Assert.DoesNotContain(mojibake, combinedZhs, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ProjectUsesOneRuntimeCodeRootAndNoSecondPreviewProject()
    {
        var project = ReadRepoText("EZMicroBalance.csproj");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var workflow = ReadRepoText(".github", "workflows", "full-local-validation.yml");
        var ciScript = ReadRepoText("scripts", "ci-full-validation.ps1");

        Assert.Contains("EZMicroBalanceCode/**/*.cs", project, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeekCode/**/*.cs", project, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek", project, StringComparison.Ordinal);
        Assert.DoesNotContain("res://EZFuturePeek", exportPreset, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.sln", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.sln", ciScript, StringComparison.Ordinal);
        Assert.DoesNotContain("IncludeFuturePeek", ciScript, StringComparison.Ordinal);
    }

}
