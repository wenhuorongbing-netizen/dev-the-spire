using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Designer event (Acts 2-3): upgrade, remove (50g), or transform cards.
/// </summary>
public sealed class Sts1Designer : EventModel
{
    private const int RemoveCost = 50;

    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Upgrade, InitialOptionKey("UPGRADE")),
            new EventOption(this, Remove, InitialOptionKey("REMOVE")),
            new EventOption(this, Transform, InitialOptionKey("TRANSFORM")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Upgrade()
    {
        // TODO: Open card upgrade UI
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.UPGRADE.description"));
        return Task.CompletedTask;
    }

    private Task Remove()
    {
        // TODO: Pay 50 gold, open card removal UI
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.REMOVE.description"));
        return Task.CompletedTask;
    }

    private Task Transform()
    {
        // TODO: Open card transform UI (choose 2 cards)
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.TRANSFORM.description"));
        return Task.CompletedTask;
    }
}
