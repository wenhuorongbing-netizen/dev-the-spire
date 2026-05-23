namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class GiantMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkGiantIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：巨躯",
            "火印宿主开场最大生命提高[blue]{Amount}%[/blue]。半血时暴露[gold]熔核[/gold]；打破熔核会波及一名副目标。",
            "半血时暴露[gold]熔核[/gold]。")
        : new PowerLoc(
            "Firemark: Giant",
            "The Firemark Host starts with +[blue]{Amount}%[/blue] [gold]Max HP[/gold]. At half HP, it exposes a Molten Core; breaking it splashes one secondary target.",
            "Exposes a Molten Core at half HP.");
}
