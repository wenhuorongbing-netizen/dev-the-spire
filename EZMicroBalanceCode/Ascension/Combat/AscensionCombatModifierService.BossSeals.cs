namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static bool TryRefreshActiveBossSealMetadata(
        CombatState combatState,
        AscensionCombatTracker tracker,
        out AscensionNodeMetadata metadata) =>
        TryRefreshNodeMetadata(combatState, tracker, out metadata) &&
        HasActiveBossSeal(combatState, metadata);
}
