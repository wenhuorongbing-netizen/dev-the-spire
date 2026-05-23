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
        // The Kin Boss source encounter contains exactly two KinFollower enemies.
        // Branded Form strengthens those real follower deaths instead of relying on
        // a third trigger that cannot happen unless the base game adds summons.
        const int triggerCap = 2;
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
        var strikeDamage = metadata.IsBossBrand ? 4m : 3m;
        await PowerCmd.Apply<MartyrOathPower>(new BlockingPlayerChoiceContext(), priest, 1m, priest, null);
        await PowerCmd.Apply<MartyrOathStrikePower>(new BlockingPlayerChoiceContext(), priest, strikeDamage, priest, null);
        await RefreshEnemyIntent(priest);

        tracker.MartyrOathFollowerDeathsThisTurn++;
        if (metadata.IsBossBrand &&
            tracker.MartyrOathFollowerDeathsThisTurn >= 2 &&
            !tracker.MartyrOathSameTurnArtifactGranted)
        {
            tracker.MartyrOathSameTurnArtifactGranted = true;
            await ApplyPowerWithFinalDisplayedGain<ArtifactPower>(priest, 1, priest, null);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Martyr Oath armed Kin Priest's next debuff or attack.");
    }

    private static void ResetMartyrOathTurnCounters(AscensionCombatTracker tracker)
    {
        tracker.MartyrOathFollowerDeathsThisTurn = 0;
        tracker.MartyrOathSameTurnArtifactGranted = false;
    }
}
