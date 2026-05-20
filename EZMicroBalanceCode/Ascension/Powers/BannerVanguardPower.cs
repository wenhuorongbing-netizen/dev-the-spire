using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class VanguardBannerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerVanguardIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：先锋",
            "开场获得[blue]{Amount}[/blue]点[gold]力量[/gold]。第[blue]3[/blue]回合开始时失去这些力量。",
            "第[blue]3[/blue]回合开始时失去这些[gold]力量[/gold]。")
        : new PowerLoc(
            "Banner: Vanguard",
            "Starts with [blue]{Amount}[/blue] [gold]Strength[/gold]. At the start of round [blue]3[/blue], loses that Strength.",
            "Loses this [gold]Strength[/gold] at the start of round [blue]3[/blue].");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
}
