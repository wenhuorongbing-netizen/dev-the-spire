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
