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
            "IPatchMethod.PatchId => \"jewelry-box-extra-hover-tips\"",
            "ModPatchTarget(typeof(JewelryBox), \"ExtraHoverTips\", MethodType.Getter)",
            "IPatchMethod.PatchId => \"jewelry-box-hover-tips\"",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)",
            "IPatchMethod.PatchId => \"jewelry-box-hover-tips-excluding-relic\"",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTipsExcludingRelic), MethodType.Getter)");
    }
}
