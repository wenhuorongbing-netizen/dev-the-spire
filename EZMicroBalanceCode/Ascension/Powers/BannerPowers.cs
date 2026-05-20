using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BannerPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => 0;

    protected virtual string BannerIconPath => AscensionAssetPaths.BannerRoomIndicator;

    public override string CustomPackedIconPath => BannerIconPath;

    public override string CustomBigIconPath => BannerIconPath;

    public override abstract List<(string, string)>? Localization { get; }
}
