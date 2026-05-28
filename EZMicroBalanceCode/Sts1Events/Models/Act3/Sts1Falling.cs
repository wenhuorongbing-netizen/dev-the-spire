using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Falling event (Act 3): Let Go (remove card), Hold On (take damage), or Fly (transform card).
/// A15: 40% damage instead of 30%.
/// </summary>
public sealed class Sts1Falling : EventModel
{
    private const decimal DamagePctNormal = 0.30m;
    private const decimal DamagePctA15 = 0.40m;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, LetGo, InitialOptionKey("LET_GO")),
            new EventOption(this, HoldOn, InitialOptionKey("HOLD_ON")),
            new EventOption(this, Fly, InitialOptionKey("FLY"))
        };
    }

    private Task LetGo()
    {
        // TODO: Open card removal UI
        SetEventFinished(L10NLookup("STS1_FALLING.pages.LET_GO.description"));
        return Task.CompletedTask;
    }

    private async Task HoldOn()
    {
        var damagePct = HasA15 ? DamagePctA15 : DamagePctNormal;
        var damage = (int)((Owner?.Creature.MaxHp ?? 0m) * damagePct);
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            Owner.Creature, (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
            null, null);
        SetEventFinished(L10NLookup("STS1_FALLING.pages.HOLD_ON.description"));
    }

    private Task Fly()
    {
        // TODO: Open card transform UI
        SetEventFinished(L10NLookup("STS1_FALLING.pages.FLY.description"));
        return Task.CompletedTask;
    }
}
