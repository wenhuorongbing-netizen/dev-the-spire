using EZMicroBalance.EZMicroBalanceCode.Ascension;

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
