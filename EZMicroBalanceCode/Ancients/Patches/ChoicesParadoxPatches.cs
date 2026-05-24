namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))]
internal static class ChoicesParadoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ChoicesParadox __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = ChooseRareTemporaryCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task ChooseRareTemporaryCard(ChoicesParadox choicesParadox, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.RoundNumber != 1)
        {
            return;
        }

        var pool = ModelDb.AllCharacterCardPools
            .Concat(new[] { ModelDb.CardPool<ColorlessCardPool>() })
            .SelectMany(cardPool => cardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(IsChoicesParadoxEligibleRare)
            .Distinct()
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(
                player,
                pool,
                choicesParadox.DynamicVars.Cards.IntValue,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (generated.Count == 0)
        {
            MainFile.Logger.Warn("[Spire Plus] ChoicesParadox skipped: no eligible rare combat cards generated.");
            return;
        }

        choicesParadox.Flash();
        foreach (var card in generated)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                generated,
                player,
                new CardSelectorPrefs(new LocString("relics", "CHOICES_PARADOX.selectionScreenPrompt"), 1)))
            .FirstOrDefault();

        foreach (var card in generated.Where(card => card != selected))
        {
            combatState.RemoveCard(card);
        }

        if (selected != null)
        {
            await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player);
        }

        MainFile.Logger.Info($"[Spire Plus] ChoicesParadox applied: offered {generated.Count} rare card(s), selected {selected?.Id.Entry ?? "NONE"}.");
    }

    private static bool IsChoicesParadoxEligibleRare(CardModel card)
    {
        return card.Rarity == CardRarity.Rare &&
            card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest &&
            !card.Keywords.Contains(CardKeyword.Unplayable) &&
            card.CanBeGeneratedInCombat &&
            card.CanBeGeneratedByModifiers;
    }
}
