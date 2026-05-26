using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
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
}
