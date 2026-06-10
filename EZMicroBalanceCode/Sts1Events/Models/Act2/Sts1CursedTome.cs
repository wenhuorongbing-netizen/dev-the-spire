using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Cursed Tome event (Act 2): take HP damage for a rare relic. A15: 15 damage instead of 10.
/// </summary>
public sealed class Sts1CursedTome : EventModel
{
    private const int DamageNormal = 10;
    private const int DamageA15 = 15;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Read, InitialOptionKey("READ")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Read()
    {
        if (Owner is not { } owner) return;
        var damage = HasA15 ? DamageA15 : DamageNormal;
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            owner.Creature, (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
            null, null);
        await Sts1EventHelpers.GrantRandomRareRelic(owner);
        SetEventFinished(L10NLookup("STS1_CURSED_TOME.pages.READ.description"));
    }
}
