using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MightMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：烈力",
            "火印宿主开场获得[blue]{Amount}[/blue]点[gold]力量[/gold]。造成未被格挡的攻击伤害后获得[gold]热势[/gold]。溢火会短暂强化一名副目标。",
            "造成未被格挡的攻击伤害后获得[gold]热势[/gold]。")
        : new PowerLoc(
            "Firemark: Might",
            "The Firemark Host starts with [blue]{Amount}[/blue] [gold]Strength[/gold]. Unblocked attack damage builds Heat. Overflow briefly strengthens one secondary target.",
            "Unblocked attack damage builds Heat.");
}

internal sealed class FiremarkMightOverflowPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "溢火：烈力",
            "本次敌方回合获得[blue]{Amount}[/blue]点临时[gold]力量[/gold]。火印宿主死亡后不再产生溢火。",
            "本次敌方回合获得临时[gold]力量[/gold]。")
        : new PowerLoc(
            "Overflow: Might",
            "Gains [blue]{Amount}[/blue] temporary [gold]Strength[/gold] for this enemy turn. Overflow stops when the Firemark Host dies.",
            "Gains temporary [gold]Strength[/gold] for this enemy turn.");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side && Owner.IsAlive)
        {
            Flash();
            await AscensionPowerAmountHelper.RemoveTemporaryStrength(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}
