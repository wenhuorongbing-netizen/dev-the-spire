namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static bool TryRefreshNodeMetadata(
        CombatState combatState,
        AscensionCombatTracker tracker,
        out AscensionNodeMetadata metadata)
    {
        var current = tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState);
        if (current == null)
        {
            metadata = null!;
            return false;
        }

        tracker.NodeMetadata = current;
        metadata = current;
        return true;
    }
}
