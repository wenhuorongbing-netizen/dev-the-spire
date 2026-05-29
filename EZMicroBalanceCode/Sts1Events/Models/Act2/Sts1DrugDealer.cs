using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Drug Dealer event (Act 2): buy 3 random potions for 60 gold, or leave.
/// </summary>
public sealed class Sts1DrugDealer : EventModel
{
    private const int Cost = 60;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, BuyAll, InitialOptionKey("BUY_ALL")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task BuyAll()
    {
        await PlayerCmd.LoseGold(Cost, Owner, GoldLossType.Spent);
        for (int i = 0; i < 3; i++)
            await Sts1EventHelpers.GrantRandomPotion(Owner, Rng);
        SetEventFinished(L10NLookup("STS1_DRUG_DEALER.pages.BUY_ALL.description"));
    }
}
