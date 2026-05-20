namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyShieldwallCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        if (!HasMultiplePrimaryEnemies(combatState))
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A16 skipped: Shieldwall banner requires a multi-enemy fight.");
            return;
        }

        tracker.ShieldwallBearer = PickBannerTarget(combatState);
        if (tracker.ShieldwallBearer != null)
        {
            await PowerCmd.Apply<ShieldwallBannerbearerPower>(
                new BlockingPlayerChoiceContext(),
                tracker.ShieldwallBearer,
                GetShieldwallTurnBlock(combatState),
                tracker.ShieldwallBearer,
                null);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A16 applied: Shieldwall bannerbearer set to {tracker.ShieldwallBearer.Name}.");
        }
        else
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A16 gate active: Shieldwall had no living enemy target.");
        }
    }

    private static async Task ApplyShieldwallDeathBlock(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature creature)
    {
        if (creature != tracker.ShieldwallBearer ||
            !creature.IsDead ||
            tracker.ShieldwallDeathBlockApplied)
        {
            return;
        }

        tracker.ShieldwallDeathBlockApplied = true;
        await ApplyBlockToEnemies(
            PrimaryAliveEnemies(combatState).Where(enemy => enemy != creature),
            GetShieldwallDeathBlock(combatState));
        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Shieldwall bannerbearer death granted final Block.");
    }

    private static async Task ApplyShieldwallTurnBlock(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        var bearer = tracker.ShieldwallBearer;
        if (bearer == null ||
            !bearer.IsAlive ||
            tracker.ShieldwallLastBlockRound == combatState.RoundNumber)
        {
            return;
        }

        tracker.ShieldwallLastBlockRound = combatState.RoundNumber;
        await ApplyBlockToEnemies(
            PrimaryAliveEnemies(combatState).Where(enemy => enemy != bearer),
            GetShieldwallTurnBlock(combatState));
        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Shieldwall bannerbearer protected other enemies at enemy turn end.");
    }
}
