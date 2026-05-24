using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static Creature? FindAeonglass(CombatState combatState) =>
        AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is Aeonglass);

    private static void HydrateAeonglassTimeSandFromVisiblePower(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        var aeonglass = FindAeonglass(combatState);
        if (aeonglass == null)
        {
            return;
        }

        if (tracker.AeonglassTimeSand <= 0)
        {
            var visibleTimeSand = (int)(aeonglass.GetPower<AeonglassHourglassPower>()?.Amount ?? 0m);
            if (visibleTimeSand > 0)
            {
                tracker.AeonglassTimeSand = visibleTimeSand;
                MainFile.Logger.Info($"[Spire Plus] Ascension A19 recovered Time Sand tracker from visible power: {visibleTimeSand}.");
            }
        }

        var pendingWither = (int)(aeonglass.GetPower<AeonglassPendingWitherPower>()?.Amount ?? 0m);
        if (pendingWither > tracker.AeonglassExtraWitherFromSands)
        {
            tracker.AeonglassExtraWitherFromSands = pendingWither;
            MainFile.Logger.Info($"[Spire Plus] Ascension A19 recovered pending Wither tracker from visible power: {pendingWither}.");
        }

        var usedEchoes = (int)(aeonglass.GetPower<AeonglassLaserEchoUseCounterPower>()?.Amount ?? 0m);
        if (usedEchoes > tracker.AeonglassLaserEchoesUsed)
        {
            tracker.AeonglassLaserEchoesUsed = usedEchoes;
            MainFile.Logger.Info($"[Spire Plus] Ascension A20 recovered Time Sand laser counter from hidden power: {usedEchoes}.");
        }
    }

    private static void TrackAeonglassEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        var aeonglass = FindAeonglass(combatState);
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
        var aeonglass = FindAeonglass(combatState);
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
        await ArmAeonglassLaserEchoPreviewIfEligible(combatState, tracker, metadata);
        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Time Sand Reflow created {tracker.AeonglassTimeSand} Time Sand after Ebb.");
    }

    private static async Task TrackAeonglassEnergySpent(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card,
        int amount)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata) ||
            metadata.BossSeal?.Id != BossSealId.AeonglassHourglass ||
            card.Owner?.IsActiveForHooks != true)
        {
            return;
        }

        HydrateAeonglassTimeSandFromVisiblePower(combatState, tracker);
        if (amount <= 0 ||
            tracker.AeonglassTimeSand <= 0)
        {
            return;
        }

        var spent = Math.Min(amount, tracker.AeonglassTimeSand);
        tracker.AeonglassTimeSand -= spent;
        var aeonglass = FindAeonglass(combatState);
        var timeSand = aeonglass?.GetPower<AeonglassHourglassPower>();
        if (timeSand != null)
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), timeSand, -spent, aeonglass, card);
            if (tracker.AeonglassTimeSand <= 0)
            {
                await PowerCmd.Remove(timeSand);
                if (aeonglass != null)
                {
                    await PowerCmd.Remove(aeonglass.GetPower<AeonglassLaserEchoPower>());
                    await RefreshEnemyIntent(aeonglass);
                }
            }
        }

        MainFile.Logger.Info($"[Spire Plus] Ascension A19 tracked: spent {spent} energy to clear Time Sand.");
    }

    private static async Task ArmAeonglassLaserEchoPreviewIfEligible(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        HydrateAeonglassTimeSandFromVisiblePower(combatState, tracker);
        if (!metadata.IsBossBrand ||
            tracker.AeonglassTimeSand <= 0 ||
            tracker.AeonglassLaserEchoesUsed >= 2)
        {
            return;
        }

        var aeonglass = FindAeonglass(combatState);
        if (aeonglass == null ||
            aeonglass.HasPower<AeonglassLaserEchoPower>() ||
            aeonglass.Monster?.NextMove?.StateId != "EYE_LASERS_MOVE")
        {
            return;
        }

        // The extra hit changes damage. Put the preview power on before the
        // player spends cards so the enemy intent reflects the true risk; remove
        // it later if the team clears all Time Sand before Eye Lasers starts.
        await PowerCmd.Apply<AeonglassLaserEchoPower>(
            new BlockingPlayerChoiceContext(),
            aeonglass,
            1m,
            aeonglass,
            null);
        await RefreshEnemyIntent(aeonglass);
    }

    private static async Task SettleAeonglassTimeSand(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        HydrateAeonglassTimeSandFromVisiblePower(combatState, tracker);
        if (metadata.BossSeal?.Id != BossSealId.AeonglassHourglass ||
            tracker.AeonglassTimeSand <= 0)
        {
            return;
        }

        var aeonglass = FindAeonglass(combatState);
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
            await ApplyAeonglassLaserEchoUseCounter(aeonglass, tracker);
            if (!aeonglass.HasPower<AeonglassLaserEchoPower>())
            {
                await PowerCmd.Apply<AeonglassLaserEchoPower>(
                    new BlockingPlayerChoiceContext(),
                    aeonglass,
                    1m,
                    aeonglass,
                    null);
            }

            await RefreshEnemyIntent(aeonglass);
        }

        var pendingWither = tracker.AeonglassTimeSand;
        tracker.AeonglassExtraWitherFromSands += pendingWither;
        tracker.AeonglassTimeSand = 0;
        if (pendingWither > 0)
        {
            await PowerCmd.Apply<AeonglassPendingWitherPower>(
                new BlockingPlayerChoiceContext(),
                aeonglass,
                pendingWither,
                aeonglass,
                null);
        }

        await PowerCmd.Remove(aeonglass.GetPower<AeonglassHourglassPower>());
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: remaining Time Sand will add extra Wither on the next Increasing Intensity.");
    }

    private static async Task ApplyAeonglassLaserEchoUseCounter(
        Creature aeonglass,
        AscensionCombatTracker tracker)
    {
        var recorded = aeonglass.GetPower<AeonglassLaserEchoUseCounterPower>()?.Amount ?? 0;
        var missing = tracker.AeonglassLaserEchoesUsed - recorded;
        if (missing > 0)
        {
            await PowerCmd.Apply<AeonglassLaserEchoUseCounterPower>(
                new BlockingPlayerChoiceContext(),
                aeonglass,
                missing,
                aeonglass,
                null);
        }
    }

    private static async Task ApplyAeonglassExtraWitherAfterIncreasingIntensity(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        HydrateAeonglassTimeSandFromVisiblePower(combatState, tracker);
        var aeonglass = FindAeonglass(combatState);
        if ((!tracker.AeonglassIncreasingIntensityMoveActive &&
                aeonglass?.Monster?.NextMove?.StateId != "INCREASING_INTENSITY_MOVE") ||
            tracker.AeonglassExtraWitherFromSands <= 0)
        {
            tracker.AeonglassIncreasingIntensityMoveActive = false;
            return;
        }

        tracker.AeonglassIncreasingIntensityMoveActive = false;
        if (aeonglass == null)
        {
            tracker.AeonglassExtraWitherFromSands = 0;
            return;
        }

        var extraWither = tracker.AeonglassExtraWitherFromSands;
        tracker.AeonglassExtraWitherFromSands = 0;
        await PowerCmd.Remove(aeonglass.GetPower<AeonglassPendingWitherPower>());
        var targets = combatState.Players
            .Where(player => player.Creature.IsAlive)
            .Select(player => player.Creature)
            .ToList();
        if (targets.Count == 0)
        {
            return;
        }

        await CardPileCmd.AddToCombatAndPreview<Wither>(targets, PileType.Discard, extraWither, null);
        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Time Sand Reflow added {extraWither} extra Wither on Increasing Intensity.");
    }
}
