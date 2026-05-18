namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    public static async Task AfterCurrentHpChanged(CombatState combatState, AscensionCombatTracker tracker, Creature creature, decimal delta)
    {
        if (!combatState.Enemies.Contains(creature))
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealHpChanged(combatState, tracker, metadata, creature, delta);
        }

        if (delta >= 0m)
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await AfterBannerEnemyHpChanged(combatState, tracker, metadata.Banner!.Value, creature);
        }

        if (creature.IsDead ||
            creature.GetHpPercentRemaining() > 0.5d ||
            tracker.ThresholdShieldedEnemies.Contains(creature))
        {
            return;
        }

        await TryApplyHolyDaze(combatState, tracker, metadata);
    }

    public static async Task AfterDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealDamageReceived(combatState, tracker, metadata, target, result, dealer, cardSource);
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            await AfterFiremarkDamageReceived(combatState, tracker, metadata.Firemark!.Value, target, result, props, dealer);
        }
    }

    public static async Task AfterDeath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature creature,
        bool wasRemovalPrevented)
    {
        if (wasRemovalPrevented)
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await AfterBannerDeath(combatState, tracker, metadata.Banner!.Value, creature);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealDeath(combatState, tracker, metadata, creature);
        }
    }

    public static async Task AfterCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardPlay cardPlay)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await AfterBannerCardPlayed(combatState, tracker, metadata.Banner!.Value, cardPlay);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealCardPlayed(combatState, tracker, metadata, cardPlay);
        }
    }

    public static Task AfterCardEnteredHand(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return Task.CompletedTask;
        }

        TryAssignChosenDecree(combatState, tracker, metadata, card);
        return Task.CompletedTask;
    }
}
