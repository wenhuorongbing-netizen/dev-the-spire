namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyGiantFiremarkCombatStart(CombatState combatState, Creature host)
    {
        var maxHpPercent = GetGiantFiremarkMaxHpPercent(combatState);
        await PowerCmd.Apply<GiantMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, maxHpPercent, host, null);
        var giantMaxHp = Math.Ceiling(host.MaxHp * (1m + (maxHpPercent / 100m)));
        await CreatureCmd.SetMaxAndCurrentHp(host, giantMaxHp);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A12 applied: Giant firemark host {host.Name} max HP increased to {host.MaxHp}.");
    }

    private static async Task TrackMoltenCoreDamage(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature host,
        decimal damage)
    {
        if (!tracker.FiremarkCoreExposed &&
            !tracker.FiremarkCoreResolved &&
            host.GetHpPercentRemaining() <= 0.5d)
        {
            tracker.FiremarkCoreExposed = true;
            tracker.FiremarkCoreDamage = 0m;
            var originalMaxHp = tracker.FiremarkOriginalMaxHp > 0m ? tracker.FiremarkOriginalMaxHp : host.MaxHp;
            tracker.FiremarkCoreDamageNeeded = Math.Ceiling(originalMaxHp * GetMoltenCoreDamagePercent(combatState) / 100m);
            await PowerCmd.Apply<MoltenCoreFiremarkPower>(new BlockingPlayerChoiceContext(), host, tracker.FiremarkCoreDamageNeeded, host, null);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Giant firemark exposed Molten Core.");
        }

        if (!tracker.FiremarkCoreExposed || tracker.FiremarkCoreResolved)
        {
            return;
        }

        tracker.FiremarkCoreDamage += damage;
        if (tracker.FiremarkCoreDamage < tracker.FiremarkCoreDamageNeeded)
        {
            return;
        }

        tracker.FiremarkCoreResolved = true;
        tracker.FiremarkCoreExposed = false;
        await PowerCmd.Remove(host.GetPower<MoltenCoreFiremarkPower>());
        await CreatureCmd.SetMaxHp(host, Math.Max(1m, host.MaxHp - Math.Ceiling(host.MaxHp * 0.1m)));
        MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Molten Core broke and reduced Firemarked enemy max HP.");
    }

    private static async Task ResolveMoltenCoreWindow(AscensionCombatTracker tracker)
    {
        if (!tracker.FiremarkCoreExposed ||
            tracker.FiremarkCoreResolved ||
            tracker.FiremarkHost is not { IsAlive: true } host)
        {
            return;
        }

        tracker.FiremarkCoreExposed = false;
        tracker.FiremarkCoreResolved = true;
        await PowerCmd.Remove(host.GetPower<MoltenCoreFiremarkPower>());
        await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), host, 1m, host, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Molten Core window expired and the Firemarked enemy gained Artifact.");
    }
}
