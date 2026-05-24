using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TrackStartledShellDamageStart(AscensionCombatTracker tracker, Creature target)
    {
        if (tracker.StartledShellApplied ||
            target.Monster is not LagavulinMatriarch ||
            !target.HasPower<AsleepPower>())
        {
            return;
        }

        tracker.StartledShellWakeByPlayerDamagePending = true;
    }

    private static void ClearStartledShellDamageStart(AscensionCombatTracker tracker)
    {
        tracker.StartledShellWakeByPlayerDamagePending = false;
    }

    private static async Task TryApplyStartledShellFromDamage(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target,
        DamageResult result)
    {
        if (result.UnblockedDamage <= 0m)
        {
            tracker.StartledShellWakeByPlayerDamagePending = false;
            return;
        }

        if (tracker.StartledShellApplied ||
            target.Monster is not LagavulinMatriarch)
        {
            return;
        }

        if (target.HasPower<AsleepPower>())
        {
            // AsleepPower may remove its starting Plating later in the same damage hook.
            // Defer the v4.1 early-wake Plating until the wake is observable.
            tracker.StartledShellWakeByPlayerDamagePending = true;
            return;
        }

        if (tracker.StartledShellWakeByPlayerDamagePending)
        {
            await ApplyStartledShellPlating(tracker, metadata, target, wokeFromPlayerDamage: true);
        }
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

        await ApplyStartledShellPlating(
            tracker,
            metadata,
            matriarch,
            tracker.StartledShellWakeByPlayerDamagePending);
    }

    private static async Task ApplyStartledShellPlating(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature matriarch,
        bool wokeFromPlayerDamage)
    {
        tracker.StartledShellApplied = true;
        tracker.StartledShellWakeByPlayerDamagePending = false;
        var platingAmount = wokeFromPlayerDamage
            ? metadata.IsBossBrand ? 6 : 4
            : metadata.IsBossBrand ? 10 : 8;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), matriarch, platingAmount, matriarch, null);
        var wakeSource = wokeFromPlayerDamage ? "early-wake" : "natural-wake";
        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Startled Shell added {wakeSource} Plating.");
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
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Startled Shell reduced Plating after Soul Siphon.");
    }
}
