using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Lab event: obtain 3 random potions, or leave.
/// </summary>
public sealed class Sts1TheLab : EventModel
{
    public override bool IsShared => true;

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
        for (int i = 0; i < 3; i++)
            await Sts1EventHelpers.GrantRandomPotion(Owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_LAB.pages.OPEN.description"));
    }
}
