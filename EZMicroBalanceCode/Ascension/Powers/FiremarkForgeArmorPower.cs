namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ForgeArmorMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkForgeArmorIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：铸甲",
            "[gold]火印精英[/gold]在每个敌方回合后获得[blue]{Amount}[/blue]点[gold]熔甲[/gold]。下个玩家回合打掉这些熔甲后，下次熔甲不会生成。",
            "打掉本次熔甲后，下次熔甲不会生成。")
        : new PowerLoc(
            "Firemark: Forge Armor",
            "After each enemy turn, the [gold]Firemarked enemy[/gold] gains [blue]{Amount}[/blue] [gold]Molten Armor[/gold]. If you remove that armor next player turn, it skips the next armor gain.",
            "Remove this armor to skip the next armor gain.");
}
