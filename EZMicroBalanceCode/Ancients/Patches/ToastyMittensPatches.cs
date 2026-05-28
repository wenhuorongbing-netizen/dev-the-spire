namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]
internal static class ToastyMittensPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ToastyMittens __instance, Player player, PlayerChoiceContext choiceContext, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = OfferTopCardExhaust(__instance, player, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTopCardExhaust(ToastyMittens mittens, Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
        var cards = PileType.Draw.GetPile(player).Cards;
        var topCard = combatState.RoundNumber == 1
            ? cards.FirstOrDefault(card => !card.Keywords.Contains(CardKeyword.Innate))
            : null;
        topCard ??= cards.FirstOrDefault();

        if (topCard == null)
        {
            MainFile.Logger.Info("[Spire Plus] ToastyMittens skipped: no draw-pile card to offer.");
            return;
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true);
        if (selected != topCard)
        {
            MainFile.Logger.Info($"[Spire Plus] ToastyMittens applied: kept top card {topCard.Id.Entry}.");
            return;
        }

        mittens.Flash();
        await CardCmd.Exhaust(choiceContext, topCard);
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, mittens.DynamicVars.Strength.BaseValue, player.Creature, null);
        MainFile.Logger.Info($"[Spire Plus] ToastyMittens applied: exhausted {topCard.Id.Entry} and gained Strength.");
    }
}
