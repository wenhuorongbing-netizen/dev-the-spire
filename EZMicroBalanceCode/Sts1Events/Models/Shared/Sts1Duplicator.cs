using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Duplicator event: choose a card to duplicate, or leave.
/// </summary>
public sealed class Sts1Duplicator : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Duplicate, InitialOptionKey("DUPLICATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Duplicate()
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DuplicateSelectionPrompt, 1);
        var cards = (await CardSelectCmd.FromDeckForRewards(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.BlockingPlayerChoiceContext(),
            Owner.Deck.Cards.ToList(), Owner, prefs)).ToList();
        if (cards.Count > 0)
        {
            await CardPileCmd.Add(cards[0].ToMutable(), MegaCrit.Sts2.Core.Entities.Cards.PileType.Deck);
        }
        SetEventFinished(L10NLookup("STS1_DUPLICATOR.pages.DUPLICATE.description"));
    }
}
