using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviDebtPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => MorviAssetPaths.DebtPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.DebtPowerBigIcon;
}

internal sealed class MorviProofreadPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => MorviAssetPaths.ProofreadPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ProofreadPowerBigIcon;
}

internal sealed class MorviOpenBookPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => MorviAssetPaths.OpenBookPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OpenBookPowerBigIcon;
}

internal sealed class MorviOverdraftPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => MorviAssetPaths.OverdraftPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OverdraftPowerBigIcon;
}

internal sealed class MorviPaperstormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => MorviAssetPaths.PaperstormPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.PaperstormPowerBigIcon;
}

internal sealed class MorviBraveryPagePower : ModTemporaryAppliedPowerTemplate<MorviArchiveBraveryPage, StrengthPower>
{
    public override string CustomIconPath => MorviAssetPaths.ArchivePagePowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ArchivePagePowerBigIcon;
}

internal sealed class MorviDexterityPagePower : ModTemporaryAppliedPowerTemplate<MorviArchiveDexterityPage, DexterityPower>
{
    public override string CustomIconPath => MorviAssetPaths.ArchivePagePowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ArchivePagePowerBigIcon;
}
