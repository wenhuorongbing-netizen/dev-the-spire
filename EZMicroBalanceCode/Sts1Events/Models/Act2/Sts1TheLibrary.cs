using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 The Library event (Act 2): choose 1 of 20 cards, or rest (heal 1/3 max HP).
/// </summary>
public sealed class Sts1TheLibrary : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Read, InitialOptionKey("READ")),
            new EventOption(this, Rest, InitialOptionKey("REST")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Read()
    {
        // TODO: Show card selection UI (1 of 20 random cards)
        SetEventFinished(L10NLookup("STS1_THE_LIBRARY.pages.READ.description"));
        return Task.CompletedTask;
    }

    private async Task Rest()
    {
        var healAmount = (Owner?.Creature.MaxHp ?? 0m) / 3m;
        if (healAmount > 0)
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        SetEventFinished(L10NLookup("STS1_THE_LIBRARY.pages.REST.description"));
    }
}
