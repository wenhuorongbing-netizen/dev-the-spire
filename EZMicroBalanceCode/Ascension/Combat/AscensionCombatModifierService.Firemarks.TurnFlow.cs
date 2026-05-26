namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void RefreshFiremarkRoundDamageTracking(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        if (tracker.FiremarkDamageTrackingRound == combatState.RoundNumber)
        {
            return;
        }

        tracker.FiremarkDamageTrackingRound = combatState.RoundNumber;
        tracker.FiremarkDamageThisPlayerTurn = 0m;
        tracker.FiremarkDamageThisEnemyCycle = 0m;
    }

    private static async Task ApplyFiremarkSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark,
        CombatSide side)
    {
        if (side != CombatSide.Enemy || firemark != FiremarkKind.Might)
        {
            return;
        }

        await ApplyMightOverflow(combatState, tracker);
    }

    private static async Task ApplyFiremarkPlayerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark)
    {
        if (firemark != FiremarkKind.ForgeArmor)
        {
            return;
        }

        await ApplyForgeArmorGain(combatState, tracker);
        await ApplyForgeArmorOverflow(combatState, tracker);
    }

    private static async Task ApplyFiremarkTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark,
        CombatSide side)
    {
        switch (firemark)
        {
            case FiremarkKind.Giant when side == CombatSide.Player:
                await ResolveMoltenCoreWindow(tracker);
                break;
            case FiremarkKind.ForgeArmor when side == CombatSide.Player:
                ResolveForgeArmorShatter(tracker);
                break;
            case FiremarkKind.ConstantHeal when side == CombatSide.Enemy:
                await ResolveConstantHeal(combatState, tracker);
                break;
        }
    }
}
