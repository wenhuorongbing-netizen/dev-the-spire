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
        var artifactBefore = giant.GetPower<ArtifactPower>()?.Amount ?? 0m;
        await ApplyPowerWithFinalDisplayedGain<ArtifactPower>(
            giant,
            10,
            giant,
            null);
        var artifactAfter = giant.GetPower<ArtifactPower>()?.Amount ?? 0m;
        tracker.BoilingExplosionArtifactAdded = Math.Max(0, (int)(artifactAfter - artifactBefore));

        // EXPLODE_MOVE reads the final attack modifiers. Remove only the effects
        // that would directly lower that explosion, then let Artifact block any
        // fresh attempts during the warning turn.
        var weak = giant.GetPower<WeakPower>();
        if (weak != null)
        {
            await PowerCmd.Remove(weak);
        }

        var strength = giant.GetPower<StrengthPower>();
        if (strength is { Amount: < 0 })
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), strength, -strength.Amount, giant, null);
        }

        await ApplyBoilingExplosionVulnerability(combatState, tracker, metadata, giant);

        MainFile.Logger.Info(
            "[Spire Plus] Ascension A19 applied: Boiling Critical fortified the explosion turn with Artifact, Vulnerable pressure, Weak cleanup, and negative-Strength cleanup.");
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
            $"[Spire Plus] Ascension A19 applied: Boiling Critical applied {vulnerable} Vulnerable before the explosion.");
    }

    private static async Task ClearBoilingExplosionFortification(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        if (!tracker.BoilingExplosionFortified)
        {
            return;
        }

        var artifactToRemove = tracker.BoilingExplosionArtifactAdded;
        tracker.BoilingExplosionFortified = false;
        tracker.BoilingExplosionArtifactAdded = 0;
        if (artifactToRemove <= 0)
        {
            return;
        }

        var giant = combatState.Enemies.FirstOrDefault(enemy => enemy.Monster is WaterfallGiant);
        var artifact = giant?.GetPower<ArtifactPower>();
        if (giant == null ||
            giant.IsDead ||
            artifact == null)
        {
            return;
        }

        var removal = -Math.Min(artifact.Amount, artifactToRemove);
        if (removal != 0)
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), artifact, removal, giant, null);
        }
    }
}
