using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Ancient Writing event (Act 2): Elegance (upgrade card) or Simplicity (remove card).
/// </summary>
public sealed class Sts1AncientWriting : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Elegance, InitialOptionKey("ELEGANCE")),
            new EventOption(this, Simplicity, InitialOptionKey("SIMPLICITY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Elegance()
    {
        // TODO: Open card upgrade UI
        SetEventFinished(L10NLookup("STS1_ANCIENT_WRITING.pages.ELEGANCE.description"));
        return Task.CompletedTask;
    }

    private Task Simplicity()
    {
        // TODO: Open card removal UI
        SetEventFinished(L10NLookup("STS1_ANCIENT_WRITING.pages.SIMPLICITY.description"));
        return Task.CompletedTask;
    }
}
