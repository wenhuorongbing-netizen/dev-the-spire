using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task AfterBossSealHpChanged(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature,
        decimal delta)
    {
        if (metadata.BossSeal?.Id == BossSealId.StruggleBait &&
            creature.Monster is TheInsatiable &&
            delta > 0m)
        {
            await AddStruggleBaitEscape(combatState, tracker, metadata);
        }
    }

    private static async Task AfterBossSealDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target,
        DamageResult result,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (result.UnblockedDamage <= 0m && result.TotalDamage <= 0m)
        {
            return;
        }

        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.HolyDaze:
                await TryApplyHolyDaze(combatState, tracker, metadata);
                break;
            case BossSealId.InkReturn:
                TrackInkReturnFromDamage(tracker, target);
                break;
            case BossSealId.StartledShell:
                await TryApplyStartledShellFromDamage(tracker, metadata, target);
                break;
            case BossSealId.MisalignedShell:
                await TryApplyMisalignedBackAttackBlock(tracker, metadata, target, dealer);
                break;
        }
    }

    private static async Task AfterBossSealDeath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.MartyrOath:
                await ApplyMartyrOath(combatState, tracker, metadata, creature);
                break;
            case BossSealId.MisalignedShell:
                TrackMisalignedShellClawDeath(tracker, creature);
                break;
            case BossSealId.ResidualSample:
                await TrackResidualSamplePhase(combatState, tracker, metadata, creature);
                break;
        }
    }

    private static Task AfterBossSealCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardPlay cardPlay)
    {
        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.StruggleBait:
                if (cardPlay.Card is FranticEscape)
                {
                    tracker.FranticEscapesPlayed++;
                    tracker.StruggleBaitBrandEscapeAges.Remove(cardPlay.Card);
                }

                break;
            case BossSealId.ChosenDecree:
                TrackChosenDecreePlayed(tracker, cardPlay.Card);
                break;
        }

        if (cardPlay.Card is MarginalNote)
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 tracked: Marginal Note was played.");
        }

        return Task.CompletedTask;
    }
}
