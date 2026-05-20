using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ShieldwallBannerbearerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerShieldFormationIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：盾阵",
            "旗手存活时，敌方回合结束后，其他敌人获得[blue]{Amount}[/blue]点[gold]格挡[/gold]。旗手死亡时，其他敌人立刻获得更高的一次[gold]格挡[/gold]。",
            "敌方回合结束后保护其他敌人。")
        : new PowerLoc(
            "Banner: Shieldwall",
            "While this bannerbearer is alive, other enemies gain [blue]{Amount}[/blue] [gold]Block[/gold] after the enemy turn. When it dies, other enemies immediately gain a larger burst of [gold]Block[/gold].",
            "Protects other enemies after the enemy turn.");
}
