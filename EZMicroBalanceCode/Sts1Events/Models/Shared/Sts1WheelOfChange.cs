using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

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
        var outcome = Rng.NextInt(0, 6);
        switch (outcome)
        {
            case 0: // Gold
                await PlayerCmd.GainGold(100, Owner);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.GOLD.description"));
                break;
            case 1: // Damage
                var damage = (int)((Owner?.Creature.MaxHp ?? 0m) * DamagePct);
                await CreatureCmd.Damage(
                    new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                    Owner.Creature, (decimal)damage,
                    MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
                    null, null);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.DAMAGE.description"));
                break;
            case 2: // Relic
                // TODO: Grant random relic
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.RELIC.description"));
                break;
            case 3: // Curse
                // TODO: Add Decay curse to deck
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.CURSE.description"));
                break;
            case 4: // Heal
                var healAmount = (Owner?.Creature.MaxHp ?? 0m) - (Owner?.Creature.CurrentHp ?? 0m);
                if (healAmount > 0)
                    await CreatureCmd.Heal(Owner.Creature, healAmount);
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.HEAL.description"));
                break;
            case 5: // Card removal
                // TODO: Open card removal UI
                SetEventFinished(L10NLookup("STS1_WHEEL_OF_CHANGE.pages.REMOVE.description"));
                break;
        }
    }
}
