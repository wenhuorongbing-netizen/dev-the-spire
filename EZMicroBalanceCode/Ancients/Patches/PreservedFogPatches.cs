using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PreservedFogPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "preserved-fog-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Replace Preserved Fog pickup with remove-four-cards plus persistent Folly";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))];

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
        SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f);
        MainFile.Logger.Info("[Spire Plus] PreservedFog applied: removed up to 4 cards and added Folly without Ethereal/Retain.");
    }
}

internal sealed class FollyKeywordsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "preserved-fog-folly-keywords";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Make Preserved Fog's Folly persistent by overriding canonical keywords";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Folly), "CanonicalKeywords", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate };
        return false;
    }
}
