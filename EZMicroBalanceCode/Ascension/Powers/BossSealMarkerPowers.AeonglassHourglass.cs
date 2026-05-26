namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AeonglassHourglassBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.AeonglassHourglass;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：时砂回流",
        "永世沙漏使用[gold]消退[/gold]后生成时砂。下个玩家回合中，每花费[blue]1[/blue]点能量移除[blue]1[/blue]枚。回合结束时，每剩余[blue]1[/blue]枚时砂，使下一次[gold]加大力度[/gold]额外加入[blue]1[/blue]张[gold]枯萎[/gold]。[gold]烙印形态[/gold]生成[blue]3[/blue]枚时砂；若[gold]眼部激光[/gold]开始时仍有时砂，额外命中[blue]1[/blue]次，每场最多[blue]2[/blue]次。",
        "花费能量清时砂；剩余时砂会增加枯萎。",
        "Dedicated Ability: Time Sand Reflow",
        "After [gold]Ebb[/gold], Aeonglass creates Time Sand. During the next player turn, each energy spent removes [blue]1[/blue]. At turn end, each remaining Time Sand makes the next [gold]Increasing Intensity[/gold] add [blue]1[/blue] extra [gold]Wither[/gold]. [gold]Branded Form[/gold] creates [blue]3[/blue] Time Sand; if [gold]Eye Lasers[/gold] starts while any remain, it hits [blue]1[/blue] extra time, up to [blue]2[/blue] times.",
        "Spend energy to clear Time Sand.");
}
