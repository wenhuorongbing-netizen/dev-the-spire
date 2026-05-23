using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TrackAeonglassEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        var aeonglass = AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);
        var nextMoveId = aeonglass?.Monster?.NextMove?.StateId;
        tracker.AeonglassEbbMoveActive = nextMoveId == "EBB_MOVE";
        tracker.AeonglassIncreasingIntensityMoveActive = nextMoveId == "INCREASING_INTENSITY_MOVE";
    }

    private static async Task ApplyAeonglassTimeSandAfterEbb(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.AeonglassEbbMoveActive)
        {
            return;
        }

        tracker.AeonglassEbbMoveActive = false;
        var aeonglass = AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);
        if (aeonglass == null)
        {
            return;
        }

        tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2;
        await PowerCmd.Apply<AeonglassHourglassPower>(
            new BlockingPlayerChoiceContext(),
            aeonglass,
            tracker.AeonglassTimeSand,
            aeonglass,
            null);
        MainFile.Logger.Info($"[EZMicroBalance] Ascension A19 applied: Time Sand Reflow created {tracker.AeonglassTimeSand} Time Sand after Ebb.");
    }

    private static async Task TrackAeonglassEnergySpent(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card,
        int amount)
    {
        if (amount <= 0 ||
            tracker.AeonglassTimeSand <= 0 ||
            !TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata) ||
            metadata.BossSeal?.Id != BossSealId.AeonglassHourglass ||
            card.Owner?.IsActiveForHooks != true)
        {
            return;
        }

        var spent = Math.Min(amount, tracker.AeonglassTimeSand);
        tracker.AeonglassTimeSand -= spent;
        var aeonglass = AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);
        var timeSand = aeonglass?.GetPower<AeonglassHourglassPower>();
        if (timeSand != null)
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), timeSand, -spent, aeonglass, card);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Ascension A19 tracked: spent {spent} energy to clear Time Sand.");
    }

    private static async Task SettleAeonglassTimeSand(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.AeonglassHourglass ||
            tracker.AeonglassTimeSand <= 0)
        {
            return;
        }

        var aeonglass = AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);
        if (aeonglass == null)
        {
            tracker.AeonglassTimeSand = 0;
            return;
        }

        if (metadata.IsBossBrand &&
            tracker.AeonglassLaserEchoesUsed < 2 &&
            aeonglass.Monster?.NextMove?.StateId == "EYE_LASERS_MOVE")
        {
            tracker.AeonglassLaserEchoesUsed++;
            await PowerCmd.Apply<AeonglassLaserEchoPower>(
                new BlockingPlayerChoiceContext(),
                aeonglass,
                1m,
                aeonglass,
                null);
        }

        tracker.AeonglassExtraWitherFromSands += tracker.AeonglassTimeSand;
        tracker.AeonglassTimeSand = 0;
        await PowerCmd.Remove(aeonglass.GetPower<AeonglassHourglassPower>());
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: remaining Time Sand will add extra Wither on the next Increasing Intensity.");
    }

    private static async Task ApplyAeonglassExtraWitherAfterIncreasingIntensity(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.AeonglassIncreasingIntensityMoveActive ||
            tracker.AeonglassExtraWitherFromSands <= 0)
        {
            tracker.AeonglassIncreasingIntensityMoveActive = false;
            return;
        }

        tracker.AeonglassIncreasingIntensityMoveActive = false;
        var aeonglass = AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);
        if (aeonglass == null)
        {
            tracker.AeonglassExtraWitherFromSands = 0;
            return;
        }

        var extraWither = tracker.AeonglassExtraWitherFromSands;
        tracker.AeonglassExtraWitherFromSands = 0;
        var targets = combatState.Players
            .Where(player => player.Creature.IsAlive)
            .Select(player => player.Creature)
            .ToList();
        if (targets.Count == 0)
        {
            return;
        }

        await CardPileCmd.AddToCombatAndPreview<Wither>(targets, PileType.Discard, extraWither, null);
        MainFile.Logger.Info($"[EZMicroBalance] Ascension A19 applied: Time Sand Reflow added {extraWither} extra Wither on Increasing Intensity.");
    }
}
