using MegaCrit.Sts2.Core.Models.Afflictions;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static Creature? FindFiremarkHost(CombatState combatState)
    {
        var hostCandidates = PrimaryAliveEnemies(combatState).ToList();
        if (hostCandidates.Count == 0)
        {
            hostCandidates = AliveEnemies(combatState)
                .Where(enemy => !enemy.HasPower<MinionPower>())
                .ToList();
        }

        return hostCandidates
            .OrderByDescending(enemy => enemy.MaxHp)
            .ThenBy(enemy => combatState.Enemies.IndexOf(enemy))
            .FirstOrDefault();
    }

    private static IEnumerable<Creature> FiremarkOverflowCandidates(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        var host = tracker.FiremarkHost;
        if (host is not { IsAlive: true })
        {
            return [];
        }

        return PrimaryAliveEnemies(combatState)
            .Where(enemy => enemy != host)
            .OrderBy(enemy => combatState.Enemies.IndexOf(enemy));
    }

    private static Creature? LowestHpRatioOverflowTarget(
        CombatState combatState,
        AscensionCombatTracker tracker,
        bool damagedOnly = false)
    {
        return FiremarkOverflowCandidates(combatState, tracker)
            .Where(enemy => !damagedOnly || enemy.CurrentHp < enemy.MaxHp)
            .OrderBy(enemy => enemy.GetHpPercentRemaining())
            .ThenBy(enemy => combatState.Enemies.IndexOf(enemy))
            .FirstOrDefault();
    }
}
