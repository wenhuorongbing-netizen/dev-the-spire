using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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

    private async Task Pray()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardUpgrade(owner, count: 3);
        SetEventFinished(L10NLookup("STS1_ALTAR.pages.PRAY.description"));
    }

    private async Task Sacrifice()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardRemoval(owner);
        await Sts1EventHelpers.GrantRandomRelic(owner);
        SetEventFinished(L10NLookup("STS1_ALTAR.pages.SACRIFICE.description"));
    }
}
