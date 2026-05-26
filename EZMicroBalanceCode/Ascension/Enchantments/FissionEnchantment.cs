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
            "这张牌的[gold]耗能[/gold]降低[blue]1[/blue]。打出后进入[gold]消耗[/gold]牌堆，并正常触发[gold]消耗[/gold]效果。",
            "[gold]耗能[/gold]降低[blue]1[/blue]。正常触发[gold]消耗[/gold]效果。")
        : new CardModifierLoc(
            "Fission",
            "This card costs [blue]1[/blue] less. After play, it enters the [gold]Exhaust[/gold] pile and triggers [gold]Exhaust[/gold] effects normally.",
            "Costs [blue]1[/blue] less. Triggers [gold]Exhaust[/gold] effects normally.");

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
