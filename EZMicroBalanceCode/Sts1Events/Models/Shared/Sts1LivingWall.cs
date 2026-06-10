using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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

    private async Task Forget()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardRemoval(owner);
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.FORGET.description"));
    }

    private async Task Change()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardTransform(owner, Rng);
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.CHANGE.description"));
    }

    private async Task Trade()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardUpgrade(owner);
        SetEventFinished(L10NLookup("STS1_LIVING_WALL.pages.TRADE.description"));
    }
}
