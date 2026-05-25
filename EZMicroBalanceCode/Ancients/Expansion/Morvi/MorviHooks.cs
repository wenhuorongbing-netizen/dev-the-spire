using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        return ShouldSkipCoopCombat(CurrentRunState())
            ? Task.CompletedTask
            : MorviBlessingService.BeforeCombatStart();
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Owner != null && ShouldSkipCoopGameplay(card.Owner.RunState))
        {
            return Task.CompletedTask;
        }

        if (card.Owner?.Creature.CombatState != null && ShouldSkipCoopCombat(card.Owner.RunState))
        {
            return Task.CompletedTask;
        }

        MorviBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        return ShouldSkipCoopCombat(room.CombatState?.RunState)
            ? Task.CompletedTask
            : MorviBlessingService.AfterCombatEnd(room);
    }

    internal static bool ShouldSkipCoopCombat(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
            runState,
            "MorviCombatHooks",
            "Morvi combat card, pile, and power hooks still need two-client proof.");

    internal static bool ShouldSkipCoopGameplay(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
            runState,
            "MorviRunHooks",
            "Morvi reward, deck-state, room, and combat-preparation mutations are disabled in co-op until host-authoritative sync is proven.");

    private static IRunState? CurrentRunState() =>
        CombatManager.Instance.DebugOnlyGetState()?.RunState;
}

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
