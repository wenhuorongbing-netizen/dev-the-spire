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
        await Task.CompletedTask;
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
        if (metadata.BossSeal?.Id == BossSealId.StartledShell && result.UnblockedDamage <= 0m)
        {
            ClearStartledShellDamageStart(tracker);
        }

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
                TrackInkReturnFromDamage(tracker, metadata, target);
                break;
            case BossSealId.StartledShell:
                await TryApplyStartledShellFromDamage(tracker, metadata, target, result);
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

    private static async Task AfterBossSealCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardPlay cardPlay)
    {
        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.StruggleBait:
                if (cardPlay.Card is FranticEscape escape)
                {
                    await TrackRoyalEscapePlayed(combatState, tracker, metadata, escape);
                }

                break;
            case BossSealId.ChosenDecree:
                TrackChosenDecreePlayed(tracker, cardPlay.Card);
                break;
            case BossSealId.ResidualSample:
                TrackResidualSampleCardPlayed(tracker, cardPlay.Card);
                break;
        }

        if (cardPlay.Card is MarginalNote)
        {
            MainFile.Logger.Info("[Spire Plus] Ascension A19 tracked: Marginal Note was played.");
        }
    }
}
