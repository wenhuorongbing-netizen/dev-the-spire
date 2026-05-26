namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MarginalNoteBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：旁注",
        "[gold]知识诅咒[/gold]后，每名玩家的弃牌堆加入[blue]1[/blue]张临时[gold]旁注[/gold]。打出旁注会抽[blue]1[/blue]张牌。若回合结束时旁注仍在手牌中，知识恶魔获得[gold]深思[/gold]并消耗旁注。深思会给下一次知识诅咒添加附加代价。[gold]烙印形态[/gold]使深思上限变为[blue]3[/blue]，每回合最多增加[blue]2[/blue]层。",
        "旁注不处理会让下一次知识诅咒更重。",
        "Dedicated Ability: Marginal Note",
        "After [gold]Curse of Knowledge[/gold], add [blue]1[/blue] temporary [gold]Marginal Note[/gold] to each player's discard pile. Playing it draws [blue]1[/blue]. If a Note remains in hand at turn end, Knowledge Demon gains [gold]Deep Thought[/gold] and exhausts it. Deep Thought adds a side cost to the next Knowledge curse. [gold]Branded Form[/gold] raises the cap to [blue]3[/blue]; each turn can add at most [blue]2[/blue].",
        "Unplayed notes make the next Knowledge curse worse.");
}
