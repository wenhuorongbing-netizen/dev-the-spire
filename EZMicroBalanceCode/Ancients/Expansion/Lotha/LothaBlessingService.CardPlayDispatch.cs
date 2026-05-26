namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
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
