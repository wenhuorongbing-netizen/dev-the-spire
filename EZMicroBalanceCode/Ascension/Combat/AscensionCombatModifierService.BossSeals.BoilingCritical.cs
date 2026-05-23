using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackBoilingCriticalSteam(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        await Task.CompletedTask;
    }

    private static async Task ApplyBoilingExplosionFortification(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (tracker.BoilingExplosionFortified)
        {
            return;
        }

        var giant = AliveEnemies(combatState).FirstOrDefault(enemy =>
            enemy.Monster is WaterfallGiant &&
            enemy.Monster.NextMove.StateId == "EXPLODE_MOVE");
        if (giant == null)
        {
            return;
        }

        tracker.BoilingExplosionFortified = true;
        await ApplyPowerWithFinalDisplayedGain<ArtifactPower>(
            giant,
            10,
            giant,
            null);

        // The Waterfall Giant's explosion turn is source-backed by EXPLODE_MOVE; clear debuffs here,
        // before the player warning turn can carry Weak, negative Strength, or similar effects into the blast.
        var debuffs = giant.Powers
            .Where(power => power.GetTypeForAmount(power.Amount) == PowerType.Debuff)
            .ToList();
        foreach (var debuff in debuffs)
        {
            await PowerCmd.Remove(debuff);
        }

        var strength = giant.GetPower<StrengthPower>();
        if (strength is { Amount: < 0 })
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), strength, -strength.Amount, giant, null);
        }

        await ApplyBoilingExplosionVulnerability(combatState, tracker, metadata, giant);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A19 applied: Boiling Critical fortified the explosion turn with Artifact, Vulnerable pressure, and cleared {debuffs.Count} debuff(s).");
    }

    private static async Task ApplyBoilingExplosionVulnerability(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature giant)
    {
        if (tracker.BoilingExplosionVulnerabilityRound == combatState.RoundNumber)
        {
            return;
        }

        tracker.BoilingExplosionVulnerabilityRound = combatState.RoundNumber;
        var vulnerable = metadata.IsBossBrand ? 2m : 1m;
        foreach (var player in combatState.Players.Where(player => player.Creature.IsAlive))
        {
            await PowerCmd.Apply<VulnerablePower>(new BlockingPlayerChoiceContext(), player.Creature, vulnerable, giant, null);
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A19 applied: Boiling Critical applied {vulnerable} Vulnerable before the explosion.");
    }
}
