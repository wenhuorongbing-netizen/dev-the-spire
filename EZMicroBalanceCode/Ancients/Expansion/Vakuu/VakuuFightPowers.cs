using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuStolenVaultPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => VakuuFightAssetPaths.StolenVaultPowerIcon;

    public override string CustomBigIconPath => VakuuFightAssetPaths.StolenVaultPowerBigIcon;
}

internal sealed class VakuuBloodDebtPower : CustomPowerModel
{
    private const int DamagePerDebt = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomPackedIconPath => VakuuFightAssetPaths.BloodDebtPowerIcon;

    public override string CustomBigIconPath => VakuuFightAssetPaths.BloodDebtPowerBigIcon;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount * DamagePerDebt
            : 0m;
    }
}
