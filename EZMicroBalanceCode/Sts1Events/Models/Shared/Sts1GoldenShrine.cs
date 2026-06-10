using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Golden Shrine event: gain gold, or remove all curses from deck (if cursed).
/// A15 reduces gold from 250 to 100.
/// </summary>
public sealed class Sts1GoldenShrine : EventModel
{
    private const int GoldNormal = 250;
    private const int GoldA15 = 100;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private int GoldAmount => HasA15 ? GoldA15 : GoldNormal;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(GoldNormal)
    };

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var hasCurses = false;
        if (Owner is { } owner)
        {
            foreach (var card in owner.Deck.Cards)
            {
                if (card.Type == MegaCrit.Sts2.Core.Entities.Cards.CardType.Curse)
                {
                    hasCurses = true;
                    break;
                }
            }
        }

        return new EventOption[]
        {
            new EventOption(this, TakeGold, InitialOptionKey("TAKE_GOLD")),
            new EventOption(this, hasCurses ? Desecrate : null, InitialOptionKey("DESECRATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task TakeGold()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.GainGold(GoldAmount, owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_SHRINE.pages.TAKE_GOLD.description"));
    }

    private async Task Desecrate()
    {
        if (Owner is not { } owner) return;
        await Sts1EventHelpers.RemoveAllCurses(owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_SHRINE.pages.DESECRATE.description"));
    }
}
