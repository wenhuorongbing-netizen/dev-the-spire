using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Mausoleum event: 50/50 relic or curse, or leave. A15: always curse.
/// </summary>
public sealed class Sts1TheMausoleum : EventModel
{
    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Open, InitialOptionKey("OPEN")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Open()
    {
        if (HasA15 || Rng.NextInt(0, 2) != 0)
        {
            // Curse outcome (always on A15, 50% otherwise)
            await CardPileCmd.AddCursesToDeck(
                new[] { ModelDb.Card<Wound>() }, Owner);
            SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_CURSE.description"));
        }
        else
        {
            // Relic outcome (50% on non-A15)
            // TODO: Grant random relic
            SetEventFinished(L10NLookup("STS1_THE_MAUSOLEUM.pages.OPEN_RELIC.description"));
        }
    }
}
