using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class BloodPrizeBannerTargetPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerBountyIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：血赏",
            "第[blue]3[/blue]回合结束前击杀这名敌人，战斗后获得额外[gold]金币[/gold]。若它存活，它会获得[gold]反扑[/gold]。",
            "快速击杀可获得额外[gold]金币[/gold]。")
        : new PowerLoc(
            "Banner: Blood Prize",
            "Kill this enemy before round [blue]3[/blue] ends to gain extra [gold]Gold[/gold] after combat. If it survives, it gains retaliation.",
            "Kill quickly for extra [gold]Gold[/gold].");
}

internal sealed class BloodPrizeRetaliationPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerBountyIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "血赏反扑",
            "血赏目标存活。它获得[gold]力量[/gold]和[gold]人工制品[/gold]。",
            "血赏失败后的强化。")
        : new PowerLoc(
            "Blood Prize Retaliation",
            "The bounty target survived. It gained [gold]Strength[/gold] and [gold]Artifact[/gold].",
            "The failed bounty's retaliation.");
}
