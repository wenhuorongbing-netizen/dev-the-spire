using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AeonglassHourglassPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "时砂回流",
            "剩余[blue]{Amount}[/blue]枚时砂。玩家每花费[blue]1[/blue]点能量，移除[blue]1[/blue]枚。玩家回合结束时，每剩余[blue]1[/blue]枚时砂，使下一次[gold]加大力度[/gold]额外加入[blue]1[/blue]张[gold]枯萎[/gold]。[gold]A20烙印[/gold]下，若眼部激光开始时仍有时砂，眼部激光额外命中[blue]1[/blue]次。",
            "花费能量可以清除时砂。")
        : new PowerLoc(
            "Time Sand Reflow",
            "[blue]{Amount}[/blue] Time Sand remaining. Each energy spent removes [blue]1[/blue]. At player turn end, each remaining Time Sand makes the next [gold]Increasing Intensity[/gold] add [blue]1[/blue] extra [gold]Wither[/gold]. With [gold]Branded Form[/gold], if Eye Lasers begins while Time Sand remains, Eye Lasers hits [blue]1[/blue] extra time.",
            "Spend energy to clear Time Sand.");
}
