namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]
internal static class WhisperingEarringPatch
{
    [HarmonyPrefix]
    private static bool Prefix(WhisperingEarring __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = AutoPlayOneHighestCostCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task AutoPlayOneHighestCostCard(WhisperingEarring earring, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (combatState.RoundNumber > 3)
        {
            return;
        }

        var card = PileType.Hand.GetPile(player).Cards
            .Select((card, index) => new { Card = card, Index = index })
            .Where(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay))
            .OrderByDescending(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card)))
            .ThenBy(item => item.Index)
            .Select(item => item.Card)
            .FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var target = AncientCardHelpers.GetPreferredTarget(card, combatState, player);
        if (card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly && !card.CanPlayTargeting(target))
        {
            return;
        }

        earring.Flash();
        await VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources);
        await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
        MainFile.Logger.Info($"[Spire Plus] WhisperingEarring applied: auto-played {card.Id.Entry} on round {combatState.RoundNumber}.");
    }
}
