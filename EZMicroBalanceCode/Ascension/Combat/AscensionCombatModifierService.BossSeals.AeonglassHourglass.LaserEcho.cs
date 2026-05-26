using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
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
}
