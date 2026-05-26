namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyBossSealPlayerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Player player)
    {
        if (metadata.BossSeal?.Id == BossSealId.ChosenDecree)
        {
            ResetChosenDecreeRoundCaps(tracker, combatState.RoundNumber);
            TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, player);
        }

        if (metadata.BossSeal?.Id == BossSealId.HolyDaze)
        {
            await TryApplyHolyDaze(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.InkReturn)
        {
            TrackInkReturnIfSlipperySpent(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.StartledShell)
        {
            await TryApplyStartledShellFromWake(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.SoulTide)
        {
            await ApplySoulTidePendingBlock(combatState, tracker, metadata);
            await TrackSoulTideIntangible(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.BoilingCritical)
        {
            await TrackBoilingCriticalSteam(combatState, tracker, metadata);
            await ApplyBoilingExplosionFortification(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.StruggleBait)
        {
            await TrackStruggleBaitObservations(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.ResidualSample)
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.AeonglassHourglass)
        {
            await ArmAeonglassLaserEchoPreviewIfEligible(combatState, tracker, metadata);
        }
    }

    private static async Task ApplyBossSealSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CombatSide side)
    {
        if (side != CombatSide.Enemy || metadata.BossSeal == null)
        {
            return;
        }

        switch (metadata.BossSeal.Id)
        {
            case BossSealId.InkReturn:
                TrackInkReturnIfSlipperySpent(combatState, tracker, metadata);
                await ApplyInkReturnIfPending(combatState, tracker, metadata);
                break;
            case BossSealId.StartledShell:
                TrackStartledShellEnemyMove(combatState, tracker);
                break;
            case BossSealId.BoilingCritical:
                await TrackBoilingCriticalSteam(combatState, tracker, metadata);
                await ApplyBoilingExplosionFortification(combatState, tracker, metadata);
                break;
            case BossSealId.MarginalNote:
                TrackKnowledgeDemonEnemyMove(combatState, tracker);
                break;
            case BossSealId.AeonglassHourglass:
                TrackAeonglassEnemyMove(combatState, tracker);
                break;
            case BossSealId.ResidualSample:
                await TryApplyResidualSamples(combatState, tracker, metadata);
                break;
        }
    }

    private static async Task ApplyBossSealTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CombatSide side)
    {
        if (metadata.BossSeal == null)
        {
            return;
        }

        if (side == CombatSide.Player)
        {
            await EndHolyDaze(combatState, tracker);
            await SettleMisalignedShellCalibration(combatState, tracker, metadata);
            await SettleMarginalNotes(combatState, tracker, metadata);
            await SettleAeonglassTimeSand(combatState, tracker, metadata);
            await SettleChosenDecree(combatState, tracker, metadata);
        }
        else if (side == CombatSide.Enemy)
        {
            switch (metadata.BossSeal.Id)
            {
                case BossSealId.StartledShell:
                    await TryApplyStartledShellFromWake(combatState, tracker, metadata);
                    await SettleStartledShellSoulSiphon(combatState, tracker, metadata);
                    break;
                case BossSealId.SoulTide:
                    await TrackSoulTideIntangible(combatState, tracker, metadata);
                    // Apply here so Soul Fysh's Block is already visible when the player regains control.
                    await ApplySoulTidePendingBlock(combatState, tracker, metadata);
                    break;
                case BossSealId.BoilingCritical:
                    await TrackBoilingCriticalSteam(combatState, tracker, metadata);
                    await ClearBoilingExplosionFortification(combatState, tracker);
                    break;
                case BossSealId.StruggleBait:
                    await TrackStruggleBaitObservations(combatState, tracker, metadata);
                    break;
                case BossSealId.MarginalNote:
                    if (tracker.KnowledgeDemonCurseMoveActive)
                    {
                        tracker.KnowledgeDemonCurseMoveActive = false;
                        await AddMarginalNotes(combatState, metadata);
                    }

                    break;
                case BossSealId.AeonglassHourglass:
                    await ApplyAeonglassExtraWitherAfterIncreasingIntensity(combatState, tracker, metadata);
                    await ApplyAeonglassTimeSandAfterEbb(combatState, tracker, metadata);
                    break;
            }
        }

        // The Branded Form double-follower bonus is a same-turn reward. Reset on
        // every side-turn boundary so poison, thorns, or delayed damage cannot
        // carry one follower death into the next team turn.
        ResetMartyrOathTurnCounters(tracker);
    }
}
