using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Vampires event (Act 2): remove all Strikes, gain 5 Bites, lose max HP.
/// A15: lose 40% max HP instead of 30%.
///
/// Note: Bite card does not exist in StS2. Uses a temporary substitute pattern:
/// removes Strikes and loses max HP, but cannot add Bite cards.
/// Marked as temporary-substitute until a custom Bite card model is created.
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
        // Remove all Strikes from deck
        await Sts1EventHelpers.RemoveCardsByTag(Owner, CardTag.Strike);

        // temporary-substitute: Bite card does not exist in StS2.
        // Cannot add 5 Bite cards. This event is partially implemented.
        // TODO: Create custom Bite card model for StS2 parity.

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
