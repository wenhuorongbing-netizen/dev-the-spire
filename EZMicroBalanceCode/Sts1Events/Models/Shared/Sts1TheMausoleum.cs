using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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

    private async Task Open()
    {
        if (HasA15 || Rng.NextInt(0, 2) != 0)
        {
            await Sts1EventHelpers.AddCurses<Wound>(Owner, 1);
            SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_CURSE.description"));
        }
        else
        {
            await Sts1EventHelpers.GrantRandomRelic(Owner);
            SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_RELIC.description"));
        }
    }
}
