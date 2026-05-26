using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        LothaRunHook.ShouldSkipCoopCombat(player.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) =>
        LothaRunHook.ShouldSkipCoopCombat(CombatManager.Instance.DebugOnlyGetState()?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterTurnEnd(choiceContext, side);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        LothaRunHook.ShouldSkipCoopCombat(card.Owner?.RunState)
            ? playCount
            : LothaBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        LothaRunHook.ShouldSkipCoopCombat(card.Owner?.RunState) ||
        LothaBlessingService.ShouldPlay(card, autoPlayType);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        LothaRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (LothaRunHook.ShouldSkipCoopCombat(card.Owner?.RunState))
        {
            modifiedCost = originalCost;
            return false;
        }

        return LothaBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (LothaRunHook.ShouldSkipCoopCombat(card.Owner?.RunState))
        {
            modifiedCost = originalCost;
            return false;
        }

        return LothaBlessingService.TryModifyStarCost(card, originalCost, out modifiedCost);
    }

    public override decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource) =>
        LothaRunHook.ShouldSkipCoopCombat(giver.CombatState?.RunState)
            ? amount
            : LothaBlessingService.ModifyPowerAmountGiven(power, giver, amount, target);

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        if (LothaRunHook.ShouldSkipCoopCombat(target.CombatState?.RunState))
        {
            modifiedAmount = amount;
            return false;
        }

        return LothaBlessingService.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource) =>
        LothaRunHook.ShouldSkipCoopCombat(CombatManager.Instance.DebugOnlyGetState()?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
}
