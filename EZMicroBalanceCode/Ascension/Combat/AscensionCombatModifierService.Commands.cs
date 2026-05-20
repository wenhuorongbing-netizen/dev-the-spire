namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
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
}
