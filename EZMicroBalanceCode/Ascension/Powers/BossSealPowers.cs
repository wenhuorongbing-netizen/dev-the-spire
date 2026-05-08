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
            "本次眩晕窗口中，每次受到的伤害最多为1。结束时获得{Amount}点力量。",
            "受伤最多为1；结束时获得力量。")
        : new PowerLoc(
            "Royal Seal: Holy Daze",
            "During this stun window, damage taken from each hit is capped at 1. Gains {Amount} Strength when it ends.",
            "Damage taken is capped at 1.");

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
            "死亡爆发额外造成每层2点伤害。爆发回合开始时，玩家获得等量格挡。",
            "爆发更强，但会提前给出格挡提示。")
        : new PowerLoc(
            "Royal Seal: Boiling Critical",
            "Death explosion deals 2 more damage per stack. At the start of the explosion turn, players gain equal Block.",
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
            "下个形态会保留{Amount}份弱化样本。样本会在复苏后结算。",
            "复苏后结算弱化样本。")
        : new PowerLoc(
            "Royal Seal: Residual Sample",
            "The next phase keeps {Amount} weakened sample(s). Samples resolve after respawn.",
            "Weakened samples resolve after respawn.");

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
            "下一次由女王给予的力量减少1，然后移除此效果。",
            "下一次女王力量强化-1。")
        : new PowerLoc(
            "Royal Seal: Chosen Decree",
            "The next Strength gain from the Queen is reduced by 1, then this is removed.",
            "Next Queen Strength gain -1.");

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
