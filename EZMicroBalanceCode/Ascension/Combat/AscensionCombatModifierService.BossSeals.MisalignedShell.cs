using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TrackMisalignedShellClawDeath(AscensionCombatTracker tracker, Creature creature)
    {
        if (creature.Monster is Crusher or Rocket)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Add(creature);
        }
    }

    private static async Task SettleMisalignedShellCalibration(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.MisalignedShell)
        {
            return;
        }

        var claws = AliveEnemies(combatState)
            .Where(enemy => enemy.Monster is Crusher or Rocket)
            .ToList();
        if (claws.Count != 2)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        var first = claws[0];
        var second = claws[1];
        var firstHp = first.MaxHp <= 0 ? 0m : first.CurrentHp / first.MaxHp;
        var secondHp = second.MaxHp <= 0 ? 0m : second.CurrentHp / second.MaxHp;
        var gap = Math.Abs(firstHp - secondHp);
        var threshold = metadata.IsBossBrand ? 0.30m : 0.35m;
        if (gap < threshold)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        var higherHpClaw = firstHp >= secondHp ? first : second;
        if (tracker.MisalignedShellCalibrationUsed.Contains(higherHpClaw))
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        tracker.MisalignedShellCalibration.TryGetValue(higherHpClaw, out var calibration);
        calibration++;
        tracker.MisalignedShellCalibration[higherHpClaw] = calibration;
        await PowerCmd.Apply<KaiserCalibrationPower>(new BlockingPlayerChoiceContext(), higherHpClaw, 1m, higherHpClaw, null);
        if (calibration < 2)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        tracker.MisalignedShellCalibration[higherHpClaw] = 0;
        tracker.MisalignedShellCalibrationUsed.Add(higherHpClaw);
        await PowerCmd.Remove(higherHpClaw.GetPower<KaiserCalibrationPower>());
        await PowerCmd.Apply<KaiserCalibrationStrikePower>(
            new BlockingPlayerChoiceContext(),
            higherHpClaw,
            metadata.IsBossBrand ? 5m : 4m,
            higherHpClaw,
            null);
        await RefreshEnemyIntent(higherHpClaw);
        tracker.MisalignedShellClawsDiedThisTurn.Clear();
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Claw Calibration armed the healthier claw's next attack.");
    }
}
