using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviDebtPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.DebtPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.DebtPowerIcon;
}

internal sealed class MorviProofreadPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.ProofreadPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.ProofreadPowerIcon;
}

internal sealed class MorviOpenBookPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.OpenBookPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OpenBookPowerIcon;
}

internal sealed class MorviOverdraftPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.OverdraftPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.OverdraftPowerIcon;
}

internal sealed class MorviPaperstormPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => MorviAssetPaths.PaperstormPowerIcon;

    public override string CustomBigIconPath => MorviAssetPaths.PaperstormPowerIcon;
}

internal sealed class MorviBraveryPagePower : TemporaryStrengthPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<MorviArchiveBraveryPage>();
}

internal sealed class MorviDexterityPagePower : TemporaryDexterityPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<MorviArchiveDexterityPage>();
}
