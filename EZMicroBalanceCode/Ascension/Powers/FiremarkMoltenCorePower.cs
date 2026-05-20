using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class MoltenCoreFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkGiantIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "熔核暴露",
            "本回合对火印精英造成[blue]{Amount}[/blue]点伤害，可使其失去[blue]10%[/blue][gold]最大生命[/gold]。失败时，它获得[blue]1[/blue]层[gold]人工制品[/gold]。",
            "造成[blue]{Amount}[/blue]点伤害可打破熔核。")
        : new PowerLoc(
            "Molten Core",
            "Deal [blue]{Amount}[/blue] damage to the Firemarked enemy this turn to make it lose [blue]10%[/blue] [gold]Max HP[/gold]. If you fail, it gains [blue]1[/blue] [gold]Artifact[/gold].",
            "Deal [blue]{Amount}[/blue] damage to break the core.");
}
