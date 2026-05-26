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
}
