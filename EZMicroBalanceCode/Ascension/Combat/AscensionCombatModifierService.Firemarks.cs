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
            MainFile.Logger.Info("[Spire Plus] Ascension A12 gate active: no living enemy was available for Firemark Host selection.");
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

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A12 applied: Firemark Host is {host.Name}; overflow affects at most one secondary enemy at a time.");
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
    }
}
