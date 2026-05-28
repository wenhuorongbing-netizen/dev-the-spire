using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 N'loth event (Act 2): give up a relic to get a random relic.
/// </summary>
public sealed class Sts1Nloth : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Offer, InitialOptionKey("OFFER")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Offer()
    {
        // TODO: Open relic selection UI (choose relic to give up), then grant random relic
        SetEventFinished(L10NLookup("STS1_NLOTH.pages.OFFER.description"));
        return Task.CompletedTask;
    }
}
