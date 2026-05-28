using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Nest event (Act 2): search for relic but gain Parasite curses. A15: 3 curses instead of 2.
/// </summary>
public sealed class Sts1Nest : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Search, InitialOptionKey("SEARCH")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Search()
    {
        // TODO: Grant random relic, add Parasite curses (×2 normal, ×3 A15)
        SetEventFinished(L10NLookup("STS1_NEST.pages.SEARCH.description"));
        return Task.CompletedTask;
    }
}
