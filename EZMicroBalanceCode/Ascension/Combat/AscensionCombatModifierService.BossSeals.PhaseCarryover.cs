using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackResidualSamplePhase(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        if (creature.Monster is not TestSubject ||
            !creature.HasPower<AdaptablePower>())
        {
            return;
        }

        tracker.TestSubjectPhaseChanges++;
        var sampleCount = metadata.IsBossBrand && tracker.TestSubjectPhaseChanges == 1 ? 2m : 1m;
        await PowerCmd.Apply<ResidualSamplePower>(new BlockingPlayerChoiceContext(), creature, sampleCount, creature, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Residual Sample retained a weakened sample for the next phase.");
    }

    private static async Task TryApplyResidualSamples(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ResidualSample)
        {
            return;
        }

        var subject = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TestSubject);
        var sample = subject?.GetPower<ResidualSamplePower>();
        if (subject == null || sample == null || subject.IsDead)
        {
            return;
        }

        var amount = sample.Amount;
        await CreatureCmd.GainBlock(subject, 8m * amount, ValueProp.Move, null, fast: true);
        await PowerCmd.Remove(sample);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Residual Sample resolved as weakened phase Block.");
    }

    private static async Task ApplyA20CourtyardRecovery(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.IsBossBrand ||
            !AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(combatState.RunState) ||
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

        MainFile.Logger.Info("[EZMicroBalance] Ascension A20 applied: courtyard recovery restored 25% of missing HP and Boss 2 Brand remains armed on the map.");
    }
}
