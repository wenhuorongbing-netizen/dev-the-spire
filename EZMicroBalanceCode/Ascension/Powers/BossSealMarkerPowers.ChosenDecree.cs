namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ChosenDecreeBossSealMarkerPower : BossSealMarkerPower
{
    protected override BossSealId? SealId => BossSealId.ChosenDecree;

    public override List<(string, string)>? Localization => Loc(
        "专属能力：御令",
        "女王施加[gold]束缚[/gold]时，其中[blue]1[/blue]张束缚牌获得[gold]御令[/gold]。打出御令牌不会触发额外惩罚。打出非御令束缚牌时，女王获得[blue]1[/blue]层[gold]威仪[/gold]；没有打出束缚牌时，女王获得[blue]1[/blue]层威仪，火炬头获得[blue]1[/blue]点[gold]力量[/gold]。威仪使下一次防御或屏障动作额外获得[blue]8[/blue]格挡。[gold]烙印形态[/gold]使威仪上限变为[blue]3[/blue]。",
        "打出正确的束缚牌可以避开御令惩罚。",
        "Dedicated Ability: Royal Decree",
        "When the Queen applies [gold]Bound[/gold], one Bound card gains [gold]Royal Decree[/gold]. Playing the Decree has no extra penalty. Playing a non-Decree Bound card gives the Queen [blue]1[/blue] [gold]Majesty[/gold]; playing no Bound card gives [blue]1[/blue] Majesty and gives Torch Head [blue]1[/blue] [gold]Strength[/gold]. Majesty adds [blue]8[/blue] Block to the next defense or barrier action. [gold]Branded Form[/gold] raises the Majesty cap to [blue]3[/blue].",
        "Play the correct Bound card to avoid the decree penalty.");
}
