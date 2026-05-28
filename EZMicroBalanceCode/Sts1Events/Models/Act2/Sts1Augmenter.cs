using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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

    private async Task Transform()
    {
        await Sts1EventHelpers.OpenCardTransform(Owner, Rng, count: 2);
        SetEventFinished(L10NLookup("STS1_AUGMENTER.pages.TRANSFORM.description"));
    }

    private async Task Mutate()
    {
        await Sts1EventHelpers.OpenCardUpgrade(Owner);
        SetEventFinished(L10NLookup("STS1_AUGMENTER.pages.MUTATE.description"));
    }
}
