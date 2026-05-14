using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal abstract class LothaOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaMirrorRebuttalOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.MirrorRebuttalOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaMirrorHallEchoOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.MirrorHallEchoOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaPresumptionOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.PresumptionOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaClosedCourtOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.ClosedCourtOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaDeferredVerdictOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.DeferredVerdictOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaDeathReprieveOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.DeathReprieveOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaSingleSentenceOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.SingleSentenceOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class LothaPublicEvidenceOptionRelic : LothaOptionRelic
{
    public override string PackedIconPath => LothaAssetPaths.PublicEvidenceOptionIcon;
}
