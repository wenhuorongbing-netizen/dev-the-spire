namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart() =>
        MorviBlessingService.BeforeCombatStart();

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        MorviBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room) =>
        MorviBlessingService.AfterCombatEnd(room);
}

internal sealed class MorviCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        MorviBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) =>
        MorviBlessingService.AfterTurnEnd(choiceContext, side);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        MorviBlessingService.ShouldPlay(card, autoPlayType);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        MorviBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        MorviBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);

    public override Task BeforeCardPlayed(CardPlay cardPlay) =>
        MorviBlessingService.BeforeCardPlayed(cardPlay);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        MorviBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) =>
        MorviBlessingService.AfterCardDrawn(choiceContext, card);
}
