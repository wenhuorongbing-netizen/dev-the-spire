using MegaCrit.Sts2.Core.Entities.Powers;

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
