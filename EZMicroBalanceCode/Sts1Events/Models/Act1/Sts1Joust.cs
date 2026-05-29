using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Joust event (Act 1): bet 50g on yourself or opponent, 50/50 win 200g.
/// A15: lose 100g on failure.
/// </summary>
public sealed class Sts1Joust : EventModel
{
    private const int BetCost = 50;
    private const int WinAmount = 200;
    private const int A15Loss = 100;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, BetSelf, InitialOptionKey("BET_SELF")),
            new EventOption(this, BetOpponent, InitialOptionKey("BET_OPPONENT")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task BetSelf()
    {
        await PlayerCmd.LoseGold(BetCost, Owner, GoldLossType.Spent);
        if (Rng.NextInt(0, 2) == 0)
        {
            await PlayerCmd.GainGold(WinAmount, Owner);
            SetEventFinished(L10NLookup("STS1_JOUST.pages.WIN.description"));
        }
        else
        {
            if (HasA15)
                await PlayerCmd.LoseGold(A15Loss, Owner);
            SetEventFinished(L10NLookup("STS1_JOUST.pages.LOSE.description"));
        }
    }

    private async Task BetOpponent()
    {
        await PlayerCmd.LoseGold(BetCost, Owner, GoldLossType.Spent);
        if (Rng.NextInt(0, 2) == 0)
        {
            await PlayerCmd.GainGold(WinAmount, Owner);
            SetEventFinished(L10NLookup("STS1_JOUST.pages.WIN.description"));
        }
        else
        {
            if (HasA15)
                await PlayerCmd.LoseGold(A15Loss, Owner);
            SetEventFinished(L10NLookup("STS1_JOUST.pages.LOSE.description"));
        }
    }
}
