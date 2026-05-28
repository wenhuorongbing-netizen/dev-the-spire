using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Duplicator event: choose a card to duplicate, or leave.
/// </summary>
public sealed class Sts1Duplicator : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Duplicate, InitialOptionKey("DUPLICATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Duplicate()
    {
        // TODO: Open card selection UI to choose a card to copy
        SetEventFinished(L10NLookup("STS1_DUPLICATOR.pages.DUPLICATE.description"));
        return Task.CompletedTask;
    }
}
