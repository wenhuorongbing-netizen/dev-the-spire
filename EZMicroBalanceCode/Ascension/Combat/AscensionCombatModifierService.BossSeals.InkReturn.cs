using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TrackInkReturnFromDamage(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target)
    {
        if (target.Monster is not Vantom)
        {
            return;
        }

        ObserveInkReturnSlippery(tracker, metadata, target);
    }

    private static void TrackInkReturnIfSlipperySpent(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var vantom = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom);
        if (vantom != null)
        {
            ObserveInkReturnSlippery(tracker, metadata, vantom);
        }
    }

    private static void ObserveInkReturnSlippery(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target)
    {
        if (tracker.InkReturnTriggered)
        {
            return;
        }

        var currentSlippery = (int)(target.GetPower<SlipperyPower>()?.Amount ?? 0m);
        if (currentSlippery > 0)
        {
            tracker.InkReturnLastObservedSlippery = Math.Max(tracker.InkReturnLastObservedSlippery, currentSlippery);
            return;
        }

        if (tracker.InkReturnLastObservedSlippery <= 0)
        {
            return;
        }

        tracker.InkReturnTriggered = true;
        tracker.InkReturnPending = true;
        tracker.InkReturnRestoreAmount = CalculateInkReturnRestoreAmount(
            tracker.InkReturnLastObservedSlippery,
            metadata.IsBossBrand);
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A19 tracked: Ink Return will restore {tracker.InkReturnRestoreAmount} Slippery from {tracker.InkReturnLastObservedSlippery} cleared Slippery.");
    }

    private static int CalculateInkReturnRestoreAmount(int clearedSlippery, bool isBossBrand)
    {
        var ratio = isBossBrand ? 0.35m : 0.25m;
        var minimum = isBossBrand ? 5 : 3;
        var maximum = isBossBrand ? 18 : 12;
        var amount = (int)Math.Ceiling(clearedSlippery * ratio);
        return Math.Clamp(amount, minimum, maximum);
    }

    private static async Task ApplyInkReturnIfPending(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.InkReturnPending)
        {
            return;
        }

        var vantom = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom);
        if (vantom == null)
        {
            return;
        }

        tracker.InkReturnPending = false;
        var slippery = tracker.InkReturnRestoreAmount;
        if (slippery <= 0)
        {
            return;
        }

        // Slippery scales on enemies in multiplayer. Ink Return restores a
        // percentage of the displayed Slippery that was cleared, so correct back
        // to the final amount players should see.
        await ApplyPowerWithFinalDisplayedGain<SlipperyPower>(vantom, slippery, vantom, null);

        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Ink Return restored {slippery} final Slippery.");
    }
}
