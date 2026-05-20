namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (!player.IsActiveForHooks)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        var selectedBlessing = GetSelectedBlessing(player);
        var activeCombat = player.Creature.CombatState;

        ResetTurnState(combatState);
        HydrateDeathReprieveState(player, combatState);

        if (selectedBlessing == LothaBlessingIds.SingleSentence)
        {
            await EnsureSingleSentencePower(choiceContext, player, SingleSentenceReadyDisplayAmount);
        }

        await TryApplyPresumptionTurnStart(choiceContext, player, combatState, selectedBlessing);
        await TryOpenClosedCourtFirstTurn(choiceContext, player, combatState, selectedBlessing);

        if (selectedBlessing == LothaBlessingIds.MirrorRebuttal &&
            activeCombat?.RoundNumber == 1 &&
            !combatState.MirrorRebuttalCardPulled)
        {
            combatState.MirrorRebuttalCardPulled = true;
            await TryMoveMirrorRebuttalCardToHand(player);
        }

        if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            !combatState.DeferredVerdictGranted &&
            activeCombat != null &&
            activeCombat.RoundNumber == DeferredVerdictTurn)
        {
            combatState.DeferredVerdictGranted = true;
            combatState.DeferredVerdictActiveThisTurn = true;

            await PlayerCmd.GainEnergy(DeferredVerdictEnergy, player);
            await CardPileCmd.Draw(choiceContext, DeferredVerdictCards, player);
            await PowerCmd.Apply<LothaVerdictPower>(
                choiceContext,
                player.Creature,
                DeferredVerdictStacks,
                player.Creature,
                null);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict granted draw 4, Energy 4, and player-owned Verdict 3.");
        }
        else if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
            combatState.DeferredVerdictGranted &&
            activeCombat?.RoundNumber > DeferredVerdictTurn)
        {
            await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
        }

        if (selectedBlessing == LothaBlessingIds.PublicEvidence)
        {
            await ConsumePublicEvidenceEnlightenmentAtTurnStart(choiceContext, player);
        }

        if (selectedBlessing == LothaBlessingIds.DeathReprieve && combatState.DeathReprievePendingStart)
        {
            await StartDeathReprieveTurn(choiceContext, player, combatState, "next player turn after lethal damage");
        }
    }
}
