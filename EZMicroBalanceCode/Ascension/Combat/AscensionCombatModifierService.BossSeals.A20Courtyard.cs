namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyA20CourtyardRecovery(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.IsBossBrand ||
            !AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(combatState.RunState) ||
            combatState.RunState.Map.SecondBossMapPoint == null ||
            combatState.RunState.CurrentMapCoord != combatState.RunState.Map.BossMapPoint.coord)
        {
            return;
        }

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var missingHp = Math.Max(0m, player.Creature.MaxHp - player.Creature.CurrentHp);
            var heal = Math.Ceiling(missingHp * 0.25m);
            if (heal > 0m)
            {
                await CreatureCmd.Heal(player.Creature, heal);
            }
        }

        MainFile.Logger.Info("[Spire Plus] Ascension A20 applied: courtyard recovery restored 25% of missing HP and Boss 2 Brand remains armed on the map.");
    }
}
