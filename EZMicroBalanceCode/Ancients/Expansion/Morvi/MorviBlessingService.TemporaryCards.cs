namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private static readonly HashSet<string> TemporaryCardIds =
    [
        MorviArchiveDrawPage.CardId,
        MorviArchiveVeilPage.CardId,
        MorviArchiveBurnPage.CardId,
        MorviArchiveDiscountPage.CardId,
        MorviArchiveBraveryPage.CardId,
        MorviArchiveDexterityPage.CardId,
        MorviRedInkOverdraftCard.CardId,
        MorviWastePaper.CardId
    ];

    private static async Task CleanupMorviTemporaryCards(Player player)
    {
        var cards = player.PlayerCombatState?.AllCards
            .Where(card => card.Pile?.Type.IsCombatPile() == true && TemporaryCardIds.Contains(card.Id.Entry))
            .ToList();
        if (cards is { Count: > 0 })
        {
            await CardPileCmd.RemoveFromCombat(cards, skipVisuals: true);
        }

        ClearOpenBookMarkers(player);
    }
}
