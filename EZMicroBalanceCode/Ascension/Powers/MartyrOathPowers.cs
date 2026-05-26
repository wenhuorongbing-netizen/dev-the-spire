using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MartyrOathPower : BossSealPower
{
    private bool _debuffConsumed;

    protected override BossSealId? SealId => BossSealId.MartyrOath;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "殉誓",
        "亲族祭司施加的下一次[gold]负面状态[/gold]持续时间+[blue]{Amount}[/blue]，之后移除此效果。若祭司先攻击，殉誓会改为强化那次攻击。",
        "下一次负面状态持续更久。",
        "Martyr Oath",
        "The next [gold]debuff[/gold] applied by Kin Priest lasts [blue]{Amount}[/blue] longer, then this is removed. If the Priest attacks first, the Oath empowers that attack instead.",
        "The next debuff lasts longer.");

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        return giver == Owner &&
            target?.IsPlayer == true &&
            amount > 0m &&
            power.GetTypeForAmount(amount) == PowerType.Debuff
                ? amount + Amount
                : amount;
    }

    public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        _debuffConsumed = true;
        await PowerCmd.Remove(Owner.GetPower<MartyrOathStrikePower>());
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (_debuffConsumed && participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class MartyrOathStrikePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MartyrOath;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "殉誓攻击",
        "下一次攻击每次命中额外造成[blue]{Amount}[/blue]点伤害，之后移除此效果。",
        "下一次攻击每次命中额外造成伤害。",
        "Martyr Strike",
        "Each hit of the next attack deals [blue]{Amount}[/blue] extra damage, then this is removed.",
        "Each hit of the next attack deals extra damage.");

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return dealer == Owner && props.IsPoweredAttack()
            ? Amount
            : 0m;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack())
        {
            await PowerCmd.Remove(Owner.GetPower<MartyrOathPower>());
            await PowerCmd.Remove(this);
        }
    }
}
