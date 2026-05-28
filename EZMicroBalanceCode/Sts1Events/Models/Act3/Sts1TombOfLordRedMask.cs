using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Tomb of Lord Red Mask event (Act 3): offer 50g or all gold for a relic.
/// </summary>
public sealed class Sts1TombOfLordRedMask : EventModel
{
    private const int Offer50Cost = 50;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Offer50, InitialOptionKey("OFFER_50")),
            new EventOption(this, OfferAll, InitialOptionKey("OFFER_ALL")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Offer50()
    {
        // TODO: Deduct 50g, grant random relic
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description"));
        return Task.CompletedTask;
    }

    private Task OfferAll()
    {
        // TODO: Set gold to 0, grant random relic
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description"));
        return Task.CompletedTask;
    }
}
