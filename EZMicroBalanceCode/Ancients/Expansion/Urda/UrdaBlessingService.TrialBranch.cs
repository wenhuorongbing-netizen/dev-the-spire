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

    private static async Task ResolveTrialBranchCombat(Player player)
    {
        var progress = GetProgress(player);
        if (progress.TrialSettled || progress.TrialCombats >= TrialBranchCombats)
        {
            return;
        }

        var playedThisCombat = progress.TrialPlayedThisCombat;
        progress = progress with
        {
            TrialCombats = progress.TrialCombats + 1,
            TrialSuccessfulCombats = progress.TrialSuccessfulCombats + (playedThisCombat ? 1 : 0),
            TrialPlayedThisCombat = false
        };

        var trialCard = FindTrialPlantCard(player);
        if (!playedThisCombat)
        {
            if (trialCard != null)
            {
                ClearTrialBranchMarkerAndEnchantment(trialCard);
                await CardPileCmd.RemoveFromDeck(trialCard);
            }

            SetProgress(player, progress with
            {
                TrialPlayedThisCombat = false,
                TrialSettled = true
            });
            MainFile.Logger.Info(
                $"[Spire Plus] Urda Trial Branch failed after missed combat {progress.TrialCombats}/{TrialBranchCombats}; marked card removed from deck.");
            return;
        }

        if (progress.TrialCombats < TrialBranchCombats)
        {
            SetProgress(player, progress);
            RefreshTrialBranchEnchantment(player);
            MainFile.Logger.Info(
                $"[Spire Plus] Urda Trial Branch tracked required play {progress.TrialSuccessfulCombats}/{TrialBranchRequiredSuccesses}; combats={progress.TrialCombats}/{TrialBranchCombats}.");
            return;
        }

        if (progress.TrialSuccessfulCombats >= TrialBranchRequiredSuccesses)
        {
            if (trialCard != null)
            {
                ClearTrialBranchMarkerAndEnchantment(trialCard);
            }

            SetProgress(player, progress with { TrialSettled = true });
            MainFile.Logger.Info("[Spire Plus] Urda Trial Branch completed all three required plays; marker and enchantment cleared.");
            return;
        }

        if (trialCard != null)
        {
            ClearTrialBranchMarkerAndEnchantment(trialCard);
            await CardPileCmd.RemoveFromDeck(trialCard);
        }

        SetProgress(player, progress with { TrialSettled = true });
        MainFile.Logger.Info("[Spire Plus] Urda Trial Branch failed; marked card removed from deck.");
    }

}
