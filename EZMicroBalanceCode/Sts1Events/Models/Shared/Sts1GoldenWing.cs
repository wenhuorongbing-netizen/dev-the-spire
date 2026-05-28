using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

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

    private Task Accept()
    {
        // TODO: Obtain a random rare card from player's class
        SetEventFinished(L10NLookup("STS1_GOLDEN_WING.pages.ACCEPT.description"));
        return Task.CompletedTask;
    }
}
