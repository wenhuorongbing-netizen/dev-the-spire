namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static decimal GetActValue(CombatState combatState, decimal actOne, decimal actTwo, decimal actThree)
    {
        return Math.Clamp(combatState.RunState.CurrentActIndex, 0, 2) switch
        {
            0 => actOne,
            1 => actTwo,
            _ => actThree
        };
    }

    private static decimal GetFiremarkCoefficient(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetMightFiremarkStrength(CombatState combatState) => GetFiremarkCoefficient(combatState);

    private static decimal GetGiantFiremarkMaxHpPercent(CombatState combatState) => GetActValue(combatState, 20m, 30m, 45m);

    private static decimal GetForgeArmorBlock(CombatState combatState) => GetActValue(combatState, 5m, 10m, 20m);

    private static decimal GetConstantHealAmount(CombatState combatState) => GetActValue(combatState, 4m, 8m, 16m);

    private static decimal GetMoltenCoreDamagePercent(CombatState combatState) => GetActValue(combatState, 20m, 25m, 30m);

    private static decimal GetConstantHealInterruptDamage(CombatState combatState) => GetActValue(combatState, 12m, 24m, 48m);

    private static decimal GetVanguardStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetShieldwallTurnBlock(CombatState combatState) => GetActValue(combatState, 3m, 7m, 14m);

    private static decimal GetShieldwallDeathBlock(CombatState combatState) => GetActValue(combatState, 5m, 10m, 20m);

    private static int GetBloodPrizeGoldReward(CombatState combatState) => (int)GetActValue(combatState, 15m, 30m, 55m);

    private static decimal GetBloodPrizeRetaliationStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetBloodPrizeRetaliationArtifact(CombatState combatState) => GetActValue(combatState, 1m, 1m, 2m);

    private static decimal GetPressingLinePartialBlock(CombatState combatState) => GetActValue(combatState, 4m, 8m, 16m);

    private static decimal GetPressingLineFullBlock(CombatState combatState) => GetActValue(combatState, 6m, 12m, 24m);

    private static decimal GetPressingLineExtraDamage(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetLastStandBlock(CombatState combatState) => GetActValue(combatState, 6m, 12m, 24m);

    private static decimal GetLastStandStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);
}
