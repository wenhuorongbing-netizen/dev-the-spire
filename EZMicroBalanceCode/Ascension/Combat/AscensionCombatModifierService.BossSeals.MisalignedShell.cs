using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TryApplyMisalignedBackAttackBlock(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target,
        Creature? dealer)
    {
        if (dealer?.Player == null ||
            target.Monster is not Crusher and not Rocket ||
            !target.HasPower<BackAttackLeftPower>() && !target.HasPower<BackAttackRightPower>() ||
            !tracker.MisalignedShellBlockedTargetsThisTurn.Add(target))
        {
            return;
        }

        var block = metadata.IsBossBrand ? 8m : 6m;
        await CreatureCmd.GainBlock(target, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Misaligned Shell blocked the first back attack hit this turn.");
    }

    private static void TrackMisalignedShellClawDeath(AscensionCombatTracker tracker, Creature creature)
    {
        if (creature.Monster is Crusher or Rocket)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Add(creature);
        }
    }

    private static async Task SettleMisalignedShellClawDeaths(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (tracker.MisalignedShellArtifactApplied ||
            tracker.MisalignedShellClawsDiedThisTurn.Count != 1)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        var otherClaw = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Crusher or Rocket);
        if (otherClaw != null)
        {
            tracker.MisalignedShellArtifactApplied = true;
            var artifact = metadata.IsBossBrand ? 2m : 1m;
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), otherClaw, artifact, otherClaw, null);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Misaligned Shell gave Artifact to the surviving claw.");
        }

        tracker.MisalignedShellClawsDiedThisTurn.Clear();
    }
}
