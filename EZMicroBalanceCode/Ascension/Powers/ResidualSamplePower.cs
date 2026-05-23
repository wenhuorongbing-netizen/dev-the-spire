using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ResidualSamplePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.ResidualSample;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "实验记录",
            "下个阶段会保留[blue]{Amount}[/blue]份[gold]残留样本[/gold]。复苏后，样本会根据上一阶段记录改变本阶段的首次出牌、负面状态或洗牌结算。",
            "复苏后结算残留样本。")
        : new PowerLoc(
            "Experimental Record",
            "The next phase keeps [blue]{Amount}[/blue] [gold]Residual Sample[/gold]. After respawn, samples use the previous phase's record to affect the next phase's first card-count, debuff, or shuffle event.",
            "Residual samples resolve after respawn.");

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}
