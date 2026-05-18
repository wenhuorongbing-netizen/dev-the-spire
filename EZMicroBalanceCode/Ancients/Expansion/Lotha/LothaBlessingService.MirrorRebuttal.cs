using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    internal static bool HasMirrorRebuttalCandidates(Player player) =>
        player.Deck.Cards.Any(card => card.Owner == player && IsMirrorRebuttalDeckCardCandidate(card));

    public static bool IsMirrorRebuttalDeckCardCandidate(CardModel card) =>
        card.Type is CardType.Attack or CardType.Skill or CardType.Power &&
        !card.HasBeenRemovedFromState;

    public static void MarkMirrorRebuttalCard(Player player, CardModel card)
    {
        ClearMirrorRebuttalMarkedCards(player);
        AncientSavedStateFields.LothaMirrorRebuttalCard[card] = true;
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal marked deck card {card.Id.Entry}.");
    }

    private static void ClearMirrorRebuttalMarkedCards(Player player)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player))
        {
            AncientSavedStateFields.LothaMirrorRebuttalCard[card] = false;
        }
    }

    private static async Task TryMoveMirrorRebuttalCardToHand(Player player)
    {
        var selectedCard = player.PlayerCombatState?.AllCards.FirstOrDefault(IsMirrorRebuttalCombatCard);
        if (selectedCard == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha Mirror Rebuttal skipped combat-start pull: selected deck card was not found in combat.");
            return;
        }

        if (selectedCard.Pile?.Type == PileType.Hand)
        {
            MainFile.Logger.Info("[EZMicroBalance] Lotha Mirror Rebuttal selected card already started in hand.");
            return;
        }

        if (selectedCard.Pile?.Type.IsCombatPile() != true)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha Mirror Rebuttal skipped combat-start pull: selected card is not in a combat pile.");
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
        {
            await CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top);
            MainFile.Logger.Warn($"[EZMicroBalance] Lotha Mirror Rebuttal could not move selected card {selectedCard.Id.Entry} into a full hand; placed it on top of draw pile instead.");
            return;
        }

        var addResult = await CardPileCmd.Add(selectedCard, PileType.Hand);
        if (addResult.cardAdded.Pile?.Type == PileType.Hand)
        {
            MainFile.Logger.Info($"[EZMicroBalance] Lotha Mirror Rebuttal moved selected card {selectedCard.Id.Entry} into hand.");
        }
        else
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Lotha Mirror Rebuttal tried to move selected card {selectedCard.Id.Entry} into hand but it ended in {addResult.cardAdded.Pile?.Type.ToString() ?? "no pile"}.");
        }
    }

    private static bool IsMirrorRebuttalCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard &&
        AncientSavedStateFields.LothaMirrorRebuttalCard[deckCard];
}
