namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    public static async Task ApplyMolting(Player player)
    {
        var progress = GetProgress(player);
        progress = progress with { MoltingActive = true };
        SetProgress(player, progress);

        var removedCards = new List<CardModel>();
        var strike = FindStarterCard(player, "Strike");
        if (strike != null)
        {
            removedCards.Add(strike);
        }

        var defend = FindStarterCard(player, "Defend");
        if (defend != null)
        {
            removedCards.Add(defend);
        }

        foreach (var card in removedCards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        var husks = new[]
        {
            player.RunState.CreateCard<WitheredHusk>(player),
            player.RunState.CreateCard<WitheredHusk>(player)
        };

        var addResults = new List<CardPileAddResult>();
        foreach (var husk in husks)
        {
            addResults.Add(await CardPileCmd.Add(husk, PileType.Deck));
        }

        SpirePlusFeedback.PreviewDeckAdds(addResults, player.GetRelic<UrdaMoltingOptionRelic>(), 2f);
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Molting applied: removed {removedCards.Count} starter card(s) and added 2 Withered Husk cards.");
    }

    private static CardModel? FindStarterCard(Player player, string prefix)
    {
        return PileType.Deck.GetPile(player).Cards.FirstOrDefault(card =>
            card.IsRemovable &&
            (card.GetType().Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
             card.Id.Entry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
             card.Id.Entry.Contains($"_{prefix}", StringComparison.OrdinalIgnoreCase)));
    }
}
