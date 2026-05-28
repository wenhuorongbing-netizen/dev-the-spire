using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class JewelryBoxPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "j-e-w-e-l-r-y-b-o-x-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch JewelryBox.AfterObtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))];
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
    static string IPatchMethod.PatchId => "j-e-w-e-l-r-y-b-o-x-a-p-o-t-h-e-o-s-i-s-c-a-n-o-n-i-c-a-l-k-e-y-w-o-r-d-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch Apotheosis.get_CanonicalKeywords";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Apotheosis), "get_CanonicalKeywords", HarmonyLib.MethodType.Getter)];
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

internal sealed class JewelryBoxExtraHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "j-e-w-e-l-r-y-b-o-x-e-x-t-r-a-h-o-v-e-r-t-i-p-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch JewelryBox.get_ExtraHoverTips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JewelryBox), "get_ExtraHoverTips", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}

internal sealed class JewelryBoxHoverTipsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "j-e-w-e-l-r-y-b-o-x-h-o-v-e-r-t-i-p-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_HoverTips";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_HoverTips", HarmonyLib.MethodType.Getter)];
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

internal sealed class JewelryBoxHoverTipsExcludingRelicPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "j-e-w-e-l-r-y-b-o-x-h-o-v-e-r-t-i-p-s-e-x-c-l-u-d-i-n-g-r-e-l-i-c-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_HoverTipsExcludingRelic";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_HoverTipsExcludingRelic", HarmonyLib.MethodType.Getter)];
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


