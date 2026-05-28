using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Nest event (Act 2): search for relic but gain Parasite curses. A15: 3 curses instead of 2.
/// StS1 Parasite doesn't exist in StS2; using Clumsy as substitute.
/// </summary>
public sealed class Sts1Nest : EventModel
{
    private const int CursesNormal = 2;
    private const int CursesA15 = 3;

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
        await Sts1EventHelpers.GrantRandomRelic(Owner);
        var curseCount = HasA15 ? CursesA15 : CursesNormal;
        await Sts1EventHelpers.AddCurses<Clumsy>(Owner, curseCount);
        SetEventFinished(L10NLookup("STS1_NEST.pages.SEARCH.description"));
    }
}
