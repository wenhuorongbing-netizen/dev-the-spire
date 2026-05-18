using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class FissionEnchantment : CustomEnchantmentModel, ILocalizationProvider
{
    public override bool HasExtraCardText => true;

    protected override string? CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "裂变",
            "这张牌的[gold]能量[/gold]费用降低[blue]1[/blue]，并获得[gold]消耗[/gold]。",
            "[gold]能量[/gold]费用降低[blue]1[/blue]。")
        : new CardModifierLoc(
            "Fission",
            "This card costs [blue]1[/blue] less and gains [gold]Exhaust[/gold].",
            "Costs [blue]1[/blue] less.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Attack or CardType.Skill;
    }

    protected override void OnEnchant()
    {
        if (!Card.EnergyCost.CostsX && Card.EnergyCost.GetWithModifiers(CostModifiers.None) > 0)
        {
            Card.EnergyCost.UpgradeBy(-1);
        }

        if (!Card.Keywords.Contains(CardKeyword.Exhaust))
        {
            Card.AddKeyword(CardKeyword.Exhaust);
        }
    }
}
