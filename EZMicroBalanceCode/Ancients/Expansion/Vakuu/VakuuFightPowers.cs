using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuStolenVaultPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => VakuuFightAssetPaths.StolenVaultPowerIcon;

    public override string CustomBigIconPath => VakuuFightAssetPaths.StolenVaultPowerBigIcon;
}

internal sealed class VakuuBloodDebtPower : ModPowerTemplate
{
    private const int DamagePerDebt = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => VakuuFightAssetPaths.BloodDebtPowerIcon;

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

internal sealed class VakuuBacklashPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override string CustomIconPath => VakuuFightAssetPaths.BloodDebtPowerIcon;

    public override string CustomBigIconPath => VakuuFightAssetPaths.BloodDebtPowerBigIcon;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount
            : 0m;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side && participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
