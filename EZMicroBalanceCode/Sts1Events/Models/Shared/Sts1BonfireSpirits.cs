using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Bonfire Spirits event: remove a card and heal to full HP, or leave.
/// </summary>
public sealed class Sts1BonfireSpirits : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Offer, InitialOptionKey("OFFER")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Offer()
    {
        await Sts1EventHelpers.OpenCardRemoval(Owner);
        var maxHp = Owner?.Creature.MaxHp ?? 0m;
        var currentHp = Owner?.Creature.CurrentHp ?? 0m;
        var healAmount = maxHp - currentHp;
        if (healAmount > 0)
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        SetEventFinished(L10NLookup("STS1_BONFIRE_SPIRITS.pages.OFFER.description"));
    }
}
