using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int LothaExtraPlayCount = 2;

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

        var selectedBlessing = GetSelectedBlessing(player);
        HydrateSingleSentenceFromPower(player, combatState);
        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal &&
            !combatState.MirrorRebuttalResolved &&
            IsMirrorRebuttalCombatCard(card) &&
            IsEligibleCard(card))
        {
            combatState.MirrorRebuttalResolved = true;
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal extra-played {card.Id.Entry} two additional times.");
            return playCount + LothaExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.MirrorHallEcho &&
            !combatState.MirrorHallEchoConsumedThisTurn &&
            combatState.MirrorHallEchoArmedType == card.Type &&
            IsEligibleCard(card))
        {
            combatState.MirrorHallEchoConsumedThisTurn = true;
            combatState.MirrorHallEchoArmedType = null;
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Hall Echo extra-played {card.Id.Entry} one additional time.");
            return playCount + MirrorHallEchoExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            combatState.DeferredVerdictActiveThisTurn &&
            HasDeferredVerdictStacks(player) &&
            IsDeferredVerdictConsumerCard(card) &&
            IsEligibleCard(card))
        {
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Deferred Verdict extra-played {card.Id.Entry} one additional time.");
            return playCount + DeferredVerdictExtraPlayCount;
        }

        if (selectedBlessing == LothaBlessingIds.SingleSentence &&
            !combatState.SingleSentenceUsedThisTurn &&
            IsEligibleCard(card))
        {
            combatState.SingleSentenceUsedThisTurn = true;
            combatState.SingleSentenceRulingCard = card;
            SetSingleSentencePowerAmount(player, SingleSentenceRemainingPlayLimit);
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Single Sentence extra-played {card.Id.Entry} two additional times.");
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

        if (selectedBlessing == LothaBlessingIds.ClosedCourt)
        {
            TrackClosedCourtDiscountUse(cardPlay, combatState);
        }
    }

    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, LothaCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }

    private static bool IsEligibleCard(CardModel card) =>
        card.Type is CardType.Attack or CardType.Skill && !card.IsClone;

    private static bool IsDeferredVerdictConsumerCard(CardModel card) =>
        card.Type != CardType.Status && !card.IsClone;
}
