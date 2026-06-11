using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Golden Shrine event: pray for gold, desecrate for more gold and Regret, or leave.
/// A15 reduces Pray gold from 100 to 50.
/// </summary>
public sealed class Sts1GoldenShrine : EventModel
{
    private const int PrayGoldNormal = 100;
    private const int PrayGoldA15 = 50;
    private const int DesecrateGold = 275;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private int PrayGoldAmount => HasA15 ? PrayGoldA15 : PrayGoldNormal;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(PrayGoldNormal)
    };

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, Desecrate, InitialOptionKey("DESECRATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Pray()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.GainGold(PrayGoldAmount, owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_SHRINE.pages.PRAY.description"));
    }

    private async Task Desecrate()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.GainGold(DesecrateGold, owner);
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Regret>() }, owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_SHRINE.pages.DESECRATE.description"));
    }
}
