using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Lab event: obtain 3 random potions (2 at A15), or leave.
/// </summary>
public sealed class Sts1TheLab : EventModel
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
        if (Owner is not { } owner) return;
        int count = HasA15 ? 2 : 3;
        for (int i = 0; i < count; i++)
            await Sts1EventHelpers.GrantRandomPotion(owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_LAB.pages.OPEN.description"));
    }
}
