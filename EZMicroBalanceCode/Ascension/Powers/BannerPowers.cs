using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal abstract class BannerPower : CustomPowerModel, ILocalizationProvider
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int DisplayAmount => 0;

    public override string CustomPackedIconPath => AscensionAssetPaths.BannerRoomIndicator;

    public override string CustomBigIconPath => AscensionAssetPaths.BannerRoomIndicator;

    public override abstract List<(string, string)>? Localization { get; }
}

internal sealed class VanguardBannerPower : BannerPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：先锋",
            "开战时获得[blue]2[/blue]点[gold]力量[/gold]；第[blue]3[/blue]回合开始时失去这些力量。",
            "第[blue]3[/blue]回合开始时失去这些[gold]力量[/gold]。")
        : new PowerLoc(
            "Banner: Vanguard",
            "Starts with [blue]2[/blue] [gold]Strength[/gold]. At the start of round [blue]3[/blue], loses that Strength.",
            "Loses this [gold]Strength[/gold] at the start of round [blue]3[/blue].");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
}

internal sealed class ShieldFormationBannerbearerPower : BannerPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：盾阵旗手",
            "只要这名旗手存活，其他敌人在每回合开始时获得[blue]5[/blue]点[gold]格挡[/gold]。旗手死亡时，其他敌人获得[blue]8[/blue]点[gold]格挡[/gold]。",
            "其他敌人每回合获得[blue]5[/blue]点[gold]格挡[/gold]；旗手死亡时获得[blue]8[/blue]点[gold]格挡[/gold]。")
        : new PowerLoc(
            "Banner: Shield Formation",
            "While this bannerbearer is alive, other enemies gain [blue]5[/blue] [gold]Block[/gold] at the start of each round. When it dies, other enemies gain [blue]8[/blue] [gold]Block[/gold].",
            "Other enemies gain [gold]Block[/gold] while this bannerbearer lives.");
}

internal sealed class BountyBannerTargetPower : BannerPower
{
    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：悬赏",
            "第[blue]3[/blue]回合结束前消灭这名敌人，战斗后获得[blue]15[/blue][gold]金币[/gold]。若未完成，它获得[blue]8[/blue]点[gold]格挡[/gold]和[blue]1[/blue]层[gold]人工制品[/gold]。",
            "第[blue]3[/blue]回合结束前消灭它可获得[blue]15[/blue][gold]金币[/gold]。")
        : new PowerLoc(
            "Banner: Bounty",
            "Kill this enemy before the end of round [blue]3[/blue] to gain [blue]15[/blue] [gold]Gold[/gold] after combat. If it survives, it gains [blue]8[/blue] [gold]Block[/gold] and [blue]1[/blue] [gold]Artifact[/gold].",
            "Kill before round [blue]3[/blue] ends to gain [blue]15[/blue] [gold]Gold[/gold].");
}
