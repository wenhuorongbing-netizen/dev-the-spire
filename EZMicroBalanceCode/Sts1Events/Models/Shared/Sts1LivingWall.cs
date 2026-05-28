using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Living Wall event: Forget (remove), Change (transform), or Trade (upgrade) a card.
/// </summary>
public sealed class Sts1LivingWall : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Forget, InitialOptionKey("FORGET")),
            new EventOption(this, Change, InitialOptionKey("CHANGE")),
            new EventOption(this, Trade, InitialOptionKey("TRADE"))
        };
    }

    private Task Forget()
    {
        // TODO: Open card removal UI
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.FORGET.description"));
        return Task.CompletedTask;
    }

    private Task Change()
    {
        // TODO: Open card transform UI
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.CHANGE.description"));
        return Task.CompletedTask;
    }

    private Task Trade()
    {
        // TODO: Open card upgrade UI
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.TRADE.description"));
        return Task.CompletedTask;
    }
}
