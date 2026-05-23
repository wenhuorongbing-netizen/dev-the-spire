using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class FiremarkHeatPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "热势",
            "达到[blue]2[/blue]层后，火印精英的下一次攻击更危险。",
            "达到[blue]2[/blue]层后强化下一次攻击。")
        : new PowerLoc(
            "Heat",
            "At [blue]2[/blue] Heat, the Firemarked enemy's next attack becomes more dangerous.",
            "At [blue]2[/blue] Heat, the next attack is stronger.");
}

internal sealed class FiremarkHeatStrikePower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "热势爆发",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害，然后移除此效果。",
            "下一次攻击额外造成[blue]{Amount}[/blue]点伤害。")
        : new PowerLoc(
            "Heat Burst",
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

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack())
        {
            await PowerCmd.Remove(this);
        }
    }
}
