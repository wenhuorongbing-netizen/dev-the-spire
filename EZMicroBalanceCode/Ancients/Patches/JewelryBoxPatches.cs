namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

using STS2RitsuLib.Patching.Models;

internal sealed class JewelryBoxPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "jewelry-box-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add Jewelry Box's non-Innate Apotheosis to the deck on pickup";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))];

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
        SpirePlusFeedback.PreviewDeckAdds(result, jewelryBox, 2f);
        MainFile.Logger.Info("[Spire Plus] JewelryBox applied: added Apotheosis without Innate.");
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

internal sealed class JewelryBoxApotheosisCanonicalKeywordsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "jewelry-box-apotheosis-keywords";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Remove Innate from Apotheosis cards created by Jewelry Box";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Apotheosis), "CanonicalKeywords", MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(Apotheosis __instance, ref IEnumerable<CardKeyword> __result)
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

internal sealed class JewelryBoxExtraHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "jewelry-box-extra-hover-tips";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show Jewelry Box's non-Innate Apotheosis preview in relic extra hover tips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JewelryBox), "ExtraHoverTips", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}

internal sealed class JewelryBoxHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "jewelry-box-hover-tips";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Include Jewelry Box's non-Innate Apotheosis preview in full relic hover tips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)];

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

internal sealed class JewelryBoxHoverTipsExcludingRelicPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "jewelry-box-hover-tips-excluding-relic";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show only Jewelry Box's Apotheosis preview in option hover surfaces";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTipsExcludingRelic), MethodType.Getter)];

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
