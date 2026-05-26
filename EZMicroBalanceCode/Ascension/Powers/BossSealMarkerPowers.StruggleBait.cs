namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class StruggleBaitBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.StruggleBait;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：逃亡疲劳",
        "无厌沙虫获得[gold]力量[/gold]或推进[gold]沙坑[/gold]时，将[blue]1[/blue]张由首领能力生成的[gold]狂乱逃离[/gold]加入受影响玩家的弃牌堆。全队每打出第[blue]3[/blue]张这类逃离，无厌沙虫获得[gold]活力[/gold]：A19为[blue]2[/blue]点，[gold]烙印形态[/gold]为[blue]3[/blue]点。每个玩家回合最多触发[blue]1[/blue]次。",
        "打出多张首领生成的逃离会让沙虫获得活力。",
        "Dedicated Ability: Escape Fatigue",
        "When The Insatiable gains [gold]Strength[/gold] or advances [gold]Sandpit[/gold], add [blue]1[/blue] ability-made [gold]Frantic Escape[/gold] to the affected player's discard pile. Every [blue]3[/blue] such Escapes played by the team gives The Insatiable [gold]Vigor[/gold]: A19 [blue]2[/blue], [gold]Branded Form[/gold] [blue]3[/blue]. Triggers at most once each player turn.",
        "Ability-made Escapes give The Insatiable Vigor.");
}
