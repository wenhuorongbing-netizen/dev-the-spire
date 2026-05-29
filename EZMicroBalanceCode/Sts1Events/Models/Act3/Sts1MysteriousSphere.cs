using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Mysterious Sphere event (Act 3): fight 2 Orb Walkers for a relic, or leave.
/// </summary>
public sealed class Sts1MysteriousSphere : EventModel
{
    // Combat event: EnterCombatWithoutExitingEvent requires IsShared = true.
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Open, InitialOptionKey("OPEN")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Open()
    {
        // TODO: Enter combat with 2 Orb Walkers, reward: random relic
        SetEventFinished(L10NLookup("STS1_MYSTERIOUS_SPHERE.pages.OPEN.description"));
        return Task.CompletedTask;
    }
}
