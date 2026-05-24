namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))]
internal static class CrossbowOfferPatch
{
    [HarmonyPrefix]
    private static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (__instance is not Crossbow crossbow)
        {
            return true;
        }

        if (side != crossbow.Owner.Creature.Side)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = OfferTemporaryAttack(crossbow, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTemporaryAttack(Crossbow crossbow, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        var owner = crossbow.Owner;
        var attackPool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Attack && card.CanBeGeneratedInCombat)
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generated == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Crossbow skipped: no eligible attack generated.");
            return;
        }

        AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1);
        AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { generated }, owner, canSkip: true);
        if (selected == generated)
        {
            crossbow.Flash();
            var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner);
            if (addResult is { success: true })
            {
                MainFile.Logger.Info($"[Spire Plus] Crossbow applied: accepted temporary attack {generated.Id.Entry}.");
                return;
            }

            MainFile.Logger.Warn($"[Spire Plus] Crossbow skipped: accepted temporary attack {generated.Id.Entry} could not be added to combat.");
            return;
        }

        AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState);
        MainFile.Logger.Info($"[Spire Plus] Crossbow applied: skipped temporary attack {generated.Id.Entry}.");
    }
}

[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]
internal static class CrossbowVanillaAfterTurnPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}
