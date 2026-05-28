using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Face Trader event: trade max HP for a face relic, or leave.
/// </summary>
public sealed class Sts1FaceTrader : EventModel
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
            new EventOption(this, Trade, InitialOptionKey("TRADE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Trade()
    {
        // TODO: Grant a random face relic (Face of Cleric/Guardian/Healer/Navigator/Soldier)
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * MaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_FACE_TRADER.pages.TRADE.description"));
    }
}
