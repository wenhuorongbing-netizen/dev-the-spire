using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static bool HasMultiplePrimaryEnemies(CombatState combatState, Creature? includeDeadCreature = null)
    {
        var count = PrimaryAliveEnemies(combatState).Count();
        if (includeDeadCreature != null && IsPrimaryEnemy(includeDeadCreature))
        {
            count++;
        }

        return count >= 2;
    }

    private static bool IsLikelyAttacker(Creature enemy)
    {
        return enemy.Monster?.NextMove?.Intents.Any(intent => intent is AttackIntent) == true;
    }

    private static bool IsPrimaryEnemy(Creature enemy)
    {
        return enemy.IsPrimaryEnemy && !enemy.HasPower<MinionPower>();
    }

    private static IEnumerable<Creature> PrimaryAliveEnemies(CombatState combatState)
    {
        return AliveEnemies(combatState).Where(IsPrimaryEnemy);
    }

    private static IEnumerable<Creature> AliveEnemies(CombatState combatState)
    {
        return combatState.Enemies.Where(enemy => enemy.IsAlive);
    }
}
