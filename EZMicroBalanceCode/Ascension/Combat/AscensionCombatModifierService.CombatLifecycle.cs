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
