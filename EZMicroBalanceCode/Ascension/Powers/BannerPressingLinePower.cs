using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class PressingLineStrikePower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerPressingLineIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：压阵",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害，然后移除此效果。",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害。")
        : new PowerLoc(
            "Banner: Pressing Line",
            "The next attack deals [blue]{Amount}[/blue] extra damage, then this is removed.",
            "The next attack deals [blue]{Amount}[/blue] extra damage.");

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

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            await PowerCmd.Remove(this);
        }
    }
}
