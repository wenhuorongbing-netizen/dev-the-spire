using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class HolyDazePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.HolyDaze;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "圣昏",
            "本次眩晕期间，每次受到的伤害最多为[blue]1[/blue]。结束时获得[blue]{Amount}[/blue]点[gold]力量[/gold]。",
            "受击最多[blue]1[/blue]点；结束时获得[gold]力量[/gold]。")
        : new PowerLoc(
            "Holy Daze",
            "During this stun window, damage taken from each hit is capped at [blue]1[/blue]. Gains [blue]{Amount}[/blue] [gold]Strength[/gold] when it ends.",
            "Damage taken is capped at [blue]1[/blue].");

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return target == Owner ? 1m : decimal.MaxValue;
    }
}
