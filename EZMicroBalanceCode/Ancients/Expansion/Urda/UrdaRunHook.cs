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
        if (ShouldSkipCoopGameplay(player.RunState))
        {
            return false;
        }

        return UrdaBlessingService.MarkCardRewardIfNormalActOneCombat(player, creationOptions);
    }

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (ShouldSkipCoopGameplay(player.RunState))
        {
            return false;
        }

        return UrdaBlessingService.TryModifyCardRewardAlternatives(player, cardReward, alternatives);
    }

    public override Task AfterRewardTaken(Player player, Reward reward)
    {
        if (ShouldSkipCoopGameplay(player.RunState))
        {
            return Task.CompletedTask;
        }

        return UrdaBlessingService.AfterRewardTaken(player, reward);
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        if (ShouldSkipCoopGameplay(CurrentRunState()))
        {
            return Task.CompletedTask;
        }

        return UrdaBlessingService.BeforeRoomEntered(room);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Owner?.Creature.CombatState != null && ShouldSkipCoopCombat(card.Owner.RunState))
        {
            return Task.CompletedTask;
        }

        if (card.Pile?.Type == PileType.Hand)
        {
            _ = UrdaBlessingService.QueueSeedbedPlantFromHand(card, "card entered hand");
        }

        UrdaBlessingService.SyncPersistentState(card.Owner);

        return Task.CompletedTask;
    }

    public override Task AfterActEntered()
    {
        if (ShouldSkipCoopGameplay(CurrentRunState()))
        {
            return Task.CompletedTask;
        }

        return UrdaBlessingService.AfterActEntered();
    }

    public override Task AfterMapGenerated(ActMap map, int actIndex)
    {
        if (ShouldSkipCoopGameplay(CurrentRunState()))
        {
            return Task.CompletedTask;
        }

        UrdaBlessingService.AfterMapGenerated(map, actIndex);
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (ShouldSkipCoopGameplay(CurrentRunState()))
        {
            return Task.CompletedTask;
        }

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

    internal static bool ShouldSkipCoopGameplay(IRunState? runState) =>
        MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
            runState,
            "UrdaRunHooks",
            "Urda reward, map, room, Seed Bank, Root Sight, and relic-state mutations are disabled in co-op until host-authoritative sync is proven.");

    private static IRunState? CurrentRunState() =>
        RunManager.Instance.DebugOnlyGetState();
}
