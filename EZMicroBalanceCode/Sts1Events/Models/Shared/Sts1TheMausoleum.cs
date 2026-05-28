using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Mausoleum event: 50/50 relic or curse, or leave. A15: always curse.
/// </summary>
public sealed class Sts1TheMausoleum : EventModel
{
    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Open, InitialOptionKey("OPEN")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Open()
    {
        if (HasA15)
        {
            // A15: always get curse
            // TODO: Add Wound curse to deck
            SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_CURSE.description"));
        }
        else
        {
            // 50/50 chance
            if (Rng.NextInt(0, 2) == 0)
            {
                // TODO: Grant random relic
                SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_RELIC.description"));
            }
            else
            {
                // TODO: Add Wound curse to deck
                SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_CURSE.description"));
            }
        }
        return Task.CompletedTask;
    }
}
