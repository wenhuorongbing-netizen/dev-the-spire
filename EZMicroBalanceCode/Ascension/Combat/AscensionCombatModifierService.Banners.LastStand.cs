namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void ApplyLastStandCombatStart(CombatState combatState)
    {
        if (!HasMultiplePrimaryEnemies(combatState))
        {
            MainFile.Logger.Info("[Spire Plus] Ascension A16 skipped: Last Stand banner requires a multi-enemy fight.");
        }
    }

    private static async Task AfterBannerDeath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner,
        Creature creature)
    {
        if (banner != BannerKind.LastStand ||
            tracker.LastStandTriggered ||
            !HasMultiplePrimaryEnemies(combatState, includeDeadCreature: creature) ||
            creature.Player != null ||
            !IsPrimaryEnemy(creature))
        {
            return;
        }

        tracker.LastStandTriggered = true;
        var block = GetLastStandBlock(combatState);
        var strength = GetLastStandStrength(combatState);
        foreach (var enemy in PrimaryAliveEnemies(combatState).Where(enemy => enemy != creature))
        {
            await CreatureCmd.GainBlock(enemy, block, ValueProp.Move, null, fast: true);
            await PowerCmd.Apply<LastStandBannerPower>(new BlockingPlayerChoiceContext(), enemy, strength, enemy, null);
        }

        MainFile.Logger.Info("[Spire Plus] Ascension A16 applied: Last Stand banner triggered after the first enemy death.");
    }
}
