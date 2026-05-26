namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int LothaExtraPlayCount = 2;
    private const int MirrorRebuttalExtraPlayCount = 1;

    public static int ModifyCardPlayCount(CardModel card, int playCount)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return playCount;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        var selectedBlessing = GetSelectedBlessing(player);
        if (TryConsumeAutoPlayModifierBlock(card, combatState))
        {
            LogExtraPlayAttempt(player, selectedBlessing, card, allowed: false, reason: "autoplay", extraPlayCount: 0);
            return playCount;
        }

        HydrateSingleSentenceFromPower(player, combatState);
        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal &&
            !combatState.MirrorRebuttalResolved &&
            IsMirrorRebuttalCombatCard(card) &&
            IsEligibleCard(card))
        {
            combatState.MirrorRebuttalResolved = true;
            LogExtraPlayAttempt(player, selectedBlessing, card, allowed: true, reason: "mirror_rebuttal", extraPlayCount: MirrorRebuttalExtraPlayCount);
            MainFile.Logger.Info($"[Spire Plus] Lotha Mirror Rebuttal extra-played {card.Id.Entry} one additional time.");
            return playCount + MirrorRebuttalExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.MirrorHallEcho &&
            !combatState.MirrorHallEchoConsumedThisTurn &&
            combatState.MirrorHallEchoArmedType == card.Type &&
            IsEligibleCard(card))
        {
            combatState.MirrorHallEchoConsumedThisTurn = true;
            combatState.MirrorHallEchoArmedType = null;
            LogExtraPlayAttempt(player, selectedBlessing, card, allowed: true, reason: "mirror_hall_echo", extraPlayCount: MirrorHallEchoExtraPlayCount);
            MainFile.Logger.Info($"[Spire Plus] Lotha Mirror Hall Echo extra-played {card.Id.Entry} one additional time.");
            return playCount + MirrorHallEchoExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            combatState.DeferredVerdictActiveThisTurn &&
            HasDeferredVerdictStacks(player) &&
            IsDeferredVerdictExtraPlayCard(card))
        {
            LogExtraPlayAttempt(player, selectedBlessing, card, allowed: true, reason: "deferred_verdict", extraPlayCount: DeferredVerdictExtraPlayCount);
            MainFile.Logger.Info($"[Spire Plus] Lotha Deferred Verdict extra-played {card.Id.Entry} one additional time.");
            return playCount + DeferredVerdictExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.SingleSentence &&
            !combatState.SingleSentenceUsedThisTurn &&
            IsEligibleCard(card))
        {
            combatState.SingleSentenceUsedThisTurn = true;
            combatState.SingleSentenceRulingCard = card;
            SetSingleSentencePowerAmount(player, SingleSentenceRemainingPlayLimit);
            LogExtraPlayAttempt(player, selectedBlessing, card, allowed: true, reason: "single_sentence", extraPlayCount: LothaExtraPlayCount);
            MainFile.Logger.Info($"[Spire Plus] Lotha Single Sentence extra-played {card.Id.Entry} two additional times.");
            return playCount + LothaExtraPlayCount;
        }

        return playCount;
    }

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

    public static async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        var selectedBlessing = GetSelectedBlessing(player);
        HydrateSingleSentenceFromPower(player, combatState);

        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal)
        {
            await TryResolveMirrorRebuttalPowerFallback(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.MirrorHallEcho)
        {
            await TryResolveMirrorHallEchoPowerFallback(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict)
        {
            await TryResolveDeferredVerdictCard(choiceContext, cardPlay, combatState);
        }

        if (selectedBlessing == LothaBlessingIds.SingleSentence)
        {
            await TryResolveSingleSentencePowerFallback(choiceContext, cardPlay, combatState);
            TrackSingleSentenceRemainingPlays(cardPlay, combatState);
        }

    }

}
