using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Mind Bloom event (Act 3): fight Act 1 boss, upgrade all cards, or gain 999g + Normality curses.
/// A15: 3 Normality curses instead of 2.
/// </summary>
public sealed class Sts1MindBloom : EventModel
{
    // Combat event: EnterCombatWithoutExitingEvent requires IsShared = true.
    public override bool IsShared => true;

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
        // BLOCKED: Enter combat with random Act 1 boss requires encounter model.
        // TODO: Implement when combat encounter system is available.
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.WAR.description"));
        return Task.CompletedTask;
    }

    private Task Awake()
    {
        if (Owner is not { } owner) return Task.CompletedTask;
        // Upgrade all cards in deck
        foreach (var card in owner.Deck.Cards)
        {
            if (card.IsUpgradable)
                CardCmd.Upgrade(card);
        }
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.AWAKE.description"));
        return Task.CompletedTask;
    }

    private async Task Rich()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.GainGold(GoldAmount, owner);
        var curseCount = HasA15 ? CursesA15 : CursesNormal;
        var curses = new List<CardModel>();
        for (int i = 0; i < curseCount; i++)
            curses.Add(ModelDb.Card<Normality>());
        await CardPileCmd.AddCursesToDeck(curses, owner);
        SetEventFinished(L10NLookup("STS1_MIND_BLOOM.pages.RICH.description"));
    }
}
