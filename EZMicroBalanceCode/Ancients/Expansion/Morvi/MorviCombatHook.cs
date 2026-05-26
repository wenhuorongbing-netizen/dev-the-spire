namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        MorviRunHook.ShouldSkipCoopCombat(player.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) =>
        MorviRunHook.ShouldSkipCoopCombat(CombatManager.Instance.DebugOnlyGetState()?.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.AfterTurnEnd(choiceContext, side);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        MorviRunHook.ShouldSkipCoopCombat(card.Owner?.RunState) ||
        MorviBlessingService.ShouldPlay(card, autoPlayType);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        MorviRunHook.ShouldSkipCoopCombat(card.Owner?.RunState)
            ? playCount
            : MorviBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (MorviRunHook.ShouldSkipCoopCombat(card.Owner?.RunState))
        {
            modifiedCost = originalCost;
            return false;
        }

        return MorviBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay) =>
        MorviRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.BeforeCardPlayed(cardPlay);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        MorviRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) =>
        MorviRunHook.ShouldSkipCoopCombat(card.Owner?.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.AfterCardDrawn(choiceContext, card);
}
