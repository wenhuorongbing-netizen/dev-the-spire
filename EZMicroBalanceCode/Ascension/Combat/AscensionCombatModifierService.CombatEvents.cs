using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

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

    public static Task BeforeDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (amount <= 0m ||
            !TryRefreshNodeMetadata(combatState, tracker, out var metadata) ||
            !HasActiveBossSeal(combatState, metadata) ||
            metadata.BossSeal?.Id != BossSealId.StartledShell)
        {
            return Task.CompletedTask;
        }

        TrackStartledShellDamageStart(tracker, target);
        return Task.CompletedTask;
    }

    public static Task BeforePowerAmountChanged(
        CombatState combatState,
        AscensionCombatTracker tracker,
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0m ||
            target.Monster is not TestSubject ||
            power.GetTypeForAmount(amount) != PowerType.Debuff ||
            !TryRefreshNodeMetadata(combatState, tracker, out var metadata) ||
            !HasActiveBossSeal(combatState, metadata) ||
            metadata.BossSeal?.Id != BossSealId.ResidualSample)
        {
            return Task.CompletedTask;
        }

        tracker.TestSubjectDebuffAppliedThisPhase = true;
        return Task.CompletedTask;
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

}
