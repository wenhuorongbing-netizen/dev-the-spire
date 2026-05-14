using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal abstract class MorviOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviForbiddenLoanOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.ForbiddenLoanOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviMisprintPressOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.MisprintPressOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviRedInkOverdraftOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.RedInkOverdraftOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviOverdueLibraryOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.OverdueLibraryOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviOpenBookExamOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.OpenBookExamOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviPaperstormOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.PaperstormOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviBlueprintProofOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.BlueprintProofOptionIcon;
}

[Pool(typeof(SharedRelicPool))]
internal sealed class MorviDebtSettlementOptionRelic : MorviOptionRelic
{
    public override string PackedIconPath => MorviAssetPaths.DebtSettlementOptionIcon;
}
