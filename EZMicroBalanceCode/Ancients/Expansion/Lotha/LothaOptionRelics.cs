using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal abstract class LothaOptionRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    public override string CustomBigIconPath => CustomIconPath!;

    public override string CustomIconOutlinePath => CustomIconPath!;
}

internal sealed class LothaMirrorRebuttalOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.MirrorRebuttalOptionIcon;
}

internal sealed class LothaMirrorHallEchoOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.MirrorHallEchoOptionIcon;
}

internal sealed class LothaPresumptionOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.PresumptionOptionIcon;
}

internal sealed class LothaClosedCourtOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.ClosedCourtOptionIcon;
}

internal sealed class LothaDeferredVerdictOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.DeferredVerdictOptionIcon;
}

internal sealed class LothaDeathReprieveOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.DeathReprieveOptionIcon;
}

internal sealed class LothaSingleSentenceOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.SingleSentenceOptionIcon;
}

internal sealed class LothaPublicEvidenceOptionRelic : LothaOptionRelic
{
    public override string CustomIconPath => LothaAssetPaths.PublicEvidenceOptionIcon;
}
