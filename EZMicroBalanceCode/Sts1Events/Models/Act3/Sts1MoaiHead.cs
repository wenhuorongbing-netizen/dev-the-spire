using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Moai Head event (Act 3): Worship (+1 max HP) or Offer Gold (50g for +3 max HP).
/// </summary>
public sealed class Sts1MoaiHead : EventModel
{
    private const int WorshipMaxHp = 1;
    private const int OfferCost = 50;
    private const int OfferMaxHp = 3;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Worship, InitialOptionKey("WORSHIP")),
            new EventOption(this, Offer, InitialOptionKey("OFFER")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Worship()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, WorshipMaxHp);
        SetEventFinished(L10NLookup("STS1_MOAI_HEAD.pages.WORSHIP.description"));
    }

    private async Task Offer()
    {
        await PlayerCmd.LoseGold(OfferCost, Owner, GoldLossType.Spent);
        await CreatureCmd.GainMaxHp(Owner.Creature, OfferMaxHp);
        SetEventFinished(L10NLookup("STS1_MOAI_HEAD.pages.OFFER.description"));
    }
}
