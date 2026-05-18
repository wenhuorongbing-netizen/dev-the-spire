using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TrackInkReturnFromDamage(AscensionCombatTracker tracker, Creature target)
    {
        if (tracker.InkReturnTriggered ||
            target.Monster is not Vantom ||
            target.GetPower<SlipperyPower>() is { Amount: > 0 })
        {
            return;
        }

        tracker.InkReturnTriggered = true;
        tracker.InkReturnPending = true;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 tracked: Ink Return will restore Slippery next enemy turn.");
    }

    private static void TrackInkReturnIfSlipperySpent(CombatState combatState, AscensionCombatTracker tracker)
    {
        var vantom = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom);
        if (vantom != null)
        {
            TrackInkReturnFromDamage(tracker, vantom);
        }
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
        var slippery = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<SlipperyPower>(new BlockingPlayerChoiceContext(), vantom, slippery, vantom, null);
        if (metadata.IsBossBrand)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), vantom, 1m, vantom, null);
        }

        var resultText = metadata.IsBossBrand
            ? "restored extra Slippery and granted Strength"
            : "restored Slippery";
        MainFile.Logger.Info($"[EZMicroBalance] Ascension A19 applied: Ink Return {resultText}.");
    }
}
