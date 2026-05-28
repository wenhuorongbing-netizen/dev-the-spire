using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Cleric event: Heal (pay gold to heal) or Purify (pay gold to remove a card).
/// </summary>
public sealed class Sts1TheCleric : EventModel
{
    private const int HealCost = 35;
    private const int PurifyCost = 50;
    private const decimal HealPct = 0.25m;

    public override bool IsShared => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(HealCost),
        new HealVar(0m)
    };

    public override void CalculateVars()
    {
        var maxHp = Owner?.Creature.MaxHp ?? 0m;
        DynamicVars.Heal.BaseValue = maxHp * HealPct;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var canHeal = (Owner?.Gold ?? 0) >= HealCost;
        var canPurify = (Owner?.Gold ?? 0) >= PurifyCost;

        return new EventOption[]
        {
            new EventOption(this, Heal, InitialOptionKey("HEAL"),
                canHeal ? null : null),
            new EventOption(this, Purify, InitialOptionKey("PURIFY"),
                canPurify ? null : null),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Heal()
    {
        await PlayerCmd.GainGold(-HealCost, Owner);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
        SetEventFinished(L10NLookup("STS1_THE_CLERIC.pages.HEAL.description"));
    }

    private Task Purify()
    {
        // TODO: Open card remove UI
        // For now, just finish the event
        SetEventFinished(L10NLookup("STS1_THE_CLERIC.pages.PURIFY.description"));
        return Task.CompletedTask;
    }
}
