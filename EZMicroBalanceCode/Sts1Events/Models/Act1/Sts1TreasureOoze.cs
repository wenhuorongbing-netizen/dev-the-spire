using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Treasure Ooze event (Act 1): pay gold for relic, fight for relic+gold, or leave.
/// </summary>
public sealed class Sts1TreasureOoze : EventModel
{
    private const int OfferCost = 50;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Offer, InitialOptionKey("OFFER")),
            new EventOption(this, Fight, InitialOptionKey("FIGHT")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Offer()
    {
        await PlayerCmd.GainGold(-OfferCost, Owner);
        // TODO: Grant random relic
        SetEventFinished(L10NLookup("STS1_TREASURE_OOZE.pages.OFFER.description"));
    }

    private Task Fight()
    {
        // TODO: Enter combat with large slime, reward: gold + relic
        SetEventFinished(L10NLookup("STS1_TREASURE_OOZE.pages.FIGHT.description"));
        return Task.CompletedTask;
    }
}
