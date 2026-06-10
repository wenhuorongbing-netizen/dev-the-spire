using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Face Trader event: trade max HP for a random relic, or leave.
/// Note: StS1 face relics (Face of Cleric/Guardian/Healer/Navigator/Soldier) don't exist in StS2.
/// Uses random relic as substitute.
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
        if (Owner is not { } owner) return;
        var maxHpLoss = (int)(owner.Creature.MaxHp * MaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                owner.Creature, maxHpLoss, isFromCard: false);
        }
        // temporary-substitute: StS1 face relics don't exist in StS2; use random relic
        await Sts1EventHelpers.GrantRandomRelic(owner);
        SetEventFinished(L10NLookup("STS1_FACE_TRADER.pages.TRADE.description"));
    }
}
