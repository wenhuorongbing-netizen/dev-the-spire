namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static CardModel? FindTrialPlantCard(Player player) =>
        PileType.Deck.GetPile(player).Cards.FirstOrDefault(card => AncientSavedStateFields.UrdaTrialPlantCard[card]);

    private static IEnumerable<CardModel> FindTrialBranchCards(Player player)
    {
        foreach (var card in PileType.Deck.GetPile(player).Cards.Where(card => AncientSavedStateFields.UrdaTrialPlantCard[card]))
        {
            yield return card;
        }

        if (player.PlayerCombatState == null)
        {
            yield break;
        }

        foreach (var card in player.PlayerCombatState.AllCards.Where(card =>
            card.DeckVersion is { } deckCard &&
            AncientSavedStateFields.UrdaTrialPlantCard[deckCard]))
        {
            yield return card;
        }
    }

    internal static bool TryGetTrialBranchDisplayProgress(
        Player player,
        out int combatsLeft,
        out int playedThisCombat,
        out int playsLeft)
    {
        combatsLeft = 0;
        playedThisCombat = 0;
        playsLeft = 0;
        var progress = GetProgress(player);
        if (progress.TrialSettled || FindTrialPlantCard(player) == null)
        {
            return false;
        }

        playedThisCombat = progress.TrialPlayedThisCombat ? 1 : 0;
        combatsLeft = Math.Max(0, TrialBranchCombats - progress.TrialCombats);
        playsLeft = Math.Max(0, TrialBranchRequiredSuccesses - progress.TrialSuccessfulCombats - playedThisCombat);
        return true;
    }

    private static void RefreshTrialBranchEnchantment(Player player)
    {
        if (!TryGetTrialBranchDisplayProgress(player, out var combatsLeft, out var playedThisCombat, out var playsLeft))
        {
            return;
        }

        foreach (var card in FindTrialBranchCards(player))
        {
            if (card.Enchantment is UrdaTrialBranchEnchantment enchantment)
            {
                enchantment.SetProgress(combatsLeft, playedThisCombat, playsLeft);
            }
        }
    }

    private static void ClearTrialBranchMarkerAndEnchantment(CardModel card)
    {
        AncientSavedStateFields.UrdaTrialPlantCard[card] = false;
        if (card.Enchantment is UrdaTrialBranchEnchantment)
        {
            CardCmd.ClearEnchantment(card);
        }
    }
}
