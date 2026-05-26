namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        UrdaRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)
            ? Task.CompletedTask
            : UrdaBlessingService.AfterCardPlayed(choiceContext, cardPlay);
}
