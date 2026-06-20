using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class LothaVerdictPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => LothaAssetPaths.VerdictPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.VerdictPowerBigIcon;
}

internal sealed class LothaPresumptionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => LothaAssetPaths.PresumptionPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.PresumptionPowerBigIcon;
}

internal sealed class LothaDeathReprievePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => LothaAssetPaths.DeathReprievePowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.DeathReprievePowerBigIcon;
}

internal sealed class LothaSingleSentencePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => LothaAssetPaths.SingleSentencePowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.SingleSentencePowerBigIcon;
}

internal sealed class LothaEnlightenmentPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => LothaAssetPaths.EnlightenmentPowerIcon;

    public override string CustomBigIconPath => LothaAssetPaths.EnlightenmentPowerBigIcon;
}
