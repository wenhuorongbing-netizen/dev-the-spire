namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]
internal static class JewelryBoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(JewelryBox __instance, ref Task __result)
    {
        __result = AddNonInnateApotheosis(__instance);
        return false;
    }

    private static async Task AddNonInnateApotheosis(JewelryBox jewelryBox)
    {
        var card = CreateNonInnateApotheosis(jewelryBox.Owner);
        var result = await CardPileCmd.Add(card, PileType.Deck, clonedBy: jewelryBox);
        JewelryBoxApotheosisMarker.Mark(result.cardAdded);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] JewelryBox applied: added Apotheosis without Innate.");
    }

    public static CardModel CreateNonInnateApotheosis(Player owner)
    {
        var card = owner.RunState.CreateCard<Apotheosis>(owner);
        JewelryBoxApotheosisMarker.Mark(card);
        return card;
    }

    public static IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> CreateNonInnateApotheosisHoverTips()
    {
        var preview = ModelDb.Card<Apotheosis>().ToMutable();
        JewelryBoxApotheosisMarker.Mark(preview);
        return new[] { MegaCrit.Sts2.Core.HoverTips.HoverTipFactory.FromCard(preview) }.Concat(preview.HoverTips);
    }
}

[HarmonyPatch(typeof(Apotheosis), "get_CanonicalKeywords")]
internal static class JewelryBoxApotheosisCanonicalKeywordsPatch
{
    [HarmonyPostfix]
    private static void RemoveInnateForMarkedJewelryBoxApotheosis(Apotheosis __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (JewelryBoxApotheosisMarker.IsMarked(__instance))
        {
            __result = __result.Where(keyword => keyword != CardKeyword.Innate).ToArray();
        }
    }
}

internal static class JewelryBoxApotheosisMarker
{
    private sealed class MarkerState;

    private static readonly ConditionalWeakTable<CardModel, MarkerState> MarkedCards = new();

    public static void Mark(CardModel card)
    {
        if (card is not Apotheosis)
        {
            return;
        }

        MarkedCards.GetValue(card, _ => new MarkerState());
        AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true;
        AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate);
    }

    public static bool IsMarked(CardModel card)
    {
        return card is Apotheosis &&
            (MarkedCards.TryGetValue(card, out _) || AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]);
    }
}

[HarmonyPatch(typeof(JewelryBox), "get_ExtraHoverTips")]
internal static class JewelryBoxExtraHoverTipsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]
internal static class JewelryBoxHoverTipsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not JewelryBox)
        {
            return true;
        }

        __result = new MegaCrit.Sts2.Core.HoverTips.IHoverTip[] { __instance.HoverTip }
            .Concat(JewelryBoxPatch.CreateNonInnateApotheosisHoverTips());
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]
internal static class JewelryBoxHoverTipsExcludingRelicPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not JewelryBox)
        {
            return true;
        }

        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}
