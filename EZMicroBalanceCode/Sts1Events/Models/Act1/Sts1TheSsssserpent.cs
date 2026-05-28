using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 The Ssssserpent event (Act 1): gain 150g + Doubt curses. A15: 3 curses instead of 2.
/// </summary>
public sealed class Sts1TheSsssserpent : EventModel
{
    private const int GoldAmount = 150;
    private const int CursesNormal = 2;
    private const int CursesA15 = 3;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Accept, InitialOptionKey("ACCEPT")),
            new EventOption(this, null, InitialOptionKey("REFUSE"))
        };
    }

    private async Task Accept()
    {
        await PlayerCmd.GainGold(GoldAmount, Owner);
        var curseCount = HasA15 ? CursesA15 : CursesNormal;
        var curses = new List<CardModel>();
        for (int i = 0; i < curseCount; i++)
            curses.Add(ModelDb.Card<Doubt>());
        await CardPileCmd.AddCursesToDeck(curses, Owner);
        SetEventFinished(L10NLookup("STS1_THE_SSSSSERPENT.pages.ACCEPT.description"));
    }
}
