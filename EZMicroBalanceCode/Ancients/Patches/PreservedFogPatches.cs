using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PreservedFogPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "p-r-e-s-e-r-v-e-d-f-o-g-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch PreservedFog.AfterObtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))];
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
        SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f);
        MainFile.Logger.Info("[Spire Plus] PreservedFog applied: removed up to 4 cards and added Folly without Ethereal/Retain.");
    }
}

internal sealed class FollyKeywordsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "f-o-l-l-y-k-e-y-w-o-r-d-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch Folly.get_CanonicalKeywords";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Folly), "get_CanonicalKeywords", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate };
        return false;
    }
}


