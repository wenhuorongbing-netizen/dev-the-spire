using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TryApplyStartledShellFromDamage(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target)
    {
        if (tracker.StartledShellApplied ||
            target.Monster is not LagavulinMatriarch ||
            target.HasPower<AsleepPower>())
        {
            return;
        }

        tracker.StartledShellApplied = true;
        var plating = metadata.IsBossBrand ? 6 : 4;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), target, plating, target, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell added early-wake Plating.");
    }

    private static async Task TryApplyStartledShellFromWake(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var matriarch = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is LagavulinMatriarch);
        if (tracker.StartledShellApplied ||
            matriarch == null ||
            matriarch.HasPower<AsleepPower>())
        {
            return;
        }

        tracker.StartledShellApplied = true;
        var platingAmount = metadata.IsBossBrand ? 10 : 8;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), matriarch, platingAmount, matriarch, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell added wake Plating.");
    }

    private static void TrackStartledShellEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        tracker.StartledShellSoulSiphonTurn = AliveEnemies(combatState)
            .Any(enemy => enemy.Monster is LagavulinMatriarch &&
                enemy.Monster.NextMove.StateId == "SOUL_SIPHON_MOVE");
    }

    private static async Task SettleStartledShellSoulSiphon(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.StartledShellSoulSiphonTurn || tracker.SoulSiphonShellReduced)
        {
            tracker.StartledShellSoulSiphonTurn = false;
            return;
        }

        tracker.StartledShellSoulSiphonTurn = false;
        var matriarch = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is LagavulinMatriarch);
        var plating = matriarch?.GetPower<PlatingPower>();
        if (plating == null || plating.Amount <= 1)
        {
            return;
        }

        tracker.SoulSiphonShellReduced = true;
        var divisor = metadata.IsBossBrand ? 3m : 2m;
        await PowerCmd.ModifyAmount(
            new BlockingPlayerChoiceContext(),
            plating,
            -Math.Floor(plating.Amount / divisor),
            matriarch,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell reduced Plating after Soul Siphon.");
    }
}
