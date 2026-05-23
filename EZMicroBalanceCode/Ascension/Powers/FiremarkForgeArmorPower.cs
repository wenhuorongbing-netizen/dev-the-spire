namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ForgeArmorMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkForgeArmorIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：锻甲",
            "你的回合开始时，火印宿主获得[blue]{Amount}[/blue]点[gold]熔甲[/gold]。若本回合结束时宿主没有格挡，下一次熔甲跳过。",
            "清空宿主格挡，可跳过下一次[gold]熔甲[/gold]。")
        : new PowerLoc(
            "Firemark: Forge Armor",
            "At the start of your turn, the Firemark Host gains [blue]{Amount}[/blue] [gold]Molten Armor[/gold]. If the host has no Block at turn end, the next Molten Armor is skipped.",
            "Clear the host's Block to skip the next [gold]Molten Armor[/gold].");
}
