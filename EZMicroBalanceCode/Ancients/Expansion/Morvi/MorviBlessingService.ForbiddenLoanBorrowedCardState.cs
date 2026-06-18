using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private static void MarkBorrowedAncientCard(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card] = true;

    private static void ClearBorrowedAncientCardMarker(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card] = false;

    private static void ClearBorrowedAncientCards(Player player)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player))
        {
            ClearBorrowedAncientCardMarker(card);
        }
    }

    private static bool IsBorrowedAncientDeckCard(CardModel card) =>
        HasBorrowedAncientCardMarker(card);

    private static bool IsBorrowedAncientCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard
            ? HasBorrowedAncientCardMarker(deckCard)
            : HasBorrowedAncientCardMarker(card);

    private static bool HasBorrowedAncientCardMarker(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card];
}
