namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(CookRestSiteOption), "get_IsEnabled")]
internal static class MeatCleaverCookIsEnabledPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref bool __result)
    {
        var owner = MeatCleaverCookPatch.GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner))
        {
            __result = false;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), "get_Description")]
internal static class MeatCleaverCookDescriptionPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref LocString __result)
    {
        var owner = MeatCleaverCookPatch.GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = new LocString(
            "rest_site_ui",
            __instance.IsEnabled ? "OPTION_COOK.ezDescription" : "OPTION_COOK.ezDescriptionDisabled");
        __result.Add("Cards", MeatCleaverCookPatch.CardsToRemove);
        __result.Add("Hp", MeatCleaverCookPatch.HpToLose);
        return false;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]
internal static class MeatCleaverCookPatch
{
    public const int CardsToRemove = 2;

    public const int HpToLose = 5;

    private static readonly System.Reflection.MethodInfo OwnerGetter =
        AccessTools.PropertyGetter(typeof(RestSiteOption), "Owner");

    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref Task<bool> __result)
    {
        var owner = GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = Cook(owner);
        return false;
    }

    public static Player GetOwner(RestSiteOption option)
    {
        return (Player)OwnerGetter.Invoke(option, Array.Empty<object>())!;
    }

    public static bool CanCook(Player owner)
    {
        return owner.Creature.CurrentHp > HpToLose &&
            PileType.Deck.GetPile(owner).Cards.Count(card => card.IsRemovable) >= CardsToRemove;
    }

    private static async Task<bool> Cook(Player owner)
    {
        if (!CanCook(owner))
        {
            MainFile.Logger.Info("[Spire Plus] MeatCleaver skipped: cook unavailable due to HP or removable-card count.");
            return false;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, CardsToRemove)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var cards = (await CardSelectCmd.FromDeckForRemoval(owner, prefs)).ToList();
        if (cards.Count != CardsToRemove)
        {
            return false;
        }

        foreach (var card in cards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose);
        MainFile.Logger.Info("[Spire Plus] MeatCleaver applied: removed 2 cards and lost 5 HP.");
        return true;
    }
}
