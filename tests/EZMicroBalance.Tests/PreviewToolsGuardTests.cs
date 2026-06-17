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
        var compatibilityGoal = ReadRepoText("docs", "features", "future-peek", "goal.md");

        Assert.True(root.GetProperty("affects_gameplay").GetBoolean());
        Assert.Contains("preview tools", root.GetProperty("description").GetString(), StringComparison.Ordinal);
        Assert.Contains("Preview tools are no longer a separate mod", releaseScope, StringComparison.Ordinal);
        Assert.Contains("information advantage", previewReadme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("multiplayer", previewReadme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("live verification remains pending", previewReadme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("compatibility pointer for old task links", compatibilityGoal, StringComparison.Ordinal);
        Assert.Contains("docs/features/preview-tools/README.md", compatibilityGoal, StringComparison.Ordinal);
        Assert.Contains("local UI-only preview", compatibilityGoal, StringComparison.Ordinal);
        Assert.Contains("Map foresight and reward foresight are not implemented", compatibilityGoal, StringComparison.Ordinal);
        Assert.Contains("deterministic or host-authoritative precommit plan", compatibilityGoal, StringComparison.Ordinal);
        Assert.Contains("CardRewardAlternative", compatibilityGoal, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek", compatibilityGoal, StringComparison.Ordinal);
        Assert.DoesNotContain("standalone", compatibilityGoal, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PreviewToolsRunAsLocalUiOnlyInCoopRuns()
    {
        var policySource = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var crystalSource = ReadRepoText("EZMicroBalanceCode", "Preview", "CrystalSpherePeekPatch.cs");
        var transformSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");
        var transformDisplaySource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewCyclePatch.Display.cs");
        var transformContextSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionRngContext.cs");
        var combinedPreviewSource = ReadSourceTree("EZMicroBalanceCode", "Preview");

        Assert.DoesNotContain("SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS", policySource, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMB_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS", policySource, StringComparison.Ordinal);
        Assert.DoesNotContain("ShouldDisableUnverifiedCoopPreviewTool", policySource, StringComparison.Ordinal);
        Assert.DoesNotContain("coop_preview_tool_disabled", policySource, StringComparison.Ordinal);
        Assert.DoesNotContain("coop_preview_tool_override_enabled", policySource, StringComparison.Ordinal);
        Assert.DoesNotContain("LoggedCoopPreviewGateKeys", policySource, StringComparison.Ordinal);
        Assert.Contains("netType is NetGameType.Singleplayer or NetGameType.None &&", policySource, StringComparison.Ordinal);
        Assert.Contains("if (netType == NetGameType.Host)", policySource, StringComparison.Ordinal);
        Assert.Contains("if (netType == NetGameType.Client)", policySource, StringComparison.Ordinal);

        Assert.DoesNotContain("ShouldDisableUnverifiedCoopPreviewTool", combinedPreviewSource, StringComparison.Ordinal);
        Assert.Contains("coop_local_ui_preview_enabled", crystalSource, StringComparison.Ordinal);
        Assert.Contains("prediction_prepared_multiplayer_ui_only", transformSource, StringComparison.Ordinal);
        Assert.Contains("coop_ui_only_rng_context_registered", transformContextSource, StringComparison.Ordinal);
        Assert.Contains("Transform prediction displays only the local preview card", transformSource, StringComparison.Ordinal);
        Assert.Contains("does not create a PlayerChoice, reward alternative, or advance the real transform RNG", transformSource, StringComparison.Ordinal);
        Assert.Contains("MultiplayerFeaturePolicy.CurrentRunStateOrNull()", combinedPreviewSource, StringComparison.Ordinal);
        Assert.Contains("ClearPredictions(__instance)", transformSource, StringComparison.Ordinal);
        Assert.Contains("Clear(player)", transformContextSource, StringComparison.Ordinal);
        Assert.Contains("prediction_display_failed_fallback_vanilla", transformDisplaySource, StringComparison.Ordinal);
        Assert.Contains("LogDisplayFailure(__instance, exception)", transformDisplaySource, StringComparison.Ordinal);
        Assert.Contains("return true;", SliceFrom(transformDisplaySource, "catch (Exception exception)"), StringComparison.Ordinal);
        Assert.DoesNotContain("PlayerChoiceSynchronizer", combinedPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("new PlayerChoice", combinedPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CardRewardAlternative", combinedPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("AddReward", combinedPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("FastForward", combinedPreviewSource, StringComparison.Ordinal);
    }

    [Fact]
    public void CrystalSpherePatchOnlyTouchesTheMaskAndButton()
    {
        var source = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Preview", "CrystalSpherePeekPatch.cs"),
            ReadRepoText("EZMicroBalanceCode", "Preview", "CrystalSpherePeekState.cs"));

        Assert.Contains("NCrystalSphereScreen", source, StringComparison.Ordinal);
        Assert.Contains("coop_local_ui_preview_enabled", source, StringComparison.Ordinal);
        Assert.Contains("Crystal Sphere peek only changes local ScryMask alpha", source, StringComparison.Ordinal);
        Assert.Contains("%ScryMask", source, StringComparison.Ordinal);
        Assert.Contains("GetPeekButtonText()", source, StringComparison.Ordinal);
        Assert.Contains("Modulate", source, StringComparison.Ordinal);
        Assert.Contains("ToggleMode = true", source, StringComparison.Ordinal);
        Assert.Contains("\"预知\"", source, StringComparison.Ordinal);
        Assert.Contains("OnMinigameFinished", source, StringComparison.Ordinal);
        Assert.Contains("HideForFinishedScreen", source, StringComparison.Ordinal);
        Assert.Contains("OriginalMaskAlpha", source, StringComparison.Ordinal);
        Assert.Contains("ToggleOnSfx = \"event:/sfx/ui/clicks/ui_checkbox_on\"", source, StringComparison.Ordinal);
        Assert.Contains("ToggleOffSfx = \"event:/sfx/ui/clicks/ui_checkbox_off\"", source, StringComparison.Ordinal);
        Assert.Contains("SfxCmd.Play(pressed ? ToggleOnSfx : ToggleOffSfx, 0.85f)", source, StringComparison.Ordinal);

        Assert.DoesNotContain("ClearCell", source, StringComparison.Ordinal);
        Assert.DoesNotContain("RevealItem", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CellClicked", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddReward", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsHidden = false", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionQueueOrderSkipsExplicitReplacementSlots()
    {
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");

        Assert.Contains("if (transformation.Replacement != null)", patchSource, StringComparison.Ordinal);
        Assert.Contains("continue;", SliceFrom(patchSource, "if (transformation.Replacement != null)"), StringComparison.Ordinal);
        Assert.DoesNotContain(
            "queue.Enqueue(transformation.Replacement",
            patchSource,
            StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void VanillaTransformPreviewQueuesOnlyGeneratedTransformationSlots()
    {
        var vanillaSource = ReadLocalCoreText("Nodes", "Cards", "NTransformPreview.cs");

        Assert.Contains("cardTransformation.Replacement == null", vanillaSource, StringComparison.Ordinal);
        Assert.Contains("CycleThroughCards", SliceFrom(vanillaSource, "if (cardTransformation.Replacement == null)"), StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionDoesNotCreateRealCardsOrAdvanceRealRng()
    {
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPatch.cs");
        var displaySource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewCyclePatch.Display.cs");
        var queueSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPreviewPredictionQueue.cs");
        var predictionSource = ReadRepoText("EZMicroBalanceCode", "Preview", "TransformPredictionService.cs");
        var combined = patchSource + Environment.NewLine + displaySource + Environment.NewLine + queueSource + Environment.NewLine + predictionSource;

        Assert.Contains("TransformPredictionRngContext.TryConsume", patchSource, StringComparison.Ordinal);
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

        Assert.Contains("new Rng(snapshot.Seed, snapshot.Counter)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.TryGetValue(player", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Dictionary<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSelectionLifetimePatch", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("ClearContextWhenSelectionCompletes", lifetimeSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", lifetimeSource, StringComparison.Ordinal);
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

    [Fact]
    public void ConfigLocalizationContainsReadablePreviewRows()
    {
        var english = TestRepo.JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json");
        var simplifiedChinese = TestRepo.JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json");
        var requiredKeys = new[]
        {
            "SPIREPLUS.mod_title",
            "SPIREPLUS-CRYSTAL_SPHERE_PEEK_BUTTON.title",
            "SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.title",
            "SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.title",
            "SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.title",
            "SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.title",
            "SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title",
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
        Assert.Equal("Spire Plus", simplifiedChinese["SPIREPLUS.mod_title"]);
        Assert.Equal("预知", simplifiedChinese["SPIREPLUS-CRYSTAL_SPHERE_PEEK_BUTTON.title"]);
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
    public void PlayerFacingFuturePeekClaimsStayUiOnlyAndCardOnly()
    {
        var previewReadme = ReadRepoText("docs", "features", "preview-tools", "README.md");
        var compatibilityGoal = ReadRepoText("docs", "features", "future-peek", "goal.md");
        var websiteData = ReadRepoText("website", "content-data.js");
        var websiteApp = ReadRepoText("website", "app.js");
        var combinedPlayerText = string.Join(
            Environment.NewLine,
            previewReadme,
            compatibilityGoal,
            websiteData,
            websiteApp);

        AssertSourceContains(
            combinedPlayerText,
            "only changes the local",
            "%ScryMask",
            "must not reveal cells, spend charges, or grant rewards",
            "revealed as semi-transparent icons, allowing you to plan your flips perfectly without save-scumming.",
            "You can preview the result before making your choice, eliminating the gamble.",
            "displays the exact card you are guaranteed to receive.",
            "所有格子上的内容都会以半透明形式直接显现",
            "无需再靠存档读档来猜测",
            "右侧预览区域会直接显示你点击确认后将百分之百获得的卡牌",
            "彻底消除随机盲盒的赌博风险",
            "Map foresight and reward foresight are not implemented");

        foreach (var overclaim in new[]
                 {
                     "reveals all hidden items",
                     "reveal all hidden rewards instantly",
                     "cards/relics",
                     "card or relic",
                     "exact cards/relics",
                     "一键直接看透所有隐藏格",
                     "一键透视水晶球底下的全部物品",
                     "卡牌与遗物变换",
                     "卡牌或遗物",
                     "变化卡牌/遗物",
                     "直接看到变换后会得到什么牌或遗物"
                 })
        {
            Assert.DoesNotContain(overclaim, combinedPlayerText, StringComparison.OrdinalIgnoreCase);
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
