using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Masked Bandits event (Act 2): pay 75g or fight bandits for relic.
/// </summary>
public sealed class Sts1MaskedBandits : EventModel
{
    // Combat event: EnterCombatWithoutExitingEvent requires IsShared = true.
    public override bool IsShared => true;

    private const int PayCost = 75;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pay, InitialOptionKey("PAY")),
            new EventOption(this, Fight, InitialOptionKey("FIGHT"))
        };
    }

    private async Task Pay()
    {
        await PlayerCmd.LoseGold(PayCost, Owner, GoldLossType.Spent);
        SetEventFinished(L10NLookup("STS1_MASKED_BANDITS.pages.PAY.description"));
    }

    private Task Fight()
    {
        // TODO: Enter combat with 3 bandits, reward: gold + relic
        SetEventFinished(L10NLookup("STS1_MASKED_BANDITS.pages.FIGHT.description"));
        return Task.CompletedTask;
    }
}
