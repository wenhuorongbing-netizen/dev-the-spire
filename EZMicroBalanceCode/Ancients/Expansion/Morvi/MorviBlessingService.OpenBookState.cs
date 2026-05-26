using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private static List<CardModel> FindOpenBookSealedCards(Player player, MorviCombatState combatState)
    {
        var cards = combatState.OpenBookSealedCards
            .Concat(player.PlayerCombatState?.AllCards.Where(card => AncientSavedStateFields.MorviOpenBookSealedCard[card]) ?? [])
            .Where(card => !card.HasBeenRemovedFromState)
            .Distinct()
            .ToList();

        combatState.OpenBookSealedCards.Clear();
        combatState.OpenBookSealedCards.AddRange(cards);
        if (cards.Count > 0)
        {
            ReleaseEvidenceLog.Log(
                "MorviState",
                "open_book_restore",
                player,
                new Dictionary<string, object?>
                {
                    ["sealed"] = cards.Count
                });
        }

        return cards;
    }

    private static void ClearOpenBookMarkers(Player player)
    {
        foreach (var card in player.PlayerCombatState?.AllCards ?? [])
        {
            AncientSavedStateFields.MorviOpenBookSealedCard[card] = false;
        }
    }
}
