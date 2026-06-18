using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [ReleaseArtifactFact]
    public void PrivateBetaZipContainsOnlyInstallableActiveModFiles()
    {
        var packagePath = CurrentPackageZipPath();
        Assert.True(File.Exists(packagePath), $"Missing private beta package: {packagePath}");

        using var archive = ZipFile.OpenRead(packagePath);
        var entries = archive.Entries
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .Select(entry => entry.FullName.Replace('\\', '/'))
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "EZMicroBalance/EZMicroBalance.dll",
                "EZMicroBalance/EZMicroBalance.json",
                "EZMicroBalance/EZMicroBalance.pck",
                "EZMicroBalance/README_INSTALL.txt"
            ],
            entries);

        using var manifest = JsonDocument.Parse(ReadZipText(archive, "EZMicroBalance/EZMicroBalance.json"));
        Assert.Equal("EZMicroBalance", manifest.RootElement.GetProperty("id").GetString());
        Assert.Contains(
            manifest.RootElement.GetProperty("dependencies").EnumerateArray(),
            dependency => dependency.ValueKind == JsonValueKind.Object &&
                dependency.TryGetProperty("id", out var id) &&
                id.GetString() == "BaseLib" &&
                dependency.TryGetProperty("min_version", out var minVersion) &&
                minVersion.GetString() == "v3.1.4");

        var readme = ReadZipText(archive, "EZMicroBalance/README_INSTALL.txt");
        Assert.Contains("Spire Plus manual-test package", readme, StringComparison.Ordinal);
        Assert.Contains($"Archive: {CurrentPackageName()}.zip", readme, StringComparison.Ordinal);
        Assert.Contains("Display name: Spire Plus", readme, StringComparison.Ordinal);
        Assert.Contains("Technical compatibility id: EZMicroBalance", readme, StringComparison.Ordinal);
        Assert.Contains("Extract this archive into the Slay the Spire 2 mods folder exactly as packaged.", readme, StringComparison.Ordinal);
        Assert.Contains("If the game's Mods list shows EZMicroBalance as the mod name, the package is stale or the display-name route regressed.", readme, StringComparison.Ordinal);
        Assert.Contains("BaseLib", readme, StringComparison.Ordinal);
        Assert.Contains("EzDailyContent disabled or absent", readme, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance is a technical folder/id only; player-facing screens should say Spire Plus.", readme, StringComparison.Ordinal);
        Assert.Contains("Ancient selections now grant visible marker relics", readme, StringComparison.Ordinal);
        Assert.Contains("manual-test build, not release-ready", readme, StringComparison.Ordinal);
        Assert.Contains("Save/load, death/failure paths, and co-op still need manual proof", readme, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("source-safe", readme, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Found 16 SavedSpireFields", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            allSource,
            "[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.AfterObtained))",
            "ModPatchTarget(typeof(RelicCmd), nameof(RelicCmd.Obtain)",
            "[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]",
            "[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))]",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))",
            "ModPatchTarget(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.AfterCreated))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.FromSerializable))",
            "ModPatchTarget(typeof(Debt), nameof(Debt.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), nameof(Debt.HasTurnEndInHandEffect), MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), \"OnTurnEndInHand\")",
            "ModPatchTarget(typeof(CardModel), \"OnPlay\")",
            "ModPatchTarget(typeof(CardCmd), nameof(CardCmd.Exhaust))",
            "[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]",
            "[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]",
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]",
            "ModPatchTarget(typeof(DistinguishedCape), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Vakuu), \"GenerateInitialOptions\")",
            "ModPatchTarget(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_DisplayAmount\")]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]",
            "[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]",
            "[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]",
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))",
            "[HarmonyPatch(typeof(CookRestSiteOption), \"get_IsEnabled\")]",
            "[HarmonyPatch(typeof(CookRestSiteOption), \"get_Description\")]",
            "[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]",
            "[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]",
            "[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]",
            "[HarmonyPatch(typeof(Folly), \"get_CanonicalKeywords\")]",
            "ModPatchTarget(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))",
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "ModPatchTarget(typeof(Fiddle), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ShouldDraw))",
            "ModPatchTarget(typeof(CardPileCmd), nameof(CardPileCmd.Draw)",
            "[HarmonyPatch(typeof(IronClub), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BrilliantScarf), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))");
    }

    private static string ReadSereTalonVisualSource() =>
        string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"));

    [Fact]
    public void AncientDirectDeckGainFeedbackFlashesSourceRelicAndCardPreview()
    {
        var feedbackSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "SpirePlusFeedback.cs");
        var ancientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            feedbackSource,
            "RelicTriggerSfx = \"event:/sfx/ui/relic_activate_general\"",
            "sourceRelic.Flash()",
            "NRelicFlashVfx.Create(sourceRelic)",
            "AboveTopBarVfxContainer.AddChildSafely(flashVfx)",
            "public static void ConfirmRelicPayoff(RelicModel? sourceRelic)",
            "models.Insert(0, sourceRelic)",
            "CardCmd.PreviewCardPileAdd(successfulAdds, seconds)",
            "NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short)");

        AssertSourceContains(
            ancientSource,
            "SpirePlusFeedback.PreviewDeckAdds(results, paelsHorn, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, jewelryBox, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, cape, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, sealOfGold, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, paelsTooth)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResults, player.GetRelic<UrdaMoltingOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedbedOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaTrialBranchOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedBankOptionRelic>(), 2f)",
            "SpirePlusFeedback.ConfirmRelicPayoff(eliteRoot)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<MorviForbiddenLoanOptionRelic>(), 2f)");
    }

    [Fact]
    public void JeweledMaskCustomEnchantmentIsPowerOnlyPersistentAndCombatStartScoped()
    {
        var enchantment = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var combatSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            enchantment,
            "CustomEnchantmentModel, ILocalizationProvider",
            "public override bool HasExtraCardText => true",
            "return cardType == CardType.Power",
            "Card.EnergyCost.SetCustomBaseCost(0)");

        AssertSourceContains(
            pickupSource,
            "card => card.Type == CardType.Power && card.Enchantment == null",
            "DraftGeneratedPowerForJeweledMask(owner)",
            "owner.RunState.RemoveCard(unselected)",
            "await CardPileCmd.Add(selected, PileType.Deck)",
            "CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m)");

        AssertSourceContains(
            combatSource,
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "combatState.RoundNumber > 1",
            "AncientCardHelpers.IsJeweledMaskPower",
            "await CardPileCmd.Add(markedPower, PileType.Hand)",
            "marked power already in hand",
            "no marked power in draw pile or hand");

        Assert.Contains("Jeweled Mask free power", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Permanent 0-cost enchantment survives save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void JewelryBoxApotheosisMarkerIsScopedToCreatedCardsAndHoverPreviews()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");

        AssertSourceContains(
            source,
            "CreateNonInnateApotheosis(jewelryBox.Owner)",
            "JewelryBoxApotheosisMarker.Mark(result.cardAdded)",
            "ConditionalWeakTable<CardModel, MarkerState>",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true",
            "if (card is not Apotheosis)",
            "AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate)",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "JewelryBoxApotheosisMarker.IsMarked(__instance)",
            "keyword => keyword != CardKeyword.Innate",
            "CreateNonInnateApotheosisHoverTips",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]");
    }

    [Fact]
    public void PaelsToothSavedCounterAndStoredCardReturnAreGuarded()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<PaelsTooth, int>",
            "EZMicroBalanceNonBossCombatCounter");

        AssertSourceContains(
            source,
            "AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "if (paelsTooth.Owner.Creature.IsDead)",
            "if (paelsTooth.SerializableCards.Count == 0)",
            "if (room.RoomType == RoomType.Boss)",
            "ClearStoredCards(paelsTooth, \"act boss combat ended\")",
            "var counter = AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] + 1",
            "if (counter < 2)",
            "ChooseAndReturnStoredCard(paelsTooth)",
            "CardModel.FromSerializable(savedCard)",
            "CardSelectCmd.FromChooseABundleScreen",
            "CardCmd.Upgrade(selected, CardPreviewStyle.MessyLayout)",
            "paelsTooth.SerializableCards.Remove(selectedPreview.Saved)",
            "ClearStoredCards(paelsTooth, \"act transition\")");

        Assert.Contains("| Pael's Tooth |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Pael's Tooth stored cards", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Stored removed cards and combat counter survive save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DebtAndFollyPlayerTextMatchSourceBehavior()
    {
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.Equal("Debt", cards["DEBT.title"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
        Assert.DoesNotContain("turn", cards["DEBT.description"], StringComparison.OrdinalIgnoreCase);

        Assert.Equal("Folly", cards["FOLLY.title"]);
        Assert.Equal("Unplayable. Innate. Eternal.", cards["FOLLY.description"]);
        Assert.DoesNotContain("Ethereal", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Retain", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            debtSource,
            "DebtFromSavePatch",
            "__result = new CardKeyword[] { CardKeyword.Exhaust }",
            "__result = new DynamicVar[] { new GoldVar(5) }",
            "DebtTurnEndEffectPatch",
            "__result = false",
            "DebtTurnEndInHandPatch",
            "__result = Task.CompletedTask",
            "debt.ExhaustOnNextPlay = true",
            "Math.Min(5, debt.Owner.Gold)",
            "PlayerCmd.LoseGold(goldToLose, debt.Owner)");

        AssertSourceContains(
            vakuSource,
            "new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4)",
            "AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain)",
            "__result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate }");
    }

    [Fact]
    public void TemporaryGeneratedCardPathsCleanUpOrSelfExpire()
    {
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            turnSource,
            "CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration)",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "__result = Task.CompletedTask",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true)",
            "if (selected != topCard)",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "PowerCmd.Apply<StrengthPower>");

        AssertSourceContains(
            vakuSource,
            "var copy = cardPlay.Card.CreateClone()",
            "AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1)",
            "AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner)",
            "ConditionalWeakTable<MusicBox, State>",
            "MusicBoxStateTracker.MarkUsed(musicBox)",
            "MusicBoxStateTracker.Reset(__instance)");

        Assert.Contains("skipped card does not linger", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Top draw-pile card can be exhausted for Strength or kept.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Creates a discounted Ethereal Exhaust copy.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void MeatCleaverCookRestSiteOptionIsSafeAndScoped()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "MeatCleaverCookPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var staticHovers = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");
        var zhsStaticHovers = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Equal("Adds a [gold]Cleaver[/gold] option to rest sites: remove [blue]2[/blue] cards and lose [blue]5[/blue] HP.", relics["MEAT_CLEAVER.description"]);
        Assert.Equal("Cleaver", restSite["OPTION_COOK.name"]);
        Assert.Equal("Remove 2 cards. Lose 5 HP.", restSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("Requires at least 2 removable cards and more than 5 HP.", restSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("At a [gold]Rest Site[/gold], [gold]remove[/gold] [blue]2[/blue] cards from your [gold]Deck[/gold] and lose [blue]5[/blue] HP.", staticHovers["COOK.description"]);
        Assert.Equal("Cleaver", staticHovers["COOK.title"]);
        Assert.Equal("\u5728\u4f11\u606f\u5904\u52a0\u5165[gold]\u5207\u8089[/gold]\u9009\u9879\uff1a\u79fb\u9664[blue]2[/blue]\u5f20\u724c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsRelics["MEAT_CLEAVER.description"]);
        Assert.Equal("\u5207\u8089", zhsRestSite["OPTION_COOK.name"]);
        Assert.Equal("\u79fb\u96642\u5f20\u724c\u3002\u5931\u53bb5\u70b9\u751f\u547d\u3002", zhsRestSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("\u9700\u8981\u81f3\u5c112\u5f20\u53ef\u79fb\u9664\u724c\u4e14\u751f\u547d\u503c\u5927\u4e8e5\u3002", zhsRestSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("\u5728[gold]\u4f11\u606f\u5904[/gold]\u4ece\u4f60\u7684[gold]\u724c\u7ec4[/gold]\u4e2d[gold]\u79fb\u9664[/gold][blue]2[/blue]\u5f20\u724c\uff0c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsStaticHovers["COOK.description"]);
        Assert.Equal("\u5207\u8089", zhsStaticHovers["COOK.title"]);
        Assert.DoesNotContain("gain [green]9[/green] Max HP", staticHovers["COOK.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u83b7\u5f97[green]9[/green]\u70b9\u6700\u5927\u751f\u547d", zhsStaticHovers["COOK.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Cooking", relics["MEAT_CLEAVER.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRelics["MEAT_CLEAVER.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRestSite["OPTION_COOK.name"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsStaticHovers["COOK.title"], StringComparison.Ordinal);

        AssertSourceContains(
            source,
            "public const int CardsToRemove = 2",
            "public const int HpToLose = 5",
            "owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner)",
            "__result = false",
            "if (owner.GetRelic<MeatCleaver>() == null)",
            "__instance.IsEnabled ? \"OPTION_COOK.ezDescription\" : \"OPTION_COOK.ezDescriptionDisabled\"",
            "owner.Creature.CurrentHp > HpToLose",
            "Cards.Count(card => card.IsRemovable) >= CardsToRemove",
            "Cancelable = true",
            "RequireManualConfirmation = true",
            "if (cards.Count != CardsToRemove)",
            "await CardPileCmd.RemoveFromDeck(card)",
            "await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose)");

        Assert.DoesNotContain("MaxHp", source, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when too few removable cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when HP is not greater than 5.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Expected: Cleaver / \u5207\u8089 option removes 2 removable cards and costs 5 HP.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify no other rest-site source is affected unexpectedly.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Expected: Cook option", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u70f9\u996a", manualChecklist, StringComparison.Ordinal);
    }

}
