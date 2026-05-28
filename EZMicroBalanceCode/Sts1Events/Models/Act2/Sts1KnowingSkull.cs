using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Knowing Skull event (Act 2): ask questions for HP cost, get rare cards.
/// A15: 10 HP per question instead of 6.
/// </summary>
public sealed class Sts1KnowingSkull : EventModel
{
    private const int HpCostNormal = 6;
    private const int HpCostA15 = 10;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Question1, InitialOptionKey("QUESTION_1")),
            new EventOption(this, Question2, InitialOptionKey("QUESTION_2")),
            new EventOption(this, Question3, InitialOptionKey("QUESTION_3")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Question1()
    {
        await TakeDamage();
        // TODO: Show lore text
        SetEventFinished(L10NLookup("STS1_KNOWING_SKULL.pages.QUESTION_1.description"));
    }

    private async Task Question2()
    {
        await TakeDamage();
        // TODO: Show lore text
        SetEventFinished(L10NLookup("STS1_KNOWING_SKULL.pages.QUESTION_2.description"));
    }

    private async Task Question3()
    {
        await TakeDamage();
        // TODO: Grant random rare card
        SetEventFinished(L10NLookup("STS1_KNOWING_SKULL.pages.QUESTION_3.description"));
    }

    private async Task TakeDamage()
    {
        var hpCost = HasA15 ? HpCostA15 : HpCostNormal;
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            Owner.Creature, (decimal)hpCost,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
            null, null);
    }
}
