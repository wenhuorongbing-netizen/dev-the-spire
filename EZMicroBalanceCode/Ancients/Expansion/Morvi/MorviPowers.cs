using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviDebtPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.DebtPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.DebtPowerBigIcon;
}

internal sealed class MorviProofreadPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.ProofreadPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ProofreadPowerBigIcon;
}

internal sealed class MorviOpenBookPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.OpenBookPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OpenBookPowerBigIcon;
}

internal sealed class MorviOverdraftPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.OverdraftPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OverdraftPowerBigIcon;
}

internal sealed class MorviPaperstormPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.PaperstormPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.PaperstormPowerBigIcon;
}

internal sealed class MorviBraveryPagePower : CustomTemporaryPowerModelWrapper<MorviArchiveBraveryPage, StrengthPower>
{
    public override string CustomPackedIconPath => MorviAssetPaths.ArchivePagePowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ArchivePagePowerBigIcon;
}

internal sealed class MorviDexterityPagePower : CustomTemporaryPowerModelWrapper<MorviArchiveDexterityPage, DexterityPower>
{
    public override string CustomPackedIconPath => MorviAssetPaths.ArchivePagePowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ArchivePagePowerBigIcon;
}
