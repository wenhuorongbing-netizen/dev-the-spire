namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class SoulTideBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.SoulTide;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：魂潮",
        "灵魂异鱼进入[gold]无形[/gold]时获得[blue]1[/blue]层[gold]人工制品[/gold]。玩家回合结束时，手牌中每张[gold]呼唤[/gold]使它在下一次玩家回合开始时获得格挡。A19每张[blue]2[/blue]格挡，上限为单人[blue]8[/blue]、2人[blue]12[/blue]、3-4人[blue]16[/blue]；[gold]烙印形态[/gold]每张[blue]3[/blue]格挡，上限为单人[blue]12[/blue]、2人[blue]16[/blue]、3-4人[blue]20[/blue]。",
        "未处理的呼唤会让灵魂异鱼在下一次玩家回合开始时获得格挡。",
        "Dedicated Ability: Soul Tide",
        "When Soul Fysh becomes [gold]Intangible[/gold], it gains [blue]1[/blue] [gold]Artifact[/gold]. At player turn end, each [gold]Beckon[/gold] in hand gives it Block at the next player turn start. A19: [blue]2[/blue] Block each, capped at solo [blue]8[/blue], 2 players [blue]12[/blue], 3-4 players [blue]16[/blue]. [gold]Branded Form[/gold]: [blue]3[/blue] Block each, capped at solo [blue]12[/blue], 2 players [blue]16[/blue], 3-4 players [blue]20[/blue].",
        "Unanswered Beckons give Soul Fysh Block.");
}
