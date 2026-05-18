namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyBossSealPlayerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        tracker.MisalignedShellBlockedTargetsThisTurn.Clear();

        if (metadata.BossSeal?.Id == BossSealId.HolyDaze)
        {
            await TryApplyHolyDaze(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.InkReturn)
        {
            TrackInkReturnIfSlipperySpent(combatState, tracker);
        }

        if (metadata.BossSeal?.Id == BossSealId.StartledShell)
        {
            await TryApplyStartledShellFromWake(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.SoulTide)
        {
            await TrackSoulTideIntangible(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.BoilingCritical)
        {
            await TrackBoilingCriticalSteam(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.StruggleBait)
        {
            await TrackStruggleBaitObservations(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.ResidualSample)
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
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
                TrackInkReturnIfSlipperySpent(combatState, tracker);
                await ApplyInkReturnIfPending(combatState, tracker, metadata);
                break;
            case BossSealId.StartledShell:
                TrackStartledShellEnemyMove(combatState, tracker);
                break;
            case BossSealId.SoulTide:
                await ApplySoulTidePendingBlock(combatState, tracker);
                break;
            case BossSealId.BoilingCritical:
                await TrackBoilingCriticalSteam(combatState, tracker, metadata);
                await ApplyBoilingExplosionBlock(combatState, tracker, metadata);
                break;
            case BossSealId.MarginalNote:
                TrackKnowledgeDemonEnemyMove(combatState, tracker);
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
            await SettleMisalignedShellClawDeaths(combatState, tracker, metadata);
            await SettleMarginalNotes(combatState, metadata);
            await SettleStruggleBaitBrandEscapes(combatState, tracker, metadata);
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
                    break;
                case BossSealId.BoilingCritical:
                    await TrackBoilingCriticalSteam(combatState, tracker, metadata);
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
            }
        }
    }
}
