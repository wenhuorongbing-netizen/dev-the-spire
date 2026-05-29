using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 The Ghost event (Act 2): obtain a random rare card, or refuse.
/// </summary>
public sealed class Sts1TheGhost : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Accept, InitialOptionKey("ACCEPT")),
            new EventOption(this, null, InitialOptionKey("REFUSE"))
        };
    }

    private async Task Accept()
    {
        var options = new CardCreationOptions(
            new[] { Owner.Character.CardPool },
            CardCreationSource.Other,
            CardRarityOddsType.Uniform,
            (CardModel c) => c.Rarity == CardRarity.Rare
        ).WithFlags(CardCreationFlags.NoUpgradeRoll);

        var cards = CardFactory.CreateForReward(Owner, 1, options).Select(r => r.Card).ToList();
        if (cards.Count > 0)
            await CardPileCmd.Add(cards, PileType.Deck);
        SetEventFinished(L10NLookup("STS1_THE_GHOST.pages.ACCEPT.description"));
    }
}
