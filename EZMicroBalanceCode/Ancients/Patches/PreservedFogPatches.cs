namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]
internal static class PreservedFogPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PreservedFog __instance, ref Task __result)
    {
        __result = RemoveFourCardsAndAddPersistentFolly(__instance);
        return false;
    }

    private static async Task RemoveFourCardsAndAddPersistentFolly(PreservedFog preservedFog)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4);
        foreach (var card in await CardSelectCmd.FromDeckForRemoval(preservedFog.Owner, prefs))
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        var folly = preservedFog.Owner.RunState.CreateCard<Folly>(preservedFog.Owner);
        AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain);
        var result = await CardPileCmd.Add(folly, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] PreservedFog applied: removed up to 4 cards and added Folly without Ethereal/Retain.");
    }
}

[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]
internal static class FollyKeywordsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate };
        return false;
    }
}
