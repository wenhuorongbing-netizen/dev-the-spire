namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int TrialBranchCombats = 3;
    private const int TrialBranchRequiredSuccesses = 3;

    public static Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != UrdaBlessingIds.TrialBranch ||
            cardPlay.Card.DeckVersion is not { } deckCard ||
            !AncientSavedStateFields.UrdaTrialPlantCard[deckCard])
        {
            return Task.CompletedTask;
        }

        var progress = GetProgress(player);
        if (progress.TrialSettled || progress.TrialCombats >= TrialBranchCombats)
        {
            return Task.CompletedTask;
        }

        SetProgress(player, progress with { TrialPlayedThisCombat = true });
        RefreshTrialBranchEnchantment(player);
        return Task.CompletedTask;
    }
}
