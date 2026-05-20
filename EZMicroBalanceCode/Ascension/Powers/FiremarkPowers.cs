using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class FiremarkPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    protected virtual string FiremarkIconPath => AscensionAssetPaths.FiremarkedEliteIndicator;

    public override string CustomPackedIconPath => FiremarkIconPath;

    public override string CustomBigIconPath => FiremarkIconPath;

    public override abstract List<(string, string)>? Localization { get; }
}
