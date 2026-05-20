namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyFiremarkCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark)
    {
        var host = FindFiremarkHost(combatState);
        if (host == null)
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A12 gate active: no living enemy was available for Firemark Host selection.");
            return;
        }

        tracker.FiremarkHost = host;
        tracker.FiremarkBaseAmount = GetFiremarkCoefficient(combatState);
        tracker.FiremarkOriginalMaxHp = host.MaxHp;
        switch (firemark)
        {
            case FiremarkKind.Might:
                await ApplyMightFiremarkCombatStart(combatState, host);
                break;
            case FiremarkKind.Giant:
                await ApplyGiantFiremarkCombatStart(combatState, host);
                break;
            case FiremarkKind.ForgeArmor:
                await ApplyForgeArmorFiremarkCombatStart(combatState, host);
                break;
            case FiremarkKind.ConstantHeal:
                await ApplyConstantHealFiremarkCombatStart(combatState, host);
                break;
        }
    }

    private static Creature? FindFiremarkHost(CombatState combatState)
    {
        return AliveEnemies(combatState)
            .OrderByDescending(enemy => enemy.MaxHp)
            .ThenBy(enemy => combatState.Enemies.IndexOf(enemy))
            .FirstOrDefault();
    }

    private static async Task AfterFiremarkDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer)
    {
        var host = tracker.FiremarkHost;
        if (host == null || !host.IsAlive)
        {
            return;
        }

        if (target == host && result.UnblockedDamage > 0m)
        {
            tracker.FiremarkDamageThisPlayerTurn += result.UnblockedDamage;
            tracker.FiremarkDamageThisEnemyCycle += result.UnblockedDamage;
        }

        if (firemark == FiremarkKind.Might &&
            dealer == host &&
            target.Player != null &&
            result.UnblockedDamage > 0m &&
            props.IsPoweredAttack())
        {
            await AddFiremarkHeat(host, tracker);
        }

        if (firemark == FiremarkKind.Giant &&
            target == host &&
            result.UnblockedDamage > 0m)
        {
            await TrackMoltenCoreDamage(combatState, tracker, host, result.UnblockedDamage);
        }

        if (firemark == FiremarkKind.ForgeArmor)
        {
            TrackForgeArmorBlockedDamage(tracker, target, result);
        }
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
            case FiremarkKind.ForgeArmor when side == CombatSide.Enemy:
                await ApplyForgeArmorGain(combatState, tracker);
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
