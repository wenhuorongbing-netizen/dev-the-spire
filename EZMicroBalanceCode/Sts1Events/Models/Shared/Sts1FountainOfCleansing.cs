using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Fountain of Cleansing event: remove all curses but lose max HP, or leave.
/// </summary>
public sealed class Sts1FountainOfCleansing : EventModel
{
    private const decimal MaxHpLossPctNormal = 0.10m;
    private const decimal MaxHpLossPctA15 = 0.15m;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private decimal MaxHpLossPct => HasA15 ? MaxHpLossPctA15 : MaxHpLossPctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Drink, InitialOptionKey("DRINK")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Drink()
    {
        // TODO: Remove all curses from deck
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * MaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_FOUNTAIN_OF_CLEANSING.pages.DRINK.description"));
    }
}
