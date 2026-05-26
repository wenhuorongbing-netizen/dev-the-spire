namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    public static async Task AfterPlayerTurnStart(CombatState combatState, AscensionCombatTracker tracker, Player player)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealPlayerTurnStart(combatState, tracker, metadata, player);
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            RefreshFiremarkRoundDamageTracking(combatState, tracker);
            await ApplyFiremarkPlayerTurnStart(combatState, tracker, metadata.Firemark!.Value);
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await ApplyBannerTurnStart(combatState, tracker, metadata.Banner!.Value);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }
    }

    public static Task BeforeTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return Task.CompletedTask;
        }

        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return Task.CompletedTask;
        }

        foreach (var player in participants.Select(participant => participant.Player).OfType<Player>())
        {
            TrackSoulTideBeckonsBeforePlayerTurnEnd(combatState, tracker, metadata, player);
        }

        return Task.CompletedTask;
    }

    public static async Task BeforeSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (side == CombatSide.Player)
        {
            if (metadata.BossSeal?.Id == BossSealId.SoulTide)
            {
                // Soul Tide reads Beckons at player turn end, waits through
                // Soul Fysh's enemy turn, then grants Block exactly as the
                // next player turn starts so the player can see and answer it.
                await ApplySoulTidePendingBlock(combatState, tracker, metadata);
            }

            return;
        }

        if (side != CombatSide.Enemy)
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealSideTurnStart(combatState, tracker, metadata, side);
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            await ApplyFiremarkSideTurnStart(combatState, tracker, metadata.Firemark!.Value, side);
        }
    }

    public static async Task AfterTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealTurnEnd(combatState, tracker, metadata, side);
        }

        if (HasActiveBanner(combatState, metadata) &&
            metadata.Banner == BannerKind.BloodPrize &&
            side == CombatSide.Player)
        {
            await ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: true);
        }

        if (HasActiveBanner(combatState, metadata) &&
            metadata.Banner == BannerKind.PressingLine &&
            side == CombatSide.Player)
        {
            await ResolvePressingLineTurnEnd(combatState, tracker);
        }

        if (HasActiveBanner(combatState, metadata) &&
            metadata.Banner == BannerKind.Shieldwall &&
            side == CombatSide.Enemy)
        {
            await ApplyShieldwallTurnBlock(combatState, tracker);
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            await ApplyFiremarkTurnEnd(combatState, tracker, metadata.Firemark!.Value, side);
        }
    }
}
