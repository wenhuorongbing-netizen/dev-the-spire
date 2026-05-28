using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

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

    private Task Accept()
    {
        // TODO: Grant random rare card
        SetEventFinished(L10NLookup("STS1_THE_GHOST.pages.ACCEPT.description"));
        return Task.CompletedTask;
    }
}
