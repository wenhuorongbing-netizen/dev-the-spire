using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Mushrooms event (Act 1): 50/50 max HP gain or loss + potion. A15: always lose.
/// </summary>
public sealed class Sts1Mushrooms : EventModel
{
    private const int MaxHpChange = 5;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Eat, InitialOptionKey("EAT")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Eat()
    {
        if (HasA15 || Rng.NextInt(0, 2) == 0)
        {
            // Bad outcome: lose max HP, gain potion
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, MaxHpChange, isFromCard: false);
            // TODO: Grant random potion
            SetEventFinished(L10NLookup("STS1_MUSHROOMS.pages.EAT_BAD.description"));
        }
        else
        {
            // Good outcome: gain max HP
            await CreatureCmd.GainMaxHp(Owner.Creature, MaxHpChange);
            SetEventFinished(L10NLookup("STS1_MUSHROOMS.pages.EAT_GOOD.description"));
        }
    }
}
