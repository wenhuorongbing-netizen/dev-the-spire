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
        if (Owner is not { } owner) return;
        await PlayerCmd.LoseGold(Offer50Cost, owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomRelic(owner);
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description"));
    }

    private async Task OfferAll()
    {
        if (Owner is not { } owner) return;
        var gold = owner.Gold;
        if (gold > 0)
            await PlayerCmd.LoseGold(gold, owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomRelic(owner);
        SetEventFinished(L10NLookup("STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description"));
    }
}
