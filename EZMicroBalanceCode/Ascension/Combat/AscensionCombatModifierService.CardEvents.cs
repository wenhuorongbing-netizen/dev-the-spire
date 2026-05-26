namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    public static async Task AfterCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardPlay cardPlay)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await AfterBannerCardPlayed(combatState, tracker, metadata.Banner!.Value, cardPlay);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealCardPlayed(combatState, tracker, metadata, cardPlay);
        }
    }

    public static async Task AfterEnergySpent(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card,
        int amount)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata) &&
            metadata.BossSeal?.Id == BossSealId.AeonglassHourglass)
        {
            await TrackAeonglassEnergySpent(combatState, tracker, card, amount);
        }
    }

    public static Task AfterCardEnteredHand(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return Task.CompletedTask;
        }

        if (card.Owner is { } owner)
        {
            TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, owner);
        }

        return Task.CompletedTask;
    }
}
