using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaVerdictPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => LothaAssetPaths.VerdictPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.VerdictPowerBigIcon;
}

internal sealed class LothaPresumptionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => LothaAssetPaths.PresumptionPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.PresumptionPowerBigIcon;
}

internal sealed class LothaDeathReprievePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => LothaAssetPaths.DeathReprievePowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.DeathReprievePowerBigIcon;
}

internal sealed class LothaSingleSentencePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => LothaAssetPaths.SingleSentencePowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.SingleSentencePowerBigIcon;
}

internal sealed class LothaEnlightenmentPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => LothaAssetPaths.EnlightenmentPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.EnlightenmentPowerBigIcon;
}
