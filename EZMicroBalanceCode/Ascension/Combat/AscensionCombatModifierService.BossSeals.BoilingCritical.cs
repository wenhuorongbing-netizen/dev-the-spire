using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackBoilingCriticalSteam(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var giant = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is WaterfallGiant);
        var steam = giant?.GetPower<SteamEruptionPower>();
        if (giant == null || steam == null || steam.Amount <= 0m)
        {
            return;
        }

        var steamThreshold = metadata.IsBossBrand ? 10m : 12m;
        var milestone = (int)Math.Floor(steam.Amount / steamThreshold);
        if (milestone <= tracker.LastSteamEruptionMilestone)
        {
            return;
        }

        var gained = milestone - tracker.LastSteamEruptionMilestone;
        tracker.LastSteamEruptionMilestone = milestone;
        await PowerCmd.Apply<BoilingCriticalPower>(new BlockingPlayerChoiceContext(), giant, gained, giant, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Boiling Critical gained a Boiling stack.");
    }

    private static async Task ApplyBoilingExplosionBlock(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (tracker.BoilingExplosionBlockGranted)
        {
            return;
        }

        var giant = AliveEnemies(combatState).FirstOrDefault(enemy =>
            enemy.Monster is WaterfallGiant &&
            enemy.Monster.NextMove.StateId == "EXPLODE_MOVE");
        var boiling = giant?.GetPower<BoilingCriticalPower>();
        if (giant == null || boiling == null || boiling.Amount <= 0)
        {
            return;
        }

        tracker.BoilingExplosionBlockGranted = true;
        var blockPerStack = metadata.IsBossBrand ? 1m : 2m;
        var block = boiling.Amount * blockPerStack;
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            await CreatureCmd.GainBlock(player.Creature, block, ValueProp.Move, null, fast: true);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Boiling Critical telegraphed the explosion with player Block.");
    }
}
