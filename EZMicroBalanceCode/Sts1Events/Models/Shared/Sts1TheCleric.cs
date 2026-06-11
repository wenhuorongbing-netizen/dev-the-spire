using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 The Cleric event: Heal (pay gold to heal) or Purify (pay gold to remove a card).
/// </summary>
public sealed class Sts1TheCleric : EventModel
{
    private const int HealCost = 35;
    private const int PurifyCostNormal = 50;
    private const int PurifyCostA15 = 75;
    private const decimal HealPct = 0.25m;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private int PurifyCost => HasA15 ? PurifyCostA15 : PurifyCostNormal;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(HealCost),
        new HealVar(0m)
    };

    public override bool IsAllowed(IRunState runState)
    {
        foreach (var player in runState.Players)
        {
            if (player.Gold < HealCost)
                return false;
        }

        return runState.Players.Count > 0;
    }

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
            new EventOption(this, canHeal ? Heal : null, InitialOptionKey("HEAL")),
            new EventOption(this, canPurify ? Purify : null, InitialOptionKey("PURIFY")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Heal()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.LoseGold(HealCost, owner, GoldLossType.Spent);
        await CreatureCmd.Heal(owner.Creature, DynamicVars.Heal.IntValue);
        SetEventFinished(L10NLookup("STS1_THE_CLERIC.pages.HEAL.description"));
    }

    private async Task Purify()
    {
        if (Owner is not { } owner) return;
        await PlayerCmd.LoseGold(PurifyCost, owner, GoldLossType.Spent);
        await Sts1EventHelpers.OpenCardRemoval(owner);
        SetEventFinished(L10NLookup("STS1_THE_CLERIC.pages.PURIFY.description"));
    }
}
