using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int PowerFallbackCards = 1;

    private static Task TryResolveMirrorRebuttalPowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        _ = choiceContext;
        if (combatState.MirrorRebuttalResolved ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !CanUseMirrorRebuttalPowerReplacement(cardPlay.Card, combatState))
        {
            return Task.CompletedTask;
        }

        combatState.MirrorRebuttalResolved = true;
        combatState.PowerReplacementCardPendingBenefit = null;
        MainFile.Logger.Info("[Spire Plus] Lotha Mirror Rebuttal used the Power-card replacement benefit: cost 0.");
        return Task.CompletedTask;
    }

    private static async Task ApplyPowerReplacementBenefit(PlayerChoiceContext choiceContext, Player player) =>
        await CardPileCmd.Draw(choiceContext, PowerFallbackCards, player);

    private static bool IsPowerReplacementCostZeroCard(CardModel card, Player player, LothaCombatState combatState)
    {
        if (!IsPowerCard(card) ||
            card.Pile?.Type != PileType.Hand)
        {
            return false;
        }

        return GetSelectedBlessing(player) switch
        {
            LothaBlessingIds.MirrorRebuttal =>
                CanUseMirrorRebuttalPowerReplacement(card, combatState),
            LothaBlessingIds.MirrorHallEcho =>
                CanUseMirrorHallEchoPowerReplacement(card, combatState),
            LothaBlessingIds.DeferredVerdict =>
                CanUseDeferredVerdictPowerReplacement(card, player, combatState),
            LothaBlessingIds.SingleSentence =>
                CanUseSingleSentencePowerReplacement(card, combatState),
            _ => false,
        };
    }

    private static bool CanUseMirrorRebuttalPowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.MirrorRebuttalResolved &&
        IsPowerCard(card) &&
        IsMirrorRebuttalCombatCard(card);

    private static bool IsPowerCard(CardModel card) =>
        card.Type == CardType.Power && !card.IsClone;
}
