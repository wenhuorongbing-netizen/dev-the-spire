using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyMartyrOath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        var triggerCap = metadata.IsBossBrand ? 3 : 2;
        if (creature.Monster is not KinFollower || tracker.MartyrOathTriggers >= triggerCap)
        {
            return;
        }

        var priest = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KinPriest);
        if (priest == null)
        {
            return;
        }

        tracker.MartyrOathTriggers++;
        var block = metadata.IsBossBrand ? 14m : 12m;
        await CreatureCmd.GainBlock(priest, block, ValueProp.Move, null, fast: true);
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), priest, 1m, priest, null);
        if (priest.GetHpPercentRemaining() <= 0.5d)
        {
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), priest, 1m, priest, null);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Martyr Oath strengthened Kin Priest after a follower death.");
    }
}
