using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Transmogrifier event (Act 3): choose a card to transform.
/// </summary>
public sealed class Sts1Transmogrifier : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Transform, InitialOptionKey("TRANSFORM")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Transform()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardTransform(owner, Rng);
        SetEventFinished(L10NLookup("STS1_TRANSMOGRIFIER.pages.TRANSFORM.description"));
    }
}
