using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RoyalMajestyPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.ChosenDecree;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "威仪",
        "下一次防御或屏障动作每层额外获得[blue]8[/blue]点格挡。[gold]烙印形态[/gold]最多一次消耗[blue]2[/blue]层。",
        "下一次防御获得更多格挡。",
        "Majesty",
        "The next defense or barrier action gains [blue]8[/blue] extra Block per stack. [gold]Branded Form[/gold] can spend at most [blue]2[/blue] stacks at once.",
        "The next defense gains more Block.");

    private int LayersToSpend => Math.Min(Amount, 2);

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return target == Owner && props.HasFlag(ValueProp.Move)
            ? LayersToSpend * 8m
            : 0m;
    }

    public override async Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -LayersToSpend, Owner, cardSource);
    }
}
