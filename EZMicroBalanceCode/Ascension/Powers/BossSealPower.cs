using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BossSealPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => Amount;

    protected virtual BossSealId? SealId => null;

    private string BossSealIconPath => SealId is { } id
        ? AscensionAssetPaths.GetBossSealIndicator(id)
        : AscensionAssetPaths.BossSealIndicator;

    public override string CustomPackedIconPath => BossSealIconPath;

    public override string CustomBigIconPath => BossSealIconPath;

    public override abstract List<(string, string)>? Localization { get; }

    protected static List<(string, string)> Loc(string zhsTitle, string zhsDescription, string zhsShort, string engTitle, string engDescription, string engShort) =>
        LocManager.Instance.Language == "zhs"
            ? new PowerLoc(zhsTitle, zhsDescription, zhsShort)
            : new PowerLoc(engTitle, engDescription, engShort);
}
