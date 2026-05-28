using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Old Beggar event: pay 75 gold to remove a card, or leave.
/// </summary>
public sealed class Sts1OldBeggar : EventModel
{
    private const int GoldCost = 75;

    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var canAfford = (Owner?.Gold ?? 0) >= GoldCost;

        return new EventOption[]
        {
            new EventOption(this, OfferGold, InitialOptionKey("OFFER_GOLD")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task OfferGold()
    {
        await PlayerCmd.GainGold(-GoldCost, Owner);
        // TODO: Open card removal UI
        SetEventFinished(L10NLookup("STS1_OLD_BEGGAR.pages.OFFER_GOLD.description"));
    }
}
