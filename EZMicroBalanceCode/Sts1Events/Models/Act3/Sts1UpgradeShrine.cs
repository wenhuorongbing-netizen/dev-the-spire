using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Upgrade Shrine event (Act 3): choose a card to upgrade.
/// </summary>
public sealed class Sts1UpgradeShrine : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Pray()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.OpenCardUpgrade(owner);
        SetEventFinished(L10NLookup("STS1_UPGRADE_SHRINE.pages.PRAY.description"));
    }
}
