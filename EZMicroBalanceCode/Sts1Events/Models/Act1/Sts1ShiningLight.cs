using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;

/// <summary>
/// StS1 Shining Light event (Act 1): take damage to upgrade 2 random cards.
/// </summary>
public sealed class Sts1ShiningLight : EventModel
{
    private const decimal DamagePctNormal = 0.30m;
    private const decimal DamagePctA15 = 0.40m;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private decimal DamagePct => HasA15 ? DamagePctA15 : DamagePctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Enter, InitialOptionKey("ENTER")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Enter()
    {
        var damage = (int)((Owner?.Creature.MaxHp ?? 0m) * DamagePct);
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            Owner.Creature, (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
            null, null);
        await Sts1EventHelpers.OpenCardUpgrade(Owner, count: 2);
        SetEventFinished(L10NLookup("STS1_SHINING_LIGHT.pages.ENTER.description"));
    }
}
