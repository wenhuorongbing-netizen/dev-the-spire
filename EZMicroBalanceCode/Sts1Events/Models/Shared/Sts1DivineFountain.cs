using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Divine Fountain event: remove all Curses from your deck, or leave.
/// </summary>
public sealed class Sts1DivineFountain : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Pray()
    {
        if (Owner is not { } owner) return;
        var curses = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Type == MegaCrit.Sts2.Core.Entities.Cards.CardType.Curse)
                curses.Add(card);
        }
        if (curses.Count > 0)
            await CardPileCmd.RemoveFromDeck(curses, showPreview: false);
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.PRAY.description"));
    }
}
