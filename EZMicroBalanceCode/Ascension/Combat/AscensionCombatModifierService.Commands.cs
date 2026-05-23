namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task RefreshEnemyIntent(Creature? creature)
    {
        if (creature is not { IsMonster: true, IsAlive: true } ||
            creature.CombatState is not { } combatState)
        {
            return;
        }

        var node = creature.GetCreatureNode();
        if (node == null)
        {
            return;
        }

        try
        {
            await node.UpdateIntent(combatState.Allies);
        }
        catch (ObjectDisposedException ex)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Ascension intent refresh skipped for {creature.LogName}: {ex.Message}");
        }
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

    private static async Task<T?> ApplyPowerWithFinalDisplayedGain<T>(
        Creature creature,
        int displayedGain,
        Creature? applier,
        CardModel? cardSource)
        where T : PowerModel
    {
        if (displayedGain <= 0)
        {
            return creature.GetPower<T>();
        }

        var existingAmount = creature.GetPower<T>()?.Amount ?? 0;
        await PowerCmd.Apply<T>(new BlockingPlayerChoiceContext(), creature, 1m, applier, cardSource);
        var power = creature.GetPower<T>();
        if (power == null)
        {
            return null;
        }

        var targetAmount = existingAmount + displayedGain;
        var correction = targetAmount - power.Amount;
        if (correction != 0)
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, correction, applier, cardSource);
        }

        return creature.GetPower<T>();
    }

    private static async Task ClampPowerAmount<T>(
        Creature creature,
        int maxAmount,
        Creature? applier,
        CardModel? cardSource)
        where T : PowerModel
    {
        var power = creature.GetPower<T>();
        if (power is { Amount: > 0 } && power.Amount > maxAmount)
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, maxAmount - power.Amount, applier, cardSource);
        }
    }
}
