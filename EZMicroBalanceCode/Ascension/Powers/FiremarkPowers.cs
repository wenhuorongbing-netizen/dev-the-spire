using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class FiremarkPower : ModPowerTemplate, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    protected virtual string FiremarkIconPath => AscensionAssetPaths.FiremarkedEliteIndicator;

    public override string CustomIconPath => FiremarkIconPath;

    public override string CustomBigIconPath => FiremarkIconPath;

    public abstract List<(string, string)>? Localization { get; }
}
