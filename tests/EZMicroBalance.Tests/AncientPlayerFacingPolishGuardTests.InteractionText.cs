using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
{
    [Fact]
    public void AncientOptionHoversPreviewNamedAddedCardsWhereSupported()
    {
        var urda = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaMapUiPatches = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapClickPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightMapPreviewVisuals.cs"),
            ReadRepoText("EZMicroBalanceCode", "Map", "SpirePlusMapPointHoverComposer.cs"));
        var morvi = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var vakuu = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");

        AssertSourceContains(
            urda,
            "HoverTipFactory.FromCard<UrdaSeedbed>()",
            "HoverTipFactory.FromCard<WitheredHusk>()",
            "RootSightHoverTips",
            "EZMB_URDA.root_sight.hover.title",
            "EZMB_URDA.root_sight.hover.description");
        Assert.DoesNotContain("HoverTipFactory.FromCardWithCardHoverTips<UrdaSeedbed>()", urda, StringComparison.Ordinal);
        Assert.DoesNotContain("HoverTipFactory.FromCardWithCardHoverTips<WitheredHusk>()", urda, StringComparison.Ordinal);
        AssertSourceContains(
            urdaMapUiPatches,
            "%QuestIcon",
            "MouseFilterEnum.Ignore",
            "SpirePlusMapPointHoverComposer",
            "UrdaBlessingService.TryGetRootSightHoverTip",
            "FiremarkedEliteMapHoverPatch.TryCreateHoverTip",
            "BannerRoomMapHoverPatch.TryCreateHoverTip",
            "TryGetRootSightPreviewRoomType",
            "UrdaRootSightMapPreviewIconPatch",
            "UrdaRootSightMapQuestIconPatch",
            "UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon",
            "UrdaRootSightMapPreviewVisuals.ApplyQuestIcon",
            "ApplyRootSightOverlay(pointNode, hasRootSightMarker || canTargetWithRootSight)",
            "UnknownIconPath(roomType)",
            "UnknownOutlinePath(roomType)",
            "NHoverTipSet.Remove(__instance)",
            "NHoverTipSet.CreateAndShow",
            "UrdaRootSightMapPointClickPatch",
            "HarmonyPatch(typeof(NMapPoint), \"OnRelease\")",
            "UrdaRootSightDisabledMapPointClickPatch",
            "HarmonyPatch(typeof(NClickableControl), nameof(NClickableControl._GuiInput))",
            "__instance is not NMapPoint mapPoint",
            "InputEventMouseButton { ButtonIndex: MouseButton.Left }",
            "__instance.GetViewport()?.SetInputAsHandled()",
            "UrdaBlessingService.TryCommitRootSightSelection");
        AssertSourceContains(
            morvi,
            "HoverTipFactory.FromCardWithCardHoverTips<MorviRedInkOverdraftCard>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDrawPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveVeilPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBurnPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDiscountPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBraveryPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDexterityPage>()",
            "HoverTipFactory.FromCardWithCardHoverTips<MorviWastePaper>()");
        AssertSourceContains(
            vakuu,
            "HoverTipFactory.FromCardWithCardHoverTips<VakuuTemptation>()");
    }

    [Fact]
    public void UrdaSeedBankTextMatchesNoTrialPlantMarkerSource()
    {
        var seedBankSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionCommit.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionGuard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaSeedBankOptionRelic.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelicClickPatch.cs"));
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.DoesNotContain("UrdaTrialPlantCard", seedBankSource, StringComparison.Ordinal);
        AssertSourceContains(
            seedBankSource,
            "if (cards.Count == 0)",
            "SeedBankCardIds = string.Empty",
            "SeedBankSettled = true",
            "RefreshSeedBankRelicStatus(player)",
            "var addedCount = 0",
            "var failedSelectedIds = new List<string>()",
            "failedSelectedIds.Add(card.Id.ToString())",
            "SeedBankCardIds = string.Join(\",\", failedSelectedIds.Take(SeedBankMaxSeeds))",
            "finally",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "CreateStoredSeedsHoverTip",
            "storedSeeds.descriptionPrefix",
            "storedSeeds.descriptionFooter",
            "Seed Bank extraction preserved");
        Assert.DoesNotContain("HoverTipFactory.FromCard(card)", seedBankSource, StringComparison.Ordinal);
        Assert.DoesNotContain(".Concat(card.HoverTips)", seedBankSource, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "save up to [blue]3[/blue] cards",
            "the first is upgraded",
            "Click this relic later");
        AssertSourceContains(
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            "最多[blue]3[/blue]张",
            "第一张会升级");

        foreach (var value in new[]
        {
            engAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            zhsAncients["EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description"],
            engRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"],
            zhsRelics["EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description"]
        })
        {
            Assert.DoesNotContain("Trial Plant", value, StringComparison.Ordinal);
            Assert.DoesNotContain("试炼种植", value, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VakuuFightTextAndSourceStayExplicitAboutRiskAndRewards()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var entry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var victory = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictory.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.DoesNotContain("TaskHelper.RunSafely", patch, StringComparison.Ordinal);
        Assert.Contains("EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)", entry, StringComparison.Ordinal);
        Assert.Contains("ClearEventNode(vakuu)", entry, StringComparison.Ordinal);
        Assert.Contains("EventNodeBackingField", entry, StringComparison.Ordinal);
        Assert.Contains("CreateVictoryFallbackOption", victory, StringComparison.Ordinal);
        Assert.Contains("VictoryFallbackDescriptionKey", victory, StringComparison.Ordinal);
        Assert.Contains("targetChoiceCount = encounter.VictoryChoiceCount", victory, StringComparison.Ordinal);
        Assert.Contains("encounter.VictoryGold", victory, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch + entry + victory, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", patch + entry, StringComparison.Ordinal);
        Assert.Contains("public override RoomType RoomType => RoomType.Monster", encounter, StringComparison.Ordinal);
        Assert.Contains("ShouldGiveRewards => false", encounter, StringComparison.Ordinal);
        Assert.Contains("CustomEncounterScenePath => VakuuFightAssetPaths.EncounterScene", encounter, StringComparison.Ordinal);
        Assert.Contains("Slots => [VakuuSlot]", encounter, StringComparison.Ordinal);
        Assert.Contains("ModelDb.Monster<EzmbVakuuTrialMonster>()", encounter, StringComparison.Ordinal);
        Assert.Contains("runState.Players.Count == 1", gate, StringComparison.Ordinal);
        Assert.Contains("ShouldEnableFight", gate, StringComparison.Ordinal);
        Assert.Contains("EZMB_ENABLE_VAKUU_FIGHT", gate, StringComparison.Ordinal);

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "Fight Vakuu",
            "greed trial",
            "No normal combat rewards",
            "[gold]Contracts[/gold]",
            "cash out",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "Death ends the run");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "与瓦库进行赃物试炼",
            "本场没有普通战斗奖励",
            "[gold]契约[/gold]",
            "收手",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "若死亡会直接结束本局。");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "Fight Vakuu",
            "greed trial",
            "[gold]Contracts[/gold]",
            "cash out",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "No normal combat rewards",
            "Death ends the run");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "与瓦库进行赃物试炼",
            "[gold]契约[/gold]",
            "收手",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "本场没有普通战斗奖励",
            "若死亡会直接结束本局。");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.title");
        AssertNonEmpty(engAncients, zhsAncients, "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.description");
    }
}
