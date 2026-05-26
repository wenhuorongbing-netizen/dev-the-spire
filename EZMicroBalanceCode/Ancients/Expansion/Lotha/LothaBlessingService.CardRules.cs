namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateSingleSentenceFromPower(player, combatState);
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
            return true;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.SingleSentence ||
            !combatState.SingleSentenceUsedThisTurn)
        {
            return true;
        }

        var canPlay = combatState.SingleSentenceRemainingCardsPlayedThisTurn < SingleSentenceRemainingPlayLimit;
        if (!canPlay)
        {
            SetSingleSentencePowerAmount(player, 0);
        }

        return canPlay;
    }

}
