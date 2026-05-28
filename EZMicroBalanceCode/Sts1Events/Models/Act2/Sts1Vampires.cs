using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Vampires event (Act 2): remove all Strikes, gain 5 Bites, lose max HP.
/// A15: lose 40% max HP instead of 30%.
/// </summary>
public sealed class Sts1Vampires : EventModel
{
    private const decimal MaxHpLossPctNormal = 0.30m;
    private const decimal MaxHpLossPctA15 = 0.40m;
    private const int BiteCount = 5;

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
        // TODO: Remove all Strikes from deck
        // TODO: Add 5 Bite cards to deck
        var maxHpLossPct = HasA15 ? MaxHpLossPctA15 : MaxHpLossPctNormal;
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * maxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_VAMPIRES.pages.ACCEPT.description"));
    }
}
