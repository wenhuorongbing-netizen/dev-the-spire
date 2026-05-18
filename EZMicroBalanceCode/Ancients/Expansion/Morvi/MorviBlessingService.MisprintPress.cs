namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int MisprintExtraPlayCount = 1;
    private const int MisprintDrawCostThreshold = 1;

    public static bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (autoPlayType == AutoPlayType.None)
        {
            if (ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
            {
                combatState.AutoPlayCardPendingModifier = null;
            }
        }
        else
        {
            combatState.AutoPlayCardPendingModifier = card;
        }

        return true;
    }

    public static int ModifyCardPlayCount(CardModel card, int playCount)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return playCount;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (TryConsumeAutoPlayModifierBlock(card, combatState))
        {
            return playCount;
        }

        if (GetSelectedBlessing(player) != MorviBlessingIds.MisprintPress ||
            combatState.MisprintUsedThisTurn ||
            !IsNaturalPlayerCombatCard(card) ||
            card.Type is not (CardType.Attack or CardType.Skill))
        {
            return playCount;
        }

        combatState.MisprintUsedThisTurn = true;
        if (!card.EnergyCost.CostsX && card.EnergyCost.Canonical >= MisprintDrawCostThreshold)
        {
            combatState.MisprintDrawAfterCards.Add(card);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press added one play to {card.Id.Entry}.");
        return playCount + MisprintExtraPlayCount;
    }

    private static async Task ResolveMisprintPressAfterPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        MorviCombatState combatState)
    {
        if (!cardPlay.IsLastInSeries || !combatState.MisprintDrawAfterCards.Remove(cardPlay.Card))
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, 1m, cardPlay.Card.Owner);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press drew 1 card after {cardPlay.Card.Id.Entry}.");
    }

    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, MorviCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }
}
