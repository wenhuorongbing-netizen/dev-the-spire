using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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

    private async Task Upgrade()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardUpgrade(owner);
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.UPGRADE.description"));
    }

    private async Task Remove()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.LoseGold(RemoveCost, owner, GoldLossType.Spent);
        await Sts1EventHelpers.OpenCardRemoval(owner);
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.REMOVE.description"));
    }

    private async Task Transform()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardTransform(owner, Rng, count: 2);
        SetEventFinished(L10NLookup("STS1_DESIGNER.pages.TRANSFORM.description"));
    }
}
