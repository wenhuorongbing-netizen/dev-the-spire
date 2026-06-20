namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class AncientPlayerState
{
    public static string Get(
        Player player,
        SavedAttachedState<Player, string> runtimeField,
        SavedAttachedState<CardModel, string> deckField)
    {
        var runtimeState = runtimeField[player] ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(runtimeState))
        {
            MirrorToDeck(player, deckField, runtimeState);
            return runtimeState;
        }

        var deckState = ReadFromDeck(player, deckField);
        if (string.IsNullOrWhiteSpace(deckState))
        {
            return string.Empty;
        }

        runtimeField[player] = deckState;
        MirrorToDeck(player, deckField, deckState);
        return deckState;
    }

    public static void Set(
        Player player,
        string state,
        SavedAttachedState<Player, string> runtimeField,
        SavedAttachedState<CardModel, string> deckField)
    {
        runtimeField[player] = state;
        MirrorToDeck(player, deckField, state);
    }

    public static void SyncDeck(
        Player player,
        SavedAttachedState<Player, string> runtimeField,
        SavedAttachedState<CardModel, string> deckField)
    {
        var state = Get(player, runtimeField, deckField);
        if (!string.IsNullOrWhiteSpace(state))
        {
            MirrorToDeck(player, deckField, state);
        }
    }

    private static string ReadFromDeck(
        Player player,
        SavedAttachedState<CardModel, string> deckField)
    {
        return player.Deck.Cards
            .Where(card => card.Owner == player && !card.HasBeenRemovedFromState)
            .Select(card => deckField[card] ?? string.Empty)
            .FirstOrDefault(state => !string.IsNullOrWhiteSpace(state)) ?? string.Empty;
    }

    private static void MirrorToDeck(
        Player player,
        SavedAttachedState<CardModel, string> deckField,
        string state)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player && !card.HasBeenRemovedFromState))
        {
            deckField[card] = state;
        }
    }
}
