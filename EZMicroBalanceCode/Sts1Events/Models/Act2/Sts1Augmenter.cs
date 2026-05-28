using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Augmenter event (Act 2): Transform 2 cards, Mutate (upgrade), or Reject.
/// </summary>
public sealed class Sts1Augmenter : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Transform, InitialOptionKey("TRANSFORM")),
            new EventOption(this, Mutate, InitialOptionKey("MUTATE")),
            new EventOption(this, null, InitialOptionKey("REJECT"))
        };
    }

    private Task Transform()
    {
        // TODO: Transform 2 random cards
        SetEventFinished(L10NLookup("STS1_AUGMENTER.pages.TRANSFORM.description"));
        return Task.CompletedTask;
    }

    private Task Mutate()
    {
        // TODO: Open card upgrade UI
        SetEventFinished(L10NLookup("STS1_AUGMENTER.pages.MUTATE.description"));
        return Task.CompletedTask;
    }
}
