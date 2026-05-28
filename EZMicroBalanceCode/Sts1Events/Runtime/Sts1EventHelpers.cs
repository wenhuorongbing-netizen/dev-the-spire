using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Shared helper methods for StS1 event implementations.
/// </summary>
internal static class Sts1EventHelpers
{
    /// <summary>
    /// Grant a random relic to the player.
    /// </summary>
    public static async Task GrantRandomRelic(Player owner)
    {
        var relic = RelicFactory.PullNextRelicFromFront(owner).ToMutable();
        await RelicCmd.Obtain(relic, owner);
    }

    /// <summary>
    /// Grant a random rare relic to the player.
    /// </summary>
    public static async Task GrantRandomRareRelic(Player owner)
    {
        var relic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare).ToMutable();
        await RelicCmd.Obtain(relic, owner);
    }

    /// <summary>
    /// Grant a random potion to the player.
    /// </summary>
    public static async Task GrantRandomPotion(Player owner, Rng rng)
    {
        var potion = PotionFactory.CreateRandomPotionOutOfCombat(owner, rng).ToMutable();
        await PotionCmd.TryToProcure(potion, owner);
    }

    /// <summary>
    /// Open card removal UI for the player to remove a card.
    /// </summary>
    public static async Task OpenCardRemoval(Player owner, int count = 1)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, count);
        var cards = (await CardSelectCmd.FromDeckForRemoval(prefs: prefs, player: owner)).ToList();
        if (cards.Count > 0)
            await CardPileCmd.RemoveFromDeck(cards);
    }

    /// <summary>
    /// Open card upgrade UI for the player to upgrade a card.
    /// </summary>
    public static async Task OpenCardUpgrade(Player owner, int count = 1)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, count);
        var cards = (await CardSelectCmd.FromDeckForUpgrade(owner, prefs)).ToList();
        foreach (var card in cards)
            CardCmd.Upgrade(card);
    }

    /// <summary>
    /// Open card transform UI for the player to transform a card.
    /// </summary>
    public static async Task OpenCardTransform(Player owner, Rng rng, int count = 1)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, count);
        var cards = (await CardSelectCmd.FromDeckForTransformation(owner, prefs)).ToList();
        foreach (var card in cards)
            await CardCmd.TransformToRandom(card, rng, CardPreviewStyle.EventLayout);
    }

    /// <summary>
    /// Remove all curses from the player's deck.
    /// </summary>
    public static async Task RemoveAllCurses(Player owner)
    {
        var curses = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Type == CardType.Curse)
                curses.Add(card);
        }
        if (curses.Count > 0)
            await CardPileCmd.RemoveFromDeck(curses, showPreview: false);
    }

    /// <summary>
    /// Remove all cards with a specific tag from the player's deck.
    /// </summary>
    public static async Task RemoveCardsByTag(Player owner, CardTag tag)
    {
        var cards = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Tags.Contains(tag))
                cards.Add(card);
        }
        if (cards.Count > 0)
            await CardPileCmd.RemoveFromDeck(cards, showPreview: false);
    }

    /// <summary>
    /// Add curse cards to the player's deck.
    /// </summary>
    public static async Task AddCurses<T>(Player owner, int count) where T : CardModel
    {
        var curses = new List<CardModel>();
        for (int i = 0; i < count; i++)
            curses.Add(ModelDb.Card<T>());
        await CardPileCmd.AddCursesToDeck(curses, owner);
    }
}
