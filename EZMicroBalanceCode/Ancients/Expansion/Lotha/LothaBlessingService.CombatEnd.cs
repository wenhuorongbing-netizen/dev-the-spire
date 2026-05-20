namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static async Task AfterCombatEnd(CombatRoom room)
    {
        var players = room.CombatState.RunState.Players.Where(player => player.IsActiveForHooks).ToList();

        foreach (var player in players)
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            if (GetSelectedBlessing(player) != LothaBlessingIds.DeferredVerdict ||
                combatState.DeferredVerdictGranted ||
                player.Creature.CombatState?.RoundNumber >= DeferredVerdictTurn ||
                !player.Creature.IsAlive)
            {
                continue;
            }

            combatState.DeferredVerdictGranted = true;
            await CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal, playAnim: false);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict healed 4 HP because combat ended before turn 4.");
        }

        foreach (var player in players)
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            if (GetSelectedBlessing(player) == LothaBlessingIds.DeferredVerdict)
            {
                await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
            }

            if (GetSelectedBlessing(player) == LothaBlessingIds.DeathReprieve)
            {
                combatState.DeathReprieveActive = false;
                combatState.DeathReprievePendingStart = false;
                ResolveDeathReprieveProgress(player);
                await PowerCmd.Remove<LothaDeathReprievePower>(player.Creature);
            }
        }
    }
}
