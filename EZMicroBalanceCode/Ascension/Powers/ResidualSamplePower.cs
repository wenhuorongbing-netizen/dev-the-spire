using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ResidualSamplePower : BossSealPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "王印：残留样本",
            "下个阶段会保留[blue]{Amount}[/blue]份[gold]削弱样本[/gold]。样本会在复苏后结算。",
            "复苏后结算[gold]削弱样本[/gold]。")
        : new PowerLoc(
            "Royal Seal: Residual Sample",
            "The next phase keeps [blue]{Amount}[/blue] [gold]weakened sample(s)[/gold]. Samples resolve after respawn.",
            "[gold]Weakened samples[/gold] resolve after respawn.");

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}
