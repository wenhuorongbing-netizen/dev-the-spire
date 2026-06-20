namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class JeweledMaskFreePower : ModEnchantmentTemplate, ILocalizationProvider
{
    public override bool HasExtraCardText => true;

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "宝石面具",
            "这张牌的费用已被宝石面具永久设为0。",
            "来自宝石面具，费用为0。")
        : new CardModifierLoc(
            "Jeweled Mask",
            "This card's cost was permanently set to 0 by Jeweled Mask.",
            "Costs 0 from Jeweled Mask.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Power;
    }

    protected override void OnEnchant()
    {
        if (!Card.EnergyCost.CostsX)
        {
            Card.EnergyCost.SetCustomBaseCost(0);
        }
    }
}

