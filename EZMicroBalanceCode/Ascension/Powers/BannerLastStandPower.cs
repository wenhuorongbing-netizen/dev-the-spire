using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class LastStandBannerPower : BannerPower
{
    protected override string BannerIconPath => AscensionAssetPaths.BannerLastStandIndicator;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new PowerLoc(
            "战旗：残阵",
            "获得[blue]{Amount}[/blue]点临时[gold]力量[/gold]。敌方回合结束时失去这些力量。",
            "敌方回合结束时失去这些[gold]力量[/gold]。")
        : new PowerLoc(
            "Banner: Last Stand",
            "Gains [blue]{Amount}[/blue] temporary [gold]Strength[/gold]. Loses it at the end of the enemy turn.",
            "Loses this [gold]Strength[/gold] at the end of the enemy turn.");

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && Owner.IsAlive)
        {
            Flash();
            await AscensionPowerAmountHelper.RemoveTemporaryStrength(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}
