using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void PickupRewardCompensationAndLockoutPatchesStayScoped()
    {
        var hornSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PaelsHornPhase1Patch.cs");
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var pickupDispatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PickupRewardPatches.cs");
        var sereTalonVisualSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"));
        var tanxClawsTuningSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TanxClawsMaulTuningPatches.cs");
        var sealSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SealOfGoldPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        AssertSourceContains(
            hornSource,
            "owner.RunState.CreateCard<Relax>(owner)",
            "CardCmd.Upgrade(upgradedRelax)",
            "await CardPileCmd.Add(normalRelax, PileType.Deck)",
            "await CardPileCmd.Add(upgradedRelax, PileType.Deck)");

        AssertSourceContains(
            pickupSource,
            "if (blackStar.Owner.RunState.CurrentActIndex < 2)",
            "RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable()",
            "await RelicCmd.Obtain(relic, blackStar.Owner)",
            "new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2)",
            "CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)",
            "CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)",
            "SozuPotionGatePatch.BeginInitialPotionFill(sozu.Owner)",
            "SozuPotionGatePatch.EndInitialPotionFill(sozu.Owner)",
            "while (sozu.Owner.HasOpenPotionSlots)",
            "PotionFactory.CreateRandomPotionOutOfCombat",
            "PotionCmd.TryToProcure(potion, sozu.Owner)",
            "if (InitialPotionFillOwners.Contains(player) && player == __instance.Owner)",
            "EctoplasmGoldGatePatch.BeginInitialGold(ectoplasm.Owner)",
            "await PlayerCmd.GainGold(250m, ectoplasm.Owner)",
            "EctoplasmGoldGatePatch.EndInitialGold(ectoplasm.Owner)",
            "if (InitialGoldOwners.Contains(player) && player == __instance.Owner)",
            "for (var i = 0; i < 2; i++)",
            "sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner)",
            "DebtCardPatch.ConfigureDebt(debt)",
            "CardPileCmd.Add(debt, PileType.Deck)");
        Assert.DoesNotContain("case Claws", pickupDispatch, StringComparison.Ordinal);
        AssertSourceContains(
            pickupSource,
            "[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]",
            "private const int CurseOfferCount = 4",
            "private const int CursePickCount = 1",
            "private const int NormalWishCount = 2",
            "private const int UpgradedWishCount = 1",
            "CardSelectCmd.FromSimpleGrid",
            "new BlockingPlayerChoiceContext()",
            "SERE_TALON.selectionScreenPrompt",
            "Cancelable = false",
            "RequireManualConfirmation = true",
            "AncientCardHelpers.RemoveUnpiledRunCard(curse)",
            "CardCmd.Upgrade(wish, CardPreviewStyle.None)");
        AssertSourceContains(
            tanxClawsTuningSource,
            "[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]",
            "CardSelectCmd.FromDeckForTransformation",
            "owner.RunState.CreateCard<Maul>(owner)",
            "maul.UpgradeInternal()",
            "CardCmd.Upgrade(maul, CardPreviewStyle.None)",
            "CardCmd.Transform(transformations, owner.PlayerRng.Transformations)");
        Assert.DoesNotContain("SereTalon", tanxClawsTuningSource, StringComparison.Ordinal);
        AssertSourceContains(
            sereTalonVisualSource,
            "get_IconPath",
            "get_PackedIconPath",
            "get_PackedIconOutlinePath",
            "get_BigIconPath",
            "get_Icon",
            "get_IconOutline",
            "get_BigIcon",
            "NEventOptionButton._Ready",
            "NRelic keeps its IconSize private",
            "Outline.Visible is",
            "Ancient option buttons assign the relic icon directly during _Ready()",
            "Relic-bar and inspect nodes can reload after the model texture getters",
            "TryApply",
            "TryApplyPath",
            "TryApplyEventOptionButton",
            "TryApplyRelicNode",
            "IsNodeReady()",
            "InvalidOperationException",
            "CanUsePath(iconPath)",
            "ResourceLoader.Exists(iconPath)",
            "TryApplyPackedTexture",
            "LoadPackedTexture",
            "TryApplyTexture",
            "ResourceLoader.Load<Texture2D>(SereTalonVisualAssetPaths.PackedIcon",
            "GetNodeOrNull<TextureRect>(\"%RelicIcon\")",
            "button.Option?.Relic is not SereTalon",
            "PreloadManager.Cache.GetTexture2D(iconPath)",
            "texture is null",
            "SereTalonVisualRouteLog.SkippedPathOnce(iconPath, \"texture did not load\")",
            "Vakuu Sere Talon Ancient option icon route skipped",
            "resource path does not exist",
            "icon route skipped because",
            "relic is not SereTalon",
            "sere_talon_spire_plus.png",
            "RelicModel packed icon path",
            "RelicModel big icon path",
            "RelicModel packed icon texture",
            "RelicModel big icon texture",
            "Ancient event option button",
            "Vakuu Sere Talon icon route active");
        Assert.DoesNotContain("__instance is Claws", sereTalonVisualSource, StringComparison.Ordinal);
        Assert.True(
            File.Exists(RepoPath("EZMicroBalance", "images", "relics", "sere_talon_spire_plus.png")),
            "Sere Talon needs a Spire Plus-owned in-game icon so it is not visually confused with Tanx Claws.");
        Assert.True(
            File.Exists(RepoPath("EZMicroBalance", "images", "relics", "big", "sere_talon_spire_plus.png")),
            "Sere Talon needs a separate big relic icon for inspect/hover surfaces.");

        AssertSourceContains(
            sealSource,
            "__result += sealOfGold.DynamicVars.Energy.BaseValue",
            "__result = Task.CompletedTask");

        Assert.Contains("immediately obtain 1 random Relic", relics["BLACK_STAR.description"], StringComparison.Ordinal);
        Assert.Contains("fill all empty Potion slots", relics["SOZU.description"], StringComparison.Ordinal);
        Assert.Contains("gain 250 Gold", relics["ECTOPLASM.description"], StringComparison.Ordinal);
        Assert.Contains("Add 2 playable Debt", relics["SEAL_OF_GOLD.description"], StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void CoreAncientRelicRoutesAndIconSurfacesStayCompatible()
    {
        var vakuuEventSource = ReadLocalCoreText("Models", "Events", "Vakuu.cs");
        var tanxEventSource = ReadLocalCoreText("Models", "Events", "Tanx.cs");
        var sereTalonSource = ReadLocalCoreText("Models", "Relics", "SereTalon.cs");
        var clawsSource = ReadLocalCoreText("Models", "Relics", "Claws.cs");
        var coreNRelicSource = ReadLocalCoreText("Nodes", "Relics", "NRelic.cs");
        var coreEventOptionButtonSource = ReadLocalCoreText("Nodes", "Events", "NEventOptionButton.cs");
        var coreRelicRewardSource = ReadLocalCoreText("Rewards", "RelicReward.cs");
        var coreInspectRelicScreenSource = ReadLocalCoreText("Nodes", "Screens", "InspectScreens", "NInspectRelicScreen.cs");

        AssertSourceContains(vakuuEventSource, "RelicOption<SereTalon>()");
        Assert.DoesNotContain("RelicOption<Claws>()", vakuuEventSource, StringComparison.Ordinal);
        AssertSourceContains(tanxEventSource, "RelicOption<Claws>()");
        Assert.DoesNotContain("RelicOption<SereTalon>()", tanxEventSource, StringComparison.Ordinal);
        AssertSourceContains(
            sereTalonSource,
            "public sealed class SereTalon : RelicModel",
            "new DynamicVar(\"Curses\", 2m)",
            "new DynamicVar(\"Wishes\", 3m)",
            "base.Owner.RunState.Rng.Niche.NextItem(availableCurses)",
            "base.Owner.RunState.CreateCard(ModelDb.Card<Wish>(), base.Owner)");
        AssertSourceContains(
            clawsSource,
            "public sealed class Claws : RelicModel",
            "new CardsVar(6)",
            "HoverTipFactory.FromCardWithCardHoverTips<Maul>()",
            "new CardTransformation(c, CreateMaulFromOriginal(c, forPreview: true))",
            "base.Owner.RunState.CreateCard<Maul>(base.Owner)",
            "CardCmd.Transform(transformations, base.Owner.PlayerRng.Transformations)");
        AssertSourceContains(
            coreEventOptionButtonSource,
            "Option.Relic.Icon",
            "Option.Relic.IconOutline",
            "GetNode<TextureRect>(\"%RelicIcon\")");
        AssertSourceContains(
            coreNRelicSource,
            "case IconSize.Small:",
            "Icon.Texture = Model.Icon",
            "Outline.Visible = true",
            "Outline.Texture = Model.IconOutline",
            "case IconSize.Large:",
            "Icon.Texture = Model.BigIcon",
            "Outline.Visible = false");
        AssertSourceContains(
            coreRelicRewardSource,
            "public override TextureRect CreateIcon()",
            "textureRect.Texture = _relic.BigIcon");
        AssertSourceContains(
            coreInspectRelicScreenSource,
            "_relicImage.Texture = relicModel.BigIcon");
    }

    [Fact]
    public void DraftAndGeneratedCardFlowsRemoveUnselectedTemporaryCards()
    {
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.DoesNotContain("CreateSereTalonCurseDraft", pickupSource, StringComparison.Ordinal);

        AssertSourceContains(
            vakuSource,
            "combatState.RoundNumber != 1",
            "ModelDb.AllCharacterCardPools",
            "ModelDb.CardPool<ColorlessCardPool>()",
            ".Where(IsChoicesParadoxEligibleRare)",
            ".Distinct()",
            "CardFactory.GetDistinctForCombat",
            "CardCmd.ApplyKeyword(card, CardKeyword.Retain)",
            "foreach (var card in generated.Where(card => card != selected))",
            "combatState.RemoveCard(card)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player)",
            "card.Rarity == CardRarity.Rare",
            "card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest",
            "!card.Keywords.Contains(CardKeyword.Unplayable)",
            "card.CanBeGeneratedInCombat",
            "card.CanBeGeneratedByModifiers");

        AssertSourceContains(
            debtSource,
            "case Enthralled enthralled:",
            "await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m",
            "DebtCardPatch.ConfigureDebt(debt)",
            "debt.RemoveKeyword(CardKeyword.Unplayable)",
            "debt.AddKeyword(CardKeyword.Exhaust)",
            "debt.EnergyCost.SetCustomBaseCost(1)");

        Assert.Equal("If this is in your hand, you must play it before other cards. Gain 10 Block. Eternal.", cards["ENTHRALLED.description"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
    }
}
