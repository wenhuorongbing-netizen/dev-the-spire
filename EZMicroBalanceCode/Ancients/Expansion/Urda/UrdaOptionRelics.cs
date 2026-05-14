namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

using MegaCrit.Sts2.Core.Models.RelicPools;

internal abstract class UrdaOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}

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
internal sealed class UrdaRootedRouteOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootedRouteOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaAfterRainOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.AfterRainOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaRootSightOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootSightOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaSeedBankOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.SeedBankOptionIcon;
}
