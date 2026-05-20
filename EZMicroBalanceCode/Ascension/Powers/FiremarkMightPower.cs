namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MightMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkMightIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：烈力",
            "[gold]火印精英[/gold]开场获得[blue]{Amount}[/blue]点[gold]力量[/gold]。造成未被格挡的攻击伤害后获得[gold]热势[/gold]。",
            "造成未被格挡的攻击伤害后获得[gold]热势[/gold]。")
        : new PowerLoc(
            "Firemark: Might",
            "The [gold]Firemarked enemy[/gold] starts with [blue]{Amount}[/blue] [gold]Strength[/gold]. Unblocked attack damage builds Heat.",
            "Unblocked attack damage builds Heat.");
}
