using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
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
}
