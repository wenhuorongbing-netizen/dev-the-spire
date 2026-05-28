using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Drug Dealer event (Act 2): buy potions or leave.
/// </summary>
public sealed class Sts1DrugDealer : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, BuyAll, InitialOptionKey("BUY_ALL")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task BuyAll()
    {
        // TODO: Grant 3 random potions, deduct gold
        SetEventFinished(L10NLookup("STS1_DRUG_DEALER.pages.BUY_ALL.description"));
        return Task.CompletedTask;
    }
}
