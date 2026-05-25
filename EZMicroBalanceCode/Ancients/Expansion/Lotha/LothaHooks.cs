using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        return ShouldSkipCoopCombat(CurrentRunState())
            ? Task.CompletedTask
            : LothaBlessingService.BeforeCombatStart();
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

        LothaBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room) =>
        ShouldSkipCoopCombat(room.CombatState?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterCombatEnd(room);

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        ShouldSkipCoopCombat(target.CombatState?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room) =>
        !ShouldSkipCoopGameplay(player.RunState) &&
        LothaBlessingService.TryModifyRewardsLate(player, rewards, room);

    public override bool ShouldDieLate(Creature creature) =>
        ShouldSkipCoopCombat(creature.CombatState?.RunState) ||
        LothaBlessingService.ShouldDieLate(creature);

    public override bool ShouldDie(Creature creature) =>
        ShouldSkipCoopCombat(creature.CombatState?.RunState) ||
        LothaBlessingService.ShouldDie(creature);

    public override Task AfterPreventingDeath(Creature creature) =>
        ShouldSkipCoopCombat(creature.CombatState?.RunState)
            ? Task.CompletedTask
            : LothaBlessingService.AfterPreventingDeath(creature);

    internal static bool ShouldSkipCoopCombat(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
            runState,
            "LothaCombatHooks",
            "Lotha combat card, power, and death-prevention hooks still need two-client proof.");

    internal static bool ShouldSkipCoopGameplay(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
            runState,
            "LothaRunHooks",
            "Lotha reward, deck-state, room, death-prevention, and combat-preparation mutations are disabled in co-op until host-authoritative sync is proven.");

    private static IRunState? CurrentRunState() =>
        CombatManager.Instance.DebugOnlyGetState()?.RunState;
}

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
