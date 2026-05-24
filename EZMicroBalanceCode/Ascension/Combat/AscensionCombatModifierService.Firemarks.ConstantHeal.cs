namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyConstantHealFiremarkCombatStart(CombatState combatState, Creature host)
    {
        var heal = GetConstantHealAmount(combatState);
        await PowerCmd.Apply<ConstantHealMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, heal, host, null);
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A12 applied: Constant Heal firemark host {host.Name} will heal {heal} HP at end of turn.");
    }

    private static async Task ResolveConstantHeal(CombatState combatState, AscensionCombatTracker tracker)
    {
        var host = tracker.FiremarkHost;
        if (host == null || !host.IsAlive)
        {
            return;
        }

        if (tracker.FiremarkDamageThisEnemyCycle >= GetConstantHealInterruptDamage(combatState))
        {
            tracker.FiremarkDamageThisEnemyCycle = 0m;
            MainFile.Logger.Info("[Spire Plus] Ascension A12 applied: Constant Heal was interrupted by player damage.");
            return;
        }

        await CreatureCmd.Heal(host, GetConstantHealAmount(combatState));
        await ApplyConstantHealOverflow(combatState, tracker);
        tracker.FiremarkDamageThisEnemyCycle = 0m;
    }

    private static async Task ApplyConstantHealOverflow(CombatState combatState, AscensionCombatTracker tracker)
    {
        var target = LowestHpRatioOverflowTarget(combatState, tracker, damagedOnly: true);
        if (target == null)
        {
            return;
        }

        await CreatureCmd.Heal(target, GetConstantHealOverflowHeal(combatState));
    }
}
