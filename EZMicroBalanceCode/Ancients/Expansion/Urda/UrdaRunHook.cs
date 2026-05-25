using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRunHook : AbstractModel
{
    public UrdaRunHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => true;

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        return UrdaBlessingService.MarkCardRewardIfNormalActOneCombat(player, creationOptions);
    }

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        return UrdaBlessingService.TryModifyCardRewardAlternatives(player, cardReward, alternatives);
    }

    public override Task AfterRewardTaken(Player player, Reward reward)
    {
        return UrdaBlessingService.AfterRewardTaken(player, reward);
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.BeforeRoomEntered(room);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Owner?.Creature.CombatState != null && ShouldSkipCoopCombat(card.Owner.RunState))
        {
            return;
        }

        if (card.Pile?.Type == PileType.Hand)
        {
            await UrdaBlessingService.TryPlantSeedbedCardFromHand(card, "card entered hand");
        }

        UrdaBlessingService.SyncPersistentState(card.Owner);
    }

    public override Task AfterActEntered()
    {
        return UrdaBlessingService.AfterActEntered();
    }

    public override Task AfterMapGenerated(ActMap map, int actIndex)
    {
        UrdaBlessingService.AfterMapGenerated(map, actIndex);
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.AfterRoomEntered(room);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        return ShouldSkipCoopCombat(room.CombatState?.RunState)
            ? Task.CompletedTask
            : UrdaBlessingService.AfterCombatVictory(room);
    }

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        ShouldSkipCoopCombat(target.CombatState?.RunState)
            ? Task.CompletedTask
            : UrdaBlessingService.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

    internal static bool ShouldSkipCoopCombat(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopCombatHook(
            runState,
            "UrdaCombatHooks",
            "Urda combat card, Seedbed, Seed Bank, Root Sight, and Rooted Route hooks still need two-client proof.");
}

internal sealed class UrdaCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        UrdaRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)
            ? Task.CompletedTask
            : UrdaBlessingService.AfterCardPlayed(choiceContext, cardPlay);
}
