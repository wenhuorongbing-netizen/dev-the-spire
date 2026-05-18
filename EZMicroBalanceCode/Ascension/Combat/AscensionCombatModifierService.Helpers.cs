using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static bool HasActiveFiremark(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Firemark.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Elite &&
            AscensionFeatureGate.IsFiremarkedEliteEnabled(combatState.RunState);
    }

    private static bool HasActiveBanner(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Banner.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            AscensionFeatureGate.IsBannerRoomEnabled(combatState.RunState);
    }

    private static bool HasActiveBossSeal(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.BossSeal != null &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Boss &&
            (metadata.IsBossBrand
                ? AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(combatState.RunState)
                : AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState));
    }

    private static decimal GetActValue(CombatState combatState, decimal actOne, decimal actTwo, decimal actThree)
    {
        return Math.Clamp(combatState.RunState.CurrentActIndex, 0, 2) switch
        {
            0 => actOne,
            1 => actTwo,
            _ => actThree
        };
    }

    private static decimal GetFiremarkCoefficient(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetMightFiremarkStrength(CombatState combatState) => GetFiremarkCoefficient(combatState);

    private static decimal GetGiantFiremarkMaxHpPercent(CombatState combatState) => GetActValue(combatState, 20m, 30m, 45m);

    private static decimal GetForgeArmorBlock(CombatState combatState) => GetActValue(combatState, 5m, 10m, 20m);

    private static decimal GetConstantHealAmount(CombatState combatState) => GetActValue(combatState, 4m, 8m, 16m);

    private static decimal GetMoltenCoreDamagePercent(CombatState combatState) => GetActValue(combatState, 20m, 25m, 30m);

    private static decimal GetConstantHealInterruptDamage(CombatState combatState) => GetActValue(combatState, 12m, 24m, 48m);

    private static decimal GetVanguardStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetShieldwallTurnBlock(CombatState combatState) => GetActValue(combatState, 3m, 7m, 14m);

    private static decimal GetShieldwallDeathBlock(CombatState combatState) => GetActValue(combatState, 5m, 10m, 20m);

    private static int GetBloodPrizeGoldReward(CombatState combatState) => (int)GetActValue(combatState, 15m, 30m, 55m);

    private static decimal GetBloodPrizeRetaliationStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetBloodPrizeRetaliationArtifact(CombatState combatState) => GetActValue(combatState, 1m, 1m, 2m);

    private static decimal GetPressingLinePartialBlock(CombatState combatState) => GetActValue(combatState, 4m, 8m, 16m);

    private static decimal GetPressingLineFullBlock(CombatState combatState) => GetActValue(combatState, 6m, 12m, 24m);

    private static decimal GetPressingLineExtraDamage(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

    private static decimal GetLastStandBlock(CombatState combatState) => GetActValue(combatState, 6m, 12m, 24m);

    private static decimal GetLastStandStrength(CombatState combatState) => GetActValue(combatState, 1m, 2m, 4m);

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
        return enemy.Monster?.NextMove?.Intents.Any(intent => intent is AttackIntent) != false;
    }

    private static bool IsPrimaryEnemy(Creature enemy)
    {
        return enemy.IsPrimaryEnemy && !enemy.HasPower<MinionPower>();
    }

    private static IEnumerable<Creature> PrimaryAliveEnemies(CombatState combatState)
    {
        return AliveEnemies(combatState).Where(IsPrimaryEnemy);
    }

    private static async Task ApplyBlockAndArtifactToEnemies(CombatState combatState, decimal block, decimal artifact)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            await ApplyBlockAndArtifact(enemy, block, artifact);
        }
    }

    private static async Task ApplyBlockAndArtifact(Creature creature, decimal block, decimal artifact)
    {
        if (block > 0m)
        {
            await CreatureCmd.GainBlock(creature, block, ValueProp.Move, null, fast: true);
        }

        if (artifact > 0m)
        {
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), creature, artifact, creature, null);
        }
    }

    private static async Task ApplyBlockToEnemies(CombatState combatState, decimal block)
    {
        await ApplyBlockToEnemies(AliveEnemies(combatState), block);
    }

    private static async Task ApplyBlockToEnemies(IEnumerable<Creature> enemies, decimal block)
    {
        foreach (var enemy in enemies)
        {
            await CreatureCmd.GainBlock(enemy, block, ValueProp.Move, null, fast: true);
        }
    }

    private static async Task ApplyStrengthToEnemies(CombatState combatState, decimal amount)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), enemy, amount, enemy, null);
        }
    }

    private static async Task ApplyStrengthToEnemies(IEnumerable<Creature> enemies, decimal amount)
    {
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), enemy, amount, enemy, null);
        }
    }

    private static async Task ApplyStrengthAndArtifact(Creature creature, decimal strength, decimal artifact)
    {
        if (strength > 0m)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), creature, strength, creature, null);
        }

        if (artifact > 0m)
        {
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), creature, artifact, creature, null);
        }
    }

    private static IEnumerable<Creature> AliveEnemies(CombatState combatState)
    {
        return combatState.Enemies.Where(enemy => enemy.IsAlive);
    }
}
