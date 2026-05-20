namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class GiantMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkGiantIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：巨躯",
            "[gold]火印精英[/gold]开场最大生命提高[blue]{Amount}%[/blue]。半血时暴露[gold]熔核[/gold]。",
            "半血时暴露[gold]熔核[/gold]。")
        : new PowerLoc(
            "Firemark: Giant",
            "The [gold]Firemarked enemy[/gold] starts with +[blue]{Amount}%[/blue] [gold]Max HP[/gold]. At half HP, it exposes a Molten Core.",
            "Exposes a Molten Core at half HP.");
}
