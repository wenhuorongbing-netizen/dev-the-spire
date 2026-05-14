using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaVerdictPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => LothaAssetPaths.VerdictPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.VerdictPowerIcon;
}

internal sealed class LothaPresumptionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => LothaAssetPaths.PresumptionPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.PresumptionPowerIcon;
}

internal sealed class LothaDeathReprievePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => LothaAssetPaths.DeathReprievePowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.DeathReprievePowerIcon;
}

internal sealed class LothaEnlightenmentPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => LothaAssetPaths.EnlightenmentPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.EnlightenmentPowerIcon;
}
