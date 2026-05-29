using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

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
        return new EventOption[]
        {
            new EventOption(this, OfferGold, InitialOptionKey("OFFER_GOLD")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task OfferGold()
    {
        await PlayerCmd.LoseGold(GoldCost, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.OpenCardRemoval(Owner);
        SetEventFinished(L10NLookup("STS1_OLD_BEGGAR.pages.OFFER_GOLD.description"));
    }
}
