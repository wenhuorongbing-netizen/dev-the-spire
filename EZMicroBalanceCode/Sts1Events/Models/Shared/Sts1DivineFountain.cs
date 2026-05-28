using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Divine Fountain event: remove all Curses from your deck, or leave.
/// </summary>
public sealed class Sts1DivineFountain : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task Pray()
    {
        // TODO: Remove all curses from deck
        // CardPileCmd.RemoveAllCurses(Owner) or similar
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.PRAY.description"));
        return Task.CompletedTask;
    }
}
