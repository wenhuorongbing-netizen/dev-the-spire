using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Big Fish event: Banana (heal 1/3 max HP), Donut (+5 max HP),
/// or Shoe (random relic + Regret curse).
/// </summary>
public sealed class Sts1BigFish : EventModel
{
    public override bool IsShared => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new HealVar(0m), // computed in CalculateVars
        new MaxHpVar(5m)
    };

    public override void CalculateVars()
    {
        var maxHp = Owner?.Creature.MaxHp ?? 0m;
        DynamicVars.Heal.BaseValue = maxHp / 3m;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Banana, InitialOptionKey("BANANA")),
            new EventOption(this, Donut, InitialOptionKey("DONUT")),
            new EventOption(this, Shoe, InitialOptionKey("SHOE"))
        };
    }

    private async Task Banana()
    {
        var healAmount = DynamicVars.Heal.IntValue;
        await CreatureCmd.Heal(Owner.Creature, healAmount);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.BANANA.description"));
    }

    private async Task Donut()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.DONUT.description"));
    }

    private async Task Shoe()
    {
        var relic = RelicFactory.PullNextRelicFromFront(Owner).ToMutable();
        await RelicCmd.Obtain(relic, Owner);
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Regret>() }, Owner);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.SHOE.description"));
    }
}
