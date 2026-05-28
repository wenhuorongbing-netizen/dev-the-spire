using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Sensory Stone event (Act 3): choose 1 of 3 rare cards.
/// </summary>
public sealed class Sts1SensoryStone : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Touch, InitialOptionKey("TOUCH")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Touch()
    {
        // TODO: Show card selection UI (1 of 3 rare cards)
        SetEventFinished(L10NLookup("STS1_SENSORY_STONE.pages.TOUCH.description"));
        return Task.CompletedTask;
    }
}
