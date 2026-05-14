using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BossSealPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => AscensionAssetPaths.BossSealIndicator;

    public override string CustomBigIconPath => AscensionAssetPaths.BossSealIndicator;

    public override abstract List<(string, string)>? Localization { get; }
}

internal sealed class HolyDazePower : BossSealPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：圣昏",
            "本次眩晕窗口中，每次受到的伤害最多为[blue]1[/blue]。结束时获得{Amount}点[gold]力量[/gold]。",
            "受伤最多为[blue]1[/blue]；结束时获得[gold]力量[/gold]。")
        : new PowerLoc(
            "Royal Seal: Holy Daze",
            "During this stun window, damage taken from each hit is capped at [blue]1[/blue]. Gains {Amount} [gold]Strength[/gold] when it ends.",
            "Damage taken is capped at [blue]1[/blue].");

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return target == Owner ? 1m : decimal.MaxValue;
    }
}

internal sealed class BoilingCriticalPower : BossSealPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：沸腾临界",
            "死亡爆发每层额外造成[blue]2[/blue]点伤害。爆发回合开始时，玩家获得等量[gold]格挡[/gold]。",
            "爆发更强，但会提前给出[gold]格挡[/gold]提示。")
        : new PowerLoc(
            "Royal Seal: Boiling Critical",
            "Death explosion deals [blue]2[/blue] more damage per stack. At the start of the explosion turn, players gain equal [gold]Block[/gold].",
            "Explosion is stronger and telegraphed.");

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || Owner.Monster is not WaterfallGiant || Owner.Monster.NextMove.StateId != "EXPLODE_MOVE")
        {
            return 0m;
        }

        return Amount * 2m;
    }
}

internal sealed class ResidualSamplePower : BossSealPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：残留样本",
            "下个形态会保留{Amount}份[gold]弱化样本[/gold]。样本会在复苏后结算。",
            "复苏后结算[gold]弱化样本[/gold]。")
        : new PowerLoc(
            "Royal Seal: Residual Sample",
            "The next phase keeps {Amount} [gold]weakened sample(s)[/gold]. Samples resolve after respawn.",
            "[gold]Weakened samples[/gold] resolve after respawn.");

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}

internal sealed class ChosenDecreeReductionPower : BossSealPower
{
    private sealed class Data
    {
        public bool Used { get; set; }
    }

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：择令",
            "下一次由[gold]女王[/gold]给予的[gold]力量[/gold]减少[blue]1[/blue]，然后移除此效果。",
            "下一次女王[gold]力量[/gold]强化-[blue]1[/blue]。")
        : new PowerLoc(
            "Royal Seal: Chosen Decree",
            "The next [gold]Strength[/gold] gain from the [gold]Queen[/gold] is reduced by [blue]1[/blue], then this is removed.",
            "Next Queen [gold]Strength[/gold] gain -[blue]1[/blue].");

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner ||
            canonicalPower is not StrengthPower ||
            amount <= 0m ||
            applier?.Monster is not Queen ||
            GetInternalData<Data>().Used)
        {
            return false;
        }

        GetInternalData<Data>().Used = true;
        modifiedAmount = Math.Max(0m, amount - 1m);
        return true;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (GetInternalData<Data>().Used)
        {
            await PowerCmd.Remove(this);
        }
    }
}
