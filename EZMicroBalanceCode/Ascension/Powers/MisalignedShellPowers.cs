using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class KaiserCalibrationPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MisalignedShell;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "校准",
        "达到[blue]2[/blue]层时，下一次攻击每次命中会额外造成伤害。每只爪每场战斗最多触发[blue]1[/blue]次。",
        "正在校准下一次攻击的命中。",
        "Calibration",
        "At [blue]2[/blue] stacks, each hit of the next attack deals extra damage. Each claw can trigger this once per combat.",
        "Calibrating the next attack's hits.");
}

internal sealed class KaiserCalibrationStrikePower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MisalignedShell;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "校准攻击",
        "下一次攻击每次命中额外造成[blue]{Amount}[/blue]点伤害，之后移除此效果。",
        "下一次攻击每次命中额外造成伤害。",
        "Calibrated Strike",
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
            await PowerCmd.Remove(this);
        }
    }
}
