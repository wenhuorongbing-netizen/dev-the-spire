namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int SingleSentenceRemainingPlayLimit = 4;
    private const int SingleSentenceReadyDisplayAmount = SingleSentenceRemainingPlayLimit + 1;

    private static async Task TryResolveSingleSentencePowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (combatState.SingleSentenceUsedThisTurn ||
            combatState.SingleSentencePowerFallbackUsedThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState))
        {
            return;
        }

        combatState.SingleSentencePowerFallbackUsedThisTurn = true;
        combatState.PowerReplacementCardPendingBenefit = null;
        await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner);
        MainFile.Logger.Info("[Spire Plus] Lotha Single Sentence Power fallback cost 0, drew 1 card, and did not consume the sentence.");
    }

    private static void TrackSingleSentenceRemainingPlays(CardPlay cardPlay, LothaCombatState combatState)
    {
        if (!combatState.SingleSentenceUsedThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            cardPlay.Card.IsClone)
        {
            return;
        }

        if (ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard))
        {
            combatState.SingleSentenceRulingCard = null;
            SetSingleSentencePowerAmount(cardPlay.Card.Owner, SingleSentenceRemainingPlayLimit);
            return;
        }

        combatState.SingleSentenceRemainingCardsPlayedThisTurn++;
        SetSingleSentencePowerAmount(
            cardPlay.Card.Owner,
            SingleSentenceRemainingPlayLimit - combatState.SingleSentenceRemainingCardsPlayedThisTurn);
    }

    private static async Task EnsureSingleSentencePower(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        if (player.Creature.GetPower<LothaSingleSentencePower>() is { } existing)
        {
            existing.SetAmount(amount, silent: true);
            return;
        }

        await PowerCmd.Apply<LothaSingleSentencePower>(
            choiceContext,
            player.Creature,
            amount,
            player.Creature,
            null);
    }

    private static void SetSingleSentencePowerAmount(Player player, int amount)
    {
        player.Creature.GetPower<LothaSingleSentencePower>()?.SetAmount(Math.Max(0, amount), silent: true);
    }

    private static void HydrateSingleSentenceFromPower(Player player, LothaCombatState combatState)
    {
        if (GetSelectedBlessing(player) != LothaBlessingIds.SingleSentence ||
            combatState.SingleSentenceUsedThisTurn ||
            player.Creature.GetPower<LothaSingleSentencePower>() is not { } power ||
            power.Amount >= SingleSentenceReadyDisplayAmount)
        {
            return;
        }

        var remaining = Math.Clamp((int)power.Amount, 0, SingleSentenceRemainingPlayLimit);
        combatState.SingleSentenceUsedThisTurn = true;
        combatState.SingleSentenceRemainingCardsPlayedThisTurn = SingleSentenceRemainingPlayLimit - remaining;
        combatState.SingleSentenceRulingCard = null;
    }

    private static bool CanUseSingleSentencePowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.SingleSentenceUsedThisTurn &&
        !combatState.SingleSentencePowerFallbackUsedThisTurn &&
        IsPowerCard(card);
}
