using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Tomb of Lord Red Mask event (Act 3): offer 50g or all gold for a relic.
/// </summary>
public sealed class Sts1TombOfLordRedMask : EventModel
{
    private const int Offer50Cost = 50;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Offer50, InitialOptionKey("OFFER_50")),
            new EventOption(this, OfferAll, InitialOptionKey("OFFER_ALL")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Offer50()
    {
        await PlayerCmd.LoseGold(Offer50Cost, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomRelic(Owner);
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description"));
    }

    private async Task OfferAll()
    {
        var gold = Owner?.Gold ?? 0;
        if (gold > 0)
            await PlayerCmd.LoseGold(gold, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomRelic(Owner);
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description"));
    }
}
