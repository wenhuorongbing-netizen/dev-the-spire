using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AeonglassLaserEchoPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "时砂回流",
        "下一次眼部激光额外命中[blue]1[/blue]次，之后移除此效果。",
        "下一次眼部激光额外命中。",
        "Time Sand Reflow",
        "The next Eye Lasers hit [blue]1[/blue] extra time, then this is removed.",
        "The next Eye Lasers hit one extra time.");

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        return attack.Attacker == Owner && Owner.Monster is Aeonglass
            ? hitCount + 1
            : hitCount;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

internal sealed class AeonglassPendingWitherPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "待回流枯萎",
        "下一次[gold]加大力度[/gold]额外加入[blue]{Amount}[/blue]张[gold]枯萎[/gold]，之后移除此效果。",
        "下一次加大力度加入更多枯萎。",
        "Pending Wither",
        "The next [gold]Increasing Intensity[/gold] adds [blue]{Amount}[/blue] extra [gold]Wither[/gold], then this is removed.",
        "The next Increasing Intensity adds extra Wither.");
}

internal sealed class AeonglassLaserEchoUseCounterPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    protected override bool IsVisibleInternal => false;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "时砂激光计数",
        "本场战斗已经触发的时砂激光次数。",
        "隐藏计数。",
        "Time Sand Laser Count",
        "Hidden counter for Time Sand's extra Eye Lasers this combat.",
        "Hidden counter.");
}
