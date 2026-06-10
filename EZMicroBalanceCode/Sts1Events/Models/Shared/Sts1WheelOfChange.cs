using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Wheel of Change event: spin for a random outcome (gold/damage/relic/curse/heal/remove).
/// </summary>
public sealed class Sts1WheelOfChange : EventModel
{
    private const decimal DamagePctNormal = 0.30m;
    private const decimal DamagePctA15 = 0.40m;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private decimal DamagePct => HasA15 ? DamagePctA15 : DamagePctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Spin, InitialOptionKey("SPIN"))
        };
    }

    private async Task Spin()
    {
        if (Owner is not { } owner)
        {
            return;
        }

        var outcome = Rng.NextInt(0, 6);
        switch (outcome)
        {
            case 0: // Gold
                await PlayerCmd.GainGold(100, owner);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.GOLD.description"));
                break;
            case 1: // Damage
                var damage = (int)(owner.Creature.MaxHp * DamagePct);
                await CreatureCmd.Damage(
                    new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                    owner.Creature, (decimal)damage,
                    MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
                    null, null);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.DAMAGE.description"));
                break;
            case 2: // Relic
                await Sts1EventHelpers.GrantRandomRelic(owner);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.RELIC.description"));
                break;
            case 3: // Curse
                await Sts1EventHelpers.AddCurses<Decay>(owner, 1);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.CURSE.description"));
                break;
            case 4: // Heal
                var healAmount = owner.Creature.MaxHp - owner.Creature.CurrentHp;
                if (healAmount > 0)
                    await CreatureCmd.Heal(owner.Creature, healAmount);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.HEAL.description"));
                break;
            case 5: // Card removal
                await Sts1EventHelpers.OpenCardRemoval(owner);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.REMOVE.description"));
                break;
        }
    }
}
