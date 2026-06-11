using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Divine Fountain event: remove all Curses from your deck, or leave.
/// </summary>
public sealed class Sts1DivineFountain : EventModel
{
    public override bool IsShared => true;

    public override bool IsAllowed(IRunState runState)
    {
        foreach (var player in runState.Players)
        {
            if (!HasCurse(player))
                return false;
        }

        return runState.Players.Count > 0;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Drink, InitialOptionKey("DRINK")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Drink()
    {
        if (Owner is not { } owner) return;
        var curses = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Type == CardType.Curse)
                curses.Add(card);
        }
        if (curses.Count > 0)
            await CardPileCmd.RemoveFromDeck(curses, showPreview: false);
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.DRINK.description"));
    }

    private static bool HasCurse(Player player)
    {
        foreach (var card in player.Deck.Cards)
        {
            if (card.Type == CardType.Curse)
                return true;
        }

        return false;
    }
}
