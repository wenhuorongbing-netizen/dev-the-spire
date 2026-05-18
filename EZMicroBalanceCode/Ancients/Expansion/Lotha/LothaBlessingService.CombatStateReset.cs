namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private static void ResetCombatState(LothaCombatState combatState)
    {
        combatState.MirrorRebuttalCardPulled = false;
        combatState.MirrorRebuttalResolved = false;
        combatState.MirrorHallEchoRecordedType = null;
        combatState.MirrorHallEchoArmedType = null;
        combatState.MirrorHallEchoConsumedThisTurn = false;
        combatState.ClosedCourtUsed = false;
        combatState.ClosedCourtDiscountActiveThisTurn = false;
        combatState.ClosedCourtDiscountsRemainingThisTurn = 0;
        combatState.ClosedCourtDiscountedCardsThisTurn.Clear();
        combatState.PresumptionLost = false;
        combatState.DeferredVerdictGranted = false;
        combatState.DeferredVerdictActiveThisTurn = false;
        combatState.DeathReprieveActive = false;
        combatState.DeathReprievePendingStart = false;
        combatState.DeathReprieveStarted = false;
        combatState.SingleSentenceUsedThisTurn = false;
        combatState.SingleSentencePowerFallbackUsedThisTurn = false;
        combatState.SingleSentenceRemainingCardsPlayedThisTurn = 0;
        combatState.SingleSentenceRulingCard = null;
        combatState.AutoPlayCardPendingModifier = null;
        combatState.PowerReplacementCardPendingBenefit = null;
    }

    private static void ResetTurnState(LothaCombatState combatState)
    {
        combatState.DeferredVerdictActiveThisTurn = false;
        combatState.MirrorHallEchoArmedType = combatState.MirrorHallEchoRecordedType;
        combatState.MirrorHallEchoRecordedType = null;
        combatState.MirrorHallEchoConsumedThisTurn = false;
        combatState.ClosedCourtDiscountActiveThisTurn = false;
        combatState.ClosedCourtDiscountsRemainingThisTurn = 0;
        combatState.ClosedCourtDiscountedCardsThisTurn.Clear();
        combatState.SingleSentenceUsedThisTurn = false;
        combatState.SingleSentencePowerFallbackUsedThisTurn = false;
        combatState.SingleSentenceRemainingCardsPlayedThisTurn = 0;
        combatState.SingleSentenceRulingCard = null;
        combatState.AutoPlayCardPendingModifier = null;
        combatState.PowerReplacementCardPendingBenefit = null;
    }
}
