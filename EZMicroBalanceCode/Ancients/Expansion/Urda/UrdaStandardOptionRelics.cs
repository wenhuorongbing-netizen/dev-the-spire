using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaSeedbedOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.SeedbedOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaHumusPactOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.HumusPactOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaMoltingOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.MoltingOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaMossMapOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.MossMapOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaTrialBranchOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.TrialBranchOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaShallowRootRelicOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.ShallowRootRelicOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaEliteRootOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.EliteRootOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaRootedRouteOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootedRouteOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaAfterRainOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.AfterRainOptionIcon;
}
