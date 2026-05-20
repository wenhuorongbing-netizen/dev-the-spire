namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class ConstantHealMarkFiremarkPower : FiremarkPower
{
    protected override string FiremarkIconPath => AscensionAssetPaths.FiremarkConstantHealIndicator;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "火印：恒愈",
            "每个敌方回合结束时，[gold]火印精英[/gold]恢复[blue]{Amount}[/blue]点[gold]生命[/gold]。单回合造成该治疗量[blue]3[/blue]倍的伤害，可中断本回合治疗。",
            "造成治疗量[blue]3[/blue]倍的伤害可中断治疗。")
        : new PowerLoc(
            "Firemark: Constant Heal",
            "At the end of each enemy turn, the [gold]Firemarked enemy[/gold] heals [blue]{Amount}[/blue] [gold]HP[/gold]. Deal [blue]3[/blue] times that healing in one player turn to interrupt it.",
            "Deal [blue]3[/blue] times this healing to interrupt it.");
}
