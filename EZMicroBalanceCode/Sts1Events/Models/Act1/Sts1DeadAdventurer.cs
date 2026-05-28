using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Dead Adventurer event (Act 1): search for gold/relic or fight elite.
/// </summary>
public sealed class Sts1DeadAdventurer : EventModel
{
    private const int GoldMin = 30;
    private const int GoldMax = 50;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Search, InitialOptionKey("SEARCH")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Search()
    {
        var eliteChance = HasA15 ? 50 : 25;
        var roll = Rng.NextInt(0, 100);

        if (roll < eliteChance)
        {
            // Fight an elite
            // TODO: Enter combat with random elite
            SetEventFinished(L10NLookup("STS1_DEAD_ADVENTURER.pages.FIGHT.description"));
        }
        else if (roll < eliteChance + 25)
        {
            await Sts1EventHelpers.GrantRandomRelic(Owner);
            SetEventFinished(L10NLookup("STS1_DEAD_ADVENTURER.pages.RELIC.description"));
        }
        else
        {
            // Find gold
            var gold = Rng.NextInt(GoldMin, GoldMax + 1);
            await PlayerCmd.GainGold(gold, Owner);
            SetEventFinished(L10NLookup("STS1_DEAD_ADVENTURER.pages.GOLD.description"));
        }
    }
}
