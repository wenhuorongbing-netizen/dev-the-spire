using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Woman in Blue event: buy potions at varying prices, or leave.
/// </summary>
public sealed class Sts1TheWomanInBlue : EventModel
{
    private const int Potion1Cost = 20;
    private const int Potion2Cost = 30;
    private const int Potion3Cost = 40;

    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Buy1, InitialOptionKey("BUY_1")),
            new EventOption(this, Buy2, InitialOptionKey("BUY_2")),
            new EventOption(this, Buy3, InitialOptionKey("BUY_3")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Buy1()
    {
        await PlayerCmd.LoseGold(Potion1Cost, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomPotion(Owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_WOMAN_IN_BLUE.pages.BUY_1.description"));
    }

    private async Task Buy2()
    {
        await PlayerCmd.LoseGold(Potion2Cost, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomPotion(Owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_WOMAN_IN_BLUE.pages.BUY_2.description"));
    }

    private async Task Buy3()
    {
        await PlayerCmd.LoseGold(Potion3Cost, Owner, GoldLossType.Spent);
        await Sts1EventHelpers.GrantRandomPotion(Owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_WOMAN_IN_BLUE.pages.BUY_3.description"));
    }
}
