using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Purifier event: free card removal, or leave.
/// </summary>
public sealed class Sts1Purifier : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Purify, InitialOptionKey("PURIFY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Purify()
    {
        await Sts1EventHelpers.OpenCardRemoval(Owner);
        SetEventFinished(L10NLookup("STS1_PURIFIER.pages.PURIFY.description"));
    }
}
