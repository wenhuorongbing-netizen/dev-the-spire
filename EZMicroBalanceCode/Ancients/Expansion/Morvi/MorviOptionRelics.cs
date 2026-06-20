using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal abstract class MorviOptionRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    public override string CustomBigIconPath => CustomIconPath!;

    public override string CustomIconOutlinePath => CustomIconPath!;
}

internal sealed class MorviForbiddenLoanOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.ForbiddenLoanOptionIcon;
}

internal sealed class MorviMisprintPressOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.MisprintPressOptionIcon;
}

internal sealed class MorviRedInkOverdraftOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.RedInkOverdraftOptionIcon;
}

internal sealed class MorviOverdueLibraryOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.OverdueLibraryOptionIcon;
}

internal sealed class MorviOpenBookExamOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.OpenBookExamOptionIcon;
}

internal sealed class MorviPaperstormOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.PaperstormOptionIcon;
}

internal sealed class MorviBlueprintProofOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.BlueprintProofOptionIcon;
}

internal sealed class MorviDebtSettlementOptionRelic : MorviOptionRelic
{
    public override string CustomIconPath => MorviAssetPaths.DebtSettlementOptionIcon;
}
