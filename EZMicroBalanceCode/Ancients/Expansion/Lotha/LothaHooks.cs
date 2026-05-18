using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        return LothaBlessingService.BeforeCombatStart();
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        LothaBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room) =>
        LothaBlessingService.AfterCombatEnd(room);

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        LothaBlessingService.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room) =>
        LothaBlessingService.TryModifyRewardsLate(player, rewards, room);

    public override bool ShouldDieLate(Creature creature) =>
        LothaBlessingService.ShouldDieLate(creature);

    public override bool ShouldDie(Creature creature) =>
        LothaBlessingService.ShouldDie(creature);

    public override Task AfterPreventingDeath(Creature creature) =>
        LothaBlessingService.AfterPreventingDeath(creature);
}

internal sealed class LothaCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        LothaBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) =>
        LothaBlessingService.AfterTurnEnd(choiceContext, side);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        LothaBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        LothaBlessingService.ShouldPlay(card, autoPlayType);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        LothaBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        LothaBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        LothaBlessingService.TryModifyStarCost(card, originalCost, out modifiedCost);

    public override decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource) =>
        LothaBlessingService.ModifyPowerAmountGiven(power, giver, amount, target);

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount) =>
        LothaBlessingService.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource) =>
        LothaBlessingService.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
}
