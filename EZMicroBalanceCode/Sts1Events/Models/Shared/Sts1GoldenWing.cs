using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Golden Wing event: obtain a random rare card, or leave.
/// </summary>
public sealed class Sts1GoldenWing : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Accept, InitialOptionKey("ACCEPT")),
            new EventOption(this, null, InitialOptionKey("DECLINE"))
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
            await CardPileCmd.Add(cards, MegaCrit.Sts2.Core.Entities.Cards.PileType.Deck);
        SetEventFinished(L10NLookup("STS1_GOLDEN_WING.pages.ACCEPT.description"));
    }
}
