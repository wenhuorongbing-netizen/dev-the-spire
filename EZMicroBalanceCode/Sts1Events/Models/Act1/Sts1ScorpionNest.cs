using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Scorpion Nest event (Act 1): fight 3 Louses for a relic, or leave.
/// </summary>
public sealed class Sts1ScorpionNest : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Investigate, InitialOptionKey("INVESTIGATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Investigate()
    {
        // TODO: Enter combat with 3 Louses, reward: random relic
        SetEventFinished(L10NLookup("STS1_SCORPION_NEST.pages.INVESTIGATE.description"));
        return Task.CompletedTask;
    }
}
