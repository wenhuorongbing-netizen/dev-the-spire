using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Mind Bloom event (Act 3): fight Act 1 boss, upgrade all cards, or gain 999g + Normality curses.
/// A15: 3 Normality curses instead of 2.
/// </summary>
public sealed class Sts1MindBloom : EventModel
{
    private const int GoldAmount = 999;
    private const int CursesNormal = 2;
    private const int CursesA15 = 3;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, War, InitialOptionKey("WAR")),
            new EventOption(this, Awake, InitialOptionKey("AWAKE")),
            new EventOption(this, Rich, InitialOptionKey("RICH")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private Task War()
    {
        // TODO: Enter combat with random Act 1 boss, reward: relic
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.WAR.description"));
        return Task.CompletedTask;
    }

    private Task Awake()
    {
        // TODO: Upgrade all cards in deck
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.AWAKE.description"));
        return Task.CompletedTask;
    }

    private async Task Rich()
    {
        await PlayerCmd.GainGold(GoldAmount, Owner);
        var curseCount = HasA15 ? CursesA15 : CursesNormal;
        var curses = new List<CardModel>();
        for (int i = 0; i < curseCount; i++)
            curses.Add(ModelDb.Card<Normality>());
        await CardPileCmd.AddCursesToDeck(curses, Owner);
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.RICH.description"));
    }
}
