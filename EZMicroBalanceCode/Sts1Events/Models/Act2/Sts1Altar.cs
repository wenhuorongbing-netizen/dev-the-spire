using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Altar event (Act 2): pray (upgrade 3 random cards) or sacrifice (remove card + relic).
/// </summary>
public sealed class Sts1Altar : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, Sacrifice, InitialOptionKey("SACRIFICE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Pray()
    {
        // TODO: Upgrade 3 random cards
        SetEventFinished(L10NLookup("STS1_ALTAR.pages.PRAY.description"));
        return Task.CompletedTask;
    }

    private Task Sacrifice()
    {
        // TODO: Remove a card from deck, grant random relic
        SetEventFinished(L10NLookup("STS1_ALTAR.pages.SACRIFICE.description"));
        return Task.CompletedTask;
    }
}
