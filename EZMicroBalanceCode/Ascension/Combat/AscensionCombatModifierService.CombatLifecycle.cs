namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    public static async Task BeforeCombatStart(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (tracker.CombatModifiersInitialized)
        {
            return;
        }

        tracker.CombatModifiersInitialized = true;
        tracker.NodeMetadata = AscensionMapService.TryGetCurrentMetadata(combatState.RunState);

        var metadata = tracker.NodeMetadata;
        if (metadata == null)
        {
            return;
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            await ApplyFiremarkCombatStart(combatState, tracker, metadata.Firemark!.Value);
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await ApplyBannerCombatStart(combatState, tracker, ResolveBannerForCombat(combatState, metadata));
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealCombatStart(combatState, metadata);
        }
    }

    public static async Task AfterPlayerTurnStart(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealPlayerTurnStart(combatState, tracker, metadata);
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            if (tracker.FiremarkDamageTrackingRound != combatState.RoundNumber)
            {
                tracker.FiremarkDamageTrackingRound = combatState.RoundNumber;
                tracker.FiremarkDamageThisPlayerTurn = 0m;
                tracker.FiremarkDamageThisEnemyCycle = 0m;
            }
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

    public static async Task AfterShuffle(CombatState combatState, AscensionCombatTracker tracker, Player shuffler)
    {
        if (tracker.ChaosApplied)
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }
    }

    public static Task BeforeFlush(CombatState combatState, AscensionCombatTracker tracker, Player player)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return Task.CompletedTask;
        }

        TrackSoulTideBeckonsBeforeFlush(combatState, tracker, metadata, player);
        return Task.CompletedTask;
    }

    public static async Task BeforeSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealSideTurnStart(combatState, tracker, metadata, side);
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

    public static async Task AfterCombatEnd(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            TryAddBountyReward(combatState, tracker, metadata.Banner!.Value);
        }

        if (!tracker.ForgeTokenAwarded && HasActiveFiremark(combatState, metadata))
        {
            tracker.ForgeTokenAwarded = true;
            await ForgeTokenService.GrantAfterFiremarkedElite(combatState);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyA20CourtyardRecovery(combatState, tracker, metadata);
        }
    }
}
